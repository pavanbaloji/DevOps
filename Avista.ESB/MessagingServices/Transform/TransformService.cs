using Avista.ESB.Utilities.Logging;
using Microsoft.BizTalk.Component.Interop;
using Microsoft.BizTalk.Message.Interop;
using Microsoft.BizTalk.Streaming;
using Microsoft.Practices.ESB.GlobalPropertyContext;
using Microsoft.Practices.ESB.Itinerary;
using Microsoft.Practices.ESB.Resolver;
using Microsoft.Practices.ESB.Utilities;
using Microsoft.XLANGs.BaseTypes;
using Microsoft.XLANGs.RuntimeTypes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Xml;
using System.Xml.XPath;

namespace Avista.ESB.MessagingServices.Transform
{
    public class TransformService : IMessagingService
    {
        public const string ValidateSourceKey = "ValidateSource";
        public const string PromoteDocSpecNameKey = "PromoteDocSpecName";

        [Browsable(true), ReadOnly(true)]
        public string Name
        {
            get
            {
                return "Avista.ESB.Utilities.TransformService";
            }
        }

        public bool SupportsDisassemble
        {
            get
            {
                return false;
            }
        }

        public bool ShouldAdvanceStep(IItineraryStep step, IBaseMessage msg)
        {
            return true;
        }

        public IBaseMessage Execute(IPipelineContext context, IBaseMessage msg, string resolverString, IItineraryStep step)
        {
            Logger.WriteTrace(string.Format("******{0} Started******", this.GetType().Name));
            if (string.IsNullOrEmpty(resolverString))
            {
                throw new ArgumentException("ResolverString is required.", "resolverString");
            }
            IBaseMessage result;
            try
            {
                bool validateSource = false;
                bool promoteDocSpecName = true;
                if (step != null)
                {
                    if (step.PropertyBag.ContainsKey("ValidateSource"))
                    {
                        validateSource = (string.Compare(step.PropertyBag["ValidateSource"], "true", true, CultureInfo.CurrentCulture) == 0);
                    }
                    if (step.PropertyBag.ContainsKey("PromoteDocSpecName"))
                    {
                        promoteDocSpecName = (string.Compare(step.PropertyBag["PromoteDocSpecName"], "false", true, CultureInfo.CurrentCulture) != 0);
                    }
                }
                result = this.ExecuteTransform(context, msg, resolverString, step, validateSource, promoteDocSpecName);
            }
            catch (Exception ex)
            {
                Logger.WriteTrace(string.Format("Error occured in {0} \r\n Details: {1}", this.GetType().Name, ex.ToString()));
                throw ex;
            }
            Logger.WriteTrace(string.Format("******{0} Completed******", this.GetType().Name));
            return result;
        }

        public IBaseMessage ExecuteTransform(IPipelineContext context, IBaseMessage msg, string resolverString, IItineraryStep step, bool validateSource, bool promoteDocSpecName = true)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }
            if (msg == null)
            {
                throw new ArgumentNullException("msg");
            }
            if (string.IsNullOrEmpty(resolverString))
            {
                throw new Exception("Resolver string is required to determine map name.");
            }

            IBaseMessage result;
            ArrayList mapList = new ArrayList();
            string mapName = string.Empty;

            try
            {
                foreach (string resolver in step.ResolverCollection)
                {
                    ResolverInfo resolverInfo = ResolverMgr.GetResolverInfo(ResolutionType.Transform, resolver);

                    Dictionary<string, string> dictionary = ResolverMgr.Resolve(resolverInfo, msg, context);
                    if (dictionary.ContainsKey("TransformType") && !string.IsNullOrEmpty(dictionary["TransformType"]))
                    {
                        mapList.Add(dictionary["TransformType"]);
                        dictionary.Remove("TransformType");
                    }
                    else
                    {
                        mapList.Add(dictionary["Resolver.TransformType"]);
                        dictionary.Remove("Resolver.TransformType");
                    }
                }

                Stream stream = msg.BodyPart.GetOriginalDataStream();
                if (!stream.CanSeek)
                {
                    stream = new ReadOnlySeekableStream(stream)
                    {
                        Position = 0L
                    };
                }
                else
                {
                    stream.Position = 0L;
                }

                string btsMsgType = msg.Context.Read(BtsProperties.MessageType.Name, BtsProperties.MessageType.Namespace) as string;

                string obj = null;
                if (string.IsNullOrEmpty(btsMsgType))
                {
                    btsMsgType = Microsoft.Practices.ESB.Utilities.MessageHelper.GetMessageType(msg, context);
                }

                mapName = GetSourceMessageMatchingMapName(mapList, btsMsgType, context);

                IBaseMessage baseMessage = Microsoft.Practices.ESB.Utilities.MessageHelper.CreateNewMessage(context, msg, TransformStream(stream, mapName, validateSource, ref btsMsgType, ref obj));
                baseMessage.Context.Write(BtsProperties.SchemaStrongName.Name, BtsProperties.SchemaStrongName.Namespace, null);
                baseMessage.Context.Promote(BtsProperties.MessageType.Name, BtsProperties.MessageType.Namespace, btsMsgType);
                if (promoteDocSpecName)
                {
                    baseMessage.Context.Write(DasmProperties.DocumentSpecName.Name, DasmProperties.DocumentSpecName.Namespace, obj);
                }
                Microsoft.Practices.ESB.Utilities.MessageHelper.SetDocProperties(context, baseMessage);
                result = baseMessage;
            }
            catch (Exception)
            {
                throw;
            }
            return result;
        }

        private string GetSourceMessageMatchingMapName(ArrayList mapList, string btsMsgType, IPipelineContext context)
        {
            string mapName = string.Empty;
            if (mapList.Count > 1)
            {
                foreach (string map in mapList)
                {
                    try
                    {
                        Type type = Type.GetType(map);
                        TransformMetaData transformMetaData = TransformMetaData.For(type);
                        SchemaMetadata sourceSchemaMetadata = transformMetaData.SourceSchemas[0];
                        string mapSourceSchemaName = sourceSchemaMetadata.SchemaName;
                        IDocumentSpec documentSpec = context.GetDocumentSpecByType(btsMsgType);
                        string msgSourceSchemaName = documentSpec.DocType;

                        Logger.WriteTrace("Map Source Schema Name: " + mapSourceSchemaName);
                        Logger.WriteTrace("Msg Source Schema Name: " + msgSourceSchemaName);

                        if (string.Compare(mapSourceSchemaName, msgSourceSchemaName, false, CultureInfo.CurrentCulture) == 0)
                        {
                            Logger.WriteTrace("Match found: " + map);
                            mapName = map;
                            break;
                        }
                    }
                    catch (Exception) { }
                }
            }
            else
            {
                mapName = mapList[0].ToString();
            }

            return mapName;
        }

        private static Stream TransformStream(Stream stream, string mapName, bool validate, ref string messageType, ref string targetDocumentSpecName)
        {
            Type type = Type.GetType(mapName);
            if (null == type)
            {
                throw new Exception("Invalid MapType" + mapName);
            }

            TransformMetaData transformMetaData = TransformMetaData.For(type);
            SchemaMetadata sourceSchemaMetadata = transformMetaData.SourceSchemas[0];
            string schemaName = sourceSchemaMetadata.SchemaName;
            SchemaMetadata targetSchemaMetadata = transformMetaData.TargetSchemas[0];

            if (validate && string.Compare(messageType, schemaName, false, CultureInfo.CurrentCulture) != 0)
            {
                throw new Exception("Source Document Mismatch. MessageType: " + messageType +" Schema Name: " + schemaName);
            }

            messageType = targetSchemaMetadata.SchemaName;
            targetDocumentSpecName = targetSchemaMetadata.SchemaBase.GetType().AssemblyQualifiedName;

            XmlReader reader = XmlReader.Create(stream);

            XPathDocument input = new XPathDocument(reader);
            ITransform transform = transformMetaData.Transform;
            Stream outStream = new MemoryStream();
            transform.Transform(input, transformMetaData.ArgumentList, outStream, new XmlUrlResolver());
            outStream.Flush();
            outStream.Seek(0L, SeekOrigin.Begin);
            return outStream;
        }
    }
}
