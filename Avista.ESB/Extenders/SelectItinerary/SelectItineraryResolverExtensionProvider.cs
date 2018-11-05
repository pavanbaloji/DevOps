using System;
using Microsoft.Practices.Modeling.ExtensionProvider.Extension;
using Microsoft.Practices.Modeling.ExtensionProvider.Metadata;
using Microsoft.Practices.Services.ItineraryDsl;

namespace Avista.ESB.Extenders.SelectItinerary
{
    [ResolverExtensionProvider]
    [ExtensionProvider("5BDF9A57-33F5-48C6-88AB-A0A5C2265B6A", "SelectItinerary", "SelectItinerary Resolver Extension", typeof(ItineraryDslDomainModel))]
    public class SelectItineraryResolverExtensionProvider : ExtensionProviderBase
    {
        public SelectItineraryResolverExtensionProvider()
            : base(new Type[] { typeof(SelectItineraryResolverExtender) })
        {
        }
    }
}
