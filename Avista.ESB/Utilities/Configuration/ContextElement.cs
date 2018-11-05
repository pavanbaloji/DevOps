using System;
using System.Configuration;

namespace Avista.ESB.Utilities.Configuration
{
    /// <summary>
    /// The ContextElement class represents an element in the application configuration
    /// file used to specify context properties for the execution environment.
    /// </summary>
    public class ContextElement : ConfigurationElement
    {
        private static readonly ConfigurationProperty propEnvironment = new ConfigurationProperty(
            "environment",
            typeof(string),
            null,
            ConfigurationPropertyOptions.IsRequired
        );

        private static readonly ConfigurationProperty propTrace = new ConfigurationProperty(
            "trace",
            typeof(bool),
            false,
            ConfigurationPropertyOptions.None
        );

        private static ConfigurationPropertyCollection properties;

        /// <summary>
        /// Static constructor prepares the property collection.
        /// </summary>
        static ContextElement()
        {
            properties = new ConfigurationPropertyCollection();
            properties.Add(propEnvironment);
            properties.Add(propTrace);
        }

        /// <summary>
        /// Default constructor does nothing.
        /// </summary>
        public ContextElement()
        {
        }

        /// <summary>
        /// Gets the Environment setting. This indicated the name of the runtime environment (for example: Dev, Test, or Prod).
        /// </summary>
        [ConfigurationProperty("environment", IsRequired = true)]
        public string Environment
        {
            get { return (string)base[propEnvironment]; }
        }

        /// <summary>
        /// Gets the Trace setting. This flag indicates whether or not updates to the OrchestrationContext should be traced.
        /// </summary>
        [ConfigurationProperty("trace", IsRequired = false)]
        public bool Trace
        {
            get { return (bool)base[propTrace]; }
        }

        /// <summary>
        /// Override the Properties collection and return our custom one.
        /// </summary>
        protected override ConfigurationPropertyCollection Properties
        {
            get { return properties; }
        }
    }
}
