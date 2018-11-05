using Microsoft.Practices.Modeling.ExtensionProvider.Extension;
using Microsoft.Practices.Modeling.ExtensionProvider.Metadata;
using Microsoft.Practices.Services.ItineraryDsl;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Avista.ESB.Extenders.Content
{
    [ObjectExtender(typeof(Resolver)), CLSCompliant(false)]
    [Serializable]
    public class BtsMessageContent : ObjectExtender<Resolver>
    {
        [Browsable(false), Category("Extender Settings"), Description("Specifies the resolver version."), DisplayName("Include Full Message"), ReadOnly(true), XmlElement]
        public bool IncludeFullMsg
        {
            get
            {
                return true;
            }
        }
    }
}
