using Microsoft.Practices.Modeling.Common.Design;
using Microsoft.Practices.Modeling.ExtensionProvider.Extension;
using Microsoft.Practices.Modeling.ExtensionProvider.Metadata;
using Microsoft.Practices.Modeling.Services.Design;
using Microsoft.Practices.Services.ItineraryDsl;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Avista.ESB.Extenders.Transform
{

    [ObjectExtender(typeof(Microsoft.Practices.Services.ItineraryDsl.Resolver))]
    [Serializable]
    public class TransformExtender : ObjectExtender<Microsoft.Practices.Services.ItineraryDsl.Resolver>
    {
        [Browsable(true), Category("Extender Settings"), Description("Specifies the transform name."), DisplayName("Transform Name"), ReadOnly(false)]
        public string TransformName
        {
            get;
            set;
        }

        [Browsable(true), Category("Extender Settings"), Description("Specifies the transform type."), DisplayName("Transform Type"), Editor(typeof(BiztalkTransformTypeEditor), typeof(UITypeEditor)), ReadOnly(false), TypeConverter(typeof(TypeConverter))]
        public string TransformType
        {
            get;
            set;
        }
    }
}
