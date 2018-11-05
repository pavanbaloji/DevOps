using Avista.ESB.Utilities.DataAccess;
using Avista.ESB.Utilities.Logging;
using System;
using System.IO;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Xml;
using Avista.ESB.Utilities.Cache;


namespace Avista.ESB.MessagingServices.Enrich
{
    public class Enrich
    {
        #region Private Variables

        private string charSet = "UTF-8";
        private string contentType = "text/xml";
        private XmlDocument contentAsXmlDocument = null;
        private String contentAsText = null;
        private byte[] contentAsByteArray = null;
        private string archiveTag = "";
        private string archiveType = string.Empty;
        private string sourceSystem = string.Empty;
        private string targetSystem = string.Empty;
        private string description = string.Empty;
        private Guid messageId = Guid.Empty;

        #endregion

        #region Public Properties

        public string ContentType
        {
            get
            {
                return contentType;
            }
        }

        /// <summary>
        /// The content of the part as an XmlDocument.
        /// </summary>
        public XmlDocument ContentAsXmlDocument
        {
            get
            {
                return contentAsXmlDocument;
            }
        }

        /// <summary>
        /// The content of the part as plain text.
        /// </summary>
        public string ContentAsText
        {
            get
            {
                return contentAsText;
            }
        }

        /// <summary>
        /// The content of the part as a byte array.
        /// </summary>
        public byte[] ContentAsByteArray
        {
            get
            {
                return contentAsByteArray;
            }
        }

        public Guid MessageId
        {
            get
            {
                return messageId;
            }
        }

        #endregion

        #region Constructor

        public Enrich(string cacheKey)
        {
            SqlBackingCache cache = new SqlBackingCache();

            string xml = (string)cache.Get(cacheKey);

            if (string.IsNullOrEmpty(xml))
                throw new Exception("Cache message not found for key: " + cacheKey);

            TextReader textReader = new StringReader(xml);
            contentAsXmlDocument = new XmlDocument();
            contentAsXmlDocument.Load(textReader);
        }
        public Enrich(string interchangeId, string tag)
        {
            SqlServerConnection connection = null;
            try
            {
                connection = new SqlServerConnection("MessageArchive");
                connection.RefreshConfiguration();
                connection.Open();
                SplitArchiveTag(tag);
                LoadParts(connection, interchangeId, tag);
            }
            catch (Exception exception)
            {
                throw exception;
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

        #endregion

        #region Private Methods

        private void SplitArchiveTag(string tag)
        {
            try
            {
                if (tag != null)
                {
                    if (tag.Contains("|"))
                    {
                        string[] fields = tag.Split('|');
                        archiveTag = fields[0];
                        if (fields.Length >= 2)
                        {
                            archiveType = fields[1];
                            if (fields.Length >= 3)
                            {
                                sourceSystem = fields[2];
                                if (fields.Length >= 4)
                                {
                                    targetSystem = fields[3];
                                    if (fields.Length >= 5)
                                    {
                                        description = fields[4];
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        archiveTag = tag;
                    }
                }
            }
            catch (Exception exception)
            {
                Logger.WriteTrace(string.Format("Error occured in {0} \r\n Details: {1}", this.GetType().Name, exception.ToString()));
                throw exception;
            }
        }

        private void LoadParts(SqlServerConnection connection, string interchangeId, string tag)
        {
            string sql = string.Empty;
            Guid _interchangeId = new Guid(interchangeId);
            try
            {
                StringBuilder sqlBuilder = new StringBuilder();
                sqlBuilder.Append("select ContentType,CharSet,TextData,ImageData,Msg.MessageId As MessageId FROM [MessageArchive].[dbo].[Part] Part inner join [MessageArchive].[dbo].[Message] Msg on part.MessageId=msg.MessageId");
                sqlBuilder.Append(archiveType != String.Empty ? " inner join [MessageArchive].[dbo].[ArchiveType] At on msg.ArchiveTypeId=At.Id " : "");
                sqlBuilder.Append(sourceSystem != String.Empty ? " inner join [MessageArchive].[dbo].[Endpoint] SE on msg.SourceSystemId=SE.Id " : "");
                sqlBuilder.Append(targetSystem != String.Empty ? " inner join [MessageArchive].[dbo].[Endpoint] TE on msg.TargetSystemId=TE.Id " : "");
                sqlBuilder.Append(" where Msg.InterchangeId=@MessageId and Msg.Tag=@ArchiveTag ");
                sqlBuilder.Append(archiveType != String.Empty ? " and At.Name=@ArchiveType " : "");
                sqlBuilder.Append(sourceSystem != String.Empty ? " and SE.Name=@SourceSystem " : "");
                sqlBuilder.Append(targetSystem != String.Empty ? " and TE.Name=@TargetSystem " : "");
                sqlBuilder.Append(" and Msg.Description=@Description ");
                sql = sqlBuilder.ToString();
                SqlCommand sqlCommand = new SqlCommand(sql);
                SqlParameter parmMessageId = sqlCommand.Parameters.Add("@MessageId", SqlDbType.UniqueIdentifier);
                SqlParameter parmArchiveTag = sqlCommand.Parameters.Add("@ArchiveTag", SqlDbType.NVarChar);
                SqlParameter parmArchiveType = sqlCommand.Parameters.Add("@ArchiveType", SqlDbType.NVarChar);
                SqlParameter parmSourceSystem = sqlCommand.Parameters.Add("@SourceSystem", SqlDbType.NVarChar);
                SqlParameter parmTargetSystem = sqlCommand.Parameters.Add("@TargetSystem", SqlDbType.NVarChar);
                SqlParameter parmDescription = sqlCommand.Parameters.Add("@Description", SqlDbType.NVarChar);
                parmMessageId.Value = _interchangeId;
                parmArchiveTag.Value = archiveTag;
                parmArchiveType.Value = archiveType;
                parmSourceSystem.Value = sourceSystem;
                parmTargetSystem.Value = targetSystem;
                parmDescription.Value = description;
                using (SqlDataReader reader = connection.ExecuteReader(sqlCommand))
                {
                    while (reader.Read())
                    {
                        IDataRecord partRecord = (IDataRecord)reader;
                        contentType = (string)partRecord["ContentType"];
                        charSet = (string)partRecord["CharSet"];
                        messageId = (Guid)partRecord["MessageId"];
                        if (contentType == "text/xml")
                        {
                            string xml = (string)partRecord["TextData"];
                            TextReader textReader = new StringReader(xml);
                            contentAsXmlDocument = new XmlDocument();
                            contentAsXmlDocument.Load(textReader);
                        }
                        else if (contentType == "text/plain")
                        {
                            contentAsText = (string)partRecord["TextData"];
                        }
                        else if (contentType == "application/octet-stream" || contentType == "binary")
                        {
                            contentAsByteArray = (byte[])partRecord["ImageData"];
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                Logger.WriteTrace(string.Format("Load of MessageProperties records from message archive failed." + sql + "\r\n", exception.ToString()));
                throw exception;
            }
        }

        #endregion
    }

}
