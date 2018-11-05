using System;
using System.Xml.Serialization;

namespace Avista.ESB.Utilities.Archive
{
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://schemas.microsoft.com/Sql/2008/05/Types/Tables/dbo")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://schemas.microsoft.com/Sql/2008/05/Types/Tables/dbo", IsNullable = false)]
    public class Message
    {
        public Guid MessageId { get; set; }
        public string InterchangeId { get; set; }
        public string MessageType { get; set; }
        public string ActivityId { get; set; }
        public string Tag { get; set; }
        public DateTime? InsertedDate { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public int? ArchiveTypeId { get; set; }
        public int? SourceSystemId { get; set; }
        public int? TargetSystemId { get; set; }
        public string Description { get; set; }

    }
}
