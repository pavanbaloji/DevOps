using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;

namespace Avista.ESB.Testing
{
      public static class JsonParser
      {
            public static object ConvertJsonToXmlDocument (string jsonString)
            {
                  object anyObject = JsonConvert.DeserializeXmlNode( jsonString );
                  return anyObject;
            }

            public static XmlDocument ConvertJsonToXmlDocument (string jsonString,string schemaNamespaceUri, string namespacePrefix)
            {
                  XmlDocument rawDoc = JsonConvert.DeserializeXmlNode( jsonString);
                  XmlDocument xmlDoc = new XmlDocument();
                  xmlDoc.AppendChild( xmlDoc.CreateElement( namespacePrefix, rawDoc.DocumentElement.LocalName, schemaNamespaceUri ) );
                  xmlDoc.DocumentElement.InnerXml = rawDoc.DocumentElement.InnerXml;
                  return xmlDoc;
            }


            public static XmlDocument ConvertJsonToXmlDocument (string jsonString, string schemaRootNode, string schemaNamespaceUri,string namespacePrefix)
            {
                 XmlDocument rawDoc = JsonConvert.DeserializeXmlNode( jsonString, schemaRootNode, true );
                 XmlDocument xmlDoc = new XmlDocument();
                  xmlDoc.AppendChild( xmlDoc.CreateElement( namespacePrefix, rawDoc.DocumentElement.LocalName, schemaNamespaceUri ) );
                  xmlDoc.DocumentElement.InnerXml = rawDoc.DocumentElement.InnerXml;
                  return xmlDoc;
            }


            public static XmlDocument ConvertJsonToXmlDocument(string jsonString, string schemaRootNode, string schemaNamespaceUri, string namespacePrefix, bool writeArrayAttribute)
            {
                XmlDocument rawDoc = JsonConvert.DeserializeXmlNode(jsonString, schemaRootNode, writeArrayAttribute);
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.AppendChild(xmlDoc.CreateElement(namespacePrefix, rawDoc.DocumentElement.LocalName, schemaNamespaceUri));
                xmlDoc.DocumentElement.InnerXml = rawDoc.DocumentElement.InnerXml;
                return xmlDoc;
            }
        
            public static JObject ConvertStreamToJson(Stream stream)
            {
                  JObject Jobject = null;
                  if ( stream != null )
                  {
                        using ( StreamReader responseStream = new StreamReader( stream ))
                        {
                              string jsonString = responseStream.ReadToEnd();
                              JObject jObject = JObject.Parse( jsonString );
                              Jobject = jObject;
                        }
                  }

                  return Jobject;
            }
      }
}
