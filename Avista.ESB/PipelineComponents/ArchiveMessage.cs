using System;
using System.Collections;
using System.Collections.Specialized;
using System.Drawing;
using System.Reflection;
using System.Resources;
using System.Runtime.InteropServices;
using System.IO;
using Microsoft.BizTalk.Component.Interop;
using Microsoft.BizTalk.Message.Interop;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using System.Xml;
using Avista.ESB.Utilities.Logging;
using Avista.ESB.Utilities;


namespace Avista.ESB.PipelineComponents
{
    [ComponentCategory(CategoryTypes.CATID_PipelineComponent)]
    [ComponentCategory(CategoryTypes.CATID_Any)]
    [Guid(ArchiveMessage.ComponentID)]
    public sealed class ArchiveMessage : BaseAvistaPipelineComponent, IComponent
    {
        const string ComponentID = "B1AA3260-358D-401E-AB9A-17BFF2CC2371";

        private static string EmptyJson = "{}";
        
        #region  Component Properties

        /// <summary>
        /// Specifies a custom tag that can be use to categorize and help identify the message within the message archive.
        /// </summary>
        [System.ComponentModel.Description("Specifies a custom tag that can be use to categorize and help identify the message within the message archive.")]
        [System.ComponentModel.Browsable(true)]
        public string Tag
        {
            get;
            set;
        }
        #endregion

        #region IBaseComponent

        /// <summary>
        /// Returns the name of the component as it should appear in the pipeline designer.
        /// </summary>
        /// <seealso cref="IBaseComponent.Name"/>
        [System.ComponentModel.Description("The name of the component as it appears in the pipeline designer.")]
        [System.ComponentModel.Browsable(false)]
        public override string Name
        {
            get { return "ArchiveMessage"; }
        }

        /// <summary>
        /// Returns the description of the component as it should appaer in the pipeline designer.
        /// </summary>
        /// <seealso cref="IBaseComponent.Description"/>
        [System.ComponentModel.Description("The description of the component as it appears in the pipeline designer.")]
        [System.ComponentModel.Browsable(false)]
        public override string Description
        {
            get
            {
                return "Message Archive Component.";
            }
        }
        #endregion


        #region IPersistPropertyBag

        /// <summary>
        /// Gets the GUID that identifies the pipeline component.
        /// </summary>
        /// <seealso cref="IPersistPropertyBag.GetClassID()"/>
        /// <param name="classID">The GUID that identifies the pipeline component.</param>
        public override void GetClassID(out Guid classID)
        {
            classID = new Guid(ArchiveMessage.ComponentID);
        }

        /// <summary>
        /// Loads properties from a property bag into this instance of the pipeline component.
        /// These properties will be editable in the pipeline properties window in BizTalk Administrator.
        /// </summary>
        /// <seealso cref="IPersistPropertyBag.Load()"/>
        /// <param name="propertyBag">The property bag containing property values.</param>
        /// <param name="errorLog">Not used.</param>
        public override void Load(IPropertyBag propertyBag, int errorLog)
        {
            Tag = ReadString(propertyBag, "Tag") ?? Tag;            
        }

        /// <summary>
        /// Saves the properties of the pipeline component to a property bag.
        /// These properties will be editable in the pipeline properties window in BizTalk Administrator.
        /// </summary>
        /// <seealso cref="IPersistPropertyBag.Save()"/>
        /// <param name="propertyBag">The property bag in which the pipeline properties should be stored.</param>
        /// <param name="clearDirty">Not used.</param>
        /// <param name="saveAllProperties">Not used.</param>
        public override void Save(IPropertyBag propertyBag, bool clearDirty, bool saveAllProperties)
        {
            WriteString(propertyBag, "Tag", Tag);
        }
        #endregion

        #region IComponent

        /// <summary>
        /// Execute  (pipeline component entry)
        /// </summary>
        /// <param name="context"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public IBaseMessage Execute(IPipelineContext context, IBaseMessage message)
        {
            WriteTrace(string.Format("Inside execute of \"{0}\"", Name));

            if (string.IsNullOrEmpty(this.Tag) || string.IsNullOrWhiteSpace(this.Tag))
            {
                
                this.Tag = "||||";
            }

            try
            {
                // Archive the message.
                ArchiveManager archiveManager = new ArchiveManager();

                string messageId = archiveManager.ArchiveMessage(context, message, 0, true, this.Tag);
                WriteTrace("Message Id: '" + messageId + "' has been archived.");

                SetMetadata(message, this.Tag, messageId);
            }
            catch (Exception exception)
            {
                WriteTrace(string.Format("Error occured in {0} \r\n Details: {1}", Name, exception.ToString()));
                throw exception;
            }
            return message;
        }

        #endregion

        private void SetMetadata(IBaseMessage message, string tag, string messageId)
        {
            string strMetadata = string.Empty;
            XmlDocument metadata = new XmlDocument();
            try
            {
                strMetadata = (string)message.Context.Read("MessageMetadata", "http://www.avistacorp.com/schemas/Avista.ESB.Utilities/v1.0");
            }
            catch (Exception) { }
            try
            {
                if (string.IsNullOrEmpty(strMetadata))
                {
                    metadata.LoadXml("<Metadata></Metadata>");
                }
                else
                {
                    metadata.LoadXml(strMetadata);
                }

                // Construct a new 
                XmlElement dataElement = metadata.CreateElement("Data");
                dataElement.SetAttribute("category", "ArchiveMessageId");
                dataElement.SetAttribute("id", tag);
                dataElement.SetAttribute("type", "String");
                dataElement.InnerText = messageId;
                metadata.DocumentElement.AppendChild(dataElement);

                message.Context.Write("MessageMetadata", "http://www.avistacorp.com/schemas/Avista.ESB.Utilities/v1.0", metadata.OuterXml);
            }
            catch (Exception ex)
            {
                Logger.WriteTrace("Unable to write to the metadata after archiving messageId " + messageId + "Error Details: " + ex.ToString());
            }

        }
    }
}

