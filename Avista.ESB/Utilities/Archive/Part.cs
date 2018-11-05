using System;
using System.Xml.Serialization;

namespace Avista.ESB.Utilities.Archive
{
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="http://schemas.microsoft.com/Sql/2008/05/Types/Tables/dbo")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace="http://schemas.microsoft.com/Sql/2008/05/Types/Tables/dbo", IsNullable=false)]
    public class Part 
    {       
        public Guid MessageId { get; set; }
        public Guid PartId { get; set; }
        public string PartName { get; set; }
        public int? PartIndex { get; set; }
        public string ContentType { get; set; }
        public string CharSet { get; set; }
        public string TextData { get; set; }
        public byte[] ImageData { get; set; }
    }
}
