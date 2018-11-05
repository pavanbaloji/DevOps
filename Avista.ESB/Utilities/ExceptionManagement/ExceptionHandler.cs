using Avista.ESB.DataAccess;
using Avista.ESB.Utilities.Logging;
using Microsoft.BizTalk.XLANGs.BTXEngine;
using Microsoft.Practices.ESB.ExceptionHandling;
using Microsoft.XLANGs.BaseTypes;
using Microsoft.XLANGs.Core;
using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security;
using System.Xml;

namespace Avista.ESB.Utilities.ExceptionManagement
{
    [Serializable]
    public class ExceptionHandler
    {
        /// <summary>
        /// Holds different fault types
        /// </summary>
        public enum FaultType
        {
            SoapException,
            DeliveryException,
            SystemException,
        }

        /// <summary>
        /// Holds Xlang Service Info
        /// </summary>
        private class XlangServiceInfo
        {
            public string ServiceName { get; set; }
            public string ServiceInstanceID { get; set; }
        }

        /// <summary>
        /// Handle fault
        /// </summary>
        /// <param name="xlangMsg"></param>
        /// <param name="faultType"></param>
        public static void HandleFault(XLANGMessage xlangMsg, string btsMsgType, FaultType faultType)
        {
            string eventMessage = string.Empty;
            string serviceName = string.Empty;
            string serviceType = string.Empty;
            string transportLocation = string.Empty;
            string transportDirection = string.Empty;
            string description = string.Empty;
            Stream faultStream = null; ;
            try
            {
                faultStream = (Stream)xlangMsg[0].RetrieveAs(typeof(Stream));
               
                if(faultType == FaultType.DeliveryException)
                {
                    serviceName = GetMsgProperty(xlangMsg, typeof(ErrorReport.SendPortName));
                    description = GetMsgProperty(xlangMsg, typeof(ErrorReport.Description));
                    if (!string.IsNullOrEmpty(serviceName))
                    {
                        serviceType = "Send";
                        transportLocation = GetMsgProperty(xlangMsg, typeof(ErrorReport.OutboundTransportLocation)).ToString();
                        transportDirection = "Outbound";
                    }
                    else
                    {
                        serviceName = GetMsgProperty(xlangMsg, typeof(ErrorReport.ReceivePortName));
                        serviceType = "Receive";
                        transportLocation = GetMsgProperty(xlangMsg, typeof(ErrorReport.InboundTransportLocation));
                        transportDirection = "Inbound";
                    }

                    eventMessage = String.Format("A message of type '{0}' could not be routed by BizTalk.", btsMsgType) + Environment.NewLine +
                                   String.Format("{0} Port = {1}", serviceType, serviceName) + Environment.NewLine +
                                   String.Format("{0} Transport Location = {1}", transportDirection, transportLocation) + Environment.NewLine +
                                   description;
                }
                else if(faultType == FaultType.SoapException)
                {
                    serviceName = GetMsgProperty(xlangMsg, typeof(BTS.SPName));
                    transportLocation = GetMsgProperty(xlangMsg, typeof(BTS.InboundTransportLocation));

                    eventMessage = "A SOAP Fault message was encountered by BizTalk." + Environment.NewLine +
                                   String.Format("Send Port = {0}", serviceName) + Environment.NewLine +
                                   String.Format("Outbound Transport Location = {0}", transportLocation) + Environment.NewLine + Environment.NewLine +
                                   "Refer 'FaultMsgBody' node in the archived message for more details.";
                }
                else
                {
                    serviceName = GetMsgProperty(xlangMsg, typeof(Microsoft.BizTalk.XLANGs.BTXEngine.SendingOrchestrationType));
                    serviceName = serviceName.Contains(",") ? serviceName.Substring(0, serviceName.IndexOf(",")) : serviceName;

                    eventMessage = "A System Exception was encountered by BizTalk " + Environment.NewLine +
                                   String.Format("Orchestration Name = {0}", serviceName) + Environment.NewLine + Environment.NewLine +
                                   "Refer 'FaultMsgBody' node in the archived message for more details.";
                }

                eventMessage += Environment.NewLine + Environment.NewLine + "================= Archived Message =================" + Environment.NewLine + Environment.NewLine;

                StreamReader reader = new StreamReader(faultStream);
                eventMessage += reader.ReadToEnd() + Environment.NewLine;

                eventMessage += Environment.NewLine + "================= Context Properties =================" + Environment.NewLine;

                foreach (DictionaryEntry dictionary in GetContext(xlangMsg))
                {
                    XmlQName qName = (XmlQName)dictionary.Key;
                    eventMessage += qName.Namespace + "#" + qName.Name + " : " + dictionary.Value.ToString() + Environment.NewLine;
                }

                EsbFaultEvent faultEvent = ExceptionEventDetails.Instance.ExceptionEvents.FirstOrDefault<EsbFaultEvent>
                                            (e => e.FaultSvcName.Equals(serviceName, StringComparison.OrdinalIgnoreCase)
                                                  && e.FaultType.Equals(faultType.ToString(), StringComparison.OrdinalIgnoreCase));

                if (faultEvent != null)
                    Logger.WriteEvent(faultEvent.EventSource, eventMessage, (EventLogEntryType)Enum.Parse(typeof(EventLogEntryType), faultEvent.EventType), faultEvent.EventId);
                else
                    throw new Exception(string.Format("'{0}' fault is not configured for the service '{1}' in [AvistaESBLookup].[dbo].[EsbFaultEvents] table", faultType.ToString(), serviceName));
            }
            catch (Exception ex)
            {
                eventMessage = "A Fault was encountered by BizTalk and a further error occurred while handling the message. " + Environment.NewLine +"Error Details: "+ ex.ToString()
                    + Environment.NewLine + Environment.NewLine + "Original Fault Details: "+ Environment.NewLine +eventMessage;

                Logger.WriteError(eventMessage, 14004);
            }
            finally
            {
                if (xlangMsg != null)
                {
                    xlangMsg.Dispose();
                    xlangMsg = null;
                }
                if (faultStream != null)
                {
                    faultStream = null;
                }
            }
        }

        /// <summary>
        /// Create fault message with Exception object and request xlang message
        /// </summary>
        /// <param name="exception"></param>
        /// <param name="requestMsg"></param>
        /// <param name="faultType"></param>
        /// <returns></returns>
        public static XLANGMessage CreateFaultMessage(Exception exception, XLANGMessage requestMsg, FaultType faultType)
        {
            string faultXml = SerializeExceptionObject(exception);
            string reqXml = string.Empty;
            try
            {
                Stream reqStream = requestMsg != null ? ((Stream)requestMsg[0].RetrieveAs(typeof(Stream))) : null;
                using (StreamReader reader = new StreamReader(reqStream))
                {
                    reqXml = reader.ReadToEnd();
                }
            }
            catch (Exception) { }

            XLANGMessage result = GetFaultMessage(faultXml, reqXml, faultType);

            return result;
        }

        /// <summary>
        /// Create fault message with fault xlang message and request xlang message
        /// </summary>
        /// <param name="faultMsg"></param>
        /// <param name="requestMsg"></param>
        /// <param name="faultType"></param>
        /// <returns></returns>
        public static XLANGMessage CreateFaultMessage(XLANGMessage faultMsg, XLANGMessage requestMsg, FaultType faultType)
        {
            string faultXml = faultMsg != null ? ((XmlDocument)faultMsg[0].RetrieveAs(typeof(XmlDocument))).OuterXml.ToString() : string.Empty;
            string reqXml = requestMsg != null ? ((XmlDocument)requestMsg[0].RetrieveAs(typeof(XmlDocument))).OuterXml.ToString() : string.Empty;

            XLANGMessage result = GetFaultMessage(faultXml, reqXml, faultType);

            object obj = GetMsgProperty(faultMsg, typeof(BTS.SPName));
            if (obj != null)
            {
                SetMsgProperty(result, typeof(BTS.SPName), obj.ToString());
            }

            obj = GetMsgProperty(faultMsg, typeof(BTS.InboundTransportLocation));
            if (obj != null)
            {
                SetMsgProperty(result, typeof(BTS.InboundTransportLocation), obj.ToString());
            }

            return result;
        }

        /// <summary>
        /// Builds Fault Message
        /// </summary>
        /// <param name="faultXml"></param>
        /// <param name="requestXml"></param>
        /// <param name="faultType"></param>
        /// <returns></returns>
        private static XLANGMessage GetFaultMessage(string faultXml, string requestXml, FaultType faultType)
        {
            XmlDocument xmlDocument = new XmlDocument();
            string appName = string.Empty;
            XlangServiceInfo serviceXlangInfo;
            BizTalkServerRegistry bizTalkServerRegistry = default(BizTalkServerRegistry);
            XLANGMessage result;

            string xmlSafe = System.Net.WebUtility.HtmlEncode(requestXml);
            if (xmlSafe != requestXml)
            {
                requestXml = "Contents have been HTML Encoded: " + xmlSafe;
            }

            try
            {
                serviceXlangInfo = GetServiceXlangInfo();
                bizTalkServerRegistry = Utility.GetMgmtServerInfo();

                appName = Utility.GetApplication(Utility.ServiceType.Orchestration, bizTalkServerRegistry.BizTalkMgmtDb, bizTalkServerRegistry.BizTalkMgmtDbName, Context.RootService.ServiceId.ToString(), string.Empty);
                xmlDocument.LoadXml(GetMessage(new object[]
				{
					appName,
                    serviceXlangInfo.ServiceInstanceID,
					serviceXlangInfo.ServiceName,
                    faultType,
                    "", //ArchiveTag for future use
                    "", //FaultEventId  for future use
					"", //FaultDetails  for any additional info
                    "", //FaultSeverity  for future use
                    faultXml,
                    requestXml
				}));

                XmlDocumentSerializationProxy xmlDocumentSerializationProxy = (XmlDocumentSerializationProxy)xmlDocument;
                BTXMessage bTXMessage = new AvistaEsbInternalMsg("FaultMessage", Service.RootService.XlangStore.OwningContext);
                bTXMessage.AddPart("", "Body");
                bTXMessage[0].LoadFrom(xmlDocumentSerializationProxy.UnderlyingXmlDocument);

                SetMsgProperty(bTXMessage, typeof(ErrorReport.ErrorType), faultType.ToString());

                XLANGMessage messageWrapperForUserCode = bTXMessage.GetMessageWrapperForUserCode();
                result = messageWrapperForUserCode;
            }
            catch (Exception ex)
            {
                Logger.WriteWarning(ex.ToString(), 14000);
                throw ex;
            }
            finally
            {
                if (xmlDocument != null)
                {
                    xmlDocument = null;
                }
            }
            return result;
        }

        /// <summary>
        /// Gets template filled FaultEnvelope message
        /// </summary>
        /// <param name="arguments"></param>
        /// <returns></returns>
        private static string GetMessage(params object[] arguments)
        {
            string template = "<ns0:FaultEnvelope xmlns:ns0='http://avista.esb.common.schemas/exceptionhandling'>"               
                                    +"<Application>{0}</Application>"
                                    +"<ServiceInstanceID>{1}</ServiceInstanceID>"
                                    +"<ServiceName>{2}</ServiceName>"
                                    +"<ErrorType>{3}</ErrorType>"
                                    +"<ArchiveTag>{4}</ArchiveTag>"
                                    +"<FaultEventId>{5}</FaultEventId>"
                                    +"<FaultDescription>{6}</FaultDescription>"
                                    +"<FaultSeverity>{7}</FaultSeverity>"
                                    +"<FaultMsgBody>{8}</FaultMsgBody>"
                                    +"<ReqMsgBody>{9}</ReqMsgBody>"
                          +"</ns0:FaultEnvelope>";

            return string.Format(template,arguments);
        }

        /// <summary>
        /// Gets Serialized Exception 
        /// </summary>
        /// <param name="exObj"></param>
        /// <returns></returns>
        private static string SerializeExceptionObject(Exception exObj)
        {
            string template = "<Fault><Message>{0}</Message><Source>{1}</Source><StackTrace>{2}</StackTrace><InnerException>{3}</InnerException></Fault>";

            if (exObj != null)
            {
                return string.Format(template, SecurityElement.Escape(exObj.Message), exObj.Source, SecurityElement.Escape(exObj.StackTrace), exObj.InnerException == null ? "" : SecurityElement.Escape(exObj.InnerException.ToString()));
            }
            else
            {
                return string.Format(template, "", "", "", "");
            }
        }

        /// <summary>
        /// Gets all message context properties assocciated with xlang message
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        private static Hashtable GetContext(XLANGMessage message)
        {
            try
            {
                foreach (Segment segment in Service.RootService._segments)
                {
                    IDictionary fields = Context.FindFields(typeof(XLANGMessage), segment.ExceptionContext);

                    foreach (DictionaryEntry field in fields)
                    {
                        XMessage msg = (field.Value as XMessage);
                        if (msg == null)
                            continue;

                        if (String.Compare(msg.Name, message.Name) != 0)
                            continue;

                        return msg.GetContextProperties();
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.WriteWarning(ex.ToString(), 14000);
            }
            return new Hashtable();
        }

        /// <summary>
        /// Ges Xlang service info
        /// </summary>
        /// <returns></returns>
        private static XlangServiceInfo GetServiceXlangInfo()
        {
            XlangServiceInfo xlangServiceInfo = new XlangServiceInfo();

            try
            {
                xlangServiceInfo.ServiceName = Context.RootService.FullName;
                xlangServiceInfo.ServiceInstanceID = Context.RootService.InstanceId.ToString();
            }
            catch (Exception ex)
            {
                Logger.WriteWarning(ex.ToString(), 14000);
                throw ex;
            }
            return xlangServiceInfo;
        }

        /// <summary>
        /// Sets XLANG message context property
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="prop"></param>
        /// <param name="value"></param>
        private static void SetMsgProperty(XLANGMessage msg, Type prop, object value)
        {
            if (msg == null)
            {
                throw new System.ArgumentNullException("msg");
            }
            if (null == prop)
            {
                throw new System.ArgumentNullException("prop");
            }
            if (value == null)
            {
                throw new System.ArgumentNullException("value");
            }
            try
            {
                msg.SetPropertyValue(prop, value);
            }
            catch (InvalidPropertyTypeException ex)
            {
                Logger.WriteWarning(ex.ToString(), 14000);
            }
            catch (Microsoft.XLANGs.Core.PropertyUpdateDisallowedException ex)
            {
                Logger.WriteWarning(ex.ToString(), 14000);
            }
            catch (Exception ex)
            {
                Logger.WriteError(ex.ToString(), 14000);
                throw ex;
            }
        }

        /// <summary>
        /// Gets XLANG message context property
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="prop"></param>
        /// <returns></returns>
        private static string GetMsgProperty(XLANGMessage msg, Type prop)
        {
            if (msg == null)
            {
                throw new System.ArgumentNullException("msg");
            }
            if (null == prop)
            {
                throw new System.ArgumentNullException("prop");
            }
            object obj = null;
            string result = string.Empty;
            try
            {
                obj = msg.GetPropertyValue(prop);
                if(obj != null)
                {
                    result = obj.ToString();
                }
            }
            catch (InvalidPropertyTypeException ex)
            {
                Logger.WriteWarning(ex.ToString(), 14000);
            }
            catch (Exception ex)
            {
                Logger.WriteError(ex.ToString(), 14000);
                throw ex;
            }
            finally
            {
                if (obj != null)
                {
                    obj = null;
                }
            }
            return result;
        }
 
    }
}
