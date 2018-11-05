using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Runtime.Serialization;
using Microsoft.BizTalk.Component.Interop;
using Microsoft.BizTalk.Message.Interop;
using Microsoft.XLANGs.BaseTypes;
using Avista.ESB.Utilities.DataAccess;
using Avista.ESB.Utilities.Logging;
using Avista.ESB.Utilities.Archive;
using System.Threading.Tasks;

namespace Avista.ESB.Utilities
{
    [Serializable]
    public class ArchiveManager
    {
        #region Private variables
        private string messageId = "";
        #endregion

        #region Public methods

        /// <summary>
        /// Archives a message.
        /// </summary>
        /// <param name="message">The message to be archived.</param>
        /// <param name="expiryMinutes">The amount of time (in minutes) before the archived message expires and will be automatically deleted from the archive. Specify a value of 0 to use the default configured expiry time.</param>
        /// <param name="includeProperties">A flag indicating if properties should be archived along with the message.</param>
        /// <param name="tag">A tag value to be associated with the archived message.</param>
        /// <param name="autoDisposeXLangMessage">When called from an orchestration, this flag should be set to true to dispose XLANGMessage after use.</param>        
        /// <returns>The id of the archived message.</returns>
        public string ArchiveMessage(XLANGMessage xLangMessage, int expiryMinutes, bool includeProperties, string tag, bool autoDisposeXLangMessage)
        {
            try
            {
                ArchiveTag archiveTag = new ArchiveTag(tag);
                ArchiveBizTalkMessage bizTalkMessage = new ArchiveBizTalkMessage(xLangMessage, archiveTag, false);
                messageId = bizTalkMessage.Message.MessageId.ToString();
                ArchiveMessageAsync(bizTalkMessage, expiryMinutes, includeProperties, archiveTag);               
            }
            catch (Exception ex)
            {
                Logger.WriteWarning(string.Format("Error archiving a message {0} \r\n Details: {1}", this.GetType().Name, ex.ToString()),14005);
            }
            finally
            {
                if (autoDisposeXLangMessage)
                {
                    xLangMessage.Dispose();
                }
            }
            return messageId;
        }

        /// <summary>
        /// Archives a message.
        /// </summary>
        /// <param name="pipelineContext">The pipeline context of the message to be archived.</param>
        /// <param name="baseMessage">The message to be archived.</param>
        /// <param name="expiryMinutes">The amount of time (in minutes) before the archived message expires and will be automatically deleted from the archive. Specify a value of 0 to use the default configured expiry time.</param>
        /// <param name="includeProperties">A flag indicating if properties should be archived along with the message.</param>
        /// <param name="tag">A tag value to be associated with the archived message.</param>
        /// <returns>The id of the archived message.</returns>
        public string ArchiveMessage(IPipelineContext pipelineContext, IBaseMessage baseMessage, int expiryMinutes, bool includeProperties, string tag)
        {            
            try
            {
                ArchiveTag archiveTag = new ArchiveTag(tag);
                ArchiveBizTalkMessage bizTalkMessage = new ArchiveBizTalkMessage(pipelineContext, baseMessage, archiveTag);
                messageId = bizTalkMessage.Message.MessageId.ToString();
                ArchiveMessageAsync(bizTalkMessage, expiryMinutes, includeProperties, archiveTag);
            }
            catch (Exception ex)
            {
                Logger.WriteWarning(string.Format("Error archiving a message {0} \r\n Details: {1}", this.GetType().Name, ex.ToString()),14005);                
            }
            return messageId;
        }

        /// <summary>
        /// Archives a message asynchronously
        /// </summary>
        /// <param name="bizTalkMessage"></param>
        /// <param name="expiryMinutes"></param>
        /// <param name="includeProperties"></param>
        /// <param name="archiveTag"></param>
        private async void ArchiveMessageAsync(ArchiveBizTalkMessage bizTalkMessage, int expiryMinutes, bool includeProperties, ArchiveTag archiveTag)
        {
            try
            {
                if (expiryMinutes <= 0)
                {
                    expiryMinutes = archiveTag.ArchiveType.DefaultExpiry;
                }
                await bizTalkMessage.Archive(expiryMinutes, includeProperties, archiveTag);
                //Log success
                Logger.WriteInformation("Message " + bizTalkMessage.Message.MessageId.ToString() + " has been archived.", 325);
            }
            catch (Exception ex)
            {
                Logger.WriteWarning(string.Format("Error archiving a  message asynchronously {0} \r\n Details: {1}", this.GetType().Name, ex.ToString()), 14005);                
            }
        }
        #endregion
    }
}
