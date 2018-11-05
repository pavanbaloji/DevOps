using Microsoft.Practices.Modeling.Common.Design;
using Microsoft.Practices.Modeling.ExtensionProvider.Extension;
using Microsoft.Practices.Modeling.ExtensionProvider.Metadata;
using System;
using System.ComponentModel;
using System.Drawing.Design;

namespace Avista.ESB.Extenders.Manipulator
{

    [ObjectExtender(typeof(Microsoft.Practices.Services.ItineraryDsl.Resolver))]
    [Serializable]
    public class ManipulatorExtender : ObjectExtender<Microsoft.Practices.Services.ItineraryDsl.Resolver>
    {

        [EditorOutputProperty("ReadItem", "ReadItem"), Browsable(true), Category("Manipulator Read Settings"), Description("Specify from where the data should be retrived."), DisplayName("Read Action"), Editor(typeof(ManipulatorActionEditor), typeof(UITypeEditor)), ReadOnly(false), TypeConverter(typeof(TypeConverter))]
        public string ReadFrom
        {
            get;
            set;
        }

        [EditorInputProperty("ReadFrom", "ReadFrom"), Browsable(true), Category("Manipulator Read Settings"), Description("Specify the path or key name to retrive data."), DisplayName("Read Path or Key"), ReadOnly(false), Editor(typeof(ManipulatorDataKeyEditor), typeof(UITypeEditor))]
        public string ReadItem
        {
            get;
            set;
        }

        [EditorOutputProperty("WriteItem", "WriteItem"), Browsable(true), Category("Manipulator Write Settings"), Description("Specify write action."), DisplayName("Write Action"), Editor(typeof(ManipulatorActionEditor), typeof(UITypeEditor)), ReadOnly(false), TypeConverter(typeof(TypeConverter))]
        public string WriteTo
        {
            get;
            set;
        }


        [EditorInputProperty("WriteTo", "WriteTo"), Browsable(true), Category("Manipulator Write Settings"), Description("Specify the path or key name to write data."), DisplayName("Write Path or Key"), ReadOnly(false), Editor(typeof(ManipulatorDataKeyEditor), typeof(UITypeEditor))]
        public string WriteItem
        {
            get;
            set;
        }
    }
}
