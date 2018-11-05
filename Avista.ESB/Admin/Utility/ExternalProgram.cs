
using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;


namespace Avista.ESB.Admin.Utility
{
      /// <summary>
      /// Allows running of an external program with output and errors captured and/or written to the console.
      /// </summary>
      public class ExternalProgram
      {
            /// <summary>
            /// The path of the external program that will be executed.
            /// </summary>
            private string _program;

            /// <summary>
            /// The command line arguments to be passed to the external program.
            /// </summary>
            private string _arguments;

            /// <summary>
            /// The working directory for the process used to run the external program.
            /// </summary>
            private string _workingDirectory;

            /// <summary>
            /// Used to capture output text.
            /// </summary>
            private StringBuilder _outputText;

            /// <summary>
            /// Used to capture error text.
            /// </summary>
            private StringBuilder _errorText;

            /// <summary>
            /// A flag indicating if output should be directed to the console.
            /// </summary>
            private bool _outputToConsole = true;

            /// <summary>
            /// Gets set to true when the process has exited to signal that the call to Run can return.
            /// </summary>
            private volatile bool _exited = false;

            /// <summary>
            /// The exit code returned by the external program.
            /// </summary>
            private volatile int _exitCode;

            /// <summary>
            /// Constructs an ExternalProgram to manage execution of the program with the given arguments and working directory.
            /// </summary>
            /// <param name="program">The path of the program to be executed.</param>
            /// <param name="arguments">The command line arguments to be used.</param>
            /// <param name="workingDirectory">The working directory to be used.</param>
            public ExternalProgram (string program, string arguments, string workingDirectory)
            {
                  _program = program;
                  _arguments = arguments;
                  _workingDirectory = workingDirectory;
                  Initialize();
            }

            /// <summary>
            /// Initializes variables used to track a run of the external program.
            /// </summary>
            private void Initialize ()
            {
                  _outputText = new StringBuilder();
                  _errorText = new StringBuilder();
                  _exited = false;
                  _exitCode = 0;
            }

            /// <summary>
            /// Flag indicating whether or not output should be written to the console.
            /// </summary>
            public bool OutputToConsole
            {
                  get
                  {
                        return _outputToConsole;
                  }
                  set
                  {
                        _outputToConsole = value;
                  }
            }

            /// <summary>
            /// The standard output of the program.
            /// </summary>
            public string Output
            {
                  get
                  {
                        return _outputText.ToString();
                  }
            }

            /// <summary>
            /// The error output of the program.
            /// </summary>
            public string Errors
            {
                  get
                  {
                        return _errorText.ToString();
                  }
            }

            /// <summary>
            /// The exit code of the program.
            /// </summary>
            public int ExitCode
            {
                  get
                  {
                        return _exitCode;
                  }
            }

            /// <summary>
            /// Runs the program.
            /// </summary>
            public void Run ()
            {
                  Process process = null;
                  //-----------------------------------------------------------------
                  // Verify that the executable exists.
                  //----------------------------------------------------------------
                  if ( !File.Exists( _program ) )
                  {
                        throw new Exception( "Executable not found: " + _program );
                  }
                  else
                  {
                        Initialize();
                        process = new Process();
                        process.StartInfo.FileName = _program;
                        process.StartInfo.Arguments = _arguments;
                        process.StartInfo.WorkingDirectory = _workingDirectory;
                        process.StartInfo.CreateNoWindow = true;
                        process.StartInfo.UseShellExecute = false;
                        process.StartInfo.RedirectStandardOutput = true;
                        process.OutputDataReceived += new DataReceivedEventHandler( OutputHandler );
                        process.StartInfo.RedirectStandardError = true;
                        process.ErrorDataReceived += new DataReceivedEventHandler( ErrorHandler );
                        process.EnableRaisingEvents = true;
                        process.Exited += new EventHandler( ExitHandler );
                        //-------------------------------------------------------------------------
                        // Start the process and capture its output.
                        //-------------------------------------------------------------------------
                        process.Start();
                        process.BeginOutputReadLine();
                        process.BeginErrorReadLine();
                        while ( !_exited )
                        {
                              Thread.Sleep( 20 );
                        }
                        process.Close();
                        process.Dispose();
                  }
            }

            /// <summary>
            /// Handles standard output received from the external program.
            /// </summary>
            /// <param name="sendingProcess">The process executing the external program.</param>
            /// <param name="outLine">The output text.</param>
            private void OutputHandler (object sendingProcess, DataReceivedEventArgs outLine)
            {
                  try
                  {
                        string data = outLine.Data;
                        if ( !String.IsNullOrEmpty( data ) )
                        {
                              _outputText.AppendLine( data );
                              if ( _outputToConsole )
                              {
                                    ColorConsole.Info( data );
                              }
                        }
                  }
                  catch ( Exception exception )
                  {
                        //ContextualException contextualException = new ContextualException( "Exception in external program standard output handler.", 204, EventLogEntryType.Error, exception );
                        //ExceptionManager.HandleException( contextualException, PolicyName.SystemException );
                  }
            }

            /// <summary>
            /// Handles error output received from the external program.
            /// </summary>
            /// <param name="sendingProcess">The process executing the external program.</param>
            /// <param name="outLine">The error text.</param>
            private void ErrorHandler (object sendingProcess, DataReceivedEventArgs outLine)
            {
                  try
                  {
                        string data = outLine.Data;
                        if ( !String.IsNullOrEmpty( data ) )
                        {
                              _outputText.AppendLine( data );
                              _errorText.AppendLine( data );
                              if ( _outputToConsole )
                              {
                                    ColorConsole.Error( data );
                              }
                        }
                  }
                  catch ( Exception exception )
                  {
                        //ContextualException contextualException = new ContextualException( "Exception in external program error output handler.", 204, EventLogEntryType.Error, exception );
                        //ExceptionManager.HandleException( contextualException, PolicyName.SystemException );
                  }
            }

            /// <summary>
            /// Handles the exit of the external program.
            /// </summary>
            /// <param name="sender">The process executing the external program.</param>
            /// <param name="e">The event arguments.</param>
            private void ExitHandler (object sender, EventArgs e)
            {
                  try
                  {
                        Process process = (Process) sender;
                        _exitCode = process.ExitCode;
                  }
                  catch ( Exception exception )
                  {
                        _exitCode = 1;
                        //ContextualException contextualException = new ContextualException( "Exception in external program exit handler.", 204, EventLogEntryType.Error, exception );
                        //ExceptionManager.HandleException( contextualException, PolicyName.SystemException );
                  }
                  _exited = true;
            }
      }
}
