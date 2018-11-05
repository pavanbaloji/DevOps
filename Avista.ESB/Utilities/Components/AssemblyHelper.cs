using System;
using System.Reflection;

namespace Avista.ESB.Utilities.Components
{
    /// <remarks>
    /// A helper class for working with assemblies. This class contains static methods that can
    /// be used to dynamically load assemblies and create instances of classes.
    /// </remarks>
    public class AssemblyHelper
    {
        /// <summary>
        /// Dynamically loads an assembly based on it's name.
        /// </summary>
        /// <param name="assemblyName">The name of the assembly to be loaded.</param>
        /// <exception cref="AssemblyLoadException">The assembly could not be loaded</exception>
        /// <returns>The assembly that has been loaded.</returns>
        public static Assembly LoadAssembly(string assemblyName)
        {
            Assembly assembly = null;
            try
            {
                assembly = Assembly.Load(assemblyName);
            }
            catch (Exception exception)
            {                
                throw exception;
            }
            return assembly;
        }

        /// <summary>
        /// Creates an instance of a class from a given assembly using a constructor that takes a single string
        /// argument containing a name for the instance.
        /// </summary>
        /// <param name="instanceName">The name to be used in constructing the instance.</param>
        /// <param name="className">The name of the class for which an instance is to be created.</param>
        /// <param name="assemblyName">The name of the assembly containing the class.</param>
        /// <returns>An object representing the loaded class instance.</returns>
        public static object CreateInstance(string instanceName, string className, string assemblyName)
        {
            object instance = null;
            try
            {
                Assembly assembly = LoadAssembly(assemblyName);
                object[] arguments = { instanceName };
                instance = assembly.CreateInstance(className, true, BindingFlags.CreateInstance, null, arguments, null, null);
            }
            catch (Exception exception)
            {                
                throw exception;
            }
            // Verify that the instance was created.
            if (instance == null)
            {
                Exception exception = new Exception("Failed to create instance of a class from a given assembly");
                throw exception;
            }
            return instance;
        }

        /// <summary>
        /// Creates an instance of a class from a given assembly using a constructor that takes none or multiple arguments.
        /// </summary>
        /// <param name="instanceName">The name to be used in constructing the instance.</param>
        /// <param name="arguments">The array of objects to pass as constructor arguments.</param>
        /// <param name="className">The name of the class for which an instance is to be created.</param>
        /// <param name="assemblyName">The name of the assembly containing the class.</param>
        /// <returns>An object representing the loaded class instance.</returns>
        public static object CreateInstance(string instanceName, object[] arguments, string className, string assemblyName)
        {
            object instance = null;
            try
            {
                Assembly assembly = LoadAssembly(assemblyName);
                instance = assembly.CreateInstance(className, true, BindingFlags.CreateInstance, null, arguments, null, null);
            }
            catch (Exception exception)
            {               
                throw exception;
            }
             //Verify that the instance was created.
            if (instance == null)
            {                
                Exception exception = new Exception("Failed to create instance of a class from a given assembly");
                throw exception;
            }
            return instance;
        }
    }
}
