using Microsoft.BizTalk.Component.Interop;
using Microsoft.BizTalk.Message.Interop;
using Microsoft.BizTalk.Streaming;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Threading.Tasks;

namespace Avista.ESB.Utilities
{
    public static class MessageHelper
    {
        /// <summary>
        /// Returns a formatted XML string for a given XML document. 
        /// </summary>
        /// <param name="document">The document to be formatted.</param>
        /// <param name="omitXmlDeclaration">Fla indicating if the XML declaration should be ommitted.</param>
        /// <param name="indent">Flag indicating if the the formatted output should be indented.</param>
        /// <returns>A string containing the formatted XML.</returns>
        public static string Format(string xml, bool omitXmlDeclaration, bool indent)
        {
            string output = xml;
            try
            {
                XmlDocument document = new XmlDocument();
                document.LoadXml(xml);
                output = Format(document, omitXmlDeclaration, indent);
            }
            catch (Exception)
            {
                // If there is an exception we will just return the original xml.
            }
            return output;
        }


        /// <summary>
        /// Returns a formatted XML string for a given XML document. 
        /// </summary>
        /// <param name="document">The document to be formatted.</param>
        /// <param name="omitXmlDeclaration">Fla indicating if the XML declaration should be ommitted.</param>
        /// <param name="indent">Flag indicating if the the formatted output should be indented.</param>
        /// <returns>A string containing the formatted XML.</returns>
        public static string Format(XmlDocument document, bool omitXmlDeclaration, bool indent)
        {
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.OmitXmlDeclaration = omitXmlDeclaration;
            settings.Indent = indent;
            settings.NamespaceHandling = NamespaceHandling.OmitDuplicates;
            settings.NewLineHandling = NewLineHandling.None;
            StringBuilder builder = new StringBuilder();
            XmlWriter writer = XmlWriter.Create(builder, settings);
            document.Save(writer);
            return builder.ToString();
        }

        /// <summary>
        /// Applies indentation to every line of a multiline block of text.
        /// </summary>
        /// <param name="input">The input text to be indented.</param>
        /// <param name="indentation">The string to use for indentation.</param>
        /// <param name="indentFirstLine">Flag indicating if the first line of text should be indented. If false is specified then only the second and subsequent lines will be indented.</param>
        /// <returns>The input string with indentation applied.</returns>
        public static string Indent(string input, string indentation, bool indentFirstLine)
        {
            StringBuilder output = new StringBuilder("");
            int start = 0;
            int end = input.IndexOf(Environment.NewLine, start);
            int line = 1;
            string text = "";
            // loop through the input and add one line at a time and indent as needed.
            while (end >= 0)
            {
                text = input.Substring(start, end - start);
                if (line > 1 || indentFirstLine)
                {
                    output.Append(indentation);
                }
                output.Append(text);
                output.Append(Environment.NewLine);
                start = end + Environment.NewLine.Length;
                end = input.IndexOf(Environment.NewLine, start);
                line++;
            }
            // Add any text that follows the last newline.
            if (start + 1 < input.Length)
            {
                end = input.Length;
                text = input.Substring(start, end - start);
                if (line > 1 || indentFirstLine)
                {
                    output.Append(indentation);
                }
                output.Append(text);
                line++;
            }
            return output.ToString();
        }

        /// <summary>
        /// Converts a byte array to a hexadecimal string.
        /// </summary>
        /// <param name="data">The byte array contaning byte data to be converted.</param>
        /// <returns>The hexadecimal string resulting from the byte array.</returns>
        public static string ByteArrayToHexDump(byte[] data, int bytesPerLine)
        {
            StringBuilder stringBuilder = new StringBuilder(data.Length * 2);
            int column = 0;
            foreach (byte b in data)
            {
                column++;
                if (column > bytesPerLine)
                {
                    stringBuilder.Append(Environment.NewLine);
                    column = 1;
                }
                if (column == 1)
                {
                    stringBuilder.AppendFormat("{0:x2}", b);
                }
                else
                {
                    stringBuilder.AppendFormat(" {0:x2}", b);
                }
            }
            return stringBuilder.ToString();
        }

        /// <summary>
        /// Obtains a byte array representation of an IBaseMessagePart. The underlying message stream is read in
        /// order to populate the byte array.
        /// </summary>
        /// <param name="pipelineContext">The pipeline context.</param>
        /// <param name="messagePart">The message part.</param>
        /// <returns>A byte array representation of the IBaseMessagePart.</returns>
        public static byte[] MessageToByteArray(IPipelineContext pipelineContext, IBaseMessagePart messagePart)
        {
            byte[] buffer = null;
            Stream dataStream = null;
            try
            {
                dataStream = GetReadOnlySeekableDataStream(pipelineContext, messagePart);
                if (dataStream.Position != 0L)
                {
                    dataStream.Position = 0L;
                }
                byte[] bufferFromStream = new byte[dataStream.Length];
                int count = dataStream.Read(bufferFromStream, 0, bufferFromStream.Length);
                buffer = bufferFromStream;
            }
            catch (Exception)
            {
                // Ignore exceptions here. If the document can not be read into a byte array then the method returns null.
            }
            finally
            {
                if (dataStream != null && dataStream.CanSeek)
                {
                    dataStream.Position = 0L;
                }
            }
            return buffer;
        }


        /// <summary>
        /// Obtains a text string representation of an IBaseMessagePart. The underlying message stream is read in
        /// order to populate the string. This method will return null if the message cannot be represented as a
        /// string. This may happen in the case of a binary message.
        /// </summary>
        /// <param name="pipelineContext">The pipeline context.</param>
        /// <param name="messagePart">The message part.</param>
        /// <returns>A string representation of the IBaseMessagePart or null if the message cannot be represented as a text string.</returns>
        public static string MessageToText(IPipelineContext pipelineContext, IBaseMessagePart messagePart)
        {
            string text = null;
            Stream dataStream = null;
            try
            {
                dataStream = GetReadOnlySeekableDataStream(pipelineContext, messagePart);
                if (dataStream.Position != 0L)
                {
                    dataStream.Position = 0L;
                }
                StreamReader streamReader = new StreamReader(dataStream);
                text = streamReader.ReadToEnd();
                // Check for null characters in first kilobyte which would indicate probable binary data.
                for (int p = 1; p < 1024 && p < text.Length; p++)
                {
                    if (text[p - 1] == '\0')
                    {
                        text = null;
                        break;
                    }
                }
            }
            catch (Exception)
            {
                // Ignore exceptions here. If the document could not be converted to text then the method returns null.
            }
            finally
            {
                if (dataStream != null && dataStream.CanSeek)
                {
                    dataStream.Position = 0L;
                }
            }
            return text;
        }

        /// <summary>
        /// Returns a seekable version of a message's data stream. The stream will be wrapped in a ReadOnlySeekableDataStream if needed.
        /// </summary>
        /// <param name="pipelineContext">The pipeline context of the message.</param>
        /// <param name="messagePart">The message part for which a seekable stream is required.</param>
        /// <returns>A seekable version of the message stream.</returns>
        public static Stream GetReadOnlySeekableDataStream(IPipelineContext pipelineContext, IBaseMessagePart messagePart)
        {
            Stream dataStream = messagePart.GetOriginalDataStream();
            if (!dataStream.CanSeek)
            {
                ReadOnlySeekableStream seekableStream = new ReadOnlySeekableStream(dataStream);
                messagePart.Data = seekableStream;
                pipelineContext.ResourceTracker.AddResource(seekableStream);
                dataStream = seekableStream;
            }
            return dataStream;
        }

        public static XmlDocument MessageToXmlDocument(IPipelineContext pipelineContext, IBaseMessagePart messagePart)
        {
            XmlDocument document = null;
            Stream dataStream = null;
            try
            {
                dataStream = GetReadOnlySeekableDataStream(pipelineContext, messagePart);
                if (dataStream.Position != 0L)
                {
                    dataStream.Position = 0L;
                }
                XmlDocument documentFromStream = new XmlDocument();
                documentFromStream.Load(dataStream);
                document = documentFromStream;
            }
            catch (Exception)
            {
                // Ignore exceptions here. If the document could not be converted to XML then the method returns null.
            }
            finally
            {
                if (dataStream != null && dataStream.CanSeek)
                {
                    dataStream.Position = 0L;
                }
            }
            return document;
        }

        /// <summary>
        /// Returns the BizTalk message type using the namespace and root node name of an XML document.
        /// </summary>
        /// <param name="document">The XML document for which the message type is needed.</param>
        /// <returns>The BizTalk message type, or "Unknown" if the type cannot be determined.</returns>
        public static string GetMessageType(XmlDocument document)
        {
            string messageType = "Unknown";
            try
            {
                string namespaceUri = document.DocumentElement.NamespaceURI;
                string rootNodeName = document.DocumentElement.LocalName;
                if (String.IsNullOrEmpty(namespaceUri))
                {
                    messageType = rootNodeName;
                }
                else
                {
                    messageType = namespaceUri + "#" + rootNodeName;
                }
            }
            catch (Exception)
            {
                // In the event of an exception we will just return "Unknown".
            }
            return messageType;
        }
    }
}
