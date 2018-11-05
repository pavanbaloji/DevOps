
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;


namespace Avista.ESB.Admin.Utility
{
      /// <summary>
      ///This class provides a method for creating and deleting a file from a temproary location.
      /// </summary>
      public class TempFile : IDisposable
      {
            /// <summary>
            /// Flag that indicates whether or not the object has been disposed. 
            /// </summary>
            private bool _disposed = false;

            /// <summary>
            /// The directory of the file.
            /// </summary>
            private string _directory = "";

            /// <summary>
            /// The name of the file.
            /// </summary>
            private string _name = "";

            /// <summary>
            /// The extension of the file.
            /// </summary>
            private string _extension = "";

            /// <summary>
            /// The fully qualified file path with name and extension.
            /// </summary>
            private string _filePath = "";

            /// <summary>
            /// The encoding to be used for the file.
            /// </summary>
            private Encoding _encoding = Encoding.UTF8;

            /// <summary>
            /// Constructs an empty temporary file in the current directory.
            /// The file will have UTF-8 encoding and the extension "tmp".
            /// </summary>
            public TempFile ()
            {
                  try
                  {
                        _directory = Directory.GetCurrentDirectory();
                        _name = Guid.NewGuid().ToString();
                        _extension = "tmp";
                        _filePath = Path.Combine( _directory, _name + "." + _extension );
                        _encoding = Encoding.UTF8;
                        WriteContent( "" );
                  }
                  catch ( Exception exception )
                  {
                        //ContextualException contextualException = new ContextualException( "Error creating temporary file " + _filePath + ".", exception );
                        //contextualException.EventType = EventLogEntryType.Error;
                        //contextualException.EventId = 405;
                        //ExceptionManager.HandleException( contextualException, PolicyName.SystemException );
                        throw exception;
                  }
            }

            /// <summary>
            /// Constructs a temporary file in the current directory for the current instance of an XmlTemplate.
            /// The file will have UTF-8 encoding and the extension "xml". 
            /// </summary>
            /// <param name="xmlTemplate">An XmlTemplate containing the XML content to be used for the temporary file.</param>
            public TempFile (XmlTemplate xmlTemplate)
            {
                  try
                  {
                        _directory = Directory.GetCurrentDirectory();
                        _name = Guid.NewGuid().ToString();
                        _extension = "xml";
                        _filePath = Path.Combine( _directory, _name + "." + _extension );
                        _encoding = Encoding.UTF8;
                        WriteContent( xmlTemplate.InstanceAsXmlString() );
                  }
                  catch ( Exception exception )
                  {
                        //ContextualException contextualException = new ContextualException( "Error creating temporary file " + _filePath + ".", exception );
                        //contextualException.EventType = EventLogEntryType.Error;
                        //contextualException.EventId = 405;
                        //ExceptionManager.HandleException( contextualException, PolicyName.SystemException );
                        throw exception;
                  }
            }

            /// <summary>
            /// Constructs a temporary file with the given content and file extension.
            /// The default encoding of UTF-8 is used to create the file.
            /// </summary>
            /// <param name="content">The content to be used for the temporary file.</param>
            /// <param name="extension">The extension to be used for the temporary file.</param>
            public TempFile (string content, string extension)
            {
                  try
                  {
                        _directory = Directory.GetCurrentDirectory();
                        _name = Guid.NewGuid().ToString();
                        _extension = extension;
                        _filePath = Path.Combine( _directory, _name + "." + _extension );
                        _encoding = Encoding.UTF8;
                        WriteContent( content );
                  }
                  catch ( Exception exception )
                  {
                        //ContextualException contextualException = new ContextualException( "Error creating temporary file " + _filePath + ".", exception );
                        //contextualException.EventType = EventLogEntryType.Error;
                        //contextualException.EventId = 405;
                        //ExceptionManager.HandleException( contextualException, PolicyName.SystemException );
                        throw exception;
                  }
            }

            /// <summary>
            /// Constructs a temporary file with the given content, file extension, and encoding.
            /// </summary>
            /// <param name="content">The content to be used for the temporary file.</param>
            /// <param name="extension">The extension to be used for the temporary file.</param>
            /// <param name="encoding">The encoding to be used to cerate the temporary file.</param>
            public TempFile (string content, string extension, Encoding encoding)
            {
                  try
                  {
                        _directory = Directory.GetCurrentDirectory();
                        _name = Guid.NewGuid().ToString();
                        _extension = extension;
                        _filePath = Path.Combine( _directory, _name + "." + _extension );
                        _encoding = encoding;
                        WriteContent( content );
                  }
                  catch ( Exception exception )
                  {
                        //ContextualException contextualException = new ContextualException( "Error creating temporary file " + _filePath + ".", exception );
                        //contextualException.EventType = EventLogEntryType.Error;
                        //contextualException.EventId = 405;
                        //ExceptionManager.HandleException( contextualException, PolicyName.SystemException );
                        throw exception;
                  }
            }

            /// <summary>
            /// The fully qualified name of the temporary file.
            /// </summary>
            public string FilePath
            {
                  get
                  {
                        return _filePath;
                  }
            }

            /// <summary>
            /// Writes content to the temporary file.
            /// </summary>
            /// <param name="content">The content to be written.</param>
            public void WriteContent (string content)
            {
                  StreamWriter writer = null;
                  try
                  {
                        writer = new StreamWriter( _filePath, false, _encoding );
                        writer.Write( content );
                        writer.Flush();
                  }
                  catch ( Exception exception )
                  {
                        throw new Exception( "Error writing content to temporary file " + _filePath + ".", exception );
                  }
                  finally
                  {
                        if ( writer != null )
                        {
                              writer.Close();
                              writer = null;
                        }
                  }
            }

            /// <summary>
            /// Appends content to the temporary file.
            /// </summary>
            /// <param name="content">The content to be appended.</param>
            public void AppendContent (string content)
            {
                  StreamWriter writer = null;
                  try
                  {
                        writer = new StreamWriter( _filePath, true, _encoding );
                        writer.Write( content );
                        writer.Flush();
                  }
                  catch ( Exception exception )
                  {
                        throw new Exception( "Error appending content to temporary file " + _filePath + ".", exception );
                  }
                  finally
                  {
                        if ( writer != null )
                        {
                              writer.Close();
                              writer = null;
                        }
                  }
            }

            /// <summary>
            /// Reads the content of the temporary file and returns it as a single string.
            /// </summary>
            /// <returns>The content of the temporary file as a string.</returns>
            public string ReadAllText ()
            {
                  string text = null;
                  try
                  {
                        text = File.ReadAllText( _filePath, _encoding );
                  }
                  catch ( Exception exception )
                  {
                        throw new Exception( "Error reading text from temporary file " + _filePath + ".", exception );
                  }
                  return text;
            }

            /// <summary>
            /// Reads the content of the temporary file and returns it as an array of strings.
            /// </summary>
            /// <returns>The content of the temporary file as an array of strings.</returns>
            public string[ ] ReadAllLines ()
            {
                  string[ ] lines = null;
                  try
                  {
                        lines = File.ReadAllLines( _filePath, _encoding );
                  }
                  catch ( Exception exception )
                  {
                        throw new Exception( "Error reading lines from temporary file " + _filePath + ".", exception );
                  }
                  return lines;
            }

            ///<summary>
            ///Finalizer for the class.
            ///</summary>
            ~TempFile ()
            {
                  Dispose( false );
            }

            /// <summary>
            /// Releases all resources used by the <c>TempFile</c>.
            /// </summary>
            public void Dispose ()
            {
                  Dispose( true );
                  GC.SuppressFinalize( this );
            }

            /// <summary>
            /// Releases the unmanaged resources used by the <c>TempFile</c> and
            /// optionally releases the managed resources.
            /// </summary>
            /// <param name="disposing">
            ///   true to release both managed and unmanaged resources;
            ///   false to release only unmanaged resources.
            /// </param>
            protected virtual void Dispose (bool disposing)
            {
                  if ( !_disposed )
                  {
                        // Dispose managed resources
                        if ( disposing )
                        {
                        }
                        // Dispose unmanaged resources
                        if ( _filePath != null )
                        {
                              if ( File.Exists( _filePath ) )
                              {
                                    File.Delete( _filePath );
                              }
                              _filePath = null;
                        }
                  }
                  _disposed = true;
            }

            /// <summary>
            /// Check to see if the <c>TempFile</c> has been disposed.
            /// </summary>
            protected void CheckDisposed ()
            {
                  if ( _disposed )
                  {
                        throw new ObjectDisposedException( this.GetType().Name );
                  }
            }
      }
}
