using System;
using System.ComponentModel;
using System.Xml;
using Microsoft.Practices.Modeling.ExtensionProvider.Extension;
using Microsoft.Practices.Modeling.ExtensionProvider.Metadata;
using Microsoft.Practices.Services.ItineraryDsl;
using System.Drawing.Design;

namespace Avista.ESB.Extenders.Archive
{
    [Serializable]
    [ObjectExtender(typeof(Resolver))]
    public class ArchiveResolverExtender : ObjectExtender<Resolver>
    {
        private int _expiryMinutes = 0;
        private bool _includeProperties = true;
        private int _failureEventId = 324;
        private string _tag = null;
        private string _failureAction = "HandleException";

        [Category(ArchiveResolverExtensionProvider.ExtensionProviderPropertyCategory)]
        [Description("Specifies the number of minutes before the message should expire.")]
        [DisplayName("ExpiryMinutes")]
        [ReadOnly(false)]
        [Browsable(true)]
        public int ExpiryMinutes
        {
            get
            {
                return _expiryMinutes;
            }
            set
            {
                _expiryMinutes = value;
            }
        }

        [Category(ArchiveResolverExtensionProvider.ExtensionProviderPropertyCategory)]
        [Description("Specifies whether or not properties should be archived along with the message.")]
        [DisplayName("IncludeProperties")]
        [ReadOnly(false)]
        [Browsable(true)]
        public bool IncludeProperties
        {
            get
            {
                return _includeProperties;
            }
            set
            {
                _includeProperties = value;
            }
        }

        [Category(ArchiveResolverExtensionProvider.ExtensionProviderPropertyCategory)]
        [Description("Specifies the event id that should be displayed in the event log if there is a failure performing the archive operation.")]
        [DisplayName("FailureEventId")]
        [ReadOnly(false)]
        [Browsable(true)]
        public int FailureEventId
        {
            get
            {
                return _failureEventId;
            }
            set
            {
                _failureEventId = value;
            }
        }

        [Category(ArchiveResolverExtensionProvider.ExtensionProviderPropertyCategory)]
        [Description("Specifies a custom tag that can be use to categorize and help identify the message within the message archive.")]
        [DisplayName("Tag")]
        [ReadOnly(false)]
        [Browsable(true)]
        public string Tag
        {
            get
            {
                return _tag;
            }
            set
            {
                _tag = value;
            }
        }

        [Category(ArchiveResolverExtensionProvider.ExtensionProviderPropertyCategory)]
        [Description("Specifies the action to take when message archiving fails. Possible values: ThrowException, SendException or HandleException.")]
        [DisplayName("Failure Action")]
        [ReadOnly(false)]
        [Browsable(true)]
        [Editor(typeof(FailureActionEditor), typeof(UITypeEditor))]
        [TypeConverter(typeof(TypeConverter))]
        public string FailureAction
        {
            get
            {
                return _failureAction;
            }
            set
            {
                _failureAction = value;
            }
        }
    }
}
