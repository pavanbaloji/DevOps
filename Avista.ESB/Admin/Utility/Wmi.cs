using System;
using System.Diagnostics;
using System.Management;


namespace Avista.ESB.Admin.Utility
{
    /// <summary>
    /// Helper class for using Windows Management Instrumentation (WMI).
    /// </summary>
    public static class Wmi
    {
        /// <summary>
        /// Loads a WMI scope on a given machine for a given namespace.
        /// </summary>
        /// <param name="machineName">The machine to bind to for loading the scope.</param>
        /// <param name="namespaceName">The namespace.</param>
        /// <returns>The ManagementScope instance that was constructed.</returns>
        public static ManagementScope LoadScope(string machineName, string namespaceName)
        {
            ManagementScope mgmtScope = null;
            // Create the scope object.
            try
            {
                mgmtScope = new ManagementScope(string.Format(@"\\{0}\{1}", machineName, namespaceName));
                mgmtScope.Connect();
            }
            catch (Exception exception)
            {
                //string message = string.Format("Unable to create WMI scope on '{0}' with namespace '{1}'.", machineName, namespaceName);
                //ContextualException contextualException = new ContextualException(message, 210, EventLogEntryType.Error, exception);
                //ExceptionManager.HandleException(contextualException, PolicyName.SystemException);
                  throw exception;
            }
            return mgmtScope;
        }

        /// <summary>
        /// Loads a WMI class instance on a given machine for a given namespace and class name.
        /// </summary>
        /// <param name="machineName">The machine to bind to for loading the WMI class.</param>
        /// <param name="namespaceName">The namespace of the WMI class.</param>
        /// <param name="className">The name of the WMI class.</param>
        /// <returns>The ManagementClass instance that was constructed.</returns>
        public static ManagementClass LoadClass(string machineName, string namespaceName, string className)
        {
            ManagementClass mgmtClass = null;
            // Create the class object.
            try
            {
                ManagementScope mgmtScope = LoadScope(machineName, namespaceName);
                ManagementPath mgmtPath = new ManagementPath(className);
                ObjectGetOptions options = new ObjectGetOptions();
                mgmtClass = new ManagementClass(mgmtScope, mgmtPath, options);
                // If the className is invalid, the mgmtClass will still be constructed but it will be unuseable.
                // Therefore we will check the ClassPath property. The check itself should throw an exception if the
                // class name was invalid, but in case it does not and the ClassPath is null then we will explicitly
                // throw an exception to be caught below.
                if (mgmtClass.ClassPath == null)
                {
                    throw new Exception("The ClassPath of the WMI class is null.");
                }
            }
            catch (Exception exception)
            {
                //string message = string.Format("Unable to load WMI class '{0}' on '{1}' under namespace '{2}'.", className, machineName, namespaceName);
                //ContextualException contextualException = new ContextualException(message, 211, EventLogEntryType.Error, exception);
                //ExceptionManager.HandleException(contextualException, PolicyName.SystemException);
                throw exception;
            }
            return mgmtClass;
        }

        /// <summary>
        /// Loads a management object matching a single filter key.
        /// </summary>
        /// <param name="machineName">The machine on which to load the management object.</param>
        /// <param name="namespaceName">The WMI namespace of the object.</param>
        /// <param name="className">The WMI class name of the object.</param>
        /// <param name="key">The key name.</param>
        /// <param name="value">The key value.</param>
        /// <returns>The management object or null if no matching object was found.</returns>
        /// <exception cref="ContextualException">Thrown as an Error with EventId 212 if there is an error loading the object.</exception>
        public static ManagementObject LoadObject(string machineName, string namespaceName, string className, string key, string value)
        {
            ManagementObject mgmtObject = null;
            try
            {
                ManagementClass managementClass = LoadClass(machineName, namespaceName, className);
                foreach (ManagementObject instance in managementClass.GetInstances())
                {
                    if (value.Equals(instance[key].ToString(), StringComparison.OrdinalIgnoreCase))
                    {
                        mgmtObject = instance;
                        break;
                    }
                }
            }
            catch (Exception exception)
            {
                //string message = string.Format("Unable to load WMI object '{0}' with '{1}'='{2}' on '{3}' under namespace '{4}'.", className, key, value, machineName, namespaceName);
                //ContextualException contextualException = new ContextualException(message, 212, EventLogEntryType.Error, exception);
                //ExceptionManager.HandleException(contextualException, PolicyName.SystemException);
                throw exception;
            }
            return mgmtObject;
        }

        /// <summary>
        /// Loads a management object matching two filter keys.
        /// </summary>
        /// <param name="machineName">The machine on which to load the management object.</param>
        /// <param name="namespaceName">The WMI namespace of the object.</param>
        /// <param name="className">The WMI class name of the object.</param>
        /// <param name="key">The first key name.</param>
        /// <param name="value">The first key value.</param>
        /// <param name="key2">The second key name.</param>
        /// <param name="value2">The second key value.</param>
        /// <exception cref="ContextualException">Thrown as an Error with EventId 212 if there is an error loading the object.</exception>
        public static ManagementObject LoadObject(string machineName, string namespaceName, string className, string key1, string value1, string key2, string value2)
        {
            ManagementObject mgmtObject = null;
            try
            {
                ManagementClass managementClass = LoadClass(machineName, namespaceName, className);
                foreach (ManagementObject instance in managementClass.GetInstances())
                {
                    if (value1.Equals(instance[key1].ToString(), StringComparison.OrdinalIgnoreCase) && value2.Equals(instance[key2].ToString(), StringComparison.OrdinalIgnoreCase))
                    {
                        mgmtObject = instance;
                        break;
                    }
                }
            }
            catch (Exception exception)
            {
                //string message = string.Format("Unable to load WMI object '{0}' with '{1}'='{2}' and '{3}'='{4}' on '{5}' under namespace '{6}'.", className, key1, value1, key2, value2, machineName, namespaceName);
                //ContextualException contextualException = new ContextualException(message, 212, EventLogEntryType.Error, exception);
                //ExceptionManager.HandleException(contextualException, PolicyName.SystemException);
                  throw exception;
            }
            return mgmtObject;
        }

        /// <summary>
        /// Determines if an object corresponding to a WMI management class exists based on a single filter key.
        /// </summary>
        /// <param name="managementClass">The management class to search.</param>
        /// <param name="key">The key name.</param>
        /// <param name="value">The key value.</param>
        /// <returns>A flag indicating if a matching object exists.</returns>
        /// <exception cref="ContextualException">Thrown as an Error with EventId 213 if there is an error checking for existence of the object.</exception>
        public static bool ObjectExists(ManagementClass managementClass, string key, string value)
        {
            bool found = false;
            try
            {
                foreach (ManagementObject instance in managementClass.GetInstances())
                {
                    if (value.Equals(instance[key].ToString(), StringComparison.OrdinalIgnoreCase))
                    {
                        found = true;
                        break;
                    }
                }
            }
            catch (Exception exception)
            {
                //string message = string.Format("Error checking for existence of WMI object with '{0}'='{1}'.", key, value);
                //ContextualException contextualException = new ContextualException(message, 213, EventLogEntryType.Error, exception);
                //ExceptionManager.HandleException(contextualException, PolicyName.SystemException);
                  throw exception;
            }
            return found;
        }

        /// <summary>
        /// Determines if an object corresponding to a WMI management class exists based on two filter keys.
        /// </summary>
        /// <param name="managementClass">The management class to search.</param>
        /// <param name="key1">The first key name.</param>
        /// <param name="value1">The first key value.</param>
        /// <param name="key2">The second key name.</param>
        /// <param name="value2">The second key value.</param>
        /// <returns>A flag indicating if a matching object exists.</returns>
        /// <exception cref="ContextualException">Thrown as an Error with EventId 213 if there is an error checking for existence of the object.</exception>
        public static bool ObjectExists(ManagementClass managementClass, string key1, string value1, string key2, string value2)
        {
            bool found = false;
            try
            {
                foreach (ManagementObject instance in managementClass.GetInstances())
                {
                    if (value1.Equals(instance[key1].ToString(), StringComparison.OrdinalIgnoreCase) && value2.Equals(instance[key2].ToString(), StringComparison.OrdinalIgnoreCase))
                    {
                        found = true;
                        break;
                    }
                }
            }
            catch (Exception exception)
            {
                //string message = string.Format("Error checking for existence of WMI object with '{0}'='{1}' and '{2}'='{3}'.", key1, value1, key2, value2);
                //ContextualException contextualException = new ContextualException(message, 213, EventLogEntryType.Error, exception);
                //ExceptionManager.HandleException(contextualException, PolicyName.SystemException);
                  throw exception;
            }
            return found;
        }
    }
}
