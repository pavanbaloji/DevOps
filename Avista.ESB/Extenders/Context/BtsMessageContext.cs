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

namespace Avista.ESB.Extenders.Context
{
    [ObjectExtender(typeof(Resolver)), CLSCompliant(false)]
    [Serializable]
    public class BtsMessageContext : ObjectExtender<Resolver>
    {
        [Browsable(false), Category("Extender Settings"), Description("Specifies the resolver version."), DisplayName("IncludeAll"), ReadOnly(true), XmlElement]
        public bool IncludeAll
        {
            get
            {
                return true;
            }
        }
    }
}
