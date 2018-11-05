using System;
using Microsoft.Practices.Modeling.ExtensionProvider.Extension;
using Microsoft.Practices.Modeling.ExtensionProvider.Metadata;
using Microsoft.Practices.Services.ItineraryDsl;

namespace Avista.ESB.Extenders.Archive
{
   [ResolverExtensionProvider]
   [ExtensionProvider("C673A9F4-1520-402C-BE46-14B565916588", "Archive", "Archive Resolver Extension", typeof(ItineraryDslDomainModel))]
   public class ArchiveResolverExtensionProvider : ExtensionProviderBase
    {
      public ArchiveResolverExtensionProvider()
         : base(new Type[] { typeof(ArchiveResolverExtender) })
        {
        }
    }
}
