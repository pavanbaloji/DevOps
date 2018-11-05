
using System;
using System.IO;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;
using System.Xml.Serialization;


namespace Avista.ESB.Admin.Utility
{
    public static class SerializableObject
    {
        /// <summary>
        /// Gets the XML namespace associated with a serializable object.
        /// </summary>
        /// <param name="serializableObject">The object for which the XML namespace is being requested.</param>
        /// <returns>The XML namespace.</returns>
        public static string GetXmlNamespace(object serializableObject)
        {
            string xmlNamespace = null;
            Type serializableObjectType = null;
            try
            {
                serializableObjectType = serializableObject.GetType();
                xmlNamespace = GetXmlNamespace(serializableObjectType);
            }
            catch (Exception exception)
            {
                throw new Exception("Unable to get XML namespace for object.", exception);
            }
            return xmlNamespace;
        }

        /// <summary>
        /// Gets the XML namespace associated with a serializable object type.
        /// </summary>
        /// <param name="serializableObjectType">The object type for which the XML namespace is being requested.</param>
        /// <returns>The XML namespace.</returns>
        public static string GetXmlNamespace(Type serializableObjectType)
        {
            string xmlNamespace = null;
            try
            {
                object[] attributes = serializableObjectType.GetCustomAttributes(typeof(System.Xml.Serialization.XmlTypeAttribute), false);
                if (attributes.Length == 0)
                {
                    throw new Exception("Missing XmlType attribute on class " + serializableObjectType.Name + ".");
                }
                else
                {
                    XmlTypeAttribute xmlType = (XmlTypeAttribute)attributes[0];
                    xmlNamespace = xmlType.Namespace;
                }
            }
            catch (Exception exception)
            {
                throw new Exception("Unable to get XML namespace for type " + serializableObjectType.Name + ".", exception);
            }
            return xmlNamespace;
        }

        /// <summary>
        /// Gets an XmlSerializer for the given object.
        /// </summary>
        /// <param name="serializableObject">The object for which a serializer is needed.</param>
        /// <returns>An XML serializer.</returns>
        public static XmlSerializer GetXmlSerializer(Object serializableObject)
        {
            Type serializableObjectType = null;
            XmlSerializer serializer = null;
            try
            {
                serializableObjectType = serializableObject.GetType();
                serializer = GetXmlSerializer(serializableObjectType);
            }
            catch (Exception exception)
            {
                throw new Exception("Unable to get XmlSerializer for object.", exception);
            }
            return serializer;
        }

        /// <summary>
        /// Gets an XmlSerializer for the given type.
        /// </summary>
        /// <param name="serializableObjectType">The object type for which a serializer is needed.</param>
        /// <returns>An XML serializer.</returns>
        public static XmlSerializer GetXmlSerializer(Type serializableObjectType)
        {
            XmlSerializer serializer = null;
            string xmlNamespace = null;
            try
            {
                xmlNamespace = GetXmlNamespace(serializableObjectType);
                serializer = new XmlSerializer(serializableObjectType, xmlNamespace);
            }
            catch (Exception exception)
            {
                throw new Exception("Unable to get XmlSerializer for type " + serializableObjectType.Name + ".", exception);
            }
            return serializer;
        }

        /// <summary>
        /// Gets an XmlSerializer for the given object and XML namespace.
        /// </summary>
        /// <param name="serializableObject">The object for which a serializer is needed.</param>
        /// <param name="xmlNamespace">The namespace for which the serializer is needed.</param>
        /// <returns>An XML serializer.</returns>
        public static XmlSerializer GetXmlSerializer(Object serializableObject, string xmlNamespace)
        {
            XmlSerializer serializer = null;
            Type serializableObjectType = null;
            try
            {
                serializableObjectType = serializableObject.GetType();
                serializer = GetXmlSerializer(serializableObjectType, xmlNamespace);
            }
            catch (Exception exception)
            {
                throw new Exception("Unable to get XmlSerializer for object using namespace '" + xmlNamespace + "'.", exception);
            }
            return serializer;
        }

        /// <summary>
        /// Gets an XmlSerializer for the given type and XML namespace.
        /// </summary>
        /// <param name="serializableObjectType">The type for which a serializer is needed.</param>
        /// <param name="xmlNamespace">The namespace for which the serializer is needed.</param>
        /// <returns>An XML serializer.</returns>
        public static XmlSerializer GetXmlSerializer(Type serializableObjectType, string xmlNamespace)
        {
            XmlSerializer serializer = null;
            try
            {
                if (xmlNamespace == null)
                {
                    serializer = new XmlSerializer(serializableObjectType);
                }
                else
                {
                    serializer = new XmlSerializer(serializableObjectType, xmlNamespace);
                }
            }
            catch (Exception exception)
            {
                throw new Exception("Unable to get XmlSerializer for type " + serializableObjectType.Name + " using namespace '" + xmlNamespace + "'.", exception);
            }
            return serializer;
        }

        /// <summary>
        /// Gets an XmlSerializer for the given object, XML namespace, and root node name.
        /// </summary>
        /// <param name="serializableObject">The object for which a serializer is needed.</param>
        /// <param name="xmlNamespace">The namespace for which the serializer is needed.</param>
        /// <param name="rootElementName">The name of the root element for which the serializer is needed.</param>
        /// <returns>An XML serializer.</returns>
        public static XmlSerializer GetXmlSerializer(Object serializableObject, string xmlNamespace, string rootElementName)
        {
            XmlSerializer serializer = null;
            Type serializableObjectType = null;
            try
            {
                serializableObjectType = serializableObject.GetType();
                serializer = GetXmlSerializer(serializableObjectType, xmlNamespace, rootElementName);
            }
            catch (Exception exception)
            {
                throw new Exception("Unable to get XmlSerializer for object using namespace '" + xmlNamespace + "' and root element name '" + rootElementName + "'.", exception);
            }
            return serializer;
        }

        /// <summary>
        /// Gets an XmlSerializer for the given type, XML namespace, and root node name.
        /// </summary>
        /// <param name="serializableObjectType">The type for which a serializer is needed.</param>
        /// <param name="xmlNamespace">The namespace for which the serializer is needed.</param>
        /// <param name="rootElementName">The name of the root element for which the serializer is needed.</param>
        /// <returns>An XML serializer.</returns>
        public static XmlSerializer GetXmlSerializer(Type serializableObjectType, string xmlNamespace, string rootElementName)
        {
            XmlSerializer serializer = null;
            try
            {
                if (xmlNamespace == null && rootElementName==null)
                {
                    serializer = new XmlSerializer(serializableObjectType);
                }
                else if (xmlNamespace == null)
                {
                    XmlRootAttribute xmlRootAttribute = new XmlRootAttribute(rootElementName);
                    serializer = new XmlSerializer(serializableObjectType, xmlRootAttribute);
                }
                else if (rootElementName == null)
                {
                    serializer = new XmlSerializer(serializableObjectType, xmlNamespace);
                }
                else
                {
                    XmlRootAttribute xmlRootAttribute = new XmlRootAttribute(rootElementName);
                    serializer = new XmlSerializer(serializableObjectType, null, null, xmlRootAttribute, xmlNamespace);
                }
            }
            catch (Exception exception)
            {
                throw new Exception("Unable to get XmlSerializer for type " + serializableObjectType.Name + " using namespace '" + xmlNamespace + "' and root element name '" + rootElementName + "'.", exception);
            }
            return serializer;
        }

        /// <summary>
        /// Converts a serializable object to an XML string representation using the XmlSerializer.
        /// </summary>
        /// <param name="serializableObject">The object to be serialized to an XML string.</param>
        /// <returns>A string containing the XML representation of the serializable object.</returns>
        public static string ToXmlString(Object serializableObject)
        {
            XmlSerializer serializer = null;
            StringBuilder xmlStringBuilder = null;
            StringWriter writer = null;
            try
            {
                serializer = GetXmlSerializer(serializableObject);
                xmlStringBuilder = new StringBuilder();
                writer = new StringWriter(xmlStringBuilder);
                serializer.Serialize(writer, serializableObject);
            }
            catch (Exception exception)
            {
                throw new Exception("Unable to serialize object of type " + serializableObject.GetType().Name + " to XML.", exception);
            }
            return xmlStringBuilder.ToString();
        }

        /// <summary>
        /// Converts a serializable object to an XML string representation using the XmlSerializer.
        /// </summary>
        /// <param name="serializableObject">The object to be serialized to an XML string.</param>
        /// <returns>A string containing the XML representation of the serializable object.</returns>
        public static string ToXmlString(Object serializableObject, string xmlNamespace)
        {
            XmlSerializer serializer = null;
            StringBuilder xmlStringBuilder = null;
            StringWriter writer = null;
            try
            {
                serializer = GetXmlSerializer(serializableObject, xmlNamespace);
                xmlStringBuilder = new StringBuilder();
                writer = new StringWriter(xmlStringBuilder);
                serializer.Serialize(writer, serializableObject);
            }
            catch (Exception exception)
            {
                throw new Exception("Unable to serialize object of type " + serializableObject.GetType().Name + " to XML.", exception);
            }
            return xmlStringBuilder.ToString();
        }

        /// <summary>
        /// Converts a serializable object to an XML string representation using the XmlSerializer.
        /// </summary>
        /// <param name="serializableObject">The object to be serialized to an XML string.</param>
        /// <returns>A string containing the XML representation of the serializable object.</returns>
        public static string ToXmlString(Object serializableObject, string xmlNamespace, string rootElementName)
        {
            XmlSerializer serializer = null;
            StringBuilder xmlStringBuilder = null;
            StringWriter writer = null;
            try
            {
                serializer = GetXmlSerializer(serializableObject, xmlNamespace, rootElementName);
                xmlStringBuilder = new StringBuilder();
                writer = new StringWriter(xmlStringBuilder);
                serializer.Serialize(writer, serializableObject);
            }
            catch (Exception exception)
            {
                throw new Exception("Unable to serialize object of type " + serializableObject.GetType().Name + " to XML.", exception);
            }
            return xmlStringBuilder.ToString();
        }

        /// <summary>
        /// Converts a serializable object to an XmlDocument.
        /// </summary>
        /// <param name="serializableObject">The object to be serialized to an XmlDocument.</param>
        /// <returns>An XmlDocument containing the XML representation of the serializable object.</returns>
        public static XmlDocument ToXmlDocument(Object serializableObject)
        {
            String xmlString = null;
            XmlDocument xmlDocument = null;
            try
            {
                xmlString = ToXmlString(serializableObject);
                xmlDocument = new XmlDocument();
                xmlDocument.LoadXml(xmlString);
            }
            catch (Exception exception)
            {
                throw new Exception("Unable to serialize object of type " + serializableObject.GetType().Name + " to an XmlDocument.", exception);
            }
            return xmlDocument;
        }

        /// <summary>
        /// Converts a serializable object to an XmlDocument.
        /// </summary>
        /// <param name="serializableObject">The object to be serialized to an XmlDocument.</param>
        /// <param name="xmlNamespace">The XML namespace to be used for deserialization.</param>
        /// <returns>An XmlDocument containing the XML representation of the serializable object.</returns>
        public static XmlDocument ToXmlDocument(Object serializableObject, string xmlNamespace)
        {
            String xmlString = null;
            XmlDocument xmlDocument = null;
            try
            {
                xmlString = ToXmlString(serializableObject, xmlNamespace);
                xmlDocument = new XmlDocument();
                xmlDocument.LoadXml(xmlString);
            }
            catch (Exception exception)
            {
                throw new Exception("Unable to serialize object of type " + serializableObject.GetType().Name + " to an XmlDocument.", exception);
            }
            return xmlDocument;
        }

        /// <summary>
        /// Converts a serializable object to an XmlDocument.
        /// </summary>
        /// <param name="serializableObject">The object to be serialized to an XmlDocument.</param>
        /// <param name="xmlNamespace">The XML namespace to be used for deserialization.</param>
        /// <param name="xmlRootElement">The XML root element to be used for deserialization</param>
        /// <returns>An XmlDocument containing the XML representation of the serializable object.</returns>
        public static XmlDocument ToXmlDocument(Object serializableObject, string xmlNamespace, string xmlRootElement)
        {
            String xmlString = null;
            XmlDocument xmlDocument = null;
            try
            {
                xmlString = ToXmlString(serializableObject, xmlNamespace, xmlRootElement);
                xmlDocument = new XmlDocument();
                xmlDocument.LoadXml(xmlString);
            }
            catch (Exception exception)
            {
                throw new Exception("Unable to serialize object of type " + serializableObject.GetType().Name + " to an XmlDocument.", exception);
            }
            return xmlDocument;
        }

        /// <summary>
        /// Writes a serializable object to a file.
        /// </summary>
        /// <param name="serializableObject">The object to be serialized to a file.</param>
        /// <param name="outputPath">The path of the file to be written.</param>
        public static void ToFile(Object serializableObject, string outputPath)
        {
            Type serializableObjectType = null;
            XmlSerializer serializer = null;
            try
            {
                serializableObjectType = serializableObject.GetType();
                serializer = GetXmlSerializer(serializableObjectType);
            }
            catch (Exception exception)
            {
                throw new Exception("Unable to serialize object of type " + serializableObject.GetType().Name + " to file " + outputPath + ". ", exception);
            }
            ToFile(serializableObject, serializer, outputPath);
        }

        /// <summary>
        /// Writes a serializable object to a file.
        /// </summary>
        /// <param name="serializableObject">The object to be serialized to a file.</param>
        /// <param name="xmlNamespace">The XML namespace to be used.</param>
        /// <param name="outputPath">The path of the file to be written.</param>
        public static void ToFile(Object serializableObject, string xmlNamespace, string outputPath)
        {
            Type serializableObjectType = null;
            XmlSerializer serializer = null;
            try
            {
                serializableObjectType = serializableObject.GetType();
                serializer = GetXmlSerializer(serializableObjectType, xmlNamespace);
            }
            catch (Exception exception)
            {
                throw new Exception("Unable to serialize object of type " + serializableObject.GetType().Name + " to file " + outputPath + ". ", exception);
            }
            ToFile(serializableObject, serializer, outputPath);
        }

        /// <summary>
        /// Writes a serializable object to a file.
        /// </summary>
        /// <param name="serializableObject">The object to be serialized to a file.</param>
        /// <param name="serializer">The XmlSerializer to be used for serialization.</param>
        /// <param name="outputPath">The path of the file to be written.</param>
        public static void ToFile(Object serializableObject, XmlSerializer serializer, string outputPath)
        {
            FileStream stream = null;
            try
            {
                stream = new FileStream(outputPath, FileMode.Create);
                serializer.Serialize(stream, serializableObject);
            }
            catch (Exception exception)
            {
                throw new Exception("Unable to serialize object of type " + serializableObject.GetType().Name + " to file " + outputPath + ". ", exception);
            }
            finally
            {
                if (stream != null)
                {
                    stream.Close();
                    stream = null;
                }
            }
        }

        /// <summary>
        /// Reads a serializable object from a file using an XmlSerializer.
        /// </summary>
        /// <param name="serializableObject">The serializable object to be read from the file. This argument is only used to get type information about the object that will be deserialized.</param>
        /// <param name="inputPath">The path of the file to be read.</param>
        /// <returns>The object deserialized from the file. This object will have the same type as the serializableObject that was passed to the method.</returns>
        public static object FromFile(Type serializableObjectType, string inputPath)
        {
            object serializableObject = null;
            XmlSerializer serializer = null;
            // Deserialize the object.
            try
            {
                serializer = new XmlSerializer(serializableObjectType);
                serializableObject = FromFile(serializer, inputPath);
            }
            catch (Exception e)
            {
                throw new Exception("Error deserializing '" + serializableObjectType.Name + "' from file '" + inputPath + "'. " + e.Message);
            }
            return serializableObject;
        }

        /// <summary>
        /// Reads a serializable object from a file using an XmlSerializer.
        /// </summary>
        /// <param name="serializableObject">The serializable object to be read from the file. This argument is only used to get type information about the object that will be deserialized.</param>
        /// <param name="inputPath">The path of the file to be read.</param>
        /// <returns>The object deserialized from the file. This object will have the same type as the serializableObject that was passed to the method.</returns>
        public static object FromFile(Type serializableObjectType, string xmlNamespace, string inputPath)
        {
            object serializableObject = null;
            XmlSerializer serializer = null;
            // Deserialize the object.
            try
            {
                serializer = new XmlSerializer(serializableObjectType, xmlNamespace);
                serializableObject = FromFile(serializer, inputPath);
            }
            catch (Exception e)
            {
                throw new Exception("Error deserializing '" + serializableObjectType.Name + "' from file '" + inputPath + "'. " + e.Message);
            }
            return serializableObject;
        }

        /// <summary>
        /// Reads a serializable object from a file using the given XmlSerializer.
        /// </summary>
        /// <param name="serializer">The XmlSerializer to be used for serialization.</param>
        /// <param name="inputPath">The path of the file to be read.</param>
        /// <returns>The object deserialized from the file.</returns>
        public static object FromFile(XmlSerializer serializer, string inputPath)
        {
            object serializableObject = null;
            FileStream stream = null;
            // Open the file stream.
            try
            {
                stream = new FileStream(inputPath, FileMode.Open);
            }
            catch (Exception e)
            {
                throw new Exception("Error creating file stream for '" + inputPath + "' to deserialize object. " + e.Message);
            }
            // Deserialize the object.
            try
            {
                serializableObject = serializer.Deserialize(stream);
            }
            catch (Exception e)
            {
                throw new Exception("Error deserializing file '" + inputPath + "'. " + e.Message);
            }
            finally
            {
                if (stream != null)
                {
                    stream.Close();
                    stream = null;
                }
            }
            return serializableObject;
        }
    }
}
