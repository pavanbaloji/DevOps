using Avista.ESB.Utilities.Cache;
using Avista.ESB.Utilities.Logging;
using Microsoft.BizTalk.Component.Interop;
using Microsoft.BizTalk.Message.Interop;
using Microsoft.BizTalk.Streaming;
using Microsoft.Practices.ESB.GlobalPropertyContext;
using Microsoft.Practices.ESB.Itinerary;
using Microsoft.Practices.ESB.Resolver;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Avista.ESB.MessagingServices.Cache
{
    public class CacheService : IMessagingService
    {
        private SqlBackingCache cache;
        private int defaultCacheTimeout = 60; // Default cache timeout in minutes
        private SqlBackingCache Cache
        {
            get
            {
                if (this.cache == null)
                    this.cache = new SqlBackingCache();
                return this.cache;
            }
        }


        [Browsable(true), ReadOnly(true)]
        public string Name
        {
            get
            {
                return "Avista.ESB.Utilities.CacheService";
            }
        }

        public bool SupportsDisassemble
        {
            get
            {
                return false;
            }
        }

        public bool ShouldAdvanceStep(IItineraryStep step, IBaseMessage msg)
        {
            return true;
        }

        public IBaseMessage Execute(IPipelineContext pipelineContext, IBaseMessage message, string resolverString, IItineraryStep step)
        {
            Logger.WriteTrace(string.Format("******{0} Started******", this.GetType().Name));
            Logger.WriteTrace("Resolver String: " + resolverString);

            Stream inStream = null;
            string content = string.Empty;

            if (string.IsNullOrEmpty(resolverString))
            {
                throw new ArgumentException("ResolverString is required.", "resolverString");
            }
            try
            {
                ResolverInfo info = ResolverMgr.GetResolverInfo(ResolutionType.Transform, resolverString);
                if (info.Success)
                {
                    Dictionary<string, string> dictionary = ResolverMgr.Resolve(info, message, pipelineContext);

                    string action = dictionary["Cache.Action"];
                    string cacheMsgName = dictionary["Cache.CacheMessageName"];
                    int.TryParse(dictionary["Cache.CacheTimeOut"].ToString(), out defaultCacheTimeout);

                    string key = (string)message.Context.Read(BtsProperties.InterchangeID.Name, BtsProperties.InterchangeID.Namespace) + cacheMsgName;

                    if (action == "Get")
                    {
                        content = (string)this.Cache.Get(key);
                    }
                    else //Default action is Add
                    {
                        inStream = message.BodyPart.Data;

                        using (StreamReader reader = new StreamReader(inStream))
                        {
                            content = reader.ReadToEnd();
                        }

                        try
                        {
                            this.Cache.Add(key, content, DateTime.Now.AddMinutes(defaultCacheTimeout));
                        }
                        catch (ArgumentException ex)
                        {
                            throw new Exception("CacheService : '" + key + "' - DUPLICATE!! ERROR TYPE: " + ex.GetType().Name + "  Message: " + ex.Message);
                        }
                    }
                    inStream = new MemoryStream();
                    StreamWriter writer = new StreamWriter(inStream);
                    writer.Write(content);
                    writer.Flush();
                    inStream.Position = 0L;

                    message = Microsoft.Practices.ESB.Utilities.MessageHelper.CreateNewMessage(pipelineContext, message, inStream);
                }
                else
                {
                    throw new Exception("Unable to get cache resolver information from the resolver string: " + resolverString);
                }
            }
            catch (Exception ex)
            {
                Logger.WriteTrace(string.Format("Error occured in {0} \r\n Details: {1}", this.GetType().Name, ex.ToString()));
                throw ex;
            }

            Logger.WriteTrace(string.Format("******{0} Completed******", this.GetType().Name));

            return message;
        }
    }
}
