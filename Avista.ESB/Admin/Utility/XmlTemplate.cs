
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace Avista.ESB.Admin.Utility
{
      /// <summary>
      /// Provides functionality for working with an XML template. The template contains the basic structure
      /// and content of an XML document instance document. Macros contained in the template can be expanded
      /// to fill in parts of the document with calculated and parameterized values. The XmlTemplate class
      /// provides methods that allow instances generated from the template to be returned in a variety
      /// of forms including as an xml string, an xml document, or a deserialized object instance.
      /// </summary>
      public class XmlTemplate
      {
            /// <summary>
            /// The template text.
            /// </summary>
            private string _text = String.Empty;

            /// <summary>
            /// The Type of object that can be deserialized from instances of the template.
            /// </summary>
            private Type _objectType = null;

            /// <summary>
            /// The XML namespace of instances of the template.
            /// </summary>
            private string _xmlNamespace = null;

            /// <summary>
            /// The root element name used by the template.
            /// </summary>
            private string _rootElementName = null;

            /// <summary>
            /// Array of arguments that are used by macros when an instance is generated.
            /// </summary>
            private object[ ] _args = null;

            /// <summary>
            /// The most recent instance generated from the template.
            /// </summary>
            private string _instance = null;

            /// <summary>
            /// Default constructor for an XmlTemplate.
            /// </summary>
            public XmlTemplate ()
            {
            }

            /// <summary>
            /// Constructs an XmlTemplate for a given object type.
            /// </summary>
            /// <param name="objectType">The type of object that the template represents.</param>
            public XmlTemplate (Type objectType)
            {
                  this._objectType = objectType;
                  try
                  {
                        this._xmlNamespace = SerializableObject.GetXmlNamespace( objectType );
                  }
                  catch ( Exception exception )
                  {
                        // Warn about inability to determine xml namespace.
                        string message = "Unable to determine XML namespace for object of type " + objectType.FullName + ".";
                        throw exception;
                  }
            }

            /// <summary>
            /// Constructs an XmlTemplate for a given XML namespace.
            /// </summary>
            /// <param name="xmlNamespace">The XML namespace of the template.</param>
            public XmlTemplate (string xmlNamespace)
            {
                  this._xmlNamespace = xmlNamespace;
            }

            /// <summary>
            /// Constructs an XmlTemplate for a given object type and XML namespace.
            /// </summary>
            /// <param name="objectType">The type of object that the template represents.</param>
            /// <param name="xmlNamespace">The XML namespace of the template.</param>
            public XmlTemplate (Type objectType, string xmlNamespace)
            {
                  this._objectType = objectType;
                  this._xmlNamespace = xmlNamespace;
            }

            /// <summary>
            /// Constructs an XmlTemplate for a given object type and XML namespace.
            /// </summary>
            /// <param name="objectType">The type of object that the template represents.</param>
            /// <param name="xmlNamespace">The XML namespace of the template.</param>
            public XmlTemplate (Type objectType, string xmlNamespace, string rootElementName)
            {
                  this._objectType = objectType;
                  this._xmlNamespace = xmlNamespace;
                  this._rootElementName = rootElementName;
            }

            /// <summary>
            /// The text of the template.
            /// </summary>
            public string Text
            {
                  get
                  {
                        return _text;
                  }
                  set
                  {
                        _text = value;
                  }
            }

            /// <summary>
            /// Loads the XML template from a file.
            /// </summary>
            public void Load (string filePath)
            {
                  _text = String.Empty;
                  try
                  {
                        _text = File.ReadAllText( filePath );
                  }
                  catch ( Exception exception )
                  {
                        //string message = "Error loading XML template from file '" + filePath + "'.";
                        //ContextualException contextualException = new ContextualException( message, exception );
                        //contextualException.EventType = EventLogEntryType.Error;
                        //contextualException.EventId = 400;
                        //ExceptionManager.HandleException( contextualException, PolicyName.SystemException );
                        throw exception;

                  }
            }

            /// <summary>
            /// Loads from SOAP input
            /// </summary>
            /// <param name="soapXml"></param>
            public void LoadFromSoapXml (string soapXml)
            {
                  XDocument xDoc = XDocument.Parse( soapXml );
                  var soapBody = xDoc.Descendants().Skip( 2 ).First().ToString();
                  _text = soapBody;
            }

            /// <summary>
            /// Loads the XML template from an assembly resource.
            /// </summary>
            public void LoadFromResource (Assembly assembly, string resourceName)
            {
                  try
                  {
                        _text = LoadAsString( assembly, resourceName );
                  }
                  catch ( Exception exception )
                  {
                         string message = "Error loading XML template from resource '" + resourceName + "'.";
                         Exception newException = new Exception( message + "\n\r" + exception.StackTrace );
                  }
            }
            /// <summary>
            /// Loads a resource into a string.
            /// </summary>
            /// <param name="assemblyName">The name of the assembly containing the resource.</param>
            /// <param name="resourceName">The name of the resource.</param>
            /// <returns>A string containing the content of the resource.</returns>
            private string LoadAsString (Assembly assembly, string resourceName)
            {
                  String text = null;
                  string assemblyName = "unknown";
                  try
                  {
                        assemblyName = assembly.GetName().Name;
                        string qualifiedResourceName = assemblyName + "." + resourceName;
                        using ( Stream stream = assembly.GetManifestResourceStream( qualifiedResourceName ) )
                        {
                              using ( StreamReader sr = new StreamReader( stream ) )
                              {
                                    text = sr.ReadToEnd();
                              }
                        }
                  }
                  catch ( Exception exception )
                  {
                        string message = "Error loading resource '" + resourceName + "' from assembly '" + assemblyName + "'.";
                        Exception newException = new Exception( message + "\n\r" + exception.StackTrace );
                        throw newException;
                  }
                  return text;
            }


            /// <summary>
            /// Generates an instance from the template.
            /// </summary>
            public void Execute ()
            {
                  this._args = null;
                  ExpandMacros();
            }

            /// <summary>
            /// Generates an instance from the template using the provided arguments.
            /// </summary>
            /// <param name="args">Object array of arguments used to generate the instance.</param>
            public void Execute (object[ ] args)
            {
                  this._args = args;
                  ExpandMacros();
            }

            /// <summary>
            /// Performs macro substitutions in a message template and populates the instance.
            /// Macros take the form {Verb,Parm1,Parm2,...,ParmN}
            /// </summary>
            private void ExpandMacros ()
            {
                  try
                  {
                        _instance = _text;
                        int start = _instance.IndexOf( "{" );
                        int end = _instance.IndexOf( "}", start + 1 );
                        while ( start >= 0 && end > start )
                        {
                              int length = end - start - 1;
                              if ( length > 0 )
                              {
                                    string macro = _instance.Substring( start + 1, length );
                                    char[ ] separator = { ',' };
                                    string[ ] words = macro.Split( separator );
                                    string verb = words[ 0 ];
                                    // {PARM,index,type,format}
                                    if ( verb.Equals( "PARM" ) )
                                    {
                                          try
                                          {
                                                if ( words.Length != 4 )
                                                {
                                                      throw new Exception( "PARM macro usage is {PARM,index,type,format}." );
                                                }
                                                int index = int.Parse( words[ 1 ] );
                                                object parm = _args[ index ];
                                                string type = words[ 2 ];
                                                string format = words[ 3 ];
                                                string value = null;
                                                if ( type.Equals( "INT" ) )
                                                {
                                                      value = ((int) parm).ToString( format );
                                                }
                                                else if ( type.Equals( "STRING" ) ) // Format is ignored.
                                                {
                                                      value = (string) parm;
                                                }
                                                else if ( type.Equals( "BOOL" ) ) // Format is ignored.
                                                {
                                                      value = ((bool) parm).ToString();
                                                }
                                                else if ( type.Equals( "DATE" ) )
                                                {
                                                      value = ((DateTime) parm).ToString( format );
                                                }
                                                else if ( type.Equals( "DECIMAL" ) )
                                                {
                                                      value = ((decimal) parm).ToString( format );
                                                }
                                                else if ( type.Equals( "DOUBLE" ) )
                                                {
                                                      value = ((double) parm).ToString( format );
                                                }
                                                else if ( type.Equals( "FLOAT" ) )
                                                {
                                                      value = ((float) parm).ToString( format );
                                                }
                                                else
                                                {
                                                      throw new Exception( "Unrecognized type. Valid types are BOOL, DATE, DECIMAL, DOUBLE, FLOAT, INT, and STRING." );
                                                }
                                                _instance = _instance.Substring( 0, start ) + value + _instance.Substring( end + 1 );
                                                end = start + value.Length;
                                          }
                                          catch ( Exception exception )
                                          {
                                                throw new Exception( "Error processing PARM macro.", exception );
                                          }
                                    }
                                    // {NOW,format}
                                    else if ( verb.Equals( "NOW" ) )
                                    {
                                          try
                                          {
                                                if ( words.Length != 2 )
                                                {
                                                      throw new Exception( "NOW macro usage is {NOW,format}." );
                                                }
                                                DateTime now = DateTime.Now;
                                                string format = words[ 1 ];
                                                string value = now.ToString( format );
                                                _instance = _instance.Substring( 0, start ) + value + _instance.Substring( end + 1 );
                                                end = start + value.Length;
                                          }
                                          catch ( Exception exception )
                                          {
                                                throw new Exception( "Error processing NOW macro.", exception );
                                          }
                                    }
                                    // {RAND,Min,Max}
                                    else if ( verb.Equals( "RAND" ) )
                                    {
                                          try
                                          {
                                                if ( words.Length != 3 )
                                                {
                                                      throw new Exception( "RAND macro usage is {RAND,min,max}." );
                                                }
                                                int min = int.Parse( words[ 1 ] );
                                                int max = int.Parse( words[ 2 ] );
                                                Random random = new Random();
                                                string value = random.Next( min, max ).ToString();
                                                _instance = _instance.Substring( 0, start ) + value + _instance.Substring( end + 1 );
                                                end = start + value.Length;
                                          }
                                          catch ( Exception exception )
                                          {
                                                throw new Exception( "Error processing RAND macro.", exception );
                                          }
                                    }
                                    // {GUID}
                                    else if ( verb.Equals( "GUID" ) )
                                    {
                                          string value = Guid.NewGuid().ToString();
                                          try
                                          {
                                                if ( words.Length != 1 )
                                                {
                                                      throw new Exception( "GUID macro usage is {GUID}." );
                                                }
                                                _instance = _instance.Substring( 0, start ) + value + _instance.Substring( end + 1 );
                                                end = start + value.Length;
                                          }
                                          catch ( Exception exception )
                                          {
                                                throw new Exception( "Error processing GUID macro.", exception );
                                          }
                                    }
                                    else
                                    {
                                          // The verb was not recognized so we will set the end pointer
                                          // to the start and the next iteration will continue with the next character.
                                          // This will allow content like {{GUID}} in which the outer braces are ignored.
                                          end = start;
                                    }
                              }
                              // Look for the next macro.
                              start = _instance.IndexOf( "{", end + 1 );
                              end = _instance.IndexOf( "}", start + 1 );
                        }
                  }
                  catch ( Exception exception )
                  {
                        //ContextualException contextualException = new ContextualException( "Error expanding macros in XML template.", e );
                        //contextualException.EventType = EventLogEntryType.Error;
                        //contextualException.EventId = 401;
                        //ExceptionManager.HandleException( contextualException, PolicyName.SystemException );
                        throw exception;
                  }
            }

            /// <summary>
            /// Returns the text of the XmlTemplate.
            /// </summary>
            /// <returns>The text of the XmlTemplate.</returns>
            public override string ToString ()
            {
                  return _text;
            }

            /// <summary>
            /// Gets the current instance of the template as an XML string.
            /// </summary>
            /// <returns>The xml for the current instance.</returns>
            public string InstanceAsXmlString ()
            {
                  return _instance;
            }

            /// <summary>
            /// Gets the current instance of the template as an XmlDocument.
            /// </summary>
            /// <returns>An XmlDocument representing the current instance.</returns>
            public XmlDocument InstanceAsXmlDocument ()
            {
                  XmlDocument document = null;
                  try
                  {
                        document = new XmlDocument();
                        document.LoadXml( _instance );
                  }
                  catch ( Exception exception )
                  {
                        //ContextualException contextualException = new ContextualException( "Error generating XmlDocument from XML template instance.", exception );
                        //contextualException.EventType = EventLogEntryType.Error;
                        //contextualException.EventId = 402;
                        //ExceptionManager.HandleException( contextualException, PolicyName.SystemException );
                        throw exception;
                  }
                  return document;
            }

            /// <summary>
            /// Gets the current instance of the template as a deserialized object.
            /// </summary>
            /// <returns>An object representing the current instance.</returns>
            public object InstanceAsObject ()
            {
                  object serializableObject = null;
                  XmlSerializer serializer = null;
                  string typeName = "object";
                  try
                  {
                        // Create a serializer.
                        if ( _objectType == null )
                        {
                              throw new Exception( "Cannot convert an XmlTemplate instance to an object unless an object type is provided." );
                        }
                        else
                        {
                              // Deserialize the instance into the object.
                              typeName = _objectType.Name;
                              serializer = SerializableObject.GetXmlSerializer( _objectType, _xmlNamespace, _rootElementName );
                              StringReader reader = new StringReader( _instance );
                              serializableObject = serializer.Deserialize( reader );
                        }
                  }
                  catch ( Exception exception )
                  {
                        //ContextualException contextualException = new ContextualException( "Error generating " + typeName + " from XML template instance.", exception );
                        //contextualException.EventType = EventLogEntryType.Error;
                        //contextualException.EventId = 402;
                        //ExceptionManager.HandleException( contextualException, PolicyName.SystemException );
                        throw exception;
                  }
                  return serializableObject;
            }

            public string InstanceAsSoapObject ()
            {
                  string data = InstanceAsXmlString();
                  string ret = @"<s:Envelope xmlns:s=""http://schemas.xmlsoap.org/soap/envelope/""><s:Body xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"">" +
                               data + "</s:Body></s:Envelope>";
                  return ret;
            }

      }
}
