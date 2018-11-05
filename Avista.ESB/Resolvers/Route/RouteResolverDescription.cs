using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Avista.ESB.Resolvers.Route
{
    [XmlType(AnonymousType = true)]
    [Serializable]
    public class RouteResolverDescription
    {
        [XmlAttribute]
        public string ArchiveRequired { get; set; }

        [XmlAttribute]
        public string ArchiveTagName { get; set; }

        [XmlAttribute]
        public string WcfAction { get; set; }

        [XmlAttribute]
        public string BiztalkApplication { get; set; }

        [XmlAttribute]
        public string SendPort { get; set; }

        [XmlAttribute]
        public string ServiceType { get; set; }

        [XmlAttribute]
        public string ServiceName { get; set; }

        [XmlAttribute]
        public string ServiceState { get; set; }

        [XmlAttribute]
        public string RequestResponse { get; set; }

        [XmlAttribute]
        public string TwoWay { get; set; }

        [XmlAttribute]
        public string SoapFaultCode { get; set; }

        [XmlAttribute]
        public string DeliveryFailureCode { get; set; }
        
    }
}
