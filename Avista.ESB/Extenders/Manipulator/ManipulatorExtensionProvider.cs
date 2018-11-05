using Microsoft.Practices.Modeling.ExtensionProvider.Extension;
using Microsoft.Practices.Modeling.ExtensionProvider.Metadata;
using Microsoft.Practices.Services.ItineraryDsl;
using System;

namespace Avista.ESB.Extenders.Manipulator
{
    [ResolverExtensionProvider,
    ExtensionProvider("FAAEBFF2-B8B7-4454-818E-E31249ABD210", "Manipulator", "Manipulator Resolver Extension", typeof(ItineraryDslDomainModel))]
    public class ManipulatorExtensionProvider : ExtensionProviderBase
    {

        public ManipulatorExtensionProvider()
            : base(new Type[] { typeof(ManipulatorExtender) })
        {
        }


    }
}
