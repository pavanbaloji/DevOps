using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using Avista.ESB.Admin.Utility;
using Avista.ESB.Testing.Integration;
using Microsoft.BizTalk.TestTools.Schema;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Encoding = System.Text.Encoding;

namespace Avista.ESB.Testing
{
    /// <summary>
    /// Wrapper class for the flat file dissassembler tool.
    /// </summary>
    public class FlatFileDasm
    {
        private String _ffDasmPath = "C:\\Program Files (x86)\\Microsoft BizTalk Server 2013 R2\\SDK\\Utilities\\PipelineTools\\FFDasm.exe";
        private TestableSchemaBase _schema = null;
        private TempFile _schemaFile = null;

        /// <summary>
        /// Constructs a flat file disassembler for the given flat file schema.
        /// </summary>
        /// <param name="schema">The flat file schema to be used for disassembling flat files.</param>
        public FlatFileDasm(TestableSchemaBase schema)
        {
            // Save the schema and create a temporary file for it.
            // Use Unicode as expected by the flat file disassembler.
            _schema = schema;
            _schemaFile = new TempFile(_schema.XmlContent, "xsd", Encoding.Unicode);
        }

        /// <summary>
        /// Runs the flat file disassembler to disassemble a flat file into an XML message.
        /// The flat file is read from a stream.
        /// </summary>
        /// <param name="stream">The stream for the flat file message which will be disassembled into an XML file.</param>
        /// <returns>A temporary file containing the XML output.</returns>
        public TempFile Disassemble(Stream stream)
        {
            String message;
            using (StreamReader reader = new StreamReader(stream))
            {
                message = reader.ReadToEnd();
            }
            return Disassemble(message);
        }

        /// <summary>
        /// Runs the flat file disassembler to disassemble a flat file into an XML message.
        /// The flat file is read from a byte array.
        /// </summary>
        /// <param name="buffer">The byte array containing the flat file.</param>
        /// <returns>A temporary file containing the XML output.</returns>
        public TempFile Disassemble(byte[] buffer)
        {
            String message = Encoding.Default.GetString(buffer);
            return Disassemble(message);
        }

        /// <summary>
        /// Runs the flat file disassembler to disassemble the given flat file into an XML file.
        /// </summary>
        /// <param name="message">The flat file message to be disassembled into an XML file.</param>
        /// <returns>A temporary file containing the XML output.</returns>
        public TempFile Disassemble(String message)
        {
            TempFile inputFile = null;
            string inputFilePath = "";
            TempFile outputFile = null;
            string outputFilePath = "";
            string schemaFilePath = "";
            string args = "";
            string workingDirectory = "";
            if (String.IsNullOrEmpty(message))
            {
                throw new Exception("Cannot disassemble flat file from null or empty message.");
            }
            else
            {
                try
                {
                    // Create a temporary file for the input message.
                    inputFile = new TempFile(message, "txt", Encoding.UTF8);
                    inputFilePath = inputFile.FilePath;

                    // Create a temporary file for the output message.
                    outputFile = new TempFile("", "xml", Encoding.UTF8);
                    outputFilePath = outputFile.FilePath;

                    // Run the flat file disassembler.
                    schemaFilePath = _schemaFile.FilePath;
                    args = "\"" + inputFile.FilePath + "\" -bs \"" + _schemaFile.FilePath + "\" -m \"" + outputFile.FilePath + "\"";
                    workingDirectory = Directory.GetCurrentDirectory();
                    ExternalProgram ffDasm = new ExternalProgram(_ffDasmPath, args, workingDirectory);
                    ffDasm.Run();
                    if (ffDasm.ExitCode != 0)
                    {
                        throw new Exception(" FFDasm.exe exit code = " + ffDasm.ExitCode.ToString() + Environment.NewLine + ffDasm.Output);
                    }
                }
                catch (Exception exception)
                {
                    StringBuilder log = new StringBuilder();
                    log.AppendLine("Error disassembling flat file from message starting with: ");
                    log.AppendLine("-------------------------------------------------------");
                    log.AppendLine(message.Substring(0, Math.Min(200, message.Length)));
                    log.AppendLine("-------------------------------------------------------");
                    AppendFileInfoToLog("Schema", schemaFilePath, log);
                    AppendFileInfoToLog("Input", inputFilePath, log);
                    AppendFileInfoToLog("Output", outputFilePath, log);
                    Exception newException = new Exception( log.ToString() + "\n\r" + exception.StackTrace );
                    throw newException;
                }
            }
            // We must explicitly keep the input file alive until after the disassembler has run.
            // The output file is safe because we are returning it and the schema file is safe because it is an instance object.
            GC.KeepAlive(inputFile);

            // Return the output file.
            return outputFile;
        }

        private void AppendFileInfoToLog(string alias, string path, StringBuilder log)
        {
            bool exists = (path != "" && File.Exists(path));
            int length = 0;
            string content = "";
            if (exists)
            {
                try
                {
                    FileInfo fileInfo = new FileInfo(path);
                    length = (int)fileInfo.Length;
                    content = File.ReadAllText(path);
                }
                catch (Exception e2)
                {
                    content = "Error getting " + alias + " info. " + e2.Message;
                }
            }
            log.AppendLine(alias + " Path = " + path);
            log.AppendLine(alias + " Exists = " + exists.ToString());
            log.AppendLine(alias + " Length = " + length.ToString());
            log.AppendLine(alias + " Content:");
            log.AppendLine("-------------------------------------------------------");
            log.AppendLine(content.Substring(0, Math.Min(5000, content.Length)));
            log.AppendLine("-------------------------------------------------------");
        }
    }
}
