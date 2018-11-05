using Avista.ESB.Utilities.Configuration;
using Avista.ESB.Utilities.Logging;
using Microsoft.XLANGs.Core;
using System;

namespace Avista.ESB.Utilities
{
    /// <summary>
    /// An OrchestrationContext contains data that describes the contextual state of an orchestration instance.
    /// </summary>
    [Serializable]
    public class OrchestrationContext
    {        
        /// <summary>
        /// Used to store the environment setting as read from the config file.
        /// </summary>
        private string environment = "N/A";

        /// <summary>
        /// Used to store the action being performed by the orchestration.
        /// </summary>
        private string action = "N/A";

        /// <summary>
        /// Used to store the service name of the orchestration.
        /// </summary>
        private string serviceName = "N/A";

        /// <summary>
        /// Used to store the instance id of the orchestration;
        /// </summary>
        private string instanceId = "N/A";

        /// <summary>
        /// Used to control tracing of the orchestration context.
        /// </summary>
        private bool trace = false;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public OrchestrationContext()
        {
            // Load information from the application configuration.
            try
            {
                AvistaESBCommonSection section = AvistaESBCommonSection.GetSection();
                environment = section.Context.Environment;
                trace = section.Context.Trace;
            }
            catch (Exception)
            {
                // If the values cannot be loaded, default values will be used.
            }
            // Load information from the service context.
            try
            {
                serviceName = Context.RootService.Name;
                instanceId = Context.RootService.InstanceId.ToString();
            }
            catch (Exception)
            {
                // If the values cannot be loaded, default values will be used.
            }
        }

        /// <summary>
        /// The Environment identifies where the orchestration is running.
        /// The value is loaded from the application configuration file.
        /// It will normally be set to a value like DEV, TEST, or PROD.
        /// </summary>
        public string Environment
        {
            get
            {
                return environment;
            }
        }

        /// <summary>
        /// The Action describes what is being done by the orchestration.
        /// If tracing is turned on, the action will be traced.
        /// If an exception occurs, this value can be used to set the fault description.
        /// </summary>
        public string Action
        {
            set
            {
                action = value;
                WriteTrace("Action = " + action);
            }
            get
            {
                return action;
            }
        }

        /// <summary>
        /// The ArchiveTag to use when archiving a message to the database
        /// </summary>
        public string ArchiveTag { get; set; }

        /// <summary>
        /// Writes a trace message to the logger with orchestration context information.
        /// </summary>
        /// <param name="message">The message to be written.</param>
        public void WriteTrace(string message)
        {
            if (trace)
            {
                try
                {
                    Logger.WriteTrace(serviceName + " instance " + instanceId + System.Environment.NewLine + message);
                }
                catch (Exception) { /* Ignore exceptions that occur when tracing. */ }
            }
        }
    }
}
