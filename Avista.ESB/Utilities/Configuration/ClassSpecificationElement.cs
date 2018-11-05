using System;
using System.Configuration;

namespace Avista.ESB.Utilities.Configuration
{
    /// <summary>
    /// The ClassSpecificationElement class represents an element in the application
    /// configuration file used to specify a class.
    /// </summary>
    public class ClassSpecificationElement : ConfigurationElement
    {
        private static readonly ConfigurationProperty s_propName = new ConfigurationProperty(
            "name",
            typeof(string),
            null,
            ConfigurationPropertyOptions.IsRequired
        );

        private static readonly ConfigurationProperty s_propClass = new ConfigurationProperty(
            "class",
            typeof(string),
            null,
            ConfigurationPropertyOptions.IsRequired
        );

        private static readonly ConfigurationProperty s_propAssembly = new ConfigurationProperty(
            "assembly",
            typeof(string),
            null,
            ConfigurationPropertyOptions.IsRequired
        );

        private static ConfigurationPropertyCollection s_properties;

        /// <summary>
        /// Static constructor prepares the property collection.
        /// </summary>
        static ClassSpecificationElement()
        {
            s_properties = new ConfigurationPropertyCollection();
            s_properties.Add(s_propName);
            s_properties.Add(s_propClass);
            s_properties.Add(s_propAssembly);
        }

        /// <summary>
        /// Default constructor does nothing.
        /// </summary>
        public ClassSpecificationElement()
        {
        }
        
        /// <summary>
        /// Gets the Name setting.
        /// </summary>
        [ConfigurationProperty("name", IsRequired = true)]
        public string Name
        {
            get { return (string)base[s_propName]; }
        }

        /// <summary>
        /// Gets the Class setting.
        /// </summary>
        [ConfigurationProperty("class", IsRequired = true)]
        public string Class
        {
            get { return (string)base[s_propClass]; }
        }

        /// <summary>
        /// Gets the Assembly setting.
        /// </summary>
        [ConfigurationProperty("assembly", IsRequired = true)]
        public string Assembly
        {
            get { return (string)base[s_propAssembly]; }
        }
        
        /// <summary>
        /// Override the Properties collection and return our custom one.
        /// </summary>
        protected override ConfigurationPropertyCollection Properties
        {
            get { return s_properties; }
        }
    }
}
