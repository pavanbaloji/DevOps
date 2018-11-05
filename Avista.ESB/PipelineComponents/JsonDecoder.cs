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
using HP.Practices.ExceptionHandling;
using HP.Practices.Logging;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using System.Xml;


namespace Avista.ESB.PipelineComponents
{
    [ComponentCategory(CategoryTypes.CATID_PipelineComponent)]
    [ComponentCategory(CategoryTypes.CATID_Decoder)]
    [Guid(JsonDecoder.ComponentID)]
    public sealed class JsonDecoder : BaseAvistaPipelineComponent, IComponent
    {
        const string ComponentID = "8DF96BDB-EE29-45AD-8C30-29319B2CEE55";

        private static string EmptyJson = "{}";
        
        #region  Component Properties
        /// <summary>
        /// Flag which indicates whether or not the pipeline component to add <Header></Header> node to root node of xml/json.
        /// </summary>
        [System.ComponentModel.Description("Enables or disables adding <Header> node to root of xml/json document.")]
        [System.ComponentModel.Browsable(true)]
        public bool AddHeaderNode
        {
            get;
            set;
        }

        /// <summary>
        /// Specify semi colon (;) separated context properties which needs to be populated into <Header> node.
        /// </summary>
        [System.ComponentModel.Description("Specify semi colon (;) separated context properties which needs to be populated into <Header> node.")]
        [System.ComponentModel.Browsable(true)]
        public string ContextPropertiesForHeader
        {
            get;
            set;
        }

        /// <summary>
        /// Specify root node name to be used in the generated xml
        /// </summary>
        [System.ComponentModel.Description("Specify root node name to be used in the generated xml")]
        [System.ComponentModel.Browsable(true)]
        public string RootNode
        {
            get;
            set;
        }

        /// <summary>
        /// Specify namespace to be used in the generated xml
        /// </summary>
        [System.ComponentModel.Description("Specify namespace to be used in the generated xml")]
        [System.ComponentModel.Browsable(true)]
        public string RootNodeNamespace
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
            get { return "Avista Json Decoder"; }
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
                return "Special decoder processing for Json messaging.";
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
            classID = new Guid(JsonDecoder.ComponentID);
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
            AddHeaderNode = ReadBool(propertyBag, "AddHeaderNode") ?? AddHeaderNode;
            ContextPropertiesForHeader = ReadString(propertyBag, "ContextPropertiesForHeader") ?? ContextPropertiesForHeader;
            RootNode = ReadString(propertyBag, "RootNode") ?? RootNode;
            RootNodeNamespace = ReadString(propertyBag, "RootNodeNamespace") ?? RootNodeNamespace;
            
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
            WriteBool(propertyBag, "AddHeaderNode", AddHeaderNode);
            WriteString(propertyBag, "ContextPropertiesForHeader", ContextPropertiesForHeader);
            WriteString(propertyBag, "RootNode", RootNode);
            WriteString(propertyBag, "RootNodeNamespace", RootNodeNamespace);

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

            if (string.IsNullOrEmpty(this.RootNode))
            {
                throw new ApplicationException("Json Decoder is missing RootNode Name.");
            }
            if (string.IsNullOrEmpty(this.RootNodeNamespace))
            {
                throw new ApplicationException("Json Decoder is missing RootNode Namespace.");
            }

            try
            {
                using(Stream msgBody = ProcessMessage(context, message))
                {
                    if (msgBody != null)
                    {
                        MemoryStream memoryStream = new MemoryStream();
                        XmlDocument xmlDocument = JsonConvert.DeserializeXmlNode(msgBody, this.RootNode, false, this.RootNodeNamespace, true);
                        xmlDocument.Save(memoryStream);
                        memoryStream.Seek(0L, SeekOrigin.Begin);
                        message.BodyPart.Data = memoryStream;
                    }
                }
            }
            catch (Exception exception)
            {
                ContextualException contextualException =
                    new ContextualException("Error while decoding Json message", exception);

                ExceptionManager.HandleException(contextualException, PolicyName.SystemException);
                throw contextualException;
            }
            return message;
        }

        #endregion

        #region Private Methods
        /// <summary>
        /// Processing body and url querystring
        /// </summary>
        /// <param name="msgBody"></param>
        /// <returns></returns>
        private Stream ProcessMessage(IPipelineContext context, IBaseMessage message)
        {
            if(!AddHeaderNode)
            {
                return message.BodyPart.GetOriginalDataStream();
            }

            var msgBody = MessageHelper.MessageToText(context, message.BodyPart);

            msgBody = Regex.Replace(msgBody, @"\s+", "");
            msgBody = string.IsNullOrWhiteSpace(msgBody) ? EmptyJson : msgBody;
            WriteTrace("msgBody:" + msgBody);

            OrderedDictionary msgDict = JsonConvert.DeserializeObject<OrderedDictionary>(msgBody);

            OrderedDictionary header = PopulateHeaderFromContext(message.Context);

            msgDict.Insert(0, "Header", header);

            string newBody = JsonConvert.SerializeObject(msgDict);

            WriteTrace("msgBody with params/props: " + newBody);

            MemoryStream stream = new MemoryStream();
            StreamWriter writer = new StreamWriter(stream);
            writer.Write(newBody);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }

        private OrderedDictionary PopulateHeaderFromContext(IBaseMessageContext msgContext)
        {
            OrderedDictionary header = new OrderedDictionary(); 
            string[] contextPropertiesForHeader = ContextPropertiesForHeader.Split(';');
            
            foreach(string contextPro in contextPropertiesForHeader)
            {
                string[] propertySet = contextPro.Split('#');
                if (propertySet.Length == 2)
                {
                    string value = (string)msgContext.Read(propertySet[1], propertySet[0]);
                    header.Add(propertySet[1], value ?? string.Empty);
                }
            }

            return header;
        }
        #endregion
    }
}

