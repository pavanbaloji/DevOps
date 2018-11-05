using Avista.ESB.Utilities.Logging;
using Microsoft.BizTalk.Component.Interop;
using Microsoft.BizTalk.Message.Interop;
using Microsoft.Practices.ESB.Resolver;
using Microsoft.XLANGs.BaseTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Avista.ESB.Resolvers.SelectItinerary
{
    /// <summary>
    ///  Uses SelectItinerary Resolver to configure itinerary name for SelectItinerary Services.
    /// </summary>
    public class SelectItineraryResolver : IResolveProvider
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
                Exception exception = new Exception("Error performing SelectItinerary resolution in pipeline context.", ex);
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
                Exception exception = new Exception("Error performing SelectItinerary resolution in web service context.", ex);
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
                Exception exception = new Exception("Error performing SelectItinerary resolution in orchestration context.", ex);
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

                string itineraryName = ResolverMgr.GetConfigValue(queryParams, false, "itinerary");
                string continueOnFailure = ResolverMgr.GetConfigValue(queryParams, false, "continueOnFailure");

                // populate the dictionary object with the resolution properties
                ResolverMgr.SetResolverDictionary(resolution, resolverDictionary);

                resolverDictionary.Add("SelectItinerary.Itinerary", itineraryName);
                resolverDictionary.Add("SelectItinerary.ContinueOnFailure", continueOnFailure);

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
