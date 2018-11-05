using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Avista.ESB.MessagingServices.Manipulator
{
    internal class ManipulatorDescription
    {
        public string ReadFrom { get; set; }
        public string ReadItem { get; set; }
        public string WriteTo { get; set; }
        public string WriteItem { get; set; }
        public string ManipulateValue { get; set; }
    }

    [Serializable]
    internal class XmlStructureDescription
    {
        public string RootNodeName { get; set; }
        public string Namespace { get; set; }
        public string NamespacePrefix { get; set; }
        public string ApplyNamespacePrefixTo { get; set; }
        public string Action { get; set; }
    }

    [Serializable]
    internal class MessageContextDescription
    {
        public string PropertyName { get; set; }
        public string PropertyNamespace { get; set; }
    }
}
