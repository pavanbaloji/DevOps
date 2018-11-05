using Avista.ESB.Utilities.Logging;
using Microsoft.BizTalk.Component.Interop;
using Microsoft.BizTalk.Message.Interop;
using Microsoft.BizTalk.Streaming;
using Microsoft.Practices.ESB.GlobalPropertyContext;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;

namespace Avista.ESB.MessagingServices.Manipulator
{
    /// <summary>
    /// Manipulator provides all helper methosd required to manipulate message in itinerary.
    /// </summary>
    internal class Manipulator
    {
        /// <summary>
        /// Builds ManipulatorDescription list with read data.
        /// </summary>
        /// <param name="manipulationList"></param>
        /// <param name="message"></param>
        /// <param name="pContext"></param>
        /// <returns></returns>
        internal List<ManipulatorDescription> GetManipulationData(List<ManipulatorDescription> manipulationList, IBaseMessage message, IPipelineContext pContext)
        {
            Logger.WriteTrace("Retriving data for manipulation started.");

            XmlDocument inXml = new XmlDocument();
            Stream inStream = null;
            if (manipulationList.Exists(x => x.ReadFrom.Equals("Xpath", StringComparison.InvariantCultureIgnoreCase)))
            {
                inStream = message.BodyPart.GetOriginalDataStream();
                if (!inStream.CanSeek)
                {
                    inStream = new ReadOnlySeekableStream(inStream)
                    {
                        Position = 0L
                    };
                }
                else
                {
                    inStream.Position = 0L;
                }

                inXml.Load(inStream);
            }

            try
            {
                for (int i = 0; i < manipulationList.Count; i++)
                {
                    switch (manipulationList[i].ReadFrom)
                    {
                        case "HttpHeader":
                            manipulationList[i].ManipulateValue = GetHttpHeaderItemValue(message.Context, manipulationList[i].ReadItem);
                            break;
                        case "XPath":
                            manipulationList[i].ManipulateValue = GetXpathItemValue(manipulationList[i].ReadItem, inXml);
                            break;
                        case "MessageContext":
                            manipulationList[i].ManipulateValue = GetMessageContextItemValue(manipulationList[i].ReadItem, message.Context);
                            break;
                        case "Constant":
                            manipulationList[i].ManipulateValue = manipulationList[i].ReadItem;
                            break;
                        case "BizTalkMacros":
                            manipulationList[i].ManipulateValue = GetConstantWithMacrosReplaced(manipulationList[i].ReadItem, message, pContext);
                            break;
                        default:
                            break;
                    }
                }
            }
            finally
            {
                if (inStream != null)
                {
                    inStream.Position = 0L;
                    message.BodyPart.Data = inStream;
                }


                if (inXml != null)
                    inXml = null;
            }

            Logger.WriteTrace("Retriving data for manipulation completed.");
            return manipulationList;
        }

        /// <summary>
        /// Replaces biztalk macros if any present in the constant string
        /// </summary>
        /// <param name="constantValue"></param>
        /// <param name="msg"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        private string GetConstantWithMacrosReplaced(string constantValue, IBaseMessage msg, IPipelineContext context)
        {
            if (!constantValue.Contains('%'))
            {
                return constantValue;
            }

            string target = constantValue;

            while(target.Contains("%Date-") || target.Contains("%UtcDate-"))
            {
                string pattern = "";
                string dateformat = GetDateFormat(target, "%Date", out pattern);

                if (!string.IsNullOrEmpty(pattern) && !string.IsNullOrEmpty(dateformat))
                {
                    target = ReplaceCaseInsensitive(target, pattern, DateTime.Now.ToString(dateformat));
                }

                pattern = "";
                dateformat = GetDateFormat(target, "%UtcDate", out pattern);

                if (!string.IsNullOrEmpty(pattern) && !string.IsNullOrEmpty(dateformat))
                {
                    target = ReplaceCaseInsensitive(target, pattern, DateTime.UtcNow.ToString(dateformat));
                }
            }

            target = ReplaceCaseInsensitive(target, "%datetime%", DateTime.Now.ToString("yyyy-MM-ddTHHmmss"));
            target = ReplaceCaseInsensitive(target, "%datetime.tz%", DateTime.Now.ToString("yyyy-MM-ddTHHmmsszzz").Replace(":", ""));
            target = ReplaceCaseInsensitive(target, "%LocalTime%", DateTime.Now.ToString("HHmmss"));
            target = ReplaceCaseInsensitive(target, "%MessageID%", msg.MessageID.ToString());
            target = ReplaceCaseInsensitive(target, "%SourceFileName%", ReadProperty(msg.Context, FileProperties.ReceivedFileName.Name, FileProperties.ReceivedFileName.Namespace));

            return target;
        }
        /// <summary>
        /// Replaces macros with actual value
        /// </summary>
        /// <param name="target"></param>
        /// <param name="patternPart"></param>
        /// <param name="pattern"></param>
        /// <returns></returns>
        private string GetDateFormat(string target, string patternPart, out string pattern)
        {
            pattern = string.Empty;

            if (target.Contains(patternPart))
            {
                int patternFirstIndex = target.IndexOf(patternPart);
                int patternLastIndex = target.IndexOf("%", patternFirstIndex + 1);

                pattern = target.Substring(patternFirstIndex, (patternLastIndex - patternFirstIndex) + 1);

                int dateFormatFirstIndex = pattern.IndexOf("Date") + 5;
                int dateFormatLstIndex = pattern.Length - 1;
                int lenght = dateFormatLstIndex - dateFormatFirstIndex;

                if (lenght <= 0)
                {
                    return "yyyyMMdd";
                }
                string dateFormat = pattern.Substring(dateFormatFirstIndex, lenght);

                return dateFormat;
            }

            return "";
        }

        /// <summary>
        /// ReplaceCaseInsensitive
        /// </summary>
        /// <param name="original"></param>
        /// <param name="pattern"></param>
        /// <param name="replacement"></param>
        /// <returns></returns>
        private static string ReplaceCaseInsensitive(string original, string pattern, string replacement)
        {
            int count, position0, position1;
            count = position0 = position1 = 0;
            string upperString = original.ToUpper();
            string upperPattern = pattern.ToUpper();
            int inc = (original.Length / pattern.Length) * (replacement.Length - pattern.Length);
            char[] chars = new char[original.Length + Math.Max(0, inc)];
            while ((position1 = upperString.IndexOf(upperPattern, position0)) != -1)
            {
                for (int i = position0; i < position1; ++i)
                {
                    chars[count++] = original[i];
                }
                for (int i = 0; i < replacement.Length; ++i)
                {
                    chars[count++] = replacement[i];
                }
                position0 = position1 + pattern.Length;
            }
            if (position0 == 0) return original;
            for (int i = position0; i < original.Length; ++i)
            {
                chars[count++] = original[i];
            }
            return new string(chars, 0, count);
        }

        /// <summary>
        /// Get message context
        /// </summary>
        /// <param name="contextProperty"></param>
        /// <param name="msgContext"></param>
        /// <returns></returns>
        private string GetMessageContextItemValue(string serializedContextProperty, IBaseMessageContext msgContext)
        {
            MessageContextDescription msgContextDes = (MessageContextDescription)SerializeToObject(typeof(MessageContextDescription), serializedContextProperty);

            if (!string.IsNullOrEmpty(msgContextDes.PropertyName) && !string.IsNullOrEmpty(msgContextDes.PropertyNamespace))
            {
                return ReadProperty(msgContext, msgContextDes.PropertyName, msgContextDes.PropertyNamespace);
            }
            return string.Empty;
        }

        /// <summary>
        /// Gets Property Value.
        /// </summary>
        /// <param name="iBaseMsg"></param>
        /// <param name="pName"></param>
        /// <param name="pNamespace"></param>
        /// <returns></returns>
        private string ReadProperty(IBaseMessageContext msgContext, string pName, string pNamespace)
        {
            try
            {
                return msgContext.Read(pName, pNamespace).ToString();
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }
        /// <summary>
        /// Get Xpath value
        /// </summary>
        /// <param name="xPath"></param>
        /// <param name="bodyContent"></param>
        /// <returns></returns>
        private string GetXpathItemValue(string xPath, XmlDocument bodyContent)
        {
            XmlNamespaceManager nsmgr = new XmlNamespaceManager(bodyContent.NameTable);

            if (xPath.Contains("EVAL:"))
            {
                xPath = xPath.Remove(0, 5);
                XPathDocument xpathDoc = new XPathDocument(new XmlNodeReader(bodyContent));
                return xpathDoc.CreateNavigator().Evaluate(xPath).ToString();
            }

            XmlNode node = bodyContent.SelectSingleNode(xPath, nsmgr);
            if (node != null)
            {
                if (node.NodeType == XmlNodeType.Attribute)
                    return (node.Value);
                else
                    return (node.InnerText);
            }
            else
            {
                return "";
            }
        }

        /// <summary>
        /// Get inbound http header item
        /// </summary>
        /// <param name="messageContext"></param>
        /// <param name="readItem"></param>
        /// <returns></returns>
        private string GetHttpHeaderItemValue(IBaseMessageContext messageContext, string readItem)
        {
            string httpheader = ReadProperty(messageContext, "InboundHttpHeaders", "http://schemas.microsoft.com/BizTalk/2006/01/Adapters/WCF-properties");

            string[] separator = { Environment.NewLine };
            string[] hearkeyValue = httpheader.Split(separator, StringSplitOptions.RemoveEmptyEntries);
            string readValue = string.Empty;

            foreach (string item in hearkeyValue)
            {
                if (item.Contains(readItem))
                {
                    string[] keyValue = item.Split(':');
                    readValue = keyValue.Length == 2 ? keyValue[1].Trim() : "";
                    break;
                }
            }

            return readValue;
        }

        /// <summary>
        /// Write content to HttpHeader, Xpath, Context.
        /// </summary>
        /// <param name="manipulatorList"></param>
        /// <param name="msg"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        internal IBaseMessage WriteManipulationData(List<ManipulatorDescription> manipulatorList, IBaseMessage msg, IPipelineContext context)
        {
            Logger.WriteTrace("Writing Manipulated data started.");
            XmlDocument xml = new XmlDocument();
            Stream stream = null;
            IBaseMessage result = msg;
            if (manipulatorList.Exists(x => x.WriteTo.Equals("XPath", StringComparison.InvariantCultureIgnoreCase)
                || x.WriteTo.Equals("XmlStructure", StringComparison.InvariantCultureIgnoreCase)))
            {
                stream = msg.BodyPart.GetOriginalDataStream();
                if (!stream.CanSeek)
                {
                    stream = new ReadOnlySeekableStream(stream)
                    {
                        Position = 0L
                    };
                }
                else
                {
                    stream.Position = 0L;
                }

                xml.Load(stream);
            }

            try
            {
                foreach (ManipulatorDescription dec in manipulatorList)
                {
                    switch (dec.WriteTo)
                    {
                        case "HttpHeader":
                            msg.Context = WriteToHttpHeader(dec.ManipulateValue, dec.WriteItem, msg.Context);
                            break;
                        case "XPath":
                            xml = WriteToXpath(dec.ManipulateValue, dec.WriteItem, xml);
                            break;
                        case "MessageContext":
                            msg.Context = WriteToMessageContextAndPromote(dec.ManipulateValue, dec.WriteItem, msg.Context, false);
                            break;
                        case "PromoteMessageContext":
                            msg.Context = WriteToMessageContextAndPromote(dec.ManipulateValue, dec.WriteItem, msg.Context, true);
                            break;
                        case "XmlStructure":
                            xml = WriteToXmlStructure(xml, dec.WriteItem);
                            break;
                        default:
                            break;
                    }
                }

                if (manipulatorList.Exists(x => x.WriteTo.Equals("XPath", StringComparison.InvariantCultureIgnoreCase)
                    || x.WriteTo.Equals("XmlStructure", StringComparison.InvariantCultureIgnoreCase)))
                {
                    string btsMsgType = string.IsNullOrEmpty(xml.DocumentElement.NamespaceURI) ? xml.DocumentElement.LocalName : xml.DocumentElement.NamespaceURI + "#" + xml.DocumentElement.LocalName;

                    Encoding encoding = !string.IsNullOrEmpty(msg.BodyPart.Charset) ? Encoding.GetEncoding(msg.BodyPart.Charset) : Encoding.UTF8;
                    byte[] byteArray = Encoding.UTF8.GetBytes(xml.OuterXml);
                    MemoryStream xmlStream = new MemoryStream(byteArray);

                    xmlStream.Flush();
                    xmlStream.Position = 0L;

                    msg.BodyPart.Data = xmlStream;
                    msg.BodyPart.Data.Position = 0L;
                    msg.Context.Promote(BtsProperties.MessageType.Name, BtsProperties.MessageType.Namespace, btsMsgType);
                }
            }
            finally
            {
                if (stream != null)
                    stream.Position = 0L;

                if (xml != null)
                    xml = null;
            }
            Logger.WriteTrace("Writing Manipulated data completed.");
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="xml"></param>
        /// <param name="serializedValues"></param>
        /// <returns></returns>
        private XmlDocument WriteToXmlStructure(XmlDocument xml, string serializedValues)
        {
            XmlStructureDescription xmlDescription = (XmlStructureDescription)SerializeToObject(typeof(XmlStructureDescription), serializedValues);

            if (xmlDescription.Action == "Rename Root Node")
                return WriteToXmlRootNode(xml, xmlDescription.RootNodeName, xmlDescription.Namespace, xmlDescription.NamespacePrefix, xmlDescription.ApplyNamespacePrefixTo);
            else if (xmlDescription.Action == "Wrap Root Node")
                return WrapXmlRootNode(xml, xmlDescription.RootNodeName, xmlDescription.Namespace, xmlDescription.NamespacePrefix);
            else if (xmlDescription.Action == "Remove Namespace Prefix")
                return RemoveXmlNamespacePrefix(xml);
            else
                return xml;
        }

        /// <summary>
        /// Wraps xml document with new root node. the method accepts xpath ans build root node.
        /// </summary>
        /// <param name="xmlDoc"></param>
        /// <param name="xpath"></param>
        /// <returns></returns>
        public XmlDocument WrapXmlRootNode(XmlDocument xmlDoc, string newRootName, string newRootNamespace, string newRootNamespacePrefix)
        {
            XDocument xDoc = XDocument.Load(new XmlNodeReader(xmlDoc));
            XNamespace newRootElementNamespace = newRootNamespace;

            XDocument newDoc = XDocument.Parse("<" + newRootName + "/>");

            if (!string.IsNullOrEmpty(newRootNamespace))
            {
                newDoc.Root.Name = newRootElementNamespace + newRootName;
                if (string.IsNullOrEmpty(newRootNamespacePrefix))
                    newDoc.Root.Add(new XAttribute(XNamespace.Xmlns + "nx", newRootElementNamespace));
                else
                    newDoc.Root.Add(new XAttribute(XNamespace.Xmlns + newRootNamespacePrefix, newRootElementNamespace));
            }

            newDoc.Root.Add(xDoc.Elements());

            xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(newDoc.ToString());

            return xmlDoc;
        }

        /// <summary>
        /// Removes namespace prefix from xml document
        /// </summary>
        /// <param name="xmlDoc"></param>
        /// <returns></returns>
        public XmlDocument RemoveXmlNamespacePrefix(XmlDocument xmlDoc)
        {
            XDocument xDoc = XDocument.Load(new XmlNodeReader(xmlDoc));

            xDoc.Descendants().Attributes().Where(a => a.IsNamespaceDeclaration).Remove();

            xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xDoc.ToString());

            return xmlDoc;
        }

        /// <summary>
        /// Rename xml document root node.
        /// </summary>
        /// <param name="xmlDoc"></param>
        /// <param name="xpath"></param>
        /// <returns></returns>
        public XmlDocument WriteToXmlRootNode(XmlDocument xmlDoc, string newRootName, string newRootNamespace, string newRootNamespacePrefix, string applyNamespacePrefixTo)
        {
            XDocument xDoc = XDocument.Load(new XmlNodeReader(xmlDoc));

            bool applyNamespacetoRootOnly = !string.IsNullOrEmpty(applyNamespacePrefixTo) && applyNamespacePrefixTo.Equals("FullDocument");

            XNamespace oldRootNamespace = xDoc.Root.Name.NamespaceName;

            if (!newRootNamespace.Equals(oldRootNamespace.NamespaceName, StringComparison.InvariantCultureIgnoreCase))
            {
                if (string.IsNullOrEmpty(newRootNamespacePrefix))
                    newRootNamespacePrefix = xDoc.Root.GetPrefixOfNamespace(oldRootNamespace);


                xDoc.Descendants().Attributes().Where(a => a.IsNamespaceDeclaration && a.Value == oldRootNamespace).Remove();

                if (!string.IsNullOrEmpty(newRootNamespace))
                {
                    XNamespace newRootElementNamespace = newRootNamespace;

                    xDoc.Root.Name = newRootElementNamespace + newRootName;

                    if (!string.IsNullOrEmpty(newRootNamespacePrefix))
                        xDoc.Root.Add(new XAttribute(XNamespace.Xmlns + newRootNamespacePrefix, newRootElementNamespace));

                    if(applyNamespacetoRootOnly)
                    {
                        foreach (XElement el in xDoc.Descendants())
                        {
                            if (el.Name.NamespaceName == oldRootNamespace)
                            {
                                el.Name = newRootElementNamespace + el.Name.LocalName;
                            }
                        }
                    }
                }
                else
                {
                    xDoc.Root.Name = newRootName;

                    foreach (XElement el in xDoc.Descendants())
                    {
                        if (el.Name.NamespaceName == oldRootNamespace)
                        {
                            el.Name = el.Name.LocalName;
                        }
                    }
                }

            }
            else
            {
                xDoc.Root.Name = oldRootNamespace + newRootName;
            }
            xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xDoc.ToString());
            return xmlDoc;
        }

        /// <summary>
        /// Writes to message context
        /// </summary>
        /// <param name="value"></param>
        /// <param name="propertyName"></param>
        /// <param name="messageContext"></param>
        /// <returns></returns>
        private IBaseMessageContext WriteToMessageContextAndPromote(string value, string serializedContextProperty, IBaseMessageContext messageContext, bool promoteProperty)
        {
            MessageContextDescription msgContextDes = (MessageContextDescription)SerializeToObject(typeof(MessageContextDescription), serializedContextProperty);

            if (!string.IsNullOrEmpty(value))
            {
                if (!string.IsNullOrEmpty(msgContextDes.PropertyName) && !string.IsNullOrEmpty(msgContextDes.PropertyNamespace))
                {
                    //Check whether writng to OutboundTransportLocation
                    if(msgContextDes.PropertyName.Equals(BtsProperties.OutboundTransportLocation.Name, StringComparison.InvariantCultureIgnoreCase) &&
                        msgContextDes.PropertyNamespace.Equals(BtsProperties.OutboundTransportLocation.Namespace, StringComparison.InvariantCultureIgnoreCase))
                    {
                        FileInfo fileInfo = new FileInfo(value);

                        if (!Directory.Exists(fileInfo.DirectoryName))
                        {
                            Directory.CreateDirectory(fileInfo.DirectoryName);
                        }
                    }

                    if (promoteProperty)
                    {
                        messageContext.Promote(msgContextDes.PropertyName, msgContextDes.PropertyNamespace, value);
                    }
                    else
                    {
                        messageContext.Write(msgContextDes.PropertyName, msgContextDes.PropertyNamespace, value);
                    }
                }

            }

            return messageContext;
        }

        /// <summary>
        /// Write to http header
        /// </summary>
        /// <param name="value"></param>
        /// <param name="headerKeyName"></param>
        /// <param name="messageContext"></param>
        /// <returns></returns>
        private IBaseMessageContext WriteToHttpHeader(string value, string headerKeyName, IBaseMessageContext messageContext)
        {
            string httpHeader = ReadProperty(messageContext, "HttpHeaders", "http://schemas.microsoft.com/BizTalk/2006/01/Adapters/WCF-properties");

            if (!string.IsNullOrEmpty(httpHeader) && httpHeader.EndsWith(Environment.NewLine))
            {
                httpHeader += headerKeyName + ": " + value + Environment.NewLine;
            }
            else if (!string.IsNullOrEmpty(httpHeader))
            {
                httpHeader += Environment.NewLine + headerKeyName + ": " + value + Environment.NewLine;
            }
            else
            {
                httpHeader = headerKeyName + ": " + value + Environment.NewLine;
            }
            messageContext.Write("HttpHeaders", "http://schemas.microsoft.com/BizTalk/2006/01/Adapters/WCF-properties", httpHeader);

            return messageContext;
        }

        /// <summary>
        /// Write to xml document
        /// </summary>
        /// <param name="value"></param>
        /// <param name="xpath"></param>
        /// <param name="xmlDoc"></param>
        /// <returns></returns>
        private XmlDocument WriteToXpath(string value, string xpath, XmlDocument xmlDoc)
        {
            XmlNamespaceManager nsmgr = new XmlNamespaceManager(xmlDoc.NameTable);
            XmlNodeList nodes = xmlDoc.SelectNodes(xpath, nsmgr);
            if (nodes.Count == 1)
            {
                if (nodes[0].NodeType == XmlNodeType.Attribute)
                    nodes[0].Value = value;
                else
                    nodes[0].InnerText = value;
            }
            else if (nodes.Count == 0 && !string.IsNullOrEmpty(value))
            {
                CreateXmlNode(xmlDoc, xpath).InnerText = value;
            }
            else
            {
                Logger.WriteTrace("ManipulatorService: Couldn't write to xpath '" + xpath + "'");
            }
            return xmlDoc;
        }

        /// <summary>
        /// Create XmlNode with xpath.
        /// </summary>
        /// <param name="xmlDoc"></param>
        /// <param name="xpath"></param>
        /// <returns></returns>
        public XmlNode CreateXmlNode(XmlDocument xmlDoc, string xpath)
        {
            XmlNode node = xmlDoc;
            foreach (string xpathPart in xpath.SplitXpath())
            {
                XmlNodeList nodes = node.SelectNodes(xpathPart);

                if (nodes.Count > 1)
                {
                    //Pavan: some improvisation need in case of muliple nodes found. currently this code works only when there is one node.  
                    throw new ApplicationException("XmlNode '" + xpathPart + "' was found multiple times! Unable to resolve xpath: " + xpath);
                }
                else if (nodes.Count == 1)
                {
                    node = nodes[0];
                    continue;
                }

                string localName = string.Empty;
                string namespaceUri = string.Empty;

                xpathPart.SplitXpathPart(out localName, out namespaceUri);

                if (xpathPart.Contains("@"))
                {
                    string attributeName = localName.Replace("@", "");
                    var anode = string.IsNullOrEmpty(namespaceUri) ? xmlDoc.CreateAttribute(attributeName) : xmlDoc.CreateAttribute(attributeName, namespaceUri);
                    node.Attributes.Append(anode);
                    node = anode;
                }
                else
                {
                    XmlNode next = string.IsNullOrEmpty(namespaceUri) ? xmlDoc.CreateElement(localName) : xmlDoc.CreateElement(localName, namespaceUri);
                    node.AppendChild(next);
                    node = next;
                }
            }
            return node;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="serializedValues"></param>
        /// <returns></returns>
        private object SerializeToObject(Type type, string serializedValues)
        {
            serializedValues = serializedValues.StartsWith("{") ? serializedValues.Substring(1, serializedValues.LastIndexOf('}') - 1) : serializedValues;

            object instance = Activator.CreateInstance(type);
            if (string.IsNullOrEmpty(serializedValues))
            {
                return instance;
            }
            string[] array = serializedValues.Split("&".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

            for (int i = 0; i < array.Length; i++)
            {
                string[] propertyDetail = array[i].Split("=".ToCharArray(), 2, StringSplitOptions.RemoveEmptyEntries);
                if (propertyDetail.Length == 2)
                {
                    PropertyInfo property = type.GetProperty(propertyDetail[0]);
                    if (property != null)
                    {
                        property.SetValue(instance, Convert.ChangeType(propertyDetail[1], property.PropertyType, CultureInfo.CurrentCulture), BindingFlags.SetProperty, null, null, CultureInfo.CurrentCulture);
                    }
                }
                else if (propertyDetail.Length == 1)
                {
                    PropertyInfo[] properties = type.GetProperties();
                    if (properties[0] != null)
                    {
                        properties[0].SetValue(instance, Convert.ChangeType(propertyDetail[0], properties[0].PropertyType, CultureInfo.CurrentCulture), BindingFlags.SetProperty, null, null, CultureInfo.CurrentCulture);
                    }
                }
            }
            return instance;
        }
    }

    /// <summary>
    /// Xpath extension class 
    /// </summary>
    public static class XpathExtensions
    {
        /// <summary>
        /// Splits xpath into individual element set
        /// </summary>
        /// <param name="xpath"></param>
        /// <returns></returns>
        public static string[] SplitXpath(this string xpath)
        {
            if (xpath.Contains("local-name()"))
            {
                string[] separator = { "/*" };
                List<string> result = new List<string>();
                string[] split = xpath.Split(separator, StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < split.Length; i++)
                {
                    if (split[i].Contains("/@*"))
                    {
                        string[] attributeSeaparator = { "/@*" };
                        string[] attributeSplit = split[i].Split(attributeSeaparator, StringSplitOptions.RemoveEmptyEntries);

                        result.Add("*" + attributeSplit[0]);
                        result.Add("@*" + attributeSplit[1]);
                    }
                    else
                    {
                        result.Add("*" + split[i]);
                    }

                }

                return result.ToArray();
            }
            else
            {
                char[] separator = { '/' };
                return xpath.Split(separator, StringSplitOptions.RemoveEmptyEntries);
            }


        }

        /// <summary>
        /// Extracts local-name() and namespace-uri() from xpath part
        /// </summary>
        /// <param name="value"></param>
        /// <param name="localName"></param>
        /// <param name="namespaceUri"></param>
        public static void SplitXpathPart(this string value, out string localName, out string namespaceUri)
        {
            localName = "";
            namespaceUri = "";
            if (value != null)
            {
                if (value.Contains("local-name()"))
                {
                    string[] separator = { "and" };
                    value = value.Replace("[", "").Replace("]", "");
                    string[] splitPart = value.Split(separator, StringSplitOptions.RemoveEmptyEntries);

                    if (splitPart.Length == 2)
                    {
                        localName = splitPart[0].Split('=')[1].Replace("\'", "").Replace("\"", "").Trim();
                        namespaceUri = splitPart[1].Split('=')[1].Replace("\'", "").Replace("\"", "").Trim();
                    }
                    if (splitPart.Length == 1)
                    {
                        localName = splitPart[0].Split('=')[1].Replace("\'", "").Replace("\"", "").Trim();
                        namespaceUri = "";
                    }
                }
                else
                {
                    localName = value.Replace("/", "").Trim();
                    namespaceUri = "";
                }

            }
        }
    }
}
