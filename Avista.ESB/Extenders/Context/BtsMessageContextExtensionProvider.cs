using Microsoft.Practices.Modeling.ExtensionProvider.Extension;
using Microsoft.Practices.Modeling.ExtensionProvider.Metadata;
using Microsoft.Practices.Services.ItineraryDsl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Avista.ESB.Extenders.Context
{
    [ExtensionProvider("97C730E2-25E1-4874-98D5-604F0D056690", "MessageContext", "BizTalk Message Context Resolver Extension", typeof(ItineraryDslDomainModel)), ResolverExtensionProvider]
    public class BtsMessageContextExtensionProvider : ExtensionProviderBase
    {
        public BtsMessageContextExtensionProvider()
            : base(new Type[]
		{
			typeof(BtsMessageContext)
		})
        {
        }
    }
}
