using Microsoft.Practices.Modeling.ExtensionProvider.Extension;
using Microsoft.Practices.Modeling.ExtensionProvider.Metadata;
using Microsoft.Practices.Services.ItineraryDsl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Avista.ESB.Extenders.Content
{
    [ExtensionProvider("9E926CB9-9AD8-41BC-A2DE-6BB37941F337", "MessageContent", "BizTalk Message Content Resolver Extension", typeof(ItineraryDslDomainModel)), ResolverExtensionProvider]
    public class BtsMessageContentExtensionProvider : ExtensionProviderBase
    {
        public BtsMessageContentExtensionProvider()
            : base(new Type[]
		{
			typeof(BtsMessageContent)
		})
        {
        }
    }
}
