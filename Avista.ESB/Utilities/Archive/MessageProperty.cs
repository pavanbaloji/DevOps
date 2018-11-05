using System;
using System.Xml.Serialization;

namespace Avista.ESB.Utilities.Archive
{
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="http://schemas.microsoft.com/Sql/2008/05/Types/Tables/dbo")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace="http://schemas.microsoft.com/Sql/2008/05/Types/Tables/dbo", IsNullable=false)]
    public class MessageProperty
    {       
        public Guid MessageId { get; set; }
        public string ContextData { get; set; }
    }
}
