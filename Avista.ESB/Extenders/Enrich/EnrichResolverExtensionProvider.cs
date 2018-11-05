using System;
using Microsoft.Practices.Modeling.ExtensionProvider.Extension;
using Microsoft.Practices.Modeling.ExtensionProvider.Metadata;
using Microsoft.Practices.Services.ItineraryDsl;

namespace Avista.ESB.Extenders.Enrich
{
    [ResolverExtensionProvider]
    [ExtensionProvider("26462D8B-ED6A-492D-945F-DB501D2CAA86", "Enrich", "Enrichment Resolver Extension", typeof(ItineraryDslDomainModel))]
    public class EnrichResolverExtensionProvider : ExtensionProviderBase
    {
        public EnrichResolverExtensionProvider()
            : base(new Type[] { typeof(EnrichResolverExtender) })
        {
        }
    }
}
