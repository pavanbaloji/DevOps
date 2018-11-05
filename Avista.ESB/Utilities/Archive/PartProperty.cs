using System;
using System.Xml.Serialization;

namespace Avista.ESB.Utilities.Archive
{
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="http://schemas.microsoft.com/Sql/2008/05/Types/Tables/dbo")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace="http://schemas.microsoft.com/Sql/2008/05/Types/Tables/dbo", IsNullable=false)]
    public class PartProperty 
    {      
        public Guid PartId { get; set; }
        public int? PropertyIndex { get; set; }
        public string Namespace { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
    }
}
