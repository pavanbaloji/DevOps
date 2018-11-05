using System;
using System.Reflection;


namespace Avista.ESB.Testing.Mock
{
    public abstract class MockServiceBase
    {
        protected abstract Assembly CurrentAssembly { get; }

        public string LoadResourceAsString(string resourceName, bool wrapInSoapEnvelope = true)
        {
            if (string.IsNullOrWhiteSpace(resourceName)) throw new ArgumentNullException("resourceName");
            string ret = ResourceHelper.LoadAsString(CurrentAssembly, resourceName);
            if (wrapInSoapEnvelope)
                return string.Format(
                    @"<s:Envelope xmlns:s=""http://schemas.xmlsoap.org/soap/envelope/""><s:Body xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"">{0}</s:Body></s:Envelope>", ret);
            return ret;
        }

        public string LoadResource12AsString(string resourceName, string action, string destServiceUrl = "http://localhost", bool wrapInSoapEnvelope = true)
        {
            if (string.IsNullOrWhiteSpace(resourceName)) throw new ArgumentNullException("resourceName");
            string ret = ResourceHelper.LoadAsString(CurrentAssembly, resourceName);
            if (wrapInSoapEnvelope)
                return GetSoap12Encapsulated(ret, action, destServiceUrl);
            return ret;
        }

        /// <summary>
        ///     Return content, action params, Soap 1.2 encapsulated
        /// </summary>
        /// <param name="content"></param>
        /// <param name="action"></param>
        /// <param name="destServiceUrl"></param>
        /// <returns></returns>
        protected string GetSoap12Encapsulated(string content, string action, string destServiceUrl = "http://localhost")
        {
            string guid = Guid.NewGuid().ToString();
            return string.Format(Resource.soap12Envelope, action, guid, destServiceUrl ?? "", content);
        }

        public static string CreateFaultException(Exception ex)
        {
            if (ex == null) throw new ArgumentNullException("ex");

            if (ex.InnerException == null)
            {
                return string.Format(@"<s:Envelope xmlns:s=""http://schemas.xmlsoap.org/soap/envelope/"">
     <s:Header />
     <s:Body>
       <s:Fault>
         <faultcode xmlns:a=""http://schemas.microsoft.com/net/2005/12/windowscommunicationfoundation/dispatcher"">a:InternalServiceFault</faultcode>
         <faultstring xml:lang=""en-US"">{0}</faultstring>
         <detail>
           <ExceptionDetail xmlns=""http://schemas.datacontract.org/2004/07/System.ServiceModel"" xmlns:i=""http://www.w3.org/2001/XMLSchema-instance"">
             <HelpLink i:nil=""true"" />
             <Message>{1}</Message>
             <StackTrace>{2}</StackTrace>
             <Type>{3}</Type>
           </ExceptionDetail>
         </detail>
       </s:Fault>
     </s:Body>
   </s:Envelope>",
                ex.Message,
                ex.Message,
                ex.StackTrace,
                ex.GetType().FullName); 
            }

            return string.Format(@"<s:Envelope xmlns:s=""http://schemas.xmlsoap.org/soap/envelope/"">
     <s:Header />
     <s:Body>
       <s:Fault>
         <faultcode xmlns:a=""http://schemas.microsoft.com/net/2005/12/windowscommunicationfoundation/dispatcher"">a:InternalServiceFault</faultcode>
         <faultstring xml:lang=""en-US"">{0}</faultstring>
         <detail>
           <ExceptionDetail xmlns=""http://schemas.datacontract.org/2004/07/System.ServiceModel"" xmlns:i=""http://www.w3.org/2001/XMLSchema-instance"">
             <HelpLink i:nil=""true"" />
             <InnerException>
               <HelpLink i:nil=""true"" />
               <InnerException i:nil=""true"" />
               <Message>{1}</Message>
               <StackTrace>{2}</StackTrace>
               <Type>{3}</Type>
             </InnerException>
             <Message>{4}</Message>
             <StackTrace>{5}</StackTrace>
             <Type>{6}</Type>
           </ExceptionDetail>
         </detail>
       </s:Fault>
     </s:Body>
   </s:Envelope>", 
                ex.Message, 
                ex.InnerException.Message, 
                ex.InnerException.StackTrace,
                ex.InnerException.GetType().FullName,
                ex.Message, 
                ex.StackTrace, 
                ex.GetType().FullName);
        }

        /// <summary>
        ///     Create generic Soap 1.2 fault based on params
        /// </summary>
        /// <param name="faultCode"></param>
        /// <param name="faultSubCode"></param>
        /// <param name="reason"></param>
        /// <param name="detail"></param>
        /// <returns></returns>
        protected string CreateGenericSoap12Fault(string faultCode,
            string faultSubCode,
            string reason = null,
            string detail = null)
        {
            return string.Format(Resource.soap12GenericFault, faultCode, faultSubCode, reason ?? "", detail ?? "");
        }
    }
}