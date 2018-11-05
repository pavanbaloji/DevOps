using Avista.ESB.Utilities.Logging;
using Microsoft.BizTalk.Component.Interop;
using Microsoft.BizTalk.Message.Interop;
using Microsoft.Practices.ESB.Resolver;
using Microsoft.XLANGs.BaseTypes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Avista.ESB.Resolvers.Route
{
    /// <summary>
    /// Uses Route Resolver to config static sendports in Routing Service.
    /// 
    /// </summary>
    public class RouteResolver : IResolveProvider
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
                return ResolveStatic(config, resolver, resolution);
            }
            catch (Exception ex)
            {
                Exception exception = new Exception("Error performing Route resolution in pipeline context.", ex);
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
            #region Argument Check
            if (String.IsNullOrEmpty(config))
                throw new ArgumentNullException("config");
            if (String.IsNullOrEmpty(resolver))
                throw new ArgumentNullException("resolver");

            #endregion Argument Check

            Resolution emptyResolution = new Resolution();

            try
            {
                // resolve with rules and return dictionary
                return ResolveStatic(config, resolver, emptyResolution);
            }
            catch (Exception ex)
            {
                Exception exception = new Exception("Error performing Route resolution in web service context.", ex);
                Logger.WriteTrace(string.Format("Error occured in {0} \r\n Details: {1}", this.GetType().Name, exception.ToString()));
                throw exception;
            }
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
                return ResolveStatic(resolverInfo.Config, resolverInfo.Resolver, resolution);
            }
            catch (System.Exception ex)
            {
                Exception exception = new Exception("Error performing Route resolution in orchestration context.", ex);
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
        private static Dictionary<string, string> ResolveStatic(string config, string resolver, Resolution resolution)
        {
            RouteResolverDescription facts = new RouteResolverDescription();
            Dictionary<string, string> queryParams = null;
            Dictionary<string, string> ResolverDictionary = new Dictionary<string, string>();

            // fix the resolver if it needs it
            if (!resolver.Contains(ResolverMgr.MonikerSeparator))
                resolver = resolver + ResolverMgr.MonikerSeparator;

            try
            {
                // Retrieve dictionary view of query params that were passed to pipeline
                queryParams = ResolverMgr.GetFacts(config, resolver);

                // Retreive the values

                facts.BiztalkApplication = ResolverMgr.GetConfigValue(queryParams, false, "biztalkApplication");
                facts.SendPort = ResolverMgr.GetConfigValue(queryParams, false, "sendPort");
                facts.ServiceName = ResolverMgr.GetConfigValue(queryParams, false, "serviceName");
                facts.ServiceType = ResolverMgr.GetConfigValue(queryParams, false, "serviceType");
                facts.ServiceState = ResolverMgr.GetConfigValue(queryParams, false, "serviceState");
                facts.RequestResponse = ResolverMgr.GetConfigValue(queryParams, false, "isRequestResponse");
                facts.TwoWay = ResolverMgr.GetConfigValue(queryParams, false, "isTwoWay");

                facts.ArchiveRequired = (ResolverMgr.GetConfigValue(queryParams, false, "archiveRequired"));
                facts.ArchiveTagName = ResolverMgr.GetConfigValue(queryParams, false, "archiveTagName");
                facts.SoapFaultCode = ResolverMgr.GetConfigValue(queryParams, false, "soapFaultCode");
                facts.DeliveryFailureCode = ResolverMgr.GetConfigValue(queryParams, false, "deliveryFailureCode");
                facts.WcfAction = ResolverMgr.GetConfigValue(queryParams, false, "wcfAction");

                // populate the dictionary object with the resolution properties
                ResolverMgr.SetResolverDictionary(resolution, ResolverDictionary);

                //Add custom resolution properties which are user by Samples.BizTalk.ESB.MessagingServices.TRANSFORM.TransformItineraryService
                ResolverDictionary.Add("Resolver.BiztalkApplication", facts.BiztalkApplication);
                ResolverDictionary.Add("Resolver.SendPort", facts.SendPort);
                ResolverDictionary.Add("Resolver.ServiceName", facts.ServiceName);
                ResolverDictionary.Add("Resolver.ServiceType", facts.ServiceType);
                ResolverDictionary.Add("Resolver.ServiceState", facts.ServiceState);
                ResolverDictionary.Add("Resolver.RequestResponse", facts.RequestResponse);
                ResolverDictionary.Add("Resolver.TwoWay", facts.TwoWay);
                ResolverDictionary.Add("Resolver.ArchiveRequired", facts.ArchiveRequired);
                ResolverDictionary.Add("Resolver.ArchiveTagName", facts.ArchiveTagName);
                ResolverDictionary.Add("Resolver.SoapFaultCode", facts.SoapFaultCode);
                ResolverDictionary.Add("Resolver.DeliveryFailureCode", facts.DeliveryFailureCode);
                ResolverDictionary.Add("Resolver.WcfAction", facts.WcfAction);
                return ResolverDictionary;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (null != resolution)
                    resolution = null;
                if (null != facts)
                    facts = null;
                if (null != queryParams)
                {
                    queryParams.Clear();
                    queryParams = null;
                }
                if (null != ResolverDictionary)
                {
                    ResolverDictionary = null;
                }
            }


        }
        #endregion
    }
}
