using Avista.ESB.Utilities.Logging;
using Microsoft.BizTalk.Component.Interop;
using Microsoft.BizTalk.Message.Interop;
using Microsoft.Practices.ESB.Resolver;
using Microsoft.XLANGs.BaseTypes;
using Microsoft.XLANGs.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace Avista.ESB.Resolvers.Context
{
    public class MessageContextResolver : IResolveProvider
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

            Resolution resolution = new Resolution();

            try
            {
                // Populate context
                ResolverMgr.SetContext(resolution, message, pipelineContext);

                // resolve with rules and return dictionary
                return ResolveStatic(message.Context);
            }
            catch (Exception ex)
            {
                Exception exception = new Exception("Error performing Context resolution in pipeline context.", ex);
                Logger.WriteTrace(string.Format("Error occured in {0} \r\n Details: {1}", this.GetType().Name, exception.ToString()));
                throw exception;
            }
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

                // resolve with rules and return dictionary
                return ResolveStatic(message);
            }
            catch (System.Exception ex)
            {
                Exception exception = new Exception("Error performing Context resolution in orchestration context.", ex);
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


        #region Private Methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="config"></param>
        /// <param name="resolver"></param>
        /// <param name="resolution"></param>
        /// <returns></returns>
        private static Dictionary<string, string> ResolveStatic(XLANGMessage message)
        {
            Dictionary<string, string> resolverDictionary = new Dictionary<string, string>();
            string result = null;
            StringWriter stringWriter = null;
            XmlTextWriter xmlTextWriter = null;
            try
            {
                stringWriter = new StringWriter();
                xmlTextWriter = new XmlTextWriter(stringWriter);
                xmlTextWriter.Formatting = Formatting.Indented;
                xmlTextWriter.WriteStartDocument();
                xmlTextWriter.WriteStartElement("ContextProperties");

                foreach (DictionaryEntry dictionary in GetContext(message))
                {
                    XmlQName qName = (XmlQName)dictionary.Key;
                    xmlTextWriter.WriteStartElement("Property");
                    xmlTextWriter.WriteAttributeString("name", qName.Name);
                    xmlTextWriter.WriteAttributeString("namespace", qName.Namespace);
                    xmlTextWriter.WriteString(dictionary.Value.ToString());
                    xmlTextWriter.WriteEndElement();
                }
                xmlTextWriter.WriteFullEndElement();
                xmlTextWriter.WriteEndDocument();
                result = stringWriter.ToString();
            }
            catch
            {
                throw;
            }
            finally
            {
                if (xmlTextWriter != null)
                {
                    xmlTextWriter.Close();
                }
                if (stringWriter != null)
                {
                    stringWriter.Close();
                    stringWriter.Dispose();
                }
            }

            resolverDictionary.Add("MessageContext", result);

            return resolverDictionary;
        }

        /// <summary>
        /// Gets all message context properties assocciated with xlang message
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        private static Hashtable GetContext(XLANGMessage message)
        {
            foreach (Segment segment in Service.RootService._segments)
            {
                IDictionary fields = Microsoft.XLANGs.Core.Context.FindFields(typeof(XLANGMessage), segment.ExceptionContext);

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
            return new Hashtable();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="config"></param>
        /// <param name="resolver"></param>
        /// <param name="resolution"></param>
        /// <returns></returns>
        private static Dictionary<string, string> ResolveStatic(IBaseMessageContext messageContext)
        {
            Dictionary<string, string> resolverDictionary = new Dictionary<string, string>();
            string result = null;
            StringWriter stringWriter = null;
            XmlTextWriter xmlTextWriter = null;
            try
            {
                stringWriter = new StringWriter();
                xmlTextWriter = new XmlTextWriter(stringWriter);
                xmlTextWriter.Formatting = Formatting.Indented;
                xmlTextWriter.WriteStartDocument();
                xmlTextWriter.WriteStartElement("ContextProperties");
                int num = 0;
                while ((long)num < (long)((ulong)messageContext.CountProperties))
                {
                    string propName;
                    string propNamespace;
                    string propValue = messageContext.ReadAt(num, out propName, out propNamespace).ToString();
                    xmlTextWriter.WriteStartElement("Property");
                    xmlTextWriter.WriteAttributeString("name", propName);
                    xmlTextWriter.WriteAttributeString("namespace", propNamespace);
                    xmlTextWriter.WriteString(propValue);
                    xmlTextWriter.WriteEndElement();
                    num++;
                }
                xmlTextWriter.WriteFullEndElement();
                xmlTextWriter.WriteEndDocument();
                result = stringWriter.ToString();
            }
            catch
            {
                throw;
            }
            finally
            {
                if (xmlTextWriter != null)
                {
                    xmlTextWriter.Close();
                }
                if (stringWriter != null)
                {
                    stringWriter.Close();
                    stringWriter.Dispose();
                }
            }

            resolverDictionary.Add("MessageContext", result);

            return resolverDictionary;
        }
        #endregion
    }
}
