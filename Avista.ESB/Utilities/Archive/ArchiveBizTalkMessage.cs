using Avista.ESB.Utilities.Logging;
using Microsoft.BizTalk.Component.Interop;
using Microsoft.BizTalk.Message.Interop;
using Microsoft.XLANGs.BaseTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Threading.Tasks;
using System.Xml;
using Microsoft.XLANGs.Core;
using System.IO;
using System.Collections;
using Avista.ESB.Utilities.DataAccess;
using System.Data.SqlClient;
using System.Data;
using System.Xml.Linq;
using System.Xml.XPath;
using System.Xml.Serialization;

namespace Avista.ESB.Utilities.Archive
{
    public class ArchiveBizTalkMessage
    {
        private string partType = string.Empty;
        private static MethodInfo msgUnwrapMethod = null;
        private static MethodInfo partUnwrapMethod = null;

        #region Properties
        public Message Message { get; set; }
        public List<MessageProperty> MessageProperties { get; set; }
        public List<Part> Parts { get; set; }
        public List<PartProperty> PartsProperties { get; set; }
        #endregion

        #region Constructors
        /// <summary>
        /// 
        /// </summary>
        public ArchiveBizTalkMessage()
        {
            Type messageWrapperType = typeof(MessageWrapperForUserCode);
            msgUnwrapMethod = messageWrapperType.GetMethod("Unwrap", BindingFlags.Instance | BindingFlags.NonPublic);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="xLangMessage"></param>
        /// <param name="tag"></param>
        /// <param name="autoDispose"></param>
        public ArchiveBizTalkMessage(XLANGMessage xLangMessage, ArchiveTag tag, bool autoDispose = true)
        {
            this.Message = new Message();
            this.MessageProperties = new List<MessageProperty>();
            this.Parts = new List<Part>();
            this.PartsProperties = new List<PartProperty>();

            Guid messageId = Guid.NewGuid();

            try
            {
                if (xLangMessage is MessageWrapperForUserCode)
                {
                    //-------------------------------------------------------------------------
                    // Add Message.
                    //-------------------------------------------------------------------------
                    this.Message.MessageId = messageId;
                    if (tag.ArchiveType.Id >= 0)
                        this.Message.ArchiveTypeId = tag.ArchiveType.Id;

                    this.Message.Tag = tag.Tag;

                    if (tag.SourceSystem.Id >= 0)
                        this.Message.SourceSystemId = tag.SourceSystem.Id;

                    if (tag.TargetSystem.Id >= 0)
                        this.Message.TargetSystemId = tag.TargetSystem.Id;

                    this.Message.Description = tag.Description;
                    this.Message.InsertedDate = DateTime.UtcNow;

                    Type messageWrapperType = typeof(MessageWrapperForUserCode);
                    msgUnwrapMethod = messageWrapperType.GetMethod("Unwrap", BindingFlags.Instance | BindingFlags.NonPublic);

                    MessageWrapperForUserCode messageWrapper = (MessageWrapperForUserCode)xLangMessage;
                    XMessage xMessage = (XMessage)(msgUnwrapMethod.Invoke(messageWrapper, null));

                    if (xMessage != null)
                    {
                        try
                        {
                            //-------------------------------------------------------------------------
                            // Add the parts.
                            //-------------------------------------------------------------------------
                            int partCount = xLangMessage.Count;
                            for (int partIndex = 0; partIndex < partCount; partIndex++)
                            {
                                XLANGPart part = xLangMessage[partIndex];
                                try
                                {
                                    Part prt = GetMessagePart(messageId, partIndex, xMessage, part);
                                    if (prt != null)
                                    {
                                        this.Parts.Add(prt);
                                        //-------------------------------------------------------------------------
                                        // Add the parts properties.
                                        //-------------------------------------------------------------------------
                                        List<PartProperty> prtProperties = GetPartProperties(prt.PartId, part);
                                        foreach (PartProperty p in prtProperties)
                                        {
                                            this.PartsProperties.Add(p);
                                        }
                                    }
                                }
                                finally
                                {
                                    // part is actually a PartWrapperForUserCode. Calling its Dispose method causes
                                    // the PartWrapperForUserCode to be detached from the owning MessageWrapperForUserCode.
                                    part.Dispose();
                                }
                            }
                            //-------------------------------------------------------------------------
                            // Add the message properties.
                            //-------------------------------------------------------------------------
                            Hashtable propertyHashTable = xMessage.GetContextProperties();
                            if (propertyHashTable != null)
                            {
                                XmlQNameTable propertyTable = new XmlQNameTable(propertyHashTable);
                                int propertyIndex = 0;
                                MessageProperty msgProperties = new MessageProperty();
                                msgProperties.MessageId = messageId;
                                XElement ContextData = new XElement("ContextData");
                                List<string> listOfContextProperties = new List<string>();
                                listOfContextProperties = GetListOfContextProperties();
                                foreach (DictionaryEntry property in propertyTable)
                                {
                                    XmlQName qName = (XmlQName)property.Key;

                                    if (listOfContextProperties.Contains(qName.Name))
                                    {
                                        ContextData.Add(
                                                new XElement("Property", new XAttribute("Name", qName.Name),
                                                                         new XAttribute("Namespace", qName.Namespace),
                                                                         new XAttribute("Value", property.Value.ToString())));
                                    }

                                    if (qName.Namespace == "http://schemas.microsoft.com/BizTalk/2003/system-properties")
                                    {
                                        if (qName.Name == "InterchangeID")
                                        {
                                            string value = property.Value.ToString().Trim();
                                            this.Message.InterchangeId = GetGUIDWithoutBraces(value);
                                        }
                                        else if (qName.Name == "MessageType")
                                        {
                                            this.Message.MessageType = property.Value.ToString();
                                        }
                                    }
                                    else if (qName.Namespace == "http://schemas.microsoft.com/BizTalk/2003/messagetracking-properties")
                                    {
                                        if (qName.Name == "ActivityIdentity")
                                        {
                                            string value = property.Value.ToString().Trim();
                                            this.Message.ActivityId = GetGUIDWithoutBraces(value);
                                        }
                                    }
                                    propertyIndex++;
                                }
                                msgProperties.ContextData = ContextData.ToString();
                                this.MessageProperties.Add(msgProperties);
                                // If the message type is still unknown, try to get it from part[0]. 
                                if (string.IsNullOrEmpty(this.Message.MessageType) || this.Message.MessageType == "Unknown")
                                {
                                    if (!string.IsNullOrEmpty(partType))
                                    {
                                        this.Message.MessageType = partType;
                                    }
                                }
                            }

                        }
                        finally
                        {
                            // When the MessageWrapperForUserCode is unrwapped the reference count
                            // for the message is incremented, so we must release it now.
                            xMessage.Release();
                        }
                    }
                    else
                    {
                        throw new Exception("Could not unwrap XMessage from MessageWrapperForUserCode.");
                    }
                }
                else
                {
                    throw new Exception("Expected XLANGMessage to be a MessageWrapperForUserCode. " + xLangMessage.GetType().FullName + " is not a recognized XLANGMessage type.");
                }
            }
            catch (Exception ex)
            {
                Logger.WriteTrace(string.Format("Error constructing BizTalkMessage from XLangMessage {0} \r\n Details: {1}", this.GetType().Name, ex.ToString()));
                throw ex;
            }
            finally
            {
                if (autoDispose)
                {
                    xLangMessage.Dispose();
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pipelineContext"></param>
        /// <param name="message"></param>
        /// <param name="tag"></param>
        public ArchiveBizTalkMessage(IPipelineContext pipelineContext, IBaseMessage message, ArchiveTag tag)
        {
            this.Message = new Message();
            this.MessageProperties = new List<MessageProperty>();
            this.Parts = new List<Part>();
            this.PartsProperties = new List<PartProperty>();

            Guid messageId = Guid.NewGuid();

            try
            {
                //-------------------------------------------------------------------------
                // Add message.
                //-------------------------------------------------------------------------
                this.Message.MessageId = messageId;
                if (tag.ArchiveType.Id >= 0)
                    this.Message.ArchiveTypeId =  tag.ArchiveType.Id;

                this.Message.Tag = tag.Tag;

                if (tag.SourceSystem.Id >= 0)
                    this.Message.SourceSystemId = tag.SourceSystem.Id;

                if (tag.TargetSystem.Id >= 0)
                    this.Message.TargetSystemId = tag.TargetSystem.Id;

                this.Message.Description = tag.Description;
                this.Message.InsertedDate = DateTime.UtcNow;

                //-------------------------------------------------------------------------
                // Add the parts.
                //-------------------------------------------------------------------------
                int partCount = message.PartCount;

                for (int partIndex = 0; partIndex < partCount; partIndex++)
                {
                    string partName = string.Empty;
                    IBaseMessagePart part = message.GetPartByIndex(partIndex, out partName);
                    Part prt = GetMessagePart(messageId, partIndex, pipelineContext, part, partName);
                    if (prt != null)
                    {
                        this.Parts.Add(prt);
                        //-------------------------------------------------------------------------
                        // Add the parts properties.
                        //-------------------------------------------------------------------------
                        List<PartProperty> prtProperties = GetPartProperties(prt.PartId, part);
                        foreach (PartProperty p in prtProperties)
                        {
                            this.PartsProperties.Add(p);
                        }
                    }
                }
                //-------------------------------------------------------------------------
                // Add the message properties.
                //-------------------------------------------------------------------------
                int propertyCount = (int)message.Context.CountProperties;
                MessageProperty msgProperties = new MessageProperty();
                msgProperties.MessageId = messageId;
                XElement ContextData = new XElement("ContextData");
                List<string> listOfContextProperties = new List<string>();
                listOfContextProperties = GetListOfContextProperties();
                for (int propertyIndex = 0; propertyIndex < propertyCount; propertyIndex++)
                {

                    string propertyName = null;
                    string propertyNamespace = null;
                    object propertyValue = message.Context.ReadAt(propertyIndex, out propertyName, out propertyNamespace);
                    if (listOfContextProperties.Contains(propertyName))
                    {
                        ContextData.Add(
                                new XElement("Property", new XAttribute("Name", propertyName),
                                                         new XAttribute("Namespace", propertyNamespace),
                                                         new XAttribute("Value", propertyValue.ToString())));
                    }

                    if (propertyNamespace == "http://schemas.microsoft.com/BizTalk/2003/system-properties")
                    {
                        if (propertyName == "InterchangeID")
                        {
                            string value = propertyValue.ToString().Trim();
                            this.Message.InterchangeId = GetGUIDWithoutBraces(value);
                        }
                        else if (propertyName == "MessageType")
                        {
                            this.Message.MessageType = propertyValue.ToString();
                        }
                    }
                    else if (propertyNamespace == "http://schemas.microsoft.com/BizTalk/2003/messagetracking-properties")
                    {
                        if (propertyName == "ActivityIdentity")
                        {
                            string value = propertyValue.ToString().Trim();
                            this.Message.ActivityId = GetGUIDWithoutBraces(value);
                        }
                    }
                }
                msgProperties.ContextData = ContextData.ToString();
                this.MessageProperties.Add(msgProperties);
                // If the message type is still unknown, try to get it from part[0]. 
                if (string.IsNullOrEmpty(this.Message.MessageType) || this.Message.MessageType == "Unknown")
                {
                    if (!string.IsNullOrEmpty(partType))
                    {
                        this.Message.MessageType = partType;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.WriteTrace(string.Format("Error constructing BizTalkMessage from IBaseMessage {0} \r\n Details: {1}", this.GetType().Name, ex.ToString()));
                throw ex;
            }
        }

        #endregion

        #region public methods

        /// <summary>
        /// Archive the message and its parts and optionally archives the message and part properties.
        /// </summary>
        /// <param name="expiryMinutes">The amount of time (in minutes) before the archived message expires and will be automatically deleted from the archive.</param>
        /// <param name="includeProperties">A flag indicating if properties should be archived along with the message.</param>
        /// <param name="tag">An ArchiveTag to be associated with the archived message.</param>
        public Task Archive(int expiryMinutes, bool includeProperties, ArchiveTag tag)
        {
            return Task.Factory.StartNew(() =>
            {
                try
                {
                    if (tag.ArchiveType.Active)
                    {
                        using (SqlServerConnection connection = new SqlServerConnection("MessageArchive"))
                        {
                            connection.RefreshConfiguration();
                            connection.Open();
                            connection.BeginTransaction();
                            try
                            {
                                ArchiveMessage(expiryMinutes, connection);
                                if (includeProperties)
                                {
                                    ArchiveMessageProperties(connection);
                                }
                                ArchiveParts(connection);
                                if (includeProperties)
                                {
                                    ArchivePartProperties(connection);
                                }
                                connection.Commit();
                            }
                            catch (Exception ex)
                            {
                                if (connection.IsOpen())
                                {
                                    connection.Rollback();
                                }
                                throw ex;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.WriteError(string.Format("Error archiving a message to database. The message will be writen to archive directory for later archival.  \r\n Details: {0}",ex.ToString()), 128);
                    HandleFailedArchiveMessage();
                }

            });

        }

        #endregion

        #region Private Methods
        /// <summary>
        /// 
        /// </summary>
        private void HandleFailedArchiveMessage()
        {
            StringBuilder sbArchiveMsg = new StringBuilder("<ns0:Request xmlns:ns0='http://Avista.ESB.Utilities.Schemas.TableBulkOperation'>");

            //Message insert request
            string message = SerializeToString<Message>(this.Message);
            if (!string.IsNullOrEmpty(message))
                sbArchiveMsg.Append(string.Format("<ns1:Insert xmlns:ns1='http://schemas.microsoft.com/Sql/2008/05/TableOp/dbo/Message'><ns1:Rows>{0}</ns1:Rows></ns1:Insert>", message));

            //Message properties insert request
            StringBuilder messageProperties = new StringBuilder();
            foreach (MessageProperty property in this.MessageProperties)
            {
                messageProperties.Append(SerializeToString<MessageProperty>(property));
            }
            if (!string.IsNullOrEmpty(messageProperties.ToString()))
                sbArchiveMsg.Append(string.Format("<ns3:Insert xmlns:ns3='http://schemas.microsoft.com/Sql/2008/05/TableOp/dbo/MessageProperty'><ns3:Rows>{0}</ns3:Rows></ns3:Insert>", messageProperties.ToString()));

            //Message parts insert request
            StringBuilder messageParts = new StringBuilder();
            foreach (Part part in this.Parts)
            {
                messageParts.Append(SerializeToString<Part>(part));
            }
            if (!string.IsNullOrEmpty(messageParts.ToString()))
                sbArchiveMsg.Append(string.Format("<ns4:Insert xmlns:ns4='http://schemas.microsoft.com/Sql/2008/05/TableOp/dbo/Part'><ns4:Rows>{0}</ns4:Rows></ns4:Insert>", messageParts.ToString()));

            //Part properties insert request
            StringBuilder partProperties = new StringBuilder();
            foreach (PartProperty property in this.PartsProperties)
            {
                partProperties.Append(SerializeToString<PartProperty>(property));
            }
            if (!string.IsNullOrEmpty(partProperties.ToString()))
                sbArchiveMsg.Append(string.Format("<ns5:Insert xmlns:ns5='http://schemas.microsoft.com/Sql/2008/05/TableOp/dbo/PartProperty'><ns5:Rows>{0}</ns5:Rows></ns5:Insert>", partProperties.ToString()));

            //closing tag
            sbArchiveMsg.Append("</ns0:Request>");


            File.WriteAllText(string.Format(@"C:\Program Files (x86)\Avista\Avista.ESB.Common\Ports\Archives\{0}", this.Message.MessageId.ToString() + ".xml"), sbArchiveMsg.ToString());

        }

        /// <summary>
        /// Serializes object to xml string
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        private string SerializeToString<T>(T value)
        {
            var emptyNamepsaces = new XmlSerializerNamespaces(new[] { XmlQualifiedName.Empty });
            var serializer = new XmlSerializer(value.GetType());
            var settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.OmitXmlDeclaration = true;

            using (var stream = new StringWriter())
            using (var writer = XmlWriter.Create(stream, settings))
            {
                serializer.Serialize(writer, value, emptyNamepsaces);
                return stream.ToString();
            }
        }

        /// <summary>
        /// Gets Message Part from IBaseMessagePart
        /// </summary>
        /// <param name="messageId"></param>
        /// <param name="partIndex"></param>
        /// <param name="pipelineContext"></param>
        /// <param name="part"></param>
        /// <param name="partName"></param>
        /// <returns></returns>
        private Part GetMessagePart(Guid messageId, int partIndex, IPipelineContext pipelineContext, IBaseMessagePart part, string partName)
        {
            Part prt = null;
            if (part != null)
            {
                prt = new Part();
                prt.PartId = Guid.NewGuid();
                prt.MessageId = messageId;
                prt.PartIndex = partIndex;
                prt.PartName = partName;
                prt.CharSet = "UTF-8";

                XmlDocument contentAsXmlDocument = MessageHelper.MessageToXmlDocument(pipelineContext, part);
                if (contentAsXmlDocument != null)
                {
                    prt.TextData = contentAsXmlDocument.OuterXml;
                    prt.ContentType = "text/xml";
                    partType = MessageHelper.GetMessageType(contentAsXmlDocument);
                }
                else
                {
                    string contentAsText = MessageHelper.MessageToText(pipelineContext, part);

                    if (contentAsText != null)
                    {
                        prt.TextData = contentAsText;
                        prt.ContentType = "text/plain";
                        partType = "FlatFile";
                    }
                    else
                    {
                        prt.ImageData = MessageHelper.MessageToByteArray(pipelineContext, part);
                        prt.ContentType = "application/octet-stream";
                        partType = "Binary";
                    }
                }
            }

            return prt;
        }

        /// <summary>
        /// Gets Message Part from XLANGMessage
        /// </summary>
        /// <param name="messageId"></param>
        /// <param name="partIndex"></param>
        /// <param name="xMessage"></param>
        /// <param name="part"></param>
        /// <returns></returns>
        private Part GetMessagePart(Guid messageId, int partIndex, XMessage xMessage, XLANGPart part)
        {
            Part prt = null;
            if (part != null)
            {
                prt = new Part();
                prt.PartId = Guid.NewGuid();
                prt.MessageId = messageId;
                prt.PartIndex = partIndex;
                prt.PartName = part.Name;
                prt.CharSet = "UTF-8";

                if (part is PartWrapperForUserCode)
                {
                    Type partWrapperType = typeof(PartWrapperForUserCode);
                    partUnwrapMethod = partWrapperType.GetMethod("Unwrap", BindingFlags.Instance | BindingFlags.NonPublic);

                    PartWrapperForUserCode partWrapper = (PartWrapperForUserCode)part;
                    Microsoft.XLANGs.Core.Part xPart = (Microsoft.XLANGs.Core.Part)(partUnwrapMethod.Invoke(partWrapper, null));
                    if (xPart != null)
                    {
                        try
                        {
                            try
                            {
                                XmlDocument contentAsXmlDocument = (XmlDocument)part.RetrieveAs(typeof(XmlDocument));
                                if (contentAsXmlDocument != null)
                                {
                                    prt.TextData = contentAsXmlDocument.OuterXml;
                                    prt.ContentType = "text/xml";
                                    partType = MessageHelper.GetMessageType(contentAsXmlDocument);
                                }
                            }
                            catch (Exception)
                            {
                                using (Stream partStream = (Stream)part.RetrieveAs(typeof(Stream)))
                                {
                                    using (StreamReader streamReader = new StreamReader(partStream))
                                    {
                                        prt.TextData = streamReader.ReadToEnd();
                                        prt.ContentType = "text/plain";
                                        partType = "FlatFile";
                                    }
                                }
                            }
                        }
                        finally
                        {
                            // When the PartWrapperForUserCode is unrwapped the reference count for the
                            // owning message is incremented, so we must release it now.
                            xMessage.Release();
                        }
                    }
                    else
                    {
                        throw new Exception("Could not unwrap Part from PartWrapperForUserCode.");
                    }
                }
                else
                {
                    throw new Exception("Error constructing BizTalkMessagePart. Expected XLANGPart to be a PartWrapperForUserCode. " + part.GetType().FullName + " is not a recognized XLANGPart type.");
                }
            }

            return prt;
        }

        /// <summary>
        /// Gets Part Properties from IBaseMessagePart
        /// </summary>
        /// <param name="partId"></param>
        /// <param name="part"></param>
        /// <returns></returns>
        private List<PartProperty> GetPartProperties(Guid partId, IBaseMessagePart part)
        {
            int propertyCount = (int)part.PartProperties.CountProperties;
            List<PartProperty> prtProperties = new List<PartProperty>();

            for (int propertyIndex = 0; propertyIndex < propertyCount; propertyIndex++)
            {
                PartProperty prtProp = new PartProperty();
                string propertyName = null;
                string propertyNamespace = null;
                object propertyValue = part.PartProperties.ReadAt(propertyIndex, out propertyName, out propertyNamespace);

                prtProp.PartId = partId;
                prtProp.PropertyIndex = propertyIndex;
                prtProp.Name = propertyName;
                prtProp.Namespace = propertyNamespace;
                prtProp.Value = (string)propertyValue;

                prtProperties.Add(prtProp);
            }

            return prtProperties;
        }

        /// <summary>
        /// Gets Part Properties from XLANGPart
        /// </summary>
        /// <param name="partId"></param>
        /// <param name="part"></param>
        /// <returns></returns>
        private List<PartProperty> GetPartProperties(Guid partId, XLANGPart part)
        {
            List<PartProperty> prtProperties = new List<PartProperty>();
            if (part is PartWrapperForUserCode)
            {
                PartWrapperForUserCode partWrapper = (PartWrapperForUserCode)part;
                Microsoft.XLANGs.Core.Part xPart = (Microsoft.XLANGs.Core.Part)(partUnwrapMethod.Invoke(partWrapper, null));

                if (xPart != null)
                {
                    XmlQNameTable propertyTable = xPart.GetPartProperties();
                    if (propertyTable != null)
                    {
                        int propertyIndex = 0;
                        foreach (DictionaryEntry property in propertyTable)
                        {
                            XmlQName qName = (XmlQName)property.Key;
                            PartProperty prtProp = new PartProperty();

                            prtProp.PartId = partId;
                            prtProp.PropertyIndex = propertyIndex;
                            prtProp.Name = qName.Name;
                            prtProp.Namespace = qName.Namespace;
                            prtProp.Value = property.Value.ToString();

                            propertyIndex++;
                            prtProperties.Add(prtProp);
                        }
                    }
                }

            }

            return prtProperties;
        }

        /// <summary>
        /// Returns the GUID value without braces.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private string GetGUIDWithoutBraces(string value)
        {
            // If it is a GUID with braces then we will remove the braces.
            if (value.Length == 38 && value[0] == '{' && value[37] == '}')
            {
                value = value.Substring(1, 36);
            }
            return value;
        }

        /// <summary>
        /// Archives the message (not including parts or properties).
        /// </summary>
        /// <param name="expiryMinutes">The amount of time (in minutes) before the archived message expires and will be automatically deleted from the archive.</param>
        /// <param name="tag">A tag value to be associated with the archived message.</param>
        /// <param name="connection">The connection to SQL Server that will be used to perform the archiving operation.</param>
        private void ArchiveMessage(int expiryMinutes, SqlServerConnection connection)
        {
            string sql =
                    "INSERT INTO [MessageArchive].[dbo].[Message] " +
                    "([MessageId], [InterchangeId], [MessageType], [ActivityId], [Tag], [InsertedDate], [ArchiveTypeId], [SourceSystemId], [TargetSystemId], [Description])" +
                    " VALUES " +
                    "(@MessageId, @InterchangeId, @MessageType, @ActivityId, @Tag, @InsertedDate, @ArchiveTypeId, @SourceSystemId, @TargetSystemId, @Description)";

            SqlCommand sqlCommand = new SqlCommand(sql);
            try
            {
                DateTime insertedDate = DateTime.UtcNow;
                DateTime expiryDate = insertedDate + new TimeSpan(0, expiryMinutes, 0);


                SqlParameter parmMessageId = sqlCommand.Parameters.Add("@MessageId", SqlDbType.UniqueIdentifier);
                SqlParameter parmInterchangeId = sqlCommand.Parameters.Add("@InterchangeId", SqlDbType.NVarChar, 36);
                SqlParameter parmMessageType = sqlCommand.Parameters.Add("@MessageType", SqlDbType.NVarChar, 256);
                SqlParameter parmActivityId = sqlCommand.Parameters.Add("@ActivityId", SqlDbType.NVarChar, 36);
                SqlParameter parmTag = sqlCommand.Parameters.Add("@Tag", SqlDbType.NVarChar, 50);
                SqlParameter parmInsertedDate = sqlCommand.Parameters.Add("@InsertedDate", SqlDbType.DateTime);
                SqlParameter parmArchiveType = sqlCommand.Parameters.Add("@ArchiveTypeId", SqlDbType.Int);
                SqlParameter parmSourceSystem = sqlCommand.Parameters.Add("@SourceSystemId", SqlDbType.Int);
                SqlParameter parmTargetSystem = sqlCommand.Parameters.Add("@TargetSystemId", SqlDbType.Int);
                SqlParameter parmDescription = sqlCommand.Parameters.Add("@Description", SqlDbType.NVarChar, 250);

                parmMessageId.Value = this.Message.MessageId;
                parmInterchangeId.Value = (this.Message.InterchangeId == null ? (object)DBNull.Value : (object)this.Message.InterchangeId);
                parmMessageType.Value = this.Message.MessageType;
                parmActivityId.Value = (this.Message.ActivityId == null ? (object)DBNull.Value : (object)this.Message.ActivityId);
                parmTag.Value = this.Message.Tag;
                parmInsertedDate.Value = this.Message.InsertedDate;

                parmArchiveType.Value = (this.Message.ArchiveTypeId == null ? (object)DBNull.Value : (object)this.Message.ArchiveTypeId);
                parmSourceSystem.Value = (this.Message.SourceSystemId == null ? (object)DBNull.Value : (object)this.Message.SourceSystemId);
                parmTargetSystem.Value = (this.Message.TargetSystemId == null ? (object)DBNull.Value : (object)this.Message.TargetSystemId);
                parmDescription.Value = (this.Message.Description == null ? (object)DBNull.Value : (object)this.Message.Description);

                int rows = connection.ExecuteNonQuery(sqlCommand);
            }
            catch (Exception exception)
            {
                string strSqlInsertQuery = GenerateSqlString(sqlCommand);
                throw new Exception("Insertion of the message record into the archive failed for this query : " + strSqlInsertQuery, exception);
            }
        }

        /// <summary>
        /// Archives the message properties.
        /// </summary>
        /// <param name="connection">The connection to SQL Server that will be used to perform the archiving operation.</param>
        private void ArchiveMessageProperties(SqlServerConnection connection)
        {
            try
            {
                string sql =
                    "INSERT INTO [MessageArchive].[dbo].[MessageProperty] " +
                    "([MessageId], [ContextData])" +
                    " VALUES " +
                    "(@MessageId, @ContextData)";
                int index = 0;

                foreach (MessageProperty property in this.MessageProperties)
                {
                    index++;
                    SqlCommand sqlCommand = new SqlCommand(sql);
                    SqlParameter parmMessageId = sqlCommand.Parameters.Add("@MessageId", SqlDbType.UniqueIdentifier);
                    SqlParameter parmContextData = sqlCommand.Parameters.Add("@ContextData", SqlDbType.NVarChar);

                    parmMessageId.Value = property.MessageId;
                    parmContextData.Value = property.ContextData;
                    int rows = connection.ExecuteNonQuery(sqlCommand);
                }
            }
            catch (Exception exception)
            {
                throw new Exception("Insertion of message property records into the archive failed.", exception);
            }
        }

        /// <summary>
        /// Archives the parts of the message.
        /// </summary>
        /// <param name="includeProperties">A flag indicating if properties should be archived along with the message.</param>
        /// <param name="connection">The connection to SQL Server that will be used to perform the archiving operation.</param>
        private void ArchiveParts(SqlServerConnection connection)
        {
            string sql =
                    "INSERT INTO [MessageArchive].[dbo].[Part] " +
                    "([MessageId], [PartId], [PartName], [PartIndex], [ContentType], [CharSet], [TextData], [ImageData])" +
                    " VALUES " +
                    "(@MessageId, @PartId, @PartName, @PartIndex, @ContentType, @CharSet, @TextData, @ImageData)";

            foreach (Part part in this.Parts)
            {
                try
                {
                    SqlCommand sqlCommand = new SqlCommand(sql);
                    SqlParameter parmMessageId = sqlCommand.Parameters.Add("@MessageId", SqlDbType.UniqueIdentifier);
                    SqlParameter parmPartId = sqlCommand.Parameters.Add("@PartId", SqlDbType.UniqueIdentifier);
                    SqlParameter parmPartName = sqlCommand.Parameters.Add("@PartName", SqlDbType.NVarChar, 256);
                    SqlParameter parmPartIndex = sqlCommand.Parameters.Add("@PartIndex", SqlDbType.Int);
                    SqlParameter parmContentType = sqlCommand.Parameters.Add("@ContentType", SqlDbType.NVarChar, 50);
                    SqlParameter parmCharSet = sqlCommand.Parameters.Add("@CharSet", SqlDbType.NVarChar, 50);
                    SqlParameter parmTextData = sqlCommand.Parameters.Add("@TextData", SqlDbType.NVarChar);
                    SqlParameter parmImageData = sqlCommand.Parameters.Add("@ImageData", SqlDbType.Image);
                    parmMessageId.Value = part.MessageId;
                    parmPartId.Value = part.PartId;
                    parmPartName.Value = part.PartName;
                    parmPartIndex.Value = part.PartIndex;
                    parmContentType.Value = part.ContentType;
                    parmCharSet.Value = part.CharSet;
                    parmTextData.Value = part.TextData ?? (object)DBNull.Value;
                    parmImageData.Value = ((part.ImageData != null) && (part.ImageData.Length > 0))
                        ? part.ImageData
                        : (object) DBNull.Value;

                    int rows = connection.ExecuteNonQuery(sqlCommand);
                }
                catch (Exception exception)
                {
                    throw new Exception("Insertion of the part record into the archive failed.", exception);
                }
            }
        }

        /// <summary>
        /// Archives the properties of the message part.
        /// </summary>
        /// <param name="connection">The connection to SQL Server that will be used to perform the archiving operation.</param>
        private void ArchivePartProperties(SqlServerConnection connection)
        {
            try
            {
                string sql =
                    "INSERT INTO [MessageArchive].[dbo].[PartProperty] " +
                    "([PartId], [PropertyIndex], [Namespace], [Name], [Value])" +
                    " VALUES " +
                    "(@PartId, @PropertyIndex, @Namespace, @Name, @Value)";
                int index = 0;
                foreach (PartProperty partProp in this.PartsProperties)
                {
                    index++;
                    SqlCommand sqlCommand = new SqlCommand(sql);
                    SqlParameter parmPartId = sqlCommand.Parameters.Add("@PartId", SqlDbType.UniqueIdentifier);
                    SqlParameter parmPropertyIndex = sqlCommand.Parameters.Add("@PropertyIndex", SqlDbType.Int);
                    SqlParameter parmNamespace = sqlCommand.Parameters.Add("@Namespace", SqlDbType.NVarChar, 256);
                    SqlParameter parmName = sqlCommand.Parameters.Add("@Name", SqlDbType.NVarChar, 256);
                    SqlParameter parmValue = sqlCommand.Parameters.Add("@Value", SqlDbType.NVarChar);

                    parmPartId.Value = partProp.PartId;
                    parmPropertyIndex.Value = partProp.PropertyIndex;
                    parmNamespace.Value = partProp.Namespace;
                    parmName.Value = partProp.Name;
                    parmValue.Value = partProp.Value;
                    int rows = connection.ExecuteNonQuery(sqlCommand);
                }
            }
            catch (Exception exception)
            {
                throw new Exception("Insertion of the part property records into the archive failed.", exception);
            }
        }

        /// <summary>
        /// Gets string represantaion of sqlCommand
        /// </summary>
        /// <param name="sqlCommand"></param>
        /// <returns></returns>
        private string GenerateSqlString(SqlCommand sqlCommand)
        {
            string sqlQuery = sqlCommand.CommandText;
            foreach (SqlParameter sqlParam in sqlCommand.Parameters)
            {

                sqlQuery = sqlQuery.Replace(sqlParam.ParameterName, sqlParam.Value.ToString());
            }

            return sqlQuery;
        }

        /// <summary>
        /// List of Context properties we need to inser in Message Property Table.
        /// </summary>        
        /// <returns>List of string</returns>
        private List<string> GetListOfContextProperties()
        {
            List<string> lst = new List<string>();
            lst.Add("AdapterReceiveCompleteTime");
            lst.Add("IsRequestResponse");
            lst.Add("SchemaStrongName");
            lst.Add("OutboundTransportType");
            lst.Add("DocumentSpecName");
            lst.Add("InboundHeaders");
            lst.Add("InboundTransportLocation");
            lst.Add("ReceivePipelineConfig");
            lst.Add("FileCreationTime");
            lst.Add("HeaderSpecName");
            lst.Add("ServiceType");
            lst.Add("ServiceName");
            lst.Add("FileName");
            lst.Add("ItineraryHeader");
            lst.Add("ReceivedFileName");
            lst.Add("ErrorInnerMsg");
            lst.Add("MessageType");
            lst.Add("ReceivePortName");
            lst.Add("Action");
            lst.Add("OutboundTransportLocation");
            lst.Add("ItineraryName");
            lst.Add("ReceiveLocationName");
            lst.Add("Operation");
            lst.Add("ServiceState");
            lst.Add("SPName");
            lst.Add("PortName");
            lst.Add("InboundHttpHeaders");
            lst.Add("InboundTransportType");
            lst.Add("InboundHttpStatusCode");
            return lst;
        }
        #endregion
    }
}
