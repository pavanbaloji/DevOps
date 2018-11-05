using System;
using System.Diagnostics;
using System.Reflection;
using System.IO;

namespace Avista.ESB.Testing
{
    /// <summary>
    /// Contains helper methods for working with resources.
    /// </summary>
    public static class ResourceHelper
    {
        /// <summary>
        /// Loads a resource into a string.
        /// </summary>
        /// <param name="assemblyName">The name of the assembly containing the resource.</param>
        /// <param name="resourceName">The name of the resource.</param>
        /// <returns>A string containing the content of the resource.</returns>
        public static string LoadAsString(Assembly assembly, string resourceName)
        {
            String text = null;
            string assemblyName = "unknown";
            try
            {
                assemblyName = assembly.GetName().Name;
                string qualifiedResourceName = assemblyName + "." + resourceName;
                using (Stream stream = assembly.GetManifestResourceStream(qualifiedResourceName))
                {
                    using (StreamReader sr = new StreamReader(stream))
                    {
                        text = sr.ReadToEnd();
                    }
                }
            }
            catch (Exception exception)
            {
                  string message = "Error loading resource '" + resourceName + "' from assembly '" + assemblyName + "'.";
                  Exception newException = new Exception( message + "\n\r" + exception.StackTrace );
                  throw newException;
            }
            return text;
        }
    }
}
