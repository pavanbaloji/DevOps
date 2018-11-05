using Avista.ESB.Utilities.Logging;
using Microsoft.BizTalk.Component.Interop;
using Microsoft.BizTalk.Message.Interop;
using Microsoft.Practices.ESB.Resolver;
using Microsoft.XLANGs.BaseTypes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Xml;


namespace Avista.ESB.Resolvers.Enrich
{
    public class EnrichResolver : IResolveProvider
    {
        #region IResolveProvider Members

        /// <summary>
        /// Performs resolution in a web service context.
        /// </summary>
        /// <param name="config">Configuration string entered into a Web service as the argument</param>
        /// <param name="resolver">Moniker representing the Resolver to load</param>
        /// <param name="message">XML document passed from the Web service</param>
        /// <returns>Dictionary object fully populated</returns>
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
                Exception exception = new Exception("Error performing Enrich resolution in web service context.", ex);
                Logger.WriteTrace(string.Format("Error occured in {0} \r\n Details: {1}", this.GetType().Name, exception.ToString()));
                throw exception;
            }
        }

        /// <summary>
        /// Performs resolution in a pipeline context.
        /// </summary>
        /// <param name="config">Configuration string entered into the pipeline component as argument</param>
        /// <param name="resolver">Moniker representing the Resolver to load</param>
        /// <param name="message">IBaseMessage passed from the pipeline</param>
        /// <param name="pipelineContext">IPipelineContext passed from the pipeline</param>
        /// <returns>Dictionary object fully populated</returns>
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
                Exception exception = new Exception("Error performing Enrich resolution in pipeline context.", ex);
                Logger.WriteTrace(string.Format("Error occured in {0} \r\n Details: {1}", this.GetType().Name, exception.ToString()));                
                throw exception;                                
            }
        }

        /// <summary>
        /// Performs resolution in an orchestration context.
        /// </summary>
        /// <param name="resolverInfo">Configuration string containing the configuration and resolver</param>
        /// <param name="message">XLANGMessage passed from orchestration</param>
        /// <returns>Dictionary object fully populated</returns>
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
                Exception exception = new Exception("Error performing Enrich resolution in orchestration context.", ex);
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
            Dictionary<string, string> queryParams = null;
            Dictionary<string, string> resolverDictionary = new Dictionary<string, string>();

            // fix the resolver if it needs it
            if (!resolver.Contains(ResolverMgr.MonikerSeparator))
                resolver = resolver + ResolverMgr.MonikerSeparator;

            try
            {
                // Retrieve dictionary view of query params that were passed to pipeline
                queryParams = ResolverMgr.GetFacts(config, resolver);

                // Retreive the values
                string _probe0 = ResolverMgr.GetConfigValue(queryParams, false, "probe0");
                string _probe1 = ResolverMgr.GetConfigValue(queryParams, false, "probe1");
                string _probe2 = ResolverMgr.GetConfigValue(queryParams, false, "probe2");
                string _part0Source = ResolverMgr.GetConfigValue(queryParams, true, "part0Source");
                string _part1Source = ResolverMgr.GetConfigValue(queryParams, false, "part1Source");
                string _part2Source = ResolverMgr.GetConfigValue(queryParams, false, "part2Source");
                string _part3Source = ResolverMgr.GetConfigValue(queryParams, false, "part3Source");
                string _part4Source = ResolverMgr.GetConfigValue(queryParams, false, "part4Source");
                bool _preservePart0 = System.Convert.ToBoolean(ResolverMgr.GetConfigValue(queryParams, false, "preservePart0"));
                bool _preservePart1 = System.Convert.ToBoolean(ResolverMgr.GetConfigValue(queryParams, false, "preservePart1"));
                bool _preservePart2 = System.Convert.ToBoolean(ResolverMgr.GetConfigValue(queryParams, false, "preservePart2"));
                bool _preservePart3 = System.Convert.ToBoolean(ResolverMgr.GetConfigValue(queryParams, false, "preservePart3"));
                bool _preservePart4 = System.Convert.ToBoolean(ResolverMgr.GetConfigValue(queryParams, false, "preservePart4"));
                string _transformType = ResolverMgr.GetConfigValue(queryParams, true, "transformType");
                int _failureEventId = System.Convert.ToInt32(ResolverMgr.GetConfigValue(queryParams, false, "failureEventId"));
                string _failureAction = ResolverMgr.GetConfigValue(queryParams, false, "failureAction");

                // populate the dictionary object with the resolution properties
                ResolverMgr.SetResolverDictionary(resolution, resolverDictionary);

                resolverDictionary.Add("Enrich.Probe0", _probe0.ToString() ?? "");
                resolverDictionary.Add("Enrich.Probe1", _probe1.ToString() ?? "");
                resolverDictionary.Add("Enrich.Probe2", _probe2.ToString() ?? "");
                resolverDictionary.Add("Enrich.Part0Source", _part0Source ?? "Pipeline");
                resolverDictionary.Add("Enrich.Part1Source", _part1Source ?? "");
                resolverDictionary.Add("Enrich.Part2Source", _part2Source.ToString() ?? "");
                resolverDictionary.Add("Enrich.Part3Source", _part3Source.ToString() ?? "");
                resolverDictionary.Add("Enrich.Part4Source", _part4Source.ToString() ?? "");
                resolverDictionary.Add("Enrich.PreservePart0", _preservePart0.ToString());
                resolverDictionary.Add("Enrich.PreservePart1", _preservePart1.ToString());
                resolverDictionary.Add("Enrich.PreservePart2", _preservePart2.ToString());
                resolverDictionary.Add("Enrich.PreservePart3", _preservePart3.ToString());
                resolverDictionary.Add("Enrich.PreservePart4", _preservePart4.ToString());
                resolverDictionary.Add("Enrich.TransformType", _transformType ?? "");
                resolverDictionary.Add("Enrich.FailureEventId", _failureEventId.ToString());
                resolverDictionary.Add("Enrich.FailureAction", _failureAction ?? "ThrowException");

                return resolverDictionary;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (null != resolution)
                    resolution = null;
                if (null != queryParams)
                {
                    queryParams.Clear();
                    queryParams = null;
                }
                if (null != resolverDictionary)
                {
                    resolverDictionary = null;
                }
            }

        }
        #endregion
    }
}



