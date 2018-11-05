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

namespace Avista.ESB.Extenders.Route
{

    [ObjectExtender(typeof(Microsoft.Practices.Services.ItineraryDsl.Resolver))]
    [Serializable]
    public class RouteExtender : ObjectExtender<Microsoft.Practices.Services.ItineraryDsl.Resolver>
    {
        [EditorOutputProperty("SendPort", "SendPort", new string[]
		{

		}), Browsable(true), Category("Extender Settings"), Description("Specifies the BizTalk application name."), DisplayName("Biztalk Application"), Editor(typeof(BiztalkApplicationEditor), typeof(UITypeEditor)), ReadOnly(false), TypeConverter(typeof(TypeConverter))]
        public string BiztalkApplication
        {
            get;
            set;
        }

        [EditorInputProperty("BiztalkApplication", "BiztalkApplication"), EditorOutputProperty("ServiceName", "ServiceName", new string[]
		{
			"Microsoft.Practices.ESB.Itinerary.Schemas.ServiceName"
		}), EditorOutputProperty("ServiceType", "ServiceType", new string[]
		{
			"Microsoft.Practices.ESB.Itinerary.Schemas.ServiceType"
		}), EditorOutputProperty("ServiceState", "ServiceState", new string[]
		{
			"Microsoft.Practices.ESB.Itinerary.Schemas.ServiceState"
		}), EditorOutputProperty("IsRequestResponse", "IsRequestResponse", new string[]
		{
			"Microsoft.Practices.ESB.Itinerary.Schemas.IsRequestResponse"
		}), EditorOutputProperty("IsTwoWay", "IsTwoWay", new string[]
		{

		}), Browsable(true), Category("Extender Settings"), Description("Specifies the dynamic send port name."), DisplayName("Send Port"), Editor(typeof(Avista.ESB.Extenders.BiztalkSendPortEditor), typeof(UITypeEditor)), ReadOnly(false), TypeConverter(typeof(TypeConverter))]
        public string SendPort
        {
            get;
            set;
        }

        [Browsable(true), Category("Extender Settings"), Description("Specifies the type of itinerary service."), DisplayName("Service Type"), ReadOnly(true)]
        public string ServiceType
        {
            get;
            set;
        }

        [Browsable(true), Category("Extender Settings"), Description("Specifies the itinerary service name."), DisplayName("Service Name"), ReadOnly(true)]
        public string ServiceName
        {
            get;
            set;
        }

        [Browsable(true), Category("Extender Settings"), Description("Specifies the itinerary service state."), DisplayName("Service State"), ReadOnly(true)]
        public string ServiceState
        {
            get;
            set;
        }

        [Browsable(true), Category("Extender Settings"), Description(""), DisplayName("Is Request Response"), ReadOnly(true)]
        public bool IsRequestResponse
        {
            get;
            set;
        }

        [Browsable(false)]
        public bool IsTwoWay
        {
            get;
            set;
        }

        [Browsable(true), Category("Extender Settings"), Description("Specify whether to archive request and response messages"), DisplayName("ArchiveRequired"), ReadOnly(false)]
        public bool ArchiveRequired
        {
            get;
            set;
        }

        [Browsable(true), Category("Extender Settings"), Description("Specify tag name which can be used for archive the message"), DisplayName("ArchiveTagName"), ReadOnly(false)]
        public string ArchiveTagName
        {
            get;
            set;
        }


        [Browsable(true), Category("Extender Settings"), Description("Specify Action to sendport."), DisplayName("WcfAction"), ReadOnly(false)]
        public string WcfAction
        {
            get;
            set;
        }

        [Browsable(true), Category("Extender Settings"), Description("Specify soap fault code to use in response message."), DisplayName("SoapFaultCode"), ReadOnly(false)]
        public string SoapFaultCode
        {
            get;
            set;
        }

        [Browsable(true), Category("Extender Settings"), Description("Specify delivery failure code to use in response message."), DisplayName("DeliveryFailureCode"), ReadOnly(false)]
        public string DeliveryFailureCode
        {
            get;
            set;
        }


    }
}
