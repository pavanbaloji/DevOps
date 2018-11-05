using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Avista.ESB.Utilities
{
    [System.Serializable]
    public class XmlDocumentSerializationProxy : System.Runtime.Serialization.ISerializable
    {
        private XmlDocument _doc;

        internal XmlDocument UnderlyingXmlDocument
        {
            get
            {
                return this._doc;
            }
            set
            {
                this._doc = value;
            }
        }

        internal XmlDocumentSerializationProxy()
        {
        }

        internal XmlDocumentSerializationProxy(XmlDocument doc)
        {
            this._doc = doc;
        }

        public static explicit operator XmlDocumentSerializationProxy(XmlDocument doc)
        {
            return new XmlDocumentSerializationProxy(doc);
        }

        internal XmlDocumentSerializationProxy(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext c)
        {
            if (info.GetBoolean("isNull"))
            {
                return;
            }
            string @string = info.GetString("typeName");
            string string2 = info.GetString("val");
            System.Type type = System.Type.GetType(@string);
            XmlDocument xmlDocument = (XmlDocument)System.Activator.CreateInstance(type);
            if (string2.Trim().Length > 0)
            {
                xmlDocument.LoadXml(string2);
            }
            this._doc = xmlDocument;
        }

        [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.LinkDemand, SerializationFormatter = true)]
        public void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
        {
            if (this._doc == null)
            {
                info.AddValue("isNull", true);
                return;
            }
            info.AddValue("isNull", false);
            string assemblyQualifiedName = this._doc.GetType().AssemblyQualifiedName;
            string outerXml = this._doc.OuterXml;
            info.AddValue("typeName", assemblyQualifiedName);
            info.AddValue("val", outerXml);
        }
    }
}
