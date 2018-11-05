using System;
using System.Collections;
using System.Configuration;
using System.Diagnostics;
using System.IO;

namespace Avista.ESB.Utilities.Components
{
    /// <summary>
    /// Factory class which constructs configured provider classes.
    /// </summary>
    public static class Factory
    {
        /// <summary>
        /// Create an instance of a component based on an instance name and the class and assembly
        /// in which the component provider is found.
        /// </summary>
        /// <returns>A reference to the implementation.</returns>
        public static IComponent CreateComponent(string instanceName, string className, string assemblyName)
        {
            IComponent component = null;
            try
            {
                component = (IComponent)AssemblyHelper.CreateInstance(instanceName, className, assemblyName);
                component.RefreshConfiguration();
            }
            catch (Exception exception)
            {
                System.Diagnostics.Debug.WriteLine("Failed to create component '" + instanceName + "' from class '" + className + "' in asssembly '" + assemblyName + "'." + exception.ToString());                
                throw exception;                
            }
            return component;
        }
    }
}
