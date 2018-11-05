using Microsoft.Practices.Modeling.Common.Design;
using Microsoft.Practices.Modeling.ExtensionProvider.Extension;
using Microsoft.Practices.Modeling.ExtensionProvider.Metadata;
using System;
using System.ComponentModel;
using System.Drawing.Design;

namespace Avista.ESB.Extenders.Cache
{

    [ObjectExtender(typeof(Microsoft.Practices.Services.ItineraryDsl.Resolver))]
    [Serializable]
    public class CacheExtender : ObjectExtender<Microsoft.Practices.Services.ItineraryDsl.Resolver>
    {
        private string cacheTimeOut = "60";

        [EditorOutputProperty("Action", "Action"), Browsable(true), Category("Cache Service Settings"), Description("Specify cache action."), DisplayName("Action"), Editor(typeof(CacheActionEditor), typeof(UITypeEditor)), ReadOnly(false), TypeConverter(typeof(TypeConverter))]
        public string Action
        {
            get;
            set;
        }

        [EditorInputProperty("CacheMessageName", "CacheMessageName"), Browsable(true), Category("Cache Service Settings"), Description("Specify cache message name. It should be unique in the integration."), DisplayName("Cache Message Name"), ReadOnly(false)]
        public string CacheMessageName
        {
            get;
            set;
        }

        [EditorInputProperty("CacheTimeOut", "CacheTimeOut"), Browsable(true), Category("Cache Service Settings"), Description("Specify cache time out in minutes. Default value is 60 minutes."), DisplayName("Cache Time Out"), ReadOnly(false)]
        public string CacheTimeOut
        {
            get
            {
                return cacheTimeOut;
            }
            set 
            {
                cacheTimeOut = value;
            }
        }
    }
}
