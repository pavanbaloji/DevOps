using Avista.ESB.Utilities.Logging;
using Microsoft.BizTalk.Component.Interop;
using Microsoft.BizTalk.Message.Interop;
using Microsoft.Practices.ESB.Itinerary;
using Microsoft.Practices.ESB.Resolver;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Avista.ESB.MessagingServices.Manipulator
{
    public class ManipulatorService : IMessagingService
    {
        [Browsable(true), ReadOnly(true)]
        public string Name
        {
            get
            {
                return "Avista.ESB.Utilities.ManipulatorService";
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

        public IBaseMessage Execute(IPipelineContext context, IBaseMessage msg, string resolverString, IItineraryStep step)
        {
            Logger.WriteTrace(string.Format("******{0} Started******", this.GetType().Name));
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }
            if (msg == null)
            {
                throw new ArgumentNullException("msg");
            }
            if (string.IsNullOrEmpty(resolverString))
            {
                throw new Exception("Resolver string is required.");
            }
            IBaseMessage result = msg;
            List<ManipulatorDescription> manipulatorList = new List<ManipulatorDescription>();

            try
            {
                foreach (string resolver in step.ResolverCollection)
                {
                    Logger.WriteTrace("        Resolver: " + resolver);
                    ResolverInfo resolverInfo = ResolverMgr.GetResolverInfo(ResolutionType.Transform, resolver);
                    Dictionary<string, string> dictionary = ResolverMgr.Resolve(resolverInfo, msg, context);

                    ManipulatorDescription description = new ManipulatorDescription();
                    description.ReadFrom = dictionary["Resolver.ReadFrom"];
                    description.ReadItem = dictionary["Resolver.ReadItem"];
                    description.WriteTo = dictionary["Resolver.WriteTo"];
                    description.WriteItem = dictionary["Resolver.WriteItem"];
                    manipulatorList.Add(description);

                    dictionary.Remove("Resolver.ReadFrom");
                    dictionary.Remove("Resolver.ReadItem");
                    dictionary.Remove("Resolver.WriteTo");
                    dictionary.Remove("Resolver.WriteItem");
                }
                if (manipulatorList.Count > 0)
                {
                    Manipulator manipulator = new Manipulator();
                    manipulatorList = manipulator.GetManipulationData(manipulatorList, msg, context);
                    result = manipulator.WriteManipulationData(manipulatorList, msg, context);
                }

            }
            catch (Exception ex)
            {
                Logger.WriteTrace(string.Format("Error occured in {0} \r\n Details: {1}", this.GetType().Name, ex.ToString()));
                throw ex;
            }
            Logger.WriteTrace(string.Format("******{0} Completed******", this.GetType().Name));
            return result;
        }

    }
}
