using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.XPath;
using Avista.ESB.Admin.Utility;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Avista.ESB.Testing.Integration
{
    /// <summary>
    ///     Slightly tweaked from a "SimpleHTTPServer" open source proxy on github
    ///     This is a "Mock Proxy" (thanks Kelly for the naming) - it will act as a real proxy, but it optionally
    ///     checks the BizTalkSendStop's along the way and depending on what's defined, will return data defined
    ///     in that rather than forwarding the request on to the http server
    /// </summary>
    public class MoxyServer : IDisposable
    {
        private const string Soap11Namespace = @"http://schemas.xmlsoap.org/soap/envelope/";
        private const string Soap12Namespace = @"http://www.w3.org/2003/05/soap-envelope";
        private const string Soap12AddressingNamespace = @"http://www.w3.org/2005/08/addressing";
        private const string Soap12ActionNodeXpath = @"/s:Envelope/s:Header/a:Action";

        private readonly List<IBizTalkSendStop> _bizTalkStops;
        private readonly HttpListener _listener;
        private readonly SemaphoreSlim _slim;
        private readonly TestContext _testContext;
        private bool _allStopsHit;
        private bool _disposed;

        /// <summary>
        ///     Construct server with given port.
        /// </summary>
        /// <param name="port">Port of the server.</param>
        /// <param name="bizTalkStops"></param>
        /// <param name="testContext"></param>
        public MoxyServer(int port, IEnumerable<BizTalkSendStop> bizTalkStops, TestContext testContext)
        {
            if (bizTalkStops == null) throw new ArgumentNullException("bizTalkStops");
            if (testContext == null) throw new ArgumentNullException("testContext");
            _bizTalkStops = new List<IBizTalkSendStop>(bizTalkStops);
            _testContext = testContext;

            _slim = new SemaphoreSlim(1, 1);

            _listener = new HttpListener();
            _listener.Prefixes.Add(string.Format("http://*:{0}/", port));
            _listener.IgnoreWriteExceptions = true;
        }

        public List<IBizTalkSendStop> BizTalkStops
        {
            get { return _bizTalkStops; }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public async Task<bool> Run(CancellationToken token)
        {
            if (_bizTalkStops.Any() == false)
            {
                // Exit early, no stops to listen for.
                return true;
            }

            token.Register(Stop);


            await OnListener(listener => listener.Start());

            while (!token.IsCancellationRequested && !_allStopsHit)
            {
                try
                {
                    if (!await OnListener(listener => listener.IsListening))
                    {
                        break;
                    }
                    // This call is not wrapped with the SemaphoreSlim because it's a "blocking" async call.
                    HttpListenerContext ctx = await _listener.GetContextAsync();
                    if (token.IsCancellationRequested) break;
                    ProcessRequest(ctx);
                    if (_allStopsHit) break;
                }
                catch (HttpListenerException ex)
                {
                    _testContext.WriteLine("MOXY: HttpListener error: {0}. Moxy will now halt.", ex.Message);
                    return _allStopsHit;
                }
                catch (Exception ex)
                {
                    _testContext.WriteLine("MOXY: Error while listening for requests: {0}. Moxy will now halt.", ex);
                    return _allStopsHit;
                }
            }
            Stop();
            return _allStopsHit;
        }

        private void Stop()
        {
            OnListener(listener =>
            {
                if (listener == null || !listener.IsListening) return;
                _testContext.WriteLine("MOXY: Shutting down HttpListener.");
                listener.Stop();
                listener.Close();

            }).Wait();
        }

        private async Task OnListener(Action<HttpListener> action)
        {
            await OnListener(x =>
            {
                action(x);
                return 0;
            });
        }

        /// <summary>
        ///     This wraps calls to a <see cref="HttpListener"/> in locking semantics to avoid thread deadlocks.
        /// </summary>
        /// <typeparam name="T">The type returned from calling the <see cref="HttpListener"/></typeparam>
        /// <param name="action">The member to call on the <see cref="HttpListener"/></param>
        /// <returns>The result of the call.</returns>
        private async Task<T> OnListener<T>(Func<HttpListener, T> action)
        {
            try
            {
                await _slim.WaitAsync();
                return action(_listener);
            }
            finally
            {
                _slim.Release();
            }
        }

        private void ProcessRequest(HttpListenerContext context)
        {

            string data = GetRequestPostData(context.Request);

            string contentType = context.Request.ContentType;
            _testContext.WriteLine("MOXY: Got content type: {0}", contentType);
            context.Response.ContentType = contentType;

            string soapAction = GetSoapAction(context.Request, data);
            _testContext.WriteLine("MOXY: Got soap action: {0}", soapAction);

            Uri destinationUri = new Uri(context.Request.Url.AbsoluteUri);
            _testContext.WriteLine("MOXY: Destination URI: {0}", destinationUri);

            var stop =
                _bizTalkStops.FirstOrDefault(
                    bss =>
                        string.Equals(bss.SoapAction, soapAction, StringComparison.OrdinalIgnoreCase) &&
                        string.Equals(bss.SendPortUri.PathAndQuery, destinationUri.PathAndQuery, StringComparison.OrdinalIgnoreCase) && 
                        bss.RequestCaught == false);

            if (stop == null)
                throw new UnexpectedBizTalkStopException(
                    string.Format(
                        "A BizTalkSendStop with SoapAction of \"{0}\" and a Uri of \"{1}\" was not found in the configured BizTalk send stops.",
                        soapAction,
                        destinationUri));

            _testContext.WriteLine("Got data: {0}", data);
            if (!string.IsNullOrWhiteSpace(stop.ExpectedIncomingPayload))
                if (data != stop.ExpectedIncomingPayload)
                    throw new BizTalkStopSoapActionException(string.Format("Expected data: {0} got: {1}",
                        stop.ExpectedIncomingPayload,
                        data));

            var errorGeneratingSendPort = stop as IFaultingSendPort;
            if (errorGeneratingSendPort != null)
            {
                errorGeneratingSendPort.SetErrorHttpResponse(context.Response);
            }
            else
            {
                string response;
                if (stop.SimulatedOutput != null)
                {
                    // TODO: Make simulated output run async
                    response = stop.SimulatedOutput(data, stop.ExtraIncomingArgs);
                    _testContext.WriteLine("MOXY: Simulated response: {0}", response);
                }
                else
                {
                    // TODO: Execute the pass-through async
                    response = InvokePassThrough(context.Request.RawUrl,
                        soapAction,
                        data);
                    _testContext.WriteLine("MOXY: Pass-through response: {0}", response);
                }
                byte[] buf = Encoding.UTF8.GetBytes(response);
                context.Response.ContentLength64 = buf.Length;
                using (Stream stream = context.Response.OutputStream)
                {
                    stream.Write(buf, 0, buf.Length);
                }
            }
            context.Response.Close();

            stop.RequestCaught = true;
            _allStopsHit = _bizTalkStops.All(bss => bss.RequestCaught);
        }

        /// <summary>
        ///     Get Request data
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        private string GetRequestPostData(HttpListenerRequest request)
        {
            if (!request.HasEntityBody)
                return null;

            using (var reader = new StreamReader(request.InputStream, request.ContentEncoding))
            {
                return reader.ReadToEnd();
            }
        }


        /// <summary>
        ///     Get the Soap12 Action from the xml content
        /// </summary>
        /// <param name="request"></param>
        /// <param name="requestContent"></param>
        /// <returns></returns>
        private string GetSoapAction(HttpListenerRequest request, string requestContent)
        {
            try
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(requestContent);
                SoapVersion soapVer = GetMessageSoapVersion(xmlDoc);
                _testContext.WriteLine("MOXY: Got soap version: {0}", soapVer);
                switch (soapVer)
                {
                    case SoapVersion.Soap11:
                        return request.Headers["SOAPAction"];
                    case SoapVersion.Soap12:
                        XmlNamespaceManager nsmgr = new XmlNamespaceManager(xmlDoc.NameTable);
                        nsmgr.AddNamespace("s", Soap12Namespace);
                        nsmgr.AddNamespace("a", Soap12AddressingNamespace);
                        var actionNode = xmlDoc.SelectSingleNode(Soap12ActionNodeXpath, nsmgr);
                        if (actionNode != null && !string.IsNullOrWhiteSpace(actionNode.InnerText))
                        {
                            return actionNode.InnerText;
                        }
                        break;
                }
            }
            catch (XmlException)
            {
                // kinda wonky if the XML doesn't parse, ignore: for this context Soap action is set null
            }
            catch (XPathException)
            {
                // failed xpath, due to namespace not defined, ignore: for this context Soap action is set null
            }
            return null;
        }

        /// <summary>
        ///     Determine the Soap version of the message from the namespace attributes
        /// </summary>
        /// <param name="xmlDoc"></param>
        /// <returns></returns>
        private SoapVersion GetMessageSoapVersion(XmlDocument xmlDoc)
        {
            if (xmlDoc == null || xmlDoc.DocumentElement == null) return SoapVersion.Unknown;
            IList<XmlAttribute> nsAttrs = xmlDoc.DocumentElement.Attributes.Cast<XmlAttribute>().ToList();
            if (nsAttrs.Any(attr => attr.InnerText == Soap11Namespace))
            {
                return SoapVersion.Soap11;
            }
            return nsAttrs.Any(attr => attr.InnerText == Soap12Namespace) ? SoapVersion.Soap12 : SoapVersion.Unknown;
        }

        private static string InvokePassThrough(string url, string soapAction, string body)
        {
            return RestHelper.SubmitHttpRequestRaw(url,
                "POST",
                "text/xml; charset=utf-8",
                body,
                new NameValueCollection {{"SOAPAction", soapAction}});
        }

        ~MoxyServer()
        {
            Dispose(false);
        }

        private void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                Stop();
            }

            _disposed = true;
        }

        private enum SoapVersion
        {
            Soap11,
            Soap12,
            Unknown
        }
    }

}