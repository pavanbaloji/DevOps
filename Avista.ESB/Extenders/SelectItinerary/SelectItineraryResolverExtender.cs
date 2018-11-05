using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Xml;
using Microsoft.Practices.Modeling.ExtensionProvider.Extension;
using Microsoft.Practices.Modeling.ExtensionProvider.Metadata;
using Microsoft.Practices.Modeling.Services.Design;
using Microsoft.Practices.Services.ItineraryDsl;

namespace Avista.ESB.Extenders.SelectItinerary
{
    [Serializable]
    [ObjectExtender(typeof(Resolver))]
    public class SelectItineraryResolverExtender : ObjectExtender<Resolver>
    {
        [Category(SelectItineraryResolverExtensionProvider.ExtensionProviderPropertyCategory)]
        [Description("Specifies the itinerary name.")]
        [DisplayName("Itinerary")]
        [ReadOnly(false)]
        [Browsable(true)]
        public string Itinerary
        {
            get;
            set;
        }

        [Category(SelectItineraryResolverExtensionProvider.ExtensionProviderPropertyCategory)]
        [Description("Specifies whether or not to continue on failure of selecting one itinerary.")]
        [DisplayName("ContinueOnFailure")]
        [ReadOnly(false)]
        [Browsable(true)]
        public bool ContinueOnFailure
        {
            get;
            set;
        }
    }
}
