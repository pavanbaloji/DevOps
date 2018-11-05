using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.XPath;
using System.IO;
using System.Text;
using Microsoft.Practices.ESB.Itinerary;
using Microsoft.BizTalk.Component.Interop;
using Microsoft.BizTalk.Message.Interop;
using Microsoft.Practices.ESB.GlobalPropertyContext;
using System.ComponentModel;
using Avista.ESB.Utilities.Logging;
using Microsoft.Practices.ESB.Resolver;
using Microsoft.XLANGs.RuntimeTypes;
using System.Data.SqlClient;
using System.Data;
using System.Reflection;
using Avista.ESB.Utilities.DataAccess;
using Avista.ESB.Utilities;


namespace Avista.ESB.MessagingServices.Enrich
{
    /// <summary>
    /// The EnrichMessage class implements an ESB messaging service for enriching a message by executing a map
    /// using an additional input source. Typically the additional source is a message archived using the archive
    /// service earlier in the saem itinerary. This allows data retrieved from a web service call to be added back
    /// to the message as it existed prior to the web service call.
    /// </summary>
    public class EnrichMessage : IMessagingService
    {

        private const int MAX_PROBES = 3;
        private const int MAX_PARTS = 5;
        private int failureEventId = 333;
        private string failureAction = "ThrowException";
        private string[] probe = new string[MAX_PROBES];
        private string[] partSource = new string[MAX_PARTS];
        private string[] partType = new string[MAX_PARTS];
        private XmlDocument[] partDoc = new XmlDocument[MAX_PARTS];
        private bool[] partPreserved = new bool[MAX_PARTS];
        private int partCount = 0;
        private string transformType = "";        
        private bool trace = false;
        private bool dumpAggregates = false;
        private string dumpFolder = "C:\\Windows\\Temp";
        private string messageType = "";
        private string source = "";
        private string contentType = "";
        private XmlDocument contentAsXmlDocument;
        private String contentAsString = null;
        private byte[] contentAsByteArray = null;
        private string text = String.Empty;
        private object[] args = null;
        private string instance = null;
        private static int extendedExpiryMinutes = 40000;

        /// <summary>
        /// The name of the messaging service as it will appear in an itinerary. This matches the
        /// name configured in the itinerary services section of the ESB configuration file. 
        /// </summary>
        [Browsable(true), ReadOnly(true)]
        public string Name
        {
            get { return "Avista.ESB.Utilities.EnrichMessage"; }
        }

        /// <summary>
        /// Indicates whether the custom messaging service supports disassemble and the execution of multiple resolvers.
        /// </summary>
        public bool SupportsDisassemble
        {
            get { return false; }
        }

        /// <summary>
        ///  Takes in the current itinerary step, and the current message, and returns a Boolean value that indicates whether
        ///  the dispatcher should advance the itinerary after the service executes. 
        /// </summary>
        /// <param name="step">The current itinerary step.</param>
        /// <param name="message">The current message.</param>
        /// <returns>True, indicating that the itinerary should always be advanced to the next step after executing the EnrichMessage service.</returns>
        public bool ShouldAdvanceStep(IItineraryStep step, IBaseMessage message)
        {
            return true;
        }

        /// <summary>
        /// This is the main processing method for the EnrichMessage messaging service.
        /// It delegates the work to a new instance of the <see cref="EnrichMessageHandler"/>.
        /// </summary>
        /// <param name="pipelineContext">The pipeline context.</param>
        /// <param name="message">The message.</param>
        /// <param name="resolverString">The resolver string.</param>
        /// <param name="step">The current step of the itinerary.</param>
        /// <returns>The enriched message.</returns>
        public IBaseMessage Execute(IPipelineContext pipelineContext, IBaseMessage message, string resolverString, IItineraryStep step)
        {
            IBaseMessage messageOut = null;
            bool probeMatched = false;
            try
            {
                // Loop through each resolver looking for a probe that matches.
                int resolverIndex = 0; // First resolver will be index 1.
                foreach (string resolver in step.ResolverCollection)
                {
                    if (resolver != null)
                    {
                        resolverIndex++;
                        Logger.WriteTrace("Loading resolver data for resolver #" + resolverIndex + ".");
                        InitializeResolverData();
                        LoadResolverData(pipelineContext, message, resolver);
                        probeMatched = Probe(pipelineContext, message);
                        if (probeMatched)
                        {
                            // Once a probe matches, load all the needed parts and perform the
                            // enrichment mapping to get the enriched output message.
                            LoadAllParts(pipelineContext, message);
                            messageOut = GetEnrichedMessage(pipelineContext, message);
                            break;
                        }
                    }
                }
                if (!probeMatched)
                {
                    throw new Exception("No resolvers attached to the message enrichment service matched the input message during probing.");
                }
                //After enrichment, if we are not preserving message parts, remove them from Cache
                for (int partNumber = 0; partNumber < MAX_PARTS; partNumber++)
                {
                    string source = partSource[partNumber];
                    if (!partPreserved[partNumber])                   
                    {
                        if (partDoc[partNumber] != null)
                        {                            
                            partDoc[partNumber].RemoveAll();
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                Logger.WriteTrace(string.Format("Error occured in {0} \r\n Details: {1}", this.GetType().Name, exception.ToString()));
                throw exception;
            }
            Logger.WriteTrace(string.Format("******{0} Completed******", this.GetType().Name));
            return messageOut;
        }

        /// <summary>
        /// Initialize the fields that are used for processing a single resolver.
        /// </summary>
        private void InitializeResolverData()
        {
            failureEventId = 333;
            failureAction = "ThrowException";
            for (int index = 0; index < MAX_PROBES; index++)
            {
                probe[index] = "";
            }
            for (int index = 0; index < MAX_PARTS; index++)
            {
                partSource[index] = "";
                partType[index] = "";
                partDoc[index] = null;
                partPreserved[index] = false;
            }
            transformType = "";
        }

        /// <summary>
        /// Loads the data provided by the resolver.
        /// </summary>
        /// <param name="pipelineContext">The pipeline context.</param>
        /// <param name="msg">The message.</param>
        /// <param name="resolverString">The resolver string.</param>
        private void LoadResolverData(IPipelineContext pipelineContext, IBaseMessage message, string resolverString)
        {
            try
            {
                // Load properties from resolver.
                Logger.WriteTrace("Loading resolver data from: " + resolverString);
                ResolverInfo info = ResolverMgr.GetResolverInfo(ResolutionType.Transform, resolverString);
                if (info.Success)
                {
                    Dictionary<string, string> dictionary = ResolverMgr.Resolve(info, message, pipelineContext);
                    probe[0] = dictionary["Enrich.Probe0"];
                    probe[1] = dictionary["Enrich.Probe1"];
                    probe[2] = dictionary["Enrich.Probe2"];
                    partSource[0] = dictionary["Enrich.Part0Source"];
                    partSource[1] = dictionary["Enrich.Part1Source"];
                    partSource[2] = dictionary["Enrich.Part2Source"];
                    partSource[3] = dictionary["Enrich.Part3Source"];
                    partSource[4] = dictionary["Enrich.Part4Source"];
                    partPreserved[0] = Convert.ToBoolean(dictionary["Enrich.PreservePart0"]);
                    partPreserved[1] = Convert.ToBoolean(dictionary["Enrich.PreservePart1"]);
                    partPreserved[2] = Convert.ToBoolean(dictionary["Enrich.PreservePart2"]);
                    partPreserved[3] = Convert.ToBoolean(dictionary["Enrich.PreservePart3"]);
                    partPreserved[4] = Convert.ToBoolean(dictionary["Enrich.PreservePart4"]);
                    transformType = dictionary["Enrich.TransformType"];
                    failureEventId = Convert.ToInt32(dictionary["Enrich.FailureEventId"]);
                    failureAction = dictionary["Enrich.FailureAction"];
                }
                else
                {
                    throw new Exception("Fetch of resolver info failed.");
                }
            }
            catch (Exception exception)
            {
                Logger.WriteTrace(string.Format("Unable to load resolver data. Error occured in {0} \r\n Details: {1}", this.GetType().Name, exception.ToString()));
                throw exception;
            }
        }

        /// <summary>
        /// Loads all the message parts that have not yet been loaded. Some parts may have already
        /// been loaded during probing and those will not be reloaded.
        /// </summary>
        /// <param name="pipelineContext">The pipeline message context.</param>
        /// <param name="message">The pipeline message.</param>
        private void LoadAllParts(IPipelineContext pipelineContext, IBaseMessage message)
        {
            partCount = 0;
            for (int part = 0; part < MAX_PARTS; part++)
            {
                if (!String.IsNullOrEmpty(partSource[part]))
                {
                    if (partDoc[part] == null)
                    {
                        LoadPart(pipelineContext, message, part);
                    }
                    partCount++;
                }
            }
        }

        /// <summary>
        /// After a set of resolver properties has been loaded, this method will execute each probe
        /// to determine if the current map should be executed. The results of each probe are ANDed
        /// together.
        /// </summary>
        /// <param name="pipelineContext">The pipeline message context.</param>
        /// <param name="message">The pipeline message.</param>
        /// <returns>True if the probes matched, false otherwise.</returns>
        private bool Probe(IPipelineContext pipelineContext, IBaseMessage message)
        {
            bool match = true;
            for (int index = 0; index < MAX_PROBES; index++)
            {
                match = match && Probe(pipelineContext, message, probe[index]);
            }
            return match;
        }

        /// <summary>
        /// This method performs a given probe to determine if the current map should be executed.
        /// </summary>
        /// <remarks>
        /// The type of probing to be performed is determined by parsing the probe string.
        /// The format is as follows:
        ///    Part#M:E
        /// where
        ///    Part    is a constant prefix,
        ///    #       is the digit 0,1, or 2, indicating which message part to probe,
        ///    M       is the probing method (current allowed values are "Type" to probe
        ///            by message type and "XPath" to probe by XPath),
        ///    :       is a constant separator character, and
        ///    E       is an expression used to perform the probe using the given method.
        /// </remarks>
        /// <param name="pipelineContext">The pipeline message context.</param>
        /// <param name="message">The pipeline message.</param>
        /// <param name="probe">The probe string to be used for probing.</param>
        /// <returns>True if the probe matched, false otherwise.</returns>
        private bool Probe(IPipelineContext pipelineContext, IBaseMessage message, string probe)
        {
            bool match = false;
            if (String.IsNullOrEmpty(probe))
            {
                match = true;
            }
            else
            {
                Logger.WriteTrace("Executing probe: " + probe);
                int pos = probe.IndexOf(':');
                if ((pos < 6) || (probe.Length < (pos + 1)) || (String.Compare(probe.Substring(0, 4), "Part") != 0))
                {
                    throw new Exception("Invalid probe expression: " + probe);
                }
                else
                {
                    string expression = probe.Substring(pos + 1);
                    int part = Int32.Parse(probe.Substring(4, 1));
                    if (part < 0 || part >= MAX_PARTS)
                    {
                        int upperBound = MAX_PARTS - 1;
                        throw new Exception("Invalid probe expression. Part index must be in the range 0 - " + upperBound.ToString() + ".");
                    }
                    LoadPart(pipelineContext, message, part);
                    string method = probe.Substring(5, pos - 5);
                    if (String.Compare(method, "Type") == 0)
                    {
                        match = (String.Compare(partType[part], expression) == 0);
                        if (match)
                        {
                            Logger.WriteTrace("Message type \"" + expression + "\" matched against part " + part.ToString() + ".");
                        }
                        else
                        {
                            Logger.WriteTrace("Message type \"" + expression + "\" did not match against part " + part.ToString() + ".");
                        }
                    }
                    else if (String.Compare(method, "XPath") == 0)
                    {
                        try
                        {
                            StringReader reader = new StringReader(partDoc[part].InnerXml);
                            XPathDocument xpathDocument = new XPathDocument(reader);
                            XPathNavigator xPathNavigator = xpathDocument.CreateNavigator();
                            XPathExpression xPathExpression = xPathNavigator.Compile(expression);
                            match = Convert.ToBoolean(xPathNavigator.Evaluate(xPathExpression));
                            if (match)
                            {
                                Logger.WriteTrace("XPath expression \"" + expression + "\" matched against part " + part.ToString() + ".");
                            }
                            else
                            {
                                Logger.WriteTrace("XPath expression \"" + expression + "\" did not match against part " + part.ToString() + ".");
                            }
                        }
                        catch (Exception exception)
                        {
                            Logger.WriteTrace("XPath expression \"" + expression + "\" threw an exception when evaluated against part " + part.ToString() + ". " + exception.Message);
                        }
                    }
                    else
                    {
                        throw new Exception("Invalid probe expression. Probe method '" + method + "' not recognized.");
                    }
                }
            }
            return match;
        }

        /// <summary>
        /// Gets the enriched message by executing the multi-input map.
        /// </summary>
        /// <param name="pipelineContext">The pipeline message context.</param>
        /// <param name="message">The pipeline message.</param>
        /// <returns></returns>
        public IBaseMessage GetEnrichedMessage(IPipelineContext pipelineContext, IBaseMessage message)
        {
            XmlDocument masterDocument = null;
            MemoryStream masterDocumentStream = null;
            IBaseMessage transformedMessage = null;
            if (transformType == null)
            {
                throw new Exception("Transform type is null.");
            }
            //If no map is specified and there is only one message part, pass thru pipeline unchanged
            else if (transformType == "(None)" && partCount == 1)
            {
                Logger.WriteTrace("No map specified. Message will be passed through unchanged.");
                transformedMessage = message;
            }
            else
            {
                if (partCount == 0)
                {
                    throw new Exception("No input parts were specified for the enrichment.");
                }
                else if (partCount == 1)
                {
                    // Create simple input document.
                    masterDocument = partDoc[0];
                }
                else
                {
                    //  _partCount is > 1, so depending on if a map is specified, handle differently
                    // If NO map was specified, just combine message parts at root node,
                    if (transformType == "(None)")
                    {
                        // Create aggregate input document.
                        try
                        {
                            masterDocument = new XmlDocument();
                            XmlElement root = masterDocument.CreateElement("Msg");
                            masterDocument.AppendChild(root);
                            // Combine the parts, adding dividers.
                            for (int part = 0; part < MAX_PARTS; part++)
                            {
                                if (partDoc[part] != null)
                                {
                                    XmlElement partContainer = masterDocument.CreateElement("Message_" + (part + 1).ToString(), "");
                                    XmlNode _partDoc = masterDocument.ImportNode(partDoc[part].DocumentElement, true);
                                    partContainer.AppendChild(_partDoc);
                                    root.AppendChild(partContainer);
                                }
                            }
                            DumpAggregate(masterDocument);
                        }
                        catch (Exception exception)
                        {
                            Logger.WriteTrace(string.Format("Error occured in {0} \r\n Details: {1}", this.GetType().Name, exception.ToString()));
                            throw exception;
                        }

                    }
                    else
                    // otherwise, create the aggregate/inline schema for the source document
                    {
                        try
                        {
                            Logger.WriteTrace("Creating aggregate document.");
                            masterDocument = new XmlDocument();
                            XmlElement root = masterDocument.CreateElement("ns0", "Root", "http://schemas.microsoft.com/BizTalk/2003/aggschema");
                            masterDocument.AppendChild(root);
                            // Insert the parts.
                            for (int part = 0; part < MAX_PARTS; part++)
                            {
                                if (partDoc[part] != null)
                                {
                                    XmlElement partContainer = masterDocument.CreateElement("InputMessagePart_" + part.ToString(), "");
                                    XmlNode _partDoc = masterDocument.ImportNode(partDoc[part].DocumentElement, true);
                                    partContainer.AppendChild(_partDoc);
                                    root.AppendChild(partContainer);
                                }
                            }
                            DumpAggregate(masterDocument);
                        }
                        catch (Exception exception)
                        {
                            Logger.WriteTrace(string.Format("Error occured in {0} \r\n Details: {1}", this.GetType().Name, exception.ToString()));
                            throw exception;
                        }
                    }
                }
                // Get a memory stream for the master document.
                try
                {
                    Logger.WriteTrace("Getting input memory stream for enrichment.");
                    masterDocumentStream = new MemoryStream();
                    XmlTextWriter masterDocumentWriter = new XmlTextWriter(masterDocumentStream, Encoding.UTF8);
                    masterDocumentWriter.Formatting = Formatting.Indented;
                    masterDocument.WriteTo(masterDocumentWriter);
                    masterDocumentWriter.Flush();
                    masterDocumentStream.Seek(0, SeekOrigin.Begin);
                }
                catch (Exception exception)
                {
                    throw new Exception("Error getting memory stream for enrichment input.", exception);
                }

                //Handle final tranformedMessage depending on if we are using a map
                if (transformType == "(None)")
                {
                    try
                    {
                        masterDocumentStream.Position = 0L;
                        message.BodyPart.Data = masterDocumentStream;
                        pipelineContext.ResourceTracker.AddResource(message.BodyPart.Data);
                        transformedMessage = message;
                    }
                    catch (Exception exception)
                    {
                        throw new Exception("Error building combined message.", exception);
                    }
                }
                else
                {
                    // Build the transformed message.
                    try
                    {
                        Logger.WriteTrace("Building transformed message using map: " + transformType + ".");
                        string transformedMessageType = "";
                        Stream transformStream = GetTransformStream(masterDocumentStream, transformType, ref transformedMessageType);                        
                        transformStream.Position = 0L;
                        message.BodyPart.Data = transformStream;
                        pipelineContext.ResourceTracker.AddResource(message.BodyPart.Data);                        
                        message.Context.Write(BtsProperties.SchemaStrongName.Name, BtsProperties.SchemaStrongName.Namespace, null);
                        message.Context.Promote(BtsProperties.MessageType.Name, BtsProperties.MessageType.Namespace, transformedMessageType);
                        SetDocProperties(pipelineContext, message);
                        transformedMessage = message;
                    }
                    catch (Exception exception)
                    {
                        throw new Exception("Error building transformed message.", exception);
                    }
                }
            }
            return transformedMessage;
        }

        /// <summary>
        /// Loads a message part. The part is loaded from its source and the part type
        /// and document are stored in the corresponding part fields.
        /// </summary>
        /// <param name="pipelineContext">The pipeline message context.</param>
        /// <param name="message">The pipeline message.</param>
        /// <parm name="part">The index of the part to be loaded.</parm>
        private void LoadPart(IPipelineContext pipelineContext, IBaseMessage message, int part)
        {
            try
            {
                string _source = partSource[part];
                source = _source;
                if (source == "Pipeline")
                {
                    LoadFromPipeline(pipelineContext, message);
                }
                else if (source.StartsWith("Cache"))
                {
                    LoadFromCache(pipelineContext, message);
                }
                else if (source.StartsWith("Archive"))
                {
                    LoadFromArchive(pipelineContext, message);
                }
                else if (source == "Properties")
                {
                    LoadFromProperties(pipelineContext, message);
                }
                else if (source.StartsWith("Resource"))
                {
                    LoadFromResource(pipelineContext, message);
                }
                else
                {
                    throw new Exception("Message source '" + source + "' is not recognized.");
                }
                partType[part] = messageType;
                partDoc[part] = contentAsXmlDocument;
            }
            catch (Exception exception)
            {
                Logger.WriteTrace(string.Format("Unable to load part " + part.ToString() + " from '" + partSource[part] + "'. \r\n Error occured in {0} \r\n Details: {1}", this.GetType().Name, exception.ToString()));
                throw exception;
            }
        }

        /// <summary>
        /// Loads a document from the pipeline.
        /// </summary>
        /// <param name="context">The current pipeline context.</param>
        /// <param name="message">The current message in the pipeline.</param>
        private void LoadFromPipeline(IPipelineContext context, IBaseMessage message)
        {
            try
            {
                Logger.WriteTrace("Pipeline document is being loaded.");
                Stream stream = MessageHelper.GetReadOnlySeekableDataStream(context, message.BodyPart);
                stream.Position = 0L;
                contentType = "text/xml";
                contentAsXmlDocument = MessageHelper.MessageToXmlDocument(context, message.BodyPart);                
                messageType = BtsProperties.MessageType.Name;
                if (string.IsNullOrEmpty(messageType))
                {
                    messageType = Microsoft.Practices.ESB.Utilities.MessageHelper.GetMessageType(message, context);
                }
            }
            catch (Exception exception)
            {
                Logger.WriteTrace(string.Format("Error occured in {0} \r\n Details: {1}", this.GetType().Name, exception.ToString()));
                throw exception;
            }
        }

        /// <summary>
        /// Loads a document from the Archive
        /// </summary>
        /// <param name="context">The current pipeline context.</param>
        /// <param name="message">The current message in the pipeline.</param>
        private void LoadFromCache(IPipelineContext context, IBaseMessage message)
        {
            try
            {
                Logger.WriteTrace("Cached document is being loaded.");
                string cacheMsgName = "";
                if (source.Length > 6)
                {
                    cacheMsgName = source.Substring(6);
                }

                string interchangeID =(string)message.Context.Read(BtsProperties.InterchangeID.Name, BtsProperties.InterchangeID.Namespace);
                string cacheKey =  interchangeID+ cacheMsgName;

                if (string.IsNullOrEmpty(cacheKey))
                    throw new Exception("Invalid cache key. Interchange ID: " + interchangeID + " Cache Message Name: " + cacheMsgName);

                Enrich bizTalkMessage = new Enrich(cacheKey);

                if (source.Contains("|Final"))
                    ResetArchiveToError(bizTalkMessage.MessageId);

                contentType = "text/xml";
                contentAsXmlDocument = bizTalkMessage.ContentAsXmlDocument;
                UpdateMessageType();
            }
            catch (Exception exception)
            {
                Logger.WriteTrace(string.Format("Error occured in {0} \r\n Details: {1}", this.GetType().Name, exception.ToString()));
                throw exception;
            }
        }

        /// <summary>
        /// Loads a document from the Archive
        /// </summary>
        /// <param name="context">The current pipeline context.</param>
        /// <param name="message">The current message in the pipeline.</param>
        private void LoadFromArchive(IPipelineContext context, IBaseMessage message)
        {
            string strInterchangeId=String.Empty;
            try
            {
                Logger.WriteTrace("Archived document is being loaded.");                
                string tag = "";
                if (source.Length > 8)
                {
                    tag = source.Substring(8);
                }

                strInterchangeId = (string)message.Context.Read(BtsProperties.InterchangeID.Name, BtsProperties.InterchangeID.Namespace);               
                Enrich bizTalkMessage = new Enrich(strInterchangeId, tag);

                if (source.Contains("|Final"))
                    ResetArchiveToError(bizTalkMessage.MessageId);
                
                if (bizTalkMessage.ContentType == "text/xml")
                {
                    contentType = "text/xml";
                    contentAsXmlDocument = bizTalkMessage.ContentAsXmlDocument;
                    UpdateMessageType();
                }
                else if (bizTalkMessage.ContentType == "text/plain")
                {
                    contentType = "text/plain";
                    contentAsString = bizTalkMessage.ContentAsText;
                    messageType = "";
                }
                else if (bizTalkMessage.ContentType == "binary")
                {
                    contentType = "binary";
                    contentAsByteArray = bizTalkMessage.ContentAsByteArray;
                    messageType = "";
                }
            }
            catch (Exception exception)
            {
                Logger.WriteTrace(string.Format("Error occured in {0} \r\n Details: {1}", this.GetType().Name, exception.ToString()));
                throw exception;
            }
        }

        /// <summary>
        /// Loads the current message's properties as a document.
        /// </summary>
        /// <param name="context">The current pipeline context.</param>
        /// <param name="message">The current message in the pipeline.</param>
        private void LoadFromProperties(IPipelineContext context, IBaseMessage message)
        {
            try
            {
                Logger.WriteTrace("Properties document is being loaded.");
                IBaseMessageContext messageContext = message.Context;
                XmlDocument document = new XmlDocument();
                string ns = "http://www.avistacorp.com/schemas/Avista.ESB.Utilities/v1.0";
                document.LoadXml("<PropertyList xmlns='" + ns + "' />");
                XmlElement propertyListElement = document.DocumentElement;
                for (int propertyIndex = 0; propertyIndex < messageContext.CountProperties; propertyIndex++)
                {
                    // Get values from the context property.
                    string propertyName = null;
                    string propertyNamespace = null;
                    string propertyValue = messageContext.ReadAt(propertyIndex, out propertyName, out propertyNamespace).ToString();
                    XmlElement propertyElement = document.CreateElement(null, "Property", ns);
                    // Add the property name attribute.
                    XmlAttribute propertyNameAttribute = document.CreateAttribute("name");
                    propertyNameAttribute.Value = propertyName;
                    propertyElement.Attributes.Append(propertyNameAttribute);
                    // Add the property namespace attribute.
                    XmlAttribute propertyNamespaceAttribute = document.CreateAttribute("namespace");
                    propertyNamespaceAttribute.Value = propertyNamespace;
                    propertyElement.Attributes.Append(propertyNamespaceAttribute);
                    // If the property value begins with an '<', then we'll attempt to add it as an XML fragment.
                    // Otherwise, or if creation of the fragment fails, we'll add it as a text node.
                    XmlNode child = null;
                    if (propertyValue.StartsWith("<"))
                    {
                        try
                        {
                            XmlDocument embeddedDocument = new XmlDocument();
                            embeddedDocument.LoadXml(propertyValue);
                            child = document.CreateDocumentFragment();
                            child.InnerXml = embeddedDocument.DocumentElement.OuterXml;
                        }
                        catch (Exception)
                        {
                            child = null;
                        }
                    }
                    if (child == null)
                    {
                        child = document.CreateTextNode(propertyValue);
                    }
                    propertyElement.AppendChild(child);
                    propertyListElement.AppendChild(propertyElement);
                }
                contentType = "text/xml";
                contentAsXmlDocument = document;
                UpdateMessageType();
            }
            catch (Exception exception)
            {
                Logger.WriteTrace(string.Format("Error occured in {0} \r\n Details: {1}", this.GetType().Name, exception.ToString()));
                throw exception;
            }
        }

        /// <summary>
        /// Loads a document from a resource file.
        /// </summary>
        /// <param name="context">The current pipeline context.</param>
        /// <param name="message">The current message in the pipeline.</param>
        private void LoadFromResource(IPipelineContext context, IBaseMessage message)
        {
            try
            {
                Logger.WriteTrace("Resource document is being loaded.");
                // The source is in the forms "Resource:Name:Assembly"
                string specification = source.Substring(9);
                int delimiter = specification.IndexOf(":");
                string resourceName = specification.Substring(0, delimiter);
                string assemblyName = specification.Substring(delimiter + 1);
                Assembly assembly = Assembly.Load(assemblyName);
                text = LoadAsString(assembly, resourceName);
                ExpandMacros();
                contentType = "text/xml";
                contentAsXmlDocument = InstanceAsXmlDocument();
                UpdateMessageType();
            }
            catch (Exception exception)
            {
                Logger.WriteTrace(string.Format("Error occured in {0} \r\n Details: {1}", this.GetType().Name, exception.ToString()));
                throw exception;
            }
        }

        /// <summary>
        /// Updates a single archived message in the MessageArchive database with a given archive type.
        /// </summary>
        /// <param name="interchangeId">The interchangeId of the messages whose expiry is to be extended.</param>
        /// <returns>The number of rows affected.</returns>
        public static int ResetArchiveToError(Guid archiveMessageId)
        {
            int rows = 0;
            try
            {
                // Get a connection to the database and update the messages with the given InterchangeId.
                SqlServerConnection connection = new SqlServerConnection("MessageArchive");
                try
                {
                    connection.RefreshConfiguration();
                    connection.Open();
                    connection.BeginTransaction();
                    try
                    {
                        string sql = "UPDATE [MessageArchive].[dbo].[Message] " +
                                     "SET [ArchiveTypeId] = (Select Top 1 Id from [MessageArchive].[dbo].[ArchiveType] where [Name] = 'Error'), " +
                                     "[ExpiryDate] = (Case When DATEDIFF(mi, GetDate(), [ExpiryDate]) < " + extendedExpiryMinutes.ToString() + " Then DateAdd(mi, " + extendedExpiryMinutes.ToString() + ", [ExpiryDate]) " +
                                     " ELSE [ExpiryDate] END) WHERE [MessageId] = @MessageId ";
                        SqlCommand sqlCommand = new SqlCommand(sql);
                        SqlParameter parmMessageId = sqlCommand.Parameters.Add("@MessageId", SqlDbType.UniqueIdentifier);
                        parmMessageId.Value = archiveMessageId;
                        rows = connection.ExecuteNonQuery(sqlCommand);
                        connection.Commit();
                    }
                    catch (Exception)
                    {
                        connection.Rollback();
                        throw;
                    }
                }
                finally
                {
                    if (connection != null)
                    {
                        connection.Close();
                        connection.Dispose();
                        connection = null;
                    }
                }
            }
            catch (Exception exception)
            {
                Logger.WriteTrace(string.Format("Error occured while Reseting Archive To Error \r\n Details: {1}", exception.ToString()));
                throw exception;
            }
            return rows;
        }

        /// <summary>
        /// Performs macro substitutions in a message template and populates the instance.
        /// Macros take the form {Verb,Parm1,Parm2,...,ParmN}
        /// </summary>
        private void ExpandMacros()
        {
            this.args = null;
            try
            {
                instance = text;
                int start = instance.IndexOf("{");
                int end = instance.IndexOf("}", start + 1);
                while (start >= 0 && end > start)
                {
                    int length = end - start - 1;
                    if (length > 0)
                    {
                        string macro = instance.Substring(start + 1, length);
                        char[] separator = { ',' };
                        string[] words = macro.Split(separator);
                        string verb = words[0];
                        // {PARM,index,type,format}
                        if (verb.Equals("PARM"))
                        {
                            try
                            {
                                if (words.Length != 4)
                                {
                                    throw new Exception("PARM macro usage is {PARM,index,type,format}.");
                                }
                                int index = int.Parse(words[1]);
                                object parm = args[index];
                                string type = words[2];
                                string format = words[3];
                                string value = null;
                                if (type.Equals("INT"))
                                {
                                    value = ((int)parm).ToString(format);
                                }
                                else if (type.Equals("STRING")) // Format is ignored.
                                {
                                    value = (string)parm;
                                }
                                else if (type.Equals("BOOL")) // Format is ignored.
                                {
                                    value = ((bool)parm).ToString();
                                }
                                else if (type.Equals("DATE"))
                                {
                                    value = ((DateTime)parm).ToString(format);
                                }
                                else if (type.Equals("DECIMAL"))
                                {
                                    value = ((decimal)parm).ToString(format);
                                }
                                else if (type.Equals("DOUBLE"))
                                {
                                    value = ((double)parm).ToString(format);
                                }
                                else if (type.Equals("FLOAT"))
                                {
                                    value = ((float)parm).ToString(format);
                                }
                                else
                                {
                                    throw new Exception("Unrecognized type. Valid types are BOOL, DATE, DECIMAL, DOUBLE, FLOAT, INT, and STRING.");
                                }
                                instance = instance.Substring(0, start) + value + instance.Substring(end + 1);
                                end = start + value.Length;
                            }
                            catch (Exception exception)
                            {
                                throw new Exception("Error processing PARM macro.", exception);
                            }
                        }
                        // {NOW,format}
                        else if (verb.Equals("NOW"))
                        {
                            try
                            {
                                if (words.Length != 2)
                                {
                                    throw new Exception("NOW macro usage is {NOW,format}.");
                                }
                                DateTime now = DateTime.Now;
                                string format = words[1];
                                string value = now.ToString(format);
                                instance = instance.Substring(0, start) + value + instance.Substring(end + 1);
                                end = start + value.Length;
                            }
                            catch (Exception exception)
                            {
                                throw new Exception("Error processing NOW macro.", exception);
                            }
                        }
                        // {RAND,Min,Max}
                        else if (verb.Equals("RAND"))
                        {
                            try
                            {
                                if (words.Length != 3)
                                {
                                    throw new Exception("RAND macro usage is {RAND,min,max}.");
                                }
                                int min = int.Parse(words[1]);
                                int max = int.Parse(words[2]);
                                Random random = new Random();
                                string value = random.Next(min, max).ToString();
                                instance = instance.Substring(0, start) + value + instance.Substring(end + 1);
                                end = start + value.Length;
                            }
                            catch (Exception exception)
                            {
                                throw new Exception("Error processing RAND macro.", exception);
                            }
                        }
                        // {GUID}
                        else if (verb.Equals("GUID"))
                        {
                            string value = Guid.NewGuid().ToString();
                            try
                            {
                                if (words.Length != 1)
                                {
                                    throw new Exception("GUID macro usage is {GUID}.");
                                }
                                instance = instance.Substring(0, start) + value + instance.Substring(end + 1);
                                end = start + value.Length;
                            }
                            catch (Exception exception)
                            {
                                throw new Exception("Error processing GUID macro.", exception);
                            }
                        }
                        else
                        {
                            // The verb was not recognized so we will set the end pointer
                            // to the start and the next iteration will continue with the next character.
                            // This will allow content like {{GUID}} in which the outer braces are ignored.
                            end = start;
                        }
                    }
                    // Look for the next macro.
                    start = instance.IndexOf("{", end + 1);
                    end = instance.IndexOf("}", start + 1);
                }
            }
            catch (Exception e)
            {
                Logger.WriteTrace(string.Format("Error occured in {0} \r\n Details: {1}", this.GetType().Name, e.ToString()));
                throw e;
            }
        }

        public XmlDocument InstanceAsXmlDocument()
        {
            XmlDocument document = null;
            try
            {
                document = new XmlDocument();
                document.LoadXml(instance);
            }
            catch (Exception exception)
            {
                throw exception;
            }
            return document;
        }

        public static string LoadAsString(Assembly assembly, string resourceName)
        {
            String text = null;
            string assemblyName = "unknown";
            try
            {
                assemblyName = assembly.GetName().Name;
                string qualifiedResourceName = assemblyName + "." + resourceName;
                using (Stream stream = assembly.GetManifestResourceStream(qualifiedResourceName))
                {
                    using (StreamReader sr = new StreamReader(stream))
                    {
                        text = sr.ReadToEnd();
                    }
                }
            }
            catch (Exception exception)
            {
                Logger.WriteTrace(string.Format("Error occured in LoadAsString. \r\n Details: {1}", exception.ToString()));
                throw exception;
            }
            return text;
        }

        /// <summary>
        /// Gets a transform stream for a message stream using the spcified map.
        /// </summary>
        /// <param name="stream">The message stream to be transformed.</param>
        /// <param name="mapName">The name of the map used to perform the transformation.</param>
        /// <param name="messageType">This will be set to the message type of the transformed stream.</param>
        /// <returns>A stream for the transformed message.</returns>
        private Stream GetTransformStream(Stream stream, string mapName, ref string messageType)
        {
            Stream output = null;
            try
            {
                Type mapType = Type.GetType(mapName);
                if (mapType == null)
                {
                    throw new Exception("Invalid Map type " + mapName);
                }
                else
                {
                    TransformMetaData mapMetadata = TransformMetaData.For(mapType);
                    SchemaMetadata sourceMetadata = mapMetadata.SourceSchemas[0];
                    string sourceSchemaName = sourceMetadata.SchemaName;
                    SchemaMetadata targetMetadata = mapMetadata.TargetSchemas[0];
                    // Update the message type to be the type we will have after applying the map.
                    messageType = targetMetadata.SchemaName;
                    XPathDocument input = new XPathDocument(stream);

                    output = new MemoryStream();
                    System.Xml.XmlWriter xmlWriter = new System.Xml.XmlTextWriter(output, System.Text.Encoding.Unicode);
                    mapMetadata.Transform.Transform(input, mapMetadata.ArgumentList, xmlWriter);
                    output.Flush();
                    output.Seek(0L, SeekOrigin.Begin);
                    return output;
                }
            }
            catch (Exception exception)
            {
                Logger.WriteTrace(string.Format("Error occured in {0} \r\n Details: {1}", this.GetType().Name, exception.ToString()));
                throw exception;
            }
        }

        private void DumpAggregate(XmlDocument document)
        {
            if (trace && dumpAggregates)
            {
                try
                {
                    // Build a file name from the part names.
                    string fileName = "";
                    for (int part = 0; part < MAX_PARTS; part++)
                    {
                        if (partDoc[part] != null)
                        {
                            fileName = fileName + partDoc[part].DocumentElement.LocalName + "_";
                        }
                    }
                    fileName = fileName + Guid.NewGuid().ToString() + ".xml";
                    string filePath = Path.Combine(dumpFolder, fileName);
                    // Configure settings that will be used for dumping the message.
                    XmlWriterSettings settings = new XmlWriterSettings();
                    settings.OmitXmlDeclaration = false;
                    settings.Encoding = Encoding.UTF8;
                    settings.Indent = true;
                    settings.NamespaceHandling = NamespaceHandling.OmitDuplicates;
                    settings.NewLineHandling = NewLineHandling.None;
                    // Write the message to the file.
                    using (FileStream stream = new FileStream(filePath, FileMode.CreateNew, FileAccess.ReadWrite))
                    {
                        XmlWriter writer = XmlWriter.Create(stream, settings);
                        document.Save(writer);
                        stream.Flush();
                    }
                }
                catch (Exception exception)
                {
                    Logger.WriteWarning("The EnrichMessage service encountered an error while dumping the aggregate document. " + Environment.NewLine + exception.Message);
                }
            }
        }

        public static void SetDocProperties(IPipelineContext context, IBaseMessage message)
        {            
            string schemaStrongName = message.Context.Read(BtsProperties.SchemaStrongName.Name, BtsProperties.MessageType.Namespace) as string;
            if (String.Equals(schemaStrongName, "Microsoft.XLANGs.BaseTypes.Any, Microsoft.XLANGs.BaseTypes, Version=3.0.1.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35", StringComparison.Ordinal))
            {
                schemaStrongName = null;
                message.Context.Write(BtsProperties.SchemaStrongName.Name, BtsProperties.SchemaStrongName.Namespace, null);
            }            
            string messageType = message.Context.Read(BtsProperties.MessageType.Name, BtsProperties.MessageType.Namespace) as string;
            if (String.IsNullOrEmpty(messageType))
            {
                if (!String.IsNullOrEmpty(schemaStrongName))
                {
                    IDocumentSpec documentSpecByName = null;
                    try
                    {
                        documentSpecByName = context.GetDocumentSpecByName(schemaStrongName);
                    }
                    catch (DocumentSpecException)
                    {
                    }
                    if (documentSpecByName != null)
                    {
                        messageType = documentSpecByName.DocType;
                        message.Context.Promote(BtsProperties.MessageType.Name, BtsProperties.MessageType.Namespace, messageType);
                    }
                }
            }
        }

        private void UpdateMessageType()
        {
            messageType = contentAsXmlDocument.DocumentElement.NamespaceURI + '#' + contentAsXmlDocument.DocumentElement.LocalName;
        }
    }
}
