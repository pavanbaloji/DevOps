//-----------------------------------------------------------------------------
// This file is part of the HP.Practices Application Framework
//
// Copyright (c) HP Enterprise Services. All rights reserved.
//
// THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
// KIND, WHETHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR
// PURPOSE.
//-----------------------------------------------------------------------------
using System;
using System.IO;
using System.Text;

namespace Avista.ESB.Admin.Utility
{
    public class ColorConsole
    {
        /// <summary>
        /// Lock object used for synchronization of access to the <see cref="ColorConsole"/>.
        /// </summary>
        private static object _lock = new Object();

        /// <summary>
        /// The original foreground color of the console.
        /// </summary>
        private static ConsoleColor _originalConsoleColor = System.Console.ForegroundColor;

        /// <summary>
        /// Flag indicating that the console is running in quiet mode and only error messages will be written to the console.
        /// </summary>
        private static bool _quiet = false;

        /// <summary>
        /// Flag indicating that the console is running in verbose mode and verbose messages will be written to the console.
        /// The quiet flag can override the verbose flag.
        /// </summary>
        private static bool _verbose = false;

        /// <summary>
        /// Flag indicating that the console should record the history.
        /// </summary>
        private static bool _recordHistory = false;

        /// <summary>
        /// StringBuilder used to collect history of the console.
        /// </summary>
        private static StringBuilder _history = new StringBuilder();

        /// <summary>
        /// StringBuilder used to collect history of warnings.
        /// </summary>
        private static StringBuilder _warnings = new StringBuilder();

        /// <summary>
        /// StringBuilder used to collect history of errors.
        /// </summary>
        private static StringBuilder _errors = new StringBuilder();

        /// <summary>
        /// A count of the total number of messages that have occurred since the last time ClearHistory was called.
        /// </summary>
        private static int _messageCount = 0;

        /// <summary>
        /// A count of the number of trace messages that have occurred since the last time ClearHistory was called.
        /// </summary>
        private static int _traceCount = 0;

        /// <summary>
        /// A count of the number of info messages that have occurred since the last time ClearHistory was called.
        /// </summary>
        private static int _infoCount = 0;

        /// <summary>
        /// A count of the number of warning messages that have occurred since the last time ClearHistory was called.
        /// </summary>
        private static int _warningCount = 0;

        /// <summary>
        /// A count of the number of error messages that have occurred since the last time ClearHistory was called.
        /// </summary>
        private static int _errorCount = 0;

        /// <summary>
        /// A flag indicating if there is a console. When there is no console, output can still be collected via the history methods.
        /// </summary>
        private static bool _console = true;

        /// <summary>
        /// The output event handlers will be called each time output is written to the <see cref="ColorConsole"/>.
        /// The events allow any interested party to receive notifications of output being generated.
        /// </summary>
        private static event OutputEventHandler _outputEventHandler = null;

        /// <summary>
        /// Flag indicating that the console is running in quiet mode and only error messages will be written to the console.
        /// </summary>
        public static bool Quiet
        {
            get
            {
                return _quiet;
            }
            set
            {
                _quiet = value;
            }
        }

        /// <summary>
        /// Flag indicating that the console is running in verbose mode and verbose messages will be written to the console.
        /// The quiet flag can override the verbose flag.
        /// </summary>
        public static bool Verbose
        {
            get
            {
                return _verbose;
            }
            set
            {
                _verbose = value;
            }
        }

        /// <summary>
        /// Flag indicating that the console should keep a history of messages written.
        /// Three buffers are maintained in history: One that records all messages,
        /// one that records only warnings, and one that records only errors.
        /// </summary>
        public static bool RecordHistory
        {
            get
            {
                return _recordHistory;
            }
            set
            {
                _recordHistory = value;
            }
        }

        /// <summary>
        /// Clears any history that had been recorded.
        /// </summary>
        public static void ClearHistory()
        {
            lock (_lock)
            {
                _history = new StringBuilder();
                _warnings = new StringBuilder();
                _errors = new StringBuilder();
                _messageCount = 0;
                _traceCount = 0;
                _infoCount = 0;
                _warningCount = 0;
                _errorCount = 0;
            }
        }

        /// <summary>
        /// Returns all the messages recorded since history recording was turned on.
        /// Calling ClearHistory will reset the list of messages.
        /// </summary>
        public static string History
        {
            get
            {
                string history = null;
                lock (_lock)
                {
                    history = _history.ToString();
                }
                return history;
            }
        }

        /// <summary>
        /// Returns all the warnings recorded since history recording was turned on.
        /// Calling ClearHistory will reset the list of warning messages.
        /// </summary>
        public static string Warnings
        {
            get
            {
                string warnings = null;
                lock (_lock)
                {
                    warnings = _warnings.ToString();
                }
                return warnings;
            }
        }

        /// <summary>
        /// Returns all the errors recorded since history recording was turned on.
        /// Calling ClearHistory will reset the list of error messages.
        /// </summary>
        public static string Errors
        {
            get
            {
                string errors = null;
                lock (_lock)
                {
                    errors = _errors.ToString();
                }
                return errors;
            }
        }

        /// <summary>
        /// Returns the total number of messages that have been written.
        /// Calling ClearHistory will reset the count.
        /// </summary>
        public static int MessageCount
        {
            get
            {
                return _messageCount;
            }
        }

        /// <summary>
        /// Returns the total number of trace messages that have been written.
        /// Calling ClearHistory will reset the count.
        /// </summary>
        public static int TraceCount
        {
            get
            {
                return _traceCount;
            }
        }

        /// <summary>
        /// Returns the total number of info messages that have been written.
        /// Calling ClearHistory will reset the count.
        /// </summary>
        public static int InfoCount
        {
            get
            {
                return _infoCount;
            }
        }

        /// <summary>
        /// Returns the total number of warning messages that have been written.
        /// Calling ClearHistory will reset the count.
        /// </summary>
        public static int WarningCount
        {
            get
            {
                return _warningCount;
            }
        }

        /// <summary>
        /// Returns the total number of error messages that have been written.
        /// Calling ClearHistory will reset the count.
        /// </summary>
        public static int ErrorCount
        {
            get
            {
                return _errorCount;
            }
        }

        /// <summary>
        /// Causes no output to be written to the console. Output will still be written to the history. This method should be called
        /// by GUI applications that have no console but will be using classes that use the ColorConsole to write output messages.
        /// Output can then be collected by the GUI application by using the history methods.
        /// </summary>
        public static void FreeConsole()
        {
            lock (_lock)
            {
                _console = false;
            }
        }

        /// <summary>
        /// Adds an event handler to listen for ouput events.
        /// </summary>
        /// <param name="outputEventHandler">The event handler to be added.</param>
        public static void AddOutputEventHandler(OutputEventHandler outputEventHandler)
        {
            lock (_lock)
            {
                _outputEventHandler += outputEventHandler;
            }
        }

        /// <summary>
        /// Removes an event handler that listens for ouput events.
        /// </summary>
        /// <param name="outputEventHandler">The event handler to be removed.</param>
        public static void RemoveOutputEventHandler(OutputEventHandler outputEventHandler)
        {
            lock (_lock)
            {
                _outputEventHandler -= outputEventHandler;
            }
        }

        /// <summary>
        /// Raises output events if there are output event handlers registered.
        /// </summary>
        /// <remarks>
        /// This should not be called while the lock is held or it may result in a deadlock.
        /// If an event handler uses Control.Invoke which waits for the process' main thread
        /// to handle the Invoke message but the main thread is currently trying to use the
        /// ColorConsole and waiting on the lock, then the deadlock will occur.
        /// </remarks>
        /// <param name="propertyName">The name of the property that was modified.</param>
        private static void RaiseOutputEvent(OutputType outputType, string message)
        {
           if (_outputEventHandler != null)
           {
               OutputEventArgs e = new OutputEventArgs(outputType ,message);
               _outputEventHandler(null, e);
           }
        }

        /// <summary>
        /// Displays a trace message if the console is in verbose mode and not in quiet mode. Trace messages are displayed in gray.
        /// </summary>
        /// <param name="message">The message to be displayed.</param>
        public static void Trace(string message, ConsoleColor color)
        {
            if (!_quiet && _verbose)
            {
                lock (_lock)
                {

                    WriteMessage(message, color);
                    _traceCount++;
                }
                RaiseOutputEvent(OutputType.Trace, message);
            }
        }

        /// <summary>
        /// Displays an informational message unless the console is in quiet mode. Informational messages are displayed in white.
        /// </summary>
        /// <param name="message">The message to be displayed.</param>
        public static void Info(string message)
        {
            if (!_quiet)
            {
                lock (_lock)
                {
                    WriteMessage(message, ConsoleColor.White);
                    _infoCount++;
                }
                RaiseOutputEvent(OutputType.Info, message);
            }
        }

        /// <summary>
        /// Displays a warning message unless the console is in quiet mode. Warning messages are displayed in yellow.
        /// </summary>
        /// <param name="message">The message to be displayed.</param>
        public static void Warning(string message)
        {
            if (!_quiet)
            {
                lock (_lock)
                {
                    WriteMessage(message, ConsoleColor.Yellow);
                    if (_recordHistory)
                    {
                        _warnings.AppendLine(message);
                    }
                    _warningCount++;
                }
                RaiseOutputEvent(OutputType.Warning, message);
            }
        }

        /// <summary>
        /// Displays an error message. Error messages are displayed in red.
        /// </summary>
        /// <param name="message">The error message to be displayed.</param>
        public static void Error(string message)
        {
            lock (_lock)
            {
                WriteMessage(message, ConsoleColor.Red);
                if (_recordHistory)
                {
                    _errors.AppendLine(message);
                }
                _errorCount++;
            }
            RaiseOutputEvent(OutputType.Error, message);
        }

        /// <summary>
        /// Displays a message in the given color.
        /// </summary>
        /// <param name="message">The message to be displayed.</param>
        /// <param name="color">The color to display the message in.</param>
        private static void WriteMessage(string message, ConsoleColor color)
        {
            if (_console)
            {
                System.Console.ForegroundColor = color;
                System.Console.WriteLine(message);
                System.Console.ForegroundColor = _originalConsoleColor;
            }
            if (_recordHistory)
            {
                _history.AppendLine(message);
            }
            _messageCount++;
        }
    }
}
