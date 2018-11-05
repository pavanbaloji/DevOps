using Microsoft.Practices.Modeling.ExtensionProvider.Extension;
using Microsoft.Practices.Modeling.ExtensionProvider.Metadata;
using Microsoft.Practices.Services.ItineraryDsl;
using System;

namespace Avista.ESB.Extenders.Cache
{
    [ResolverExtensionProvider,
    ExtensionProvider("F519674F-6205-4E61-952C-DE192D6B4B68", "Cache", "Cache Resolver Extension", typeof(ItineraryDslDomainModel))]
    public class CacheExtensionProvider : ExtensionProviderBase
    {

        public CacheExtensionProvider()
            : base(new Type[] { typeof(CacheExtender) })
        {
        }


    }
}
