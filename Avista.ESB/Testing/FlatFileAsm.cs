
using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using Avista.ESB.Testing.Integration;
using Microsoft.BizTalk.TestTools.Schema;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Avista.ESB.Admin;
using Avista.ESB.Admin.Utility;
namespace Avista.ESB.Testing
{
    /// <summary>
    /// Wrapper class for the flat file assembler tool.
    /// </summary>
    public class FlatFileAsm
    {
        private string _flatFileAssemblerPath = "C:\\Program Files (x86)\\Microsoft BizTalk Server 2013 R2\\SDK\\Utilities\\PipelineTools\\FFAsm.exe";
        private TestableSchemaBase _schema = null;
        private TempFile _schemaFile = null;

        /// <summary>
        /// Constructs a flat file assembler for the given flat file schema.
        /// </summary>
        /// <param name="schema">The flat file schema to be used for assembling flat files.</param>
        public FlatFileAsm(TestableSchemaBase schema)
        {
            // Save the schema and create a temporary file for it.
            // Use Unicode as expected by the flat file assembler.
            _schema = schema;
            _schemaFile = new TempFile(_schema.XmlContent, "xsd", Encoding.Unicode);
        }

        /// <summary>
        /// Runs the flat file assembler to assemble a message into a flat file. The message is read from a stream.
        /// </summary>
        /// <param name="stream">The stream for the XML message which will be assembled into a flat file.</param>
        /// <returns>A temporary file containing the flat file output.</returns>
        public TempFile Assemble(Stream stream)
        {
            string message;
            using (StreamReader reader = new StreamReader(stream))
            {
                message = reader.ReadToEnd();
            }
            return Assemble(message);
        }

        /// <summary>
        /// Runs the flat file assembler to assemble the given message into a flat file.
        /// </summary>
        /// <param name="message">The XML message to be assembled into a flat file.</param>
        /// <returns>A temporary file containing the flat file output.</returns>
        public TempFile Assemble(string message)
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
                throw new Exception("Cannot assemble flat file from null or empty message.");
            }
            else
            {
                try
                {
                    // Create a temporary file for the input message.
                    inputFile = new TempFile(message, "xml", Encoding.UTF8);
                    inputFilePath = inputFile.FilePath;

                    // Create a temporary file for the output message.
                    outputFile = new TempFile("", "txt", Encoding.UTF8);
                    outputFilePath = outputFile.FilePath;
                    
                    // Run the flat file assembler.
                    schemaFilePath = _schemaFile.FilePath;
                    args = "\"" + inputFilePath + "\" -bs \"" + _schemaFile.FilePath + "\" -m \"" + outputFile.FilePath + "\"";
                    workingDirectory = Directory.GetCurrentDirectory();
                    ExternalProgram ffAsm = new ExternalProgram(_flatFileAssemblerPath, args, workingDirectory);
                    ffAsm.Run();
                    if (ffAsm.ExitCode != 0)
                    {
                        throw new Exception("FFAsm.exe exit code = " + ffAsm.ExitCode.ToString() + Environment.NewLine + ffAsm.Output);
                    }
                }
                catch (Exception exception)
                {
                    StringBuilder log = new StringBuilder();
                    log.AppendLine("Error assembling flat file from message starting with: ");
                    log.AppendLine("-------------------------------------------------------");
                    log.AppendLine(message.Substring(0, Math.Min(200, message.Length)));
                    log.AppendLine("-------------------------------------------------------");
                    AppendFileInfoToLog("Schema", schemaFilePath, log);
                    AppendFileInfoToLog("Input", inputFilePath, log);
                    AppendFileInfoToLog("Output", outputFilePath, log);
                    Exception newException = new Exception( log.ToString() + "\n\r" + exception.StackTrace);
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
