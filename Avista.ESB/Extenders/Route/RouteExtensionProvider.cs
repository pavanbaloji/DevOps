using Microsoft.Practices.Modeling.ExtensionProvider.Extension;
using Microsoft.Practices.Modeling.ExtensionProvider.Metadata;
using Microsoft.Practices.Services.ItineraryDsl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Avista.ESB.Extenders.Route
{
    [ResolverExtensionProvider,
    ExtensionProvider("870900CE-6B4B-42BE-B238-BD2E560AAF83", "Route", "Route Resolver Extension", typeof(ItineraryDslDomainModel))]
    public class RouteExtensionProvider : ExtensionProviderBase
    {

        public RouteExtensionProvider()
            : base(new Type[] { typeof(RouteExtender) })
        {
        }


    }
}
