using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Avista.ESB.Testing
{
    public static class RestHelper
    {

        public static string SubmitHttpRequestRaw(string endpoint, string httpMethod, string contentType,
            string stringData, NameValueCollection headers = null)
        {

            var request = (HttpWebRequest)WebRequest.Create(endpoint);
            request.Method = httpMethod;

            if (headers != null)
            {
                request.Headers.Add(headers);
            }
            if (httpMethod == "POST" || httpMethod == "PUT")
            {
                request.ContentType = contentType;
                request.Accept = contentType;
                using (var streamWriter = new StreamWriter(request.GetRequestStream()))
                {
                    streamWriter.Write(stringData);
                    streamWriter.Flush();
                    streamWriter.Close();
                }
            }

            using (WebResponse ws = request.GetResponse())
            {
                Stream responseStream = ws.GetResponseStream();
                if (responseStream == null) return null;
                using (var streamReader = new StreamReader(responseStream))
                {
                    var content = streamReader.ReadToEnd();
                    return content;
                }
            }
        }

        /// <summary>
        /// SubmitHttpRequest
        /// </summary>
        /// <param name="endpoint"></param>
        /// <param name="httpMethod"></param>
        /// <param name="contentType"></param>
        /// <param name="stringData"></param>
        /// <param name="headers"></param>
        /// <returns></returns>
        public static object SubmitHttpRequest(string endpoint, 
                                               string httpMethod,
                                               string contentType, 
                                               string stringData, 
                                               NameValueCollection headers)
        {

            string content = SubmitHttpRequestRaw(endpoint, httpMethod, contentType, stringData, headers);
            if (string.IsNullOrWhiteSpace(content))
            {
                return null;
            }
            var jObject = JObject.Parse(content);
            return jObject;
        }


        /// <summary>
        ///     Process REST Http request handler
        /// </summary>
        public static object SubmitHttpRequest(string endpoint, string httpMethod, string contentType, string stringData)
        {
            string content = SubmitHttpRequestRaw(endpoint, httpMethod, contentType, stringData);
            if (string.IsNullOrWhiteSpace(content))
            {
                return null;
            }
            var jObject = JObject.Parse(content);
            return jObject;
        }

        /// <summary>
        /// SubmitHttpRequest<T>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="endpoint"></param>
        /// <param name="httpMethod"></param>
        /// <param name="contentType"></param>
        /// <param name="stringData"></param>
        /// <returns></returns>
        public static T SubmitHttpRequest<T>(string endpoint, string httpMethod, string contentType,
            string stringData)
        {
            string content = SubmitHttpRequestRaw(endpoint, httpMethod, contentType, stringData);
            if (string.IsNullOrWhiteSpace(content))
            {
                return default(T);
            }
            return JsonConvert.DeserializeObject<T>(content);

        }

        /// <summary>
        /// ReadResourceAsString
        /// </summary>
        /// <param name="resourceName"></param>
        /// <returns></returns>
        public static string ReadResourceAsString(string resourceName)
        {
            Assembly assembly = Assembly.GetExecutingAssembly();

            return ResourceHelper.LoadAsString(assembly, resourceName);
        }
    }
}