using Avista.ESB.Utilities;
using Avista.ESB.Utilities.Logging;
using Microsoft.BizTalk.Component.Interop;
using Microsoft.BizTalk.Message.Interop;
using Microsoft.BizTalk.Streaming;
using Microsoft.Practices.ESB.GlobalPropertyContext;
using Microsoft.Practices.ESB.Itinerary;
using Microsoft.Practices.ESB.Resolver;
using Microsoft.Practices.ESB.Utilities;
using Microsoft.XLANGs.BaseTypes;
using Microsoft.XLANGs.RuntimeTypes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Xml;
using System.Xml.XPath;

namespace Avista.ESB.MessagingServices.Archive
{
    public class ArchiveMessage : IMessagingService
    {
        [Browsable(true), ReadOnly(true)]
        public string Name
        {
            get
            {
                return "Avista.ESB.Utilities.ArchiveMessage";
            }
        }

        public bool SupportsDisassemble
        {
            get
            {
                return false;
            }
        }

        public bool ShouldAdvanceStep(IItineraryStep step, IBaseMessage msg)
        {
            return true;
        }

        public IBaseMessage Execute(IPipelineContext pipelineContext, IBaseMessage message, string resolverString, IItineraryStep step)
        {
            Logger.WriteTrace(string.Format("******{0} Started******", this.GetType().Name));
            Logger.WriteTrace("Resolver String: " + resolverString);

            if (string.IsNullOrEmpty(resolverString))
            {
                throw new ArgumentException("ResolverString is required.", "resolverString");
            }
            try
            {
                ResolverInfo info = ResolverMgr.GetResolverInfo(ResolutionType.Transform,resolverString);
                if (info.Success)
                {
                    Dictionary<string, string> dictionary = ResolverMgr.Resolve(info, message, pipelineContext);

                    // Archive the message.
                    ArchiveManager archiveManager = new ArchiveManager();

                    int expiryMinutes = Convert.ToInt32(dictionary["Archive.ExpiryMinutes"]);
                    bool includeProperties = Convert.ToBoolean(dictionary["Archive.IncludeProperties"]);
                    string failureEventId = dictionary["Archive.FailureEventId"];
                    string tag = dictionary["Archive.Tag"];
                    string failureAction = dictionary["Archive.FailureAction"];

                    string messageId = archiveManager.ArchiveMessage(pipelineContext, message, expiryMinutes, includeProperties, tag);
                    Logger.WriteTrace("Message Id: '" + messageId + "' has been archived.");

                    SetMetadata(message,tag, messageId);
                }
                else
                {
                    throw new Exception("Unable to get archive resolver information from the resolver string: "+resolverString);
                }
            }
            catch (Exception ex)
            {
                Logger.WriteTrace(string.Format("Error occured in {0} \r\n Details: {1}", this.GetType().Name, ex.ToString()));
                throw ex;
            }
            Logger.WriteTrace(string.Format("******{0} Completed******", this.GetType().Name));
            return message;
        }

        private void SetMetadata(IBaseMessage message, string tag, string messageId)
        {
            string strMetadata =string.Empty;
            XmlDocument metadata = new XmlDocument();
            try
            {
                strMetadata = (string)message.Context.Read("MessageMetadata", "http://www.avistacorp.com/schemas/Avista.ESB.Utilities/v1.0");
            }
            catch (Exception) { }
            try
            {
                if (string.IsNullOrEmpty(strMetadata))
                {
                    metadata.LoadXml("<Metadata></Metadata>");
                }
                else
                {
                    metadata.LoadXml(strMetadata);
                }

                // Construct a new 
                XmlElement dataElement = metadata.CreateElement("Data");
                dataElement.SetAttribute("category", "ArchiveMessageId");
                dataElement.SetAttribute("id", tag);
                dataElement.SetAttribute("type", "String");
                dataElement.InnerText = messageId;
                metadata.DocumentElement.AppendChild(dataElement);

                message.Context.Write("MessageMetadata", "http://www.avistacorp.com/schemas/Avista.ESB.Utilities/v1.0", metadata.OuterXml);
            }
            catch(Exception ex)
            {
                Logger.WriteTrace("Unable to write to the metadata after archiving messageId " + messageId + "Error Details: "+ ex.ToString());
            }

        }
    }
}
