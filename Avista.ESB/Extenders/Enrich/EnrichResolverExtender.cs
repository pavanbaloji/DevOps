using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Xml;
using Microsoft.Practices.Modeling.ExtensionProvider.Extension;
using Microsoft.Practices.Modeling.ExtensionProvider.Metadata;
using Microsoft.Practices.Modeling.Services.Design;
using Microsoft.Practices.Services.ItineraryDsl;

namespace Avista.ESB.Extenders.Enrich
{
    [Serializable]
    [ObjectExtender(typeof(Resolver))]
    public class EnrichResolverExtender : ObjectExtender<Resolver>
    {
        private string _probe0 = "";
        private string _probe1 = "";
        private string _probe2 = "";
        private string _part0Source = "Pipeline";
        private string _part1Source = "Cache";
        private string _part2Source = "";
        private string _part3Source = "";
        private string _part4Source = "";
        private bool _preservePart0 = false;
        private bool _preservePart1 = false;
        private bool _preservePart2 = false;
        private bool _preservePart3 = false;
        private bool _preservePart4 = false;
        private string _transformType = "";
        private int _failureEventId = 333;
        private string _failureAction = "ThrowException";

        [Category(EnrichResolverExtensionProvider.ExtensionProviderPropertyCategory)]
        [Description("Optional. Specifies a probing expression to probe the messages being enriched.")]
        [DisplayName("Probe 0")]
        [ReadOnly(false)]
        [Browsable(true)]
        public string Probe0
        {
            get
            {
                return _probe0;
            }
            set
            {
                _probe0 = value;
            }
        }

        [Category(EnrichResolverExtensionProvider.ExtensionProviderPropertyCategory)]
        [Description("Optional. Specifies a probing expression to probe the messages being enriched.")]
        [DisplayName("Probe 1")]
        [ReadOnly(false)]
        [Browsable(true)]
        public string Probe1
        {
            get
            {
                return _probe1;
            }
            set
            {
                _probe1 = value;
            }
        }

        [Category(EnrichResolverExtensionProvider.ExtensionProviderPropertyCategory)]
        [Description("Optional. Specifies a probing expression to probe the messages being enriched.")]
        [DisplayName("Probe 2")]
        [ReadOnly(false)]
        [Browsable(true)]
        public string Probe2
        {
            get
            {
                return _probe2;
            }
            set
            {
                _probe2 = value;
            }
        }

        [Category(EnrichResolverExtensionProvider.ExtensionProviderPropertyCategory)]
        [Description("Specifies the source of part 0 used in the enrichment.")]
        [DisplayName("Part 0 Source")]
        [ReadOnly(false)]
        [Browsable(true)]
        public string Part0Source
        {
            get
            {
                return _part0Source;
            }
            set
            {
                _part0Source = value;
            }
        }

        [Category(EnrichResolverExtensionProvider.ExtensionProviderPropertyCategory)]
        [Description("Specifies the source of part 1 used in the enrichment.")]
        [DisplayName("Part 1 Source")]
        [ReadOnly(false)]
        [Browsable(true)]
        public string Part1Source
        {
            get
            {
                return _part1Source;
            }
            set
            {
                _part1Source = value;
            }
        }

        [Category(EnrichResolverExtensionProvider.ExtensionProviderPropertyCategory)]
        [Description("Specifies the source of part 2 used in the enrichment.")]
        [DisplayName("Part 2 Source")]
        [ReadOnly(false)]
        [Browsable(true)]
        public string Part2Source
        {
            get
            {
                return _part2Source;
            }
            set
            {
                _part2Source = value;
            }
        }

        [Category(EnrichResolverExtensionProvider.ExtensionProviderPropertyCategory)]
        [Description("Specifies the source of part 3 used in the enrichment.")]
        [DisplayName("Part 3 Source")]
        [ReadOnly(false)]
        [Browsable(true)]
        public string Part3Source
        {
            get
            {
                return _part3Source;
            }
            set
            {
                _part3Source = value;
            }
        }

        [Category(EnrichResolverExtensionProvider.ExtensionProviderPropertyCategory)]
        [Description("Specifies the source of part 4 used in the enrichment.")]
        [DisplayName("Part 4 Source")]
        [ReadOnly(false)]
        [Browsable(true)]
        public string Part4Source
        {
            get
            {
                return _part4Source;
            }
            set
            {
                _part4Source = value;
            }
        }

        [Category(EnrichResolverExtensionProvider.ExtensionProviderPropertyCategory)]
        [Description("Specifies whether or not the source of part 0 should be preserved after the message enrichment is complete.")]
        [DisplayName("Preserve Part 0")]
        [ReadOnly(false)]
        [Browsable(true)]
        public bool PreservePart0
        {
            get
            {
                return _preservePart0;
            }
            set
            {
                _preservePart0 = value;
            }
        }

        [Category(EnrichResolverExtensionProvider.ExtensionProviderPropertyCategory)]
        [Description("Specifies whether or not the source of part 1 should be preserved after the message enrichment is complete.")]
        [DisplayName("Preserve Part 1")]
        [ReadOnly(false)]
        [Browsable(true)]
        public bool PreservePart1
        {
            get
            {
                return _preservePart1;
            }
            set
            {
                _preservePart1 = value;
            }
        }

        [Category(EnrichResolverExtensionProvider.ExtensionProviderPropertyCategory)]
        [Description("Specifies whether or not the source of part 2 should be preserved after the message enrichment is complete.")]
        [DisplayName("Preserve Part 2")]
        [ReadOnly(false)]
        [Browsable(true)]
        public bool PreservePart2
        {
            get
            {
                return _preservePart2;
            }
            set
            {
                _preservePart2 = value;
            }
        }

        [Category(EnrichResolverExtensionProvider.ExtensionProviderPropertyCategory)]
        [Description("Specifies whether or not the source of part 3 should be preserved after the message enrichment is complete.")]
        [DisplayName("Preserve Part 3")]
        [ReadOnly(false)]
        [Browsable(true)]
        public bool PreservePart3
        {
            get
            {
                return _preservePart3;
            }
            set
            {
                _preservePart3 = value;
            }
        }

        [Category(EnrichResolverExtensionProvider.ExtensionProviderPropertyCategory)]
        [Description("Specifies whether or not the source of part 4 should be preserved after the message enrichment is complete.")]
        [DisplayName("Preserve Part 4")]
        [ReadOnly(false)]
        [Browsable(true)]
        public bool PreservePart4
        {
            get
            {
                return _preservePart4;
            }
            set
            {
                _preservePart4 = value;
            }
        }

        [Category(EnrichResolverExtensionProvider.ExtensionProviderPropertyCategory)]
        [Description("Specifies the type of the map used to perform the enrichment.")]
        [DisplayName("Transform Type")]
        [ReadOnly(false)]
        [Browsable(true)]
        [Editor(typeof(BiztalkTransformTypeEditor), typeof(UITypeEditor)), TypeConverter(typeof(TypeConverter))]
        public string TransformType
        {
            get
            {
                return _transformType;
            }
            set
            {
                _transformType = value;
            }
        }

        [Category(EnrichResolverExtensionProvider.ExtensionProviderPropertyCategory)]
        [Description("Specifies the event id that should be displayed in the event log if there is a failure performing the enrichment operation.")]
        [DisplayName("Failure Event Id")]
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

        [Category(EnrichResolverExtensionProvider.ExtensionProviderPropertyCategory)]
        [Description("Specifies the action to take when the enrichment fails. Possible values: ThrowException, SendException or HandleException.")]
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
