using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Avista.ESB.WcfExtensions.WebHttpHeader
{
    public class HttpResponseCodeInspector : IDispatchMessageInspector
    {

        private static string HttpResponseCodeTagName = "httpStatusCode";


        public object AfterReceiveRequest(ref Message request, IClientChannel channel, InstanceContext instanceContext)
        {
            return null;
        }

        /// <summary>
        /// BeforeSendReply
        /// </summary>
        /// <param name="reply"></param>
        /// <param name="correlationState"></param>
        public void BeforeSendReply(ref Message reply, object correlationState)
        {

            // create message copy
            MessageBuffer buffer = reply.CreateBufferedCopy(Int32.MaxValue);
            // fresh copy to extract response codee from
            Message msgCopy = buffer.CreateMessage();
            // fresh unread copy to return
            Message msgReturn = buffer.CreateMessage();

            // read body
            string msgBody = ReadMsgBody(msgCopy);
            // sanity check for empty bidy
            if (string.IsNullOrWhiteSpace(msgBody)) return;
            // extract response code if it exists, search for "HttpResponseCodeTagName" above
            var httpResponseCode = ExtractMessageResponseCode(msgBody);
            if (!string.IsNullOrWhiteSpace(httpResponseCode))
            {
                // set response code in return msg
                SetStatusCode(httpResponseCode, msgReturn);
            }
            // set reply to copy
            reply = msgReturn;
        }


        /// <summary>
        /// ReadMsgBody
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public string ReadMsgBody(Message message)
        {
            var bodyReader = message.GetReaderAtBodyContents();
            bodyReader.ReadStartElement("Binary");
            byte[] rawBody = bodyReader.ReadContentAsBase64();

            string messageAsString;
            using (var reader = new StreamReader(new MemoryStream(rawBody)))
                messageAsString = reader.ReadToEnd();

            return messageAsString;
        }

        /// <summary>
        /// ExtractMessageResponseCode
        /// </summary>
        /// <param name="jsonContent"></param>
        /// <returns></returns>
        private string ExtractMessageResponseCode(string jsonContent)
        {
            string codeVal = null;
            JObject jObj = JObject.Parse(jsonContent);
            var properties = jObj.Descendants().Where(t => t.Type == JTokenType.Property);
            if (properties.Any())
            {
                foreach (JToken token in properties)
                {
                    JProperty prop = (JProperty) token;
                    if (string.Equals(prop.Name, HttpResponseCodeTagName))
                    {
                        codeVal = prop.Value.Value<string>();
                        break;
                    }
                }
            }

            return codeVal;
        }

        /// <summary>
        /// SetStatusCode
        /// </summary>
        /// <param name="statusCodeStr"></param>
        /// <param name="reply"></param>
        private void SetStatusCode(string statusCodeStr, Message reply)
        {

            System.Net.HttpStatusCode statusCode;

            if (Enum.TryParse<HttpStatusCode>(statusCodeStr, true, out statusCode))
            {
                // Here the response code is changed
                 try
                {
                    reply.Properties[HttpResponseMessageProperty.Name] = new HttpResponseMessageProperty() { StatusCode = statusCode };
                }
                catch (ArgumentOutOfRangeException ex) 
                {
                    // ignore this, it means the response code int value
                    // was out of range (100-599) as pulled from the msg,
                    // in our context we just won't set the code
                }
            }
        }
    }
}
