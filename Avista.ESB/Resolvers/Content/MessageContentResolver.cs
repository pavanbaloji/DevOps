using Avista.ESB.Utilities.Logging;
using Microsoft.BizTalk.Component.Interop;
using Microsoft.BizTalk.Message.Interop;
using Microsoft.BizTalk.Streaming;
using Microsoft.Practices.ESB.Resolver;
using Microsoft.XLANGs.BaseTypes;
using Microsoft.XLANGs.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace Avista.ESB.Resolvers.Content
{
    public class MessageContentResolver : IResolveProvider
    {

        #region IResolveProvider Members

        /// <summary>
        /// 
        /// </summary>
        /// <param name="config"></param>
        /// <param name="resolver"></param>
        /// <param name="message"></param>
        /// <param name="pipelineContext"></param>
        /// <returns></returns>
        public Dictionary<string, string> Resolve(string config, string resolver, IBaseMessage message, IPipelineContext pipelineContext)
        {
            #region Argument Check
            if (String.IsNullOrEmpty(config))
                throw new ArgumentNullException("config");
            if (String.IsNullOrEmpty(resolver))
                throw new ArgumentNullException("resolver");
            if (null == message)
                throw new ArgumentNullException("message");
            if (null == pipelineContext)
                throw new ArgumentNullException("pipelineContext");
            #endregion Argument Check


            Dictionary<string, string> resolverDictionary = new Dictionary<string, string>();
            Resolution resolution = new Resolution();
            Stream inStream = null;
            try
            {
                // Populate context
                ResolverMgr.SetContext(resolution, message, pipelineContext);

                XmlDocument xmlMessage = new XmlDocument();

                inStream = inStream = message.BodyPart.GetOriginalDataStream();
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

                xmlMessage.Load(inStream);

                resolverDictionary.Add("MessageContent", xmlMessage.OuterXml);
            }
            catch (Exception ex)
            {
                Exception exception = new Exception("Error performing Content resolution in pipeline context.", ex);
                Logger.WriteTrace(string.Format("Error occured in {0} \r\n Details: {1}", this.GetType().Name, exception.ToString()));
                throw exception;
            }
            finally
            {
                if (inStream != null)
                {
                    inStream.Position = 0L;
                    message.BodyPart.Data = inStream;
                }
            }
            return resolverDictionary;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="config"></param>
        /// <param name="resolver"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public Dictionary<string, string> Resolve(string config, string resolver, XmlDocument message)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="resolverInfo"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public Dictionary<string, string> Resolve(ResolverInfo resolverInfo, XLANGMessage message)
        {
            #region Argument Check
            if (null == message)
                throw new ArgumentNullException("message");
            #endregion Argument Check

            // Resolve the params from arguments by creating
            // class from schema, and storing the results
            Resolution resolution = new Resolution();
            try
            {
                // Populate context
                ResolverMgr.SetContext(resolution, message);

                Dictionary<string, string> resolverDictionary = new Dictionary<string, string>();

                XmlDocument xmlMessage = (XmlDocument)message[0].RetrieveAs(typeof(XmlDocument));

                resolverDictionary.Add("MessageContent", xmlMessage.OuterXml);

                return resolverDictionary;
            }
            catch (System.Exception ex)
            {
                Exception exception = new Exception("Error performing Content resolution in orchestration context.", ex);
                Logger.WriteTrace(string.Format("Error occured in {0} \r\n Details: {1}", this.GetType().Name, exception.ToString()));
                throw exception;
            }
            finally
            {
                if (null != resolution)
                    resolution = null;

            }
        }
        #endregion

    }
}
