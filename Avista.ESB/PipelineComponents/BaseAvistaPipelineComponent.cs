using System;
using System.Diagnostics;
using Microsoft.BizTalk.Component.Interop;
using System.ComponentModel;
using System.Reflection;
using System.Drawing;
using System.Resources;
using System.Collections;
using Avista.ESB.Utilities.Logging;

namespace Avista.ESB.PipelineComponents
{
    public abstract class BaseAvistaPipelineComponent : IBaseComponent, IComponentUI, IPersistPropertyBag
    {
        private static readonly ResourceManager ResourceManager = 
            new ResourceManager("Avista.ESB.PipelineComponents.Resources.PipelineComponentResources", Assembly.GetExecutingAssembly());

        #region IBaseComponent

        /// <summary>
        /// Returns the name of the component as it should appear in the pipeline designer.
        /// </summary>
        /// <seealso cref="IBaseComponent.Name"/>
        public abstract string Name { get; }

        /// <summary>
        /// Returns the description of the component as it should appaer in the pipeline designer.
        /// </summary>
        /// <seealso cref="IBaseComponent.Description"/>
        public abstract string Description { get; }

        /// <summary>
        /// Returns the version of the component as it should appear in the pipeline designer.
        /// </summary>
        /// <seealso cref="IBaseComponent.Version"/>
        [System.ComponentModel.Description("The version of the component.")]
        [System.ComponentModel.Browsable(false)]
        public string Version
        {
            get
            {
                return Assembly.GetExecutingAssembly().GetName().Version.ToString(2);
            }
        }

        #endregion

        #region IPersistPropertyBag
        /// <summary>
        /// Returns the icon to be displayed for the component in the pipeline designer.
        /// </summary>
        /// <seealso cref="IComponentUI.Icon"/>
        [System.ComponentModel.Description("The icon used for the component in the pipeline designer.")]
        [System.ComponentModel.Browsable(false)]
        public IntPtr Icon
        {
            get
            {
                IntPtr icon = (IntPtr)null;
                Bitmap bitmap = null;
                if (ResourceManager != null)
                {
                    bitmap = (Bitmap)ResourceManager.GetObject("AvistaIcon");
                    if (bitmap != null)
                    {
                        icon = bitmap.GetHicon();
                    }
                }
                return icon;
            }
        }

        /// <summary>
        /// Validates the pipeline component's properties when they are configured in a pipeline.
        /// </summary>
        /// <seealso cref="IComponentUI.Validate()"/>
        /// <param name="projectSystem">Not used.</param>
        /// <returns>An enumerator for the list of validation errors.</returns>
        public IEnumerator Validate(object projectSystem)
        {
            var errorList = new ArrayList();
            yield return errorList.GetEnumerator();
        }

        /// <summary>
        /// Initializes a new instance of the pipeline component.
        /// </summary>
        /// <seealso cref="IPersistPropertyBag.InitNew()"/>
        public void InitNew()
        {
        }

        /// <summary>
        /// Gets the GUID that identifies the pipeline component.
        /// </summary>
        /// <seealso cref="IPersistPropertyBag.GetClassID()"/>
        /// <param name="classID">The GUID that identifies the pipeline component.</param>
        public abstract void GetClassID(out Guid classID);

        /// <summary>
        /// Loads properties from a property bag into this instance of the pipeline component.
        /// These properties will be editable in the pipeline properties window in BizTalk Administrator.
        /// </summary>
        /// <seealso cref="IPersistPropertyBag.Load()"/>
        /// <param name="propertyBag">The property bag containing property values.</param>
        /// <param name="errorLog">Not used.</param>
        public virtual void Load(IPropertyBag propertyBag, int errorLog) { }

        /// <summary>
        /// Saves the properties of the pipeline component to a property bag.
        /// These properties will be editable in the pipeline properties window in BizTalk Administrator.
        /// </summary>
        /// <seealso cref="IPersistPropertyBag.Save()"/>
        /// <param name="propertyBag">The property bag in which the pipeline properties should be stored.</param>
        /// <param name="clearDirty">Not used.</param>
        /// <param name="saveAllProperties">Not used.</param>
        public virtual void Save(IPropertyBag propertyBag, bool clearDirty, bool saveAllProperties) { }

        #endregion

        #region Internal Methods
        internal bool? ReadBool(IPropertyBag propertyBag, string propertyName)
        {
            bool? value = null;
            try
            {
                if (propertyBag != null)
                {
                    object obj = null;
                    try
                    {
                        propertyBag.Read(propertyName, out obj, 0);
                    }
                    catch (Exception)
                    {
                        // If the read failed then we will be returning null.
                    }
                    if (obj != null)
                    {
                        value = (bool)obj;
                    }
                }
            }
            catch (Exception ex)
            {
                Exception exception = new Exception("Error reading property '" + propertyName + "' from property bag.", ex);
                WriteTrace(exception.ToString());
                throw exception;
            }
            return value;
        }

        internal int? ReadInt(IPropertyBag propertyBag, string propertyName)
        {
            int? value = null;
            try
            {
                if (propertyBag != null)
                {
                    object obj = null;
                    try
                    {
                        propertyBag.Read(propertyName, out obj, 0);
                    }
                    catch (Exception)
                    {
                        // If the read failed then we will be returning null.
                    }
                    if (obj != null)
                    {
                        value = (int)obj;
                    }
                }
            }
            catch (Exception ex)
            {
                Exception exception = new Exception("Error reading property '" + propertyName + "' from property bag.", ex);
                WriteTrace(exception.ToString());
                throw exception;
            }
            return value;
        }

        internal string ReadString(IPropertyBag propertyBag, string propertyName)
        {
            string value = null;
            try
            {
                if (propertyBag != null)
                {
                    object obj = null;
                    try
                    {
                        propertyBag.Read(propertyName, out obj, 0);
                    }
                    catch (Exception)
                    {
                        // If the read failed then we will be returning null.
                    }
                    if (obj != null)
                    {
                        value = (string)obj;
                    }
                }
            }
            catch (Exception ex)
            {
                Exception exception = new Exception("Error reading property '" + propertyName + "' from property bag.", ex);
                WriteTrace(exception.ToString());
                throw exception;
            }
            return value;
        }

        internal void WriteBool(IPropertyBag propertyBag, string propertyName, bool? value)
        {
            try
            {
                if (propertyBag != null)
                {
                    if (value.HasValue)
                    {
                        object obj = value.Value;
                        propertyBag.Write(propertyName, ref obj);
                    }
                }
            }
            catch (Exception ex)
            {
                Exception exception = new Exception("Error writing property '" + propertyName + "' to property bag.", ex);
                WriteTrace(exception.ToString());
                throw exception;
            }
        }

        internal void WriteInt(IPropertyBag propertyBag, string propertyName, int? value)
        {
            try
            {
                if (propertyBag != null)
                {
                    if (value.HasValue)
                    {
                        object obj = value.Value;
                        propertyBag.Write(propertyName, ref obj);
                    }
                }
            }
            catch (Exception ex)
            {
                Exception exception = new Exception("Error writing property '" + propertyName + "' to property bag.", ex);
                WriteTrace(exception.ToString());
                throw exception;
            }
        }

        internal void WriteString(IPropertyBag propertyBag, string propertyName, string value)
        {
            try
            {
                if (propertyBag != null)
                {
                    if (!String.IsNullOrEmpty(value))
                    {
                        object obj = value;
                        propertyBag.Write(propertyName, ref obj);
                    }
                }
            }
            catch (Exception ex)
            {
                Exception exception = new Exception("Error writing property '" + propertyName + "' to property bag.", ex);
                WriteTrace(exception.ToString());
                throw exception;
            }
        }

        /// <summary>
        /// Writes a trace message if tracing is turned on.
        /// </summary>
        /// <param name="message">The message to be written.</param>
        internal void WriteTrace(string message)
        {
            Logger.WriteTrace(this.GetType().Name + message);
        }
        #endregion
    }
}
