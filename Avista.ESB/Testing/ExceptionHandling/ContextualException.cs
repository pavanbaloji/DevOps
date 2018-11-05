using System;
using System.Diagnostics;
using System.Collections.Specialized;
using System.Reflection;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Text;
using System.Threading;
using Avista.ESB.Testing.Components;
using Avista.ESB.Utilities.Logging;

namespace Avista.ESB.Testing.ExceptionHandling
{
      [Serializable()]
      public class ContextualException : ApplicationException
      {
            /// <summary>
            /// Holds values for the context of the exception.
            /// </summary>
            private NameValueCollection contextInfo = null;

            /// <summary>
            /// The Event Id associated with the exception. The default value is 0.
            /// </summary>
            private int eventId = 0;

            /// <summary>
            /// The Event Type associated with the exception. The default value is Error.
            /// </summary>
            private EventLogEntryType eventType = EventLogEntryType.Error;

            /// <summary>
            /// A recursion monitor for preventing stack overflows during construction.
            /// </summary>
            private static RecursionMonitor recursionMonitor = new RecursionMonitor( 3 );

            /// <summary>
            /// Default constructor.
            /// </summary>
            public ContextualException ()
                  : base()
            {
                  Initialize();
            }

            /// <summary>
            /// Constructor that sets the exception message.
            /// </summary>
            /// <param name="message">The message that describes the exception.</param>
            public ContextualException (string message)
                  : base( message )
            {
                  Initialize();
            }

            /// <summary>
            /// Constructor that sets the exception message and event id.
            /// </summary>
            /// <param name="message">The message that describes the exception.</param>
            /// <param name="eventId">The event id to be associated with the exception.</param>
            public ContextualException (string message, int eventId)
                  : base( message )
            {
                  this.eventId = eventId;
                  Initialize();
            }

            /// <summary>
            /// Constructor that sets the exception message, event id and event type.
            /// </summary>
            /// <param name="message">The message that describes the exception.</param>
            /// <param name="eventId">The event id to be associated with the exception.</param>
            /// <param name="eventType">The event type to be associated with the exception.</param>
            public ContextualException (string message, int eventId, EventLogEntryType eventType)
                  : base( message )
            {
                  this.eventId = eventId;
                  this.eventType = eventType;
                  Initialize();
            }

            /// <summary>
            /// Constructor that sets the exception message and a nested exception.
            /// </summary>
            /// <param name="message">The message that describes the exception.</param>
            /// <param name="innerException">The inner exception that led to this exception.</param>
            public ContextualException (string message, Exception innerException)
                  : base( message, innerException )
            {
                  Initialize();
            }

            /// <summary>
            /// Constructor that sets the exception message, event id, and a nested exception.
            /// </summary>
            /// <param name="message">The message that describes the exception.</param>
            /// <param name="eventId">The event id to be associated with the exception.</param>
            /// <param name="innerException">The inner exception that led to this exception.</param>
            public ContextualException (string message, int eventId, Exception innerException)
                  : base( message, innerException )
            {
                  this.eventId = eventId;
                  Initialize();
            }

            /// <summary>
            /// Constructor that sets the exception message, event id, event type, and a nested exception.
            /// </summary>
            /// <param name="message">The message that describes the exception.</param>
            /// <param name="eventId">The event id to be associated with the exception.</param>
            /// <param name="eventType">The event type to be associated with the exception.</param>
            /// <param name="innerException">The inner exception that led to this exception.</param>
            public ContextualException (string message, int eventId, EventLogEntryType eventType, Exception innerException)
                  : base( message, innerException )
            {
                  this.eventId = eventId;
                  this.eventType = eventType;
                  Initialize();
            }

            /// <summary>
            /// Constructor used for deserialization of the exception.
            /// </summary>
            /// <param name="info">The serialization info that will be used to reconstruct the object.</param>
            /// <param name="context">The context for deserialization.</param>
            protected ContextualException (SerializationInfo info, StreamingContext context)
                  : base( info, context )
            {
                  contextInfo = (NameValueCollection) info.GetValue( "contextInfo", typeof( NameValueCollection ) );
                  eventId = (int) info.GetValue( "eventId", typeof( int ) );
                  eventType = (EventLogEntryType) info.GetValue( "eventType", typeof( EventLogEntryType ) );
            }

            /// <summary>
            /// Adds the object data for the exception to the serialization info. This method is called when the exception is being serialized.
            /// </summary>
            /// <param name="info">The serialization info to which the exception's object data will be addded.</param>
            /// <param name="context">The context of the serialization.</param>
            [SecurityPermission( SecurityAction.Demand, SerializationFormatter = true )]
            public override void GetObjectData (SerializationInfo info, StreamingContext context)
            {
                  base.GetObjectData( info, context );
                  info.AddValue( "contextInfo", contextInfo, typeof( NameValueCollection ) );
                  info.AddValue( "eventId", eventId, typeof( int ) );
                  info.AddValue( "eventType", eventType, typeof( EventLogEntryType ) );
            }

            /// <summary>
            /// Initialize the ContextualException.
            /// </summary>
            private void Initialize ()
            {
                  try
                  {
                        if ( recursionMonitor.Increment() )
                        {
                              contextInfo = new NameValueCollection();
                              RecordExceptionContext();
                        }
                  }
                  catch ( Exception exception )
                  {

                        Logger.WriteError( "An exception occurred initializing a ContextualException object." + exception );
                  }
                  finally
                  {
                        recursionMonitor.Decrement();
                  }
            }

            /// <summary>
            /// Provides a suggestion as to the event id to use if this exception is logged in an event log.
            /// </summary>
            public int EventId
            {
                  get
                  {
                        return eventId;
                  }
                  set
                  {
                        eventId = value;
                  }
            }

            /// <summary>
            /// Provides a suggestion as to the event type to use if this exception is logged in an event log.
            /// </summary>
            public EventLogEntryType EventType
            {
                  get
                  {
                        return eventType;
                  }
                  set
                  {
                        eventType = value;
                  }
            }

            /// <summary>
            /// Sets a named value describing the context in which the exception occurred.
            /// </summary>
            /// <param name="name">The name of the context value.</param>
            /// <param name="value">The value of the context value.</param>
            public void SetContextValue (string name, string value)
            {
                  contextInfo.Add( name, value );
            }

            /// <summary>
            /// Gets a named value describing the context in which the exception occurred.
            /// </summary>
            /// <param name="name">The name of the context value.</param>
            /// <returns>The value of the context value.</returns>
            public string GetContextValue (string name)
            {
                  string value = contextInfo.Get( name );
                  if ( value == null )
                  {
                        value = "";
                  }
                  return value;
            }

            /// <summary>
            /// Records the context in which the exception object was constructed.
            /// </summary>
            private void RecordExceptionContext ()
            {
                  try
                  {
                        StackTrace stackTrace = new StackTrace();
                        Process currentProcess = System.Diagnostics.Process.GetCurrentProcess();
                        for ( int stackTraceCounter = 0; (stackTraceCounter <= stackTrace.FrameCount - 1); stackTraceCounter++ )
                        {
                              // get frame
                              StackFrame stackFrame = stackTrace.GetFrame( stackTraceCounter );
                              MethodBase methodBase = stackFrame.GetMethod();
                              // avoid the constructors when Custom Exceptions constructors call in their inheritance 
                              //  path also avoid the Initialize method
                              if ( ((methodBase.Name != ".ctor") && (methodBase.Name.ToUpper() != "RecordExceptionContext".ToUpper())) )
                              {
                                    contextInfo.Add( "AssemblyName", methodBase.ReflectedType.Assembly.GetName().Name );
                                    contextInfo.Add( "ClassName", methodBase.ReflectedType.Name );
                                    contextInfo.Add( "MethodName", methodBase.Name );
                                    break;
                              }
                        }
                        // Add Environment Variables to the Collection.
                        contextInfo.Add( "MachineName", Environment.MachineName );
                        contextInfo.Add( "OSVersion", Environment.OSVersion.ToString() );
                        contextInfo.Add( "StackTrace", Environment.StackTrace );
                        contextInfo.Add( "CurrentProcessId", currentProcess.Id.ToString() );
                        contextInfo.Add( "CurrentThreadId", Thread.CurrentThread.ManagedThreadId.ToString() );
                        contextInfo.Add( "CurrentThreadName", Thread.CurrentThread.Name );
                        contextInfo.Add( "Username", Environment.UserName );
                        contextInfo.Add( "UserDomainName", Environment.UserDomainName );
                        contextInfo.Add( "UserInteractive", Environment.UserInteractive.ToString() );
                        contextInfo.Add( "Version", Environment.Version.ToString() );
                        contextInfo.Add( "WorkingSet", Environment.WorkingSet.ToString() );
                  }
                  catch ( Exception )
                  {
                  }
            }

            /// <summary>
            /// Provides a string representation of the exception.
            /// The method loops through inner exceptions and builds a tracking log.
            /// </summary>
            /// <returns>A string representation of the exception.</returns>
            public override string Message
            {
                  get
                  {
                        StringBuilder sb = new StringBuilder();
                        try
                        {
                              // Start with base message.
                              sb.Append( base.Message );
                              sb.AppendLine();

                              // Collect values.
                              string exceptionType = GetType().ToString();
                              string machineName = GetContextValue( "MachineName" );
                              string assemblyName = GetContextValue( "AssemblyName" );
                              string className = GetContextValue( "ClassName" );
                              string methodName = GetContextValue( "MethodName" );
                              string processId = GetContextValue( "CurrentProcessId" );
                              string threadId = GetContextValue( "CurrentThreadId" );
                              string threadName = GetContextValue( "CurrentThreadName" );

                              // Format the string 
                              sb.AppendFormat( "ExceptionTime --------> {0}{1}", DateTime.Now.ToString(), Environment.NewLine );
                              sb.AppendFormat( "ExceptionType --------> {0}{1}", exceptionType, Environment.NewLine );
                              sb.AppendFormat( "Machine       --------> {0}{1}", machineName, Environment.NewLine );
                              sb.AppendFormat( "Assembly      --------> {0}{1}", assemblyName, Environment.NewLine );
                              sb.AppendFormat( "Class         --------> {0}{1}", className, Environment.NewLine );
                              sb.AppendFormat( "Method        --------> {0}{1}", methodName, Environment.NewLine );
                              sb.AppendFormat( "ProcessId     --------> {0}{1}", processId, Environment.NewLine );
                              sb.AppendFormat( "ThreadId      --------> {0}{1}", threadId, Environment.NewLine );
                              sb.AppendFormat( "ThreadName    --------> {0}{1}", threadName );
                        }
                        catch ( Exception )
                        {
                              sb = new StringBuilder( base.Message );
                        }
                        return sb.ToString();
                  }
            }
      }
}

