using DiffPlex;
using DiffPlex.DiffBuilder;
using DiffPlex.DiffBuilder.Model;
 using Microsoft.BizTalk.BaseFunctoids;
using Microsoft.XLANGs.BaseTypes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Xsl;
using Microsoft.BizTalk.TestTools.Mapper;
using Microsoft.BizTalk.TestTools.Schema;
using Avista.ESB.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Avista.ESB.Admin.Utility;
namespace Avista.ESB.Testing
{
    /// <summary>
    /// Schema TestHelper
    /// </summary>
    public class SchemaTestHelper : TestHelper
    {
        /// <summary>
        /// ExecuteSchemaTestXml
        /// </summary>
        /// <typeparam name="TSchema"></typeparam>
        /// <param name="assembly"></param>
        /// <param name="sampleResource"></param>
        /// <param name="targetSchemaNamespace"></param>
        /// <param name="testContext"></param>
        /// <param name="arguments"></param>
        /// <returns></returns>
        public bool ExecuteSchemaTestXml<TSchema>(Assembly assembly, string sampleResource, string targetSchemaNamespace = null,
                           TestContext testContext = null, params object[] arguments) where TSchema : TestableSchemaBase, new()
        {
            XmlDocument xmlDocument = null;

            return ExecuteSchemaTestXml<TSchema>(assembly, sampleResource, out xmlDocument, targetSchemaNamespace, testContext, arguments);
        }
        
        /// <summary>
        /// ExecuteSchemaTestXml
        /// </summary>
        /// <typeparam name="TSchema"></typeparam>
        /// <param name="assembly"></param>
        /// <param name="sampleResource"></param>
        /// <param name="generatedDoc"></param>
        /// <param name="targetSchemaNamespace"></param>
        /// <param name="testContext"></param>
        /// <param name="arguments"></param>
        /// <returns></returns>
        public bool ExecuteSchemaTestXml<TSchema>(Assembly assembly, string sampleResource, out XmlDocument generatedDoc, string targetSchemaNamespace = null,
                           TestContext testContext = null, params object[] arguments) where TSchema : TestableSchemaBase, new()
        {
            TestableSchemaBase schemaToTest;
            XmlTemplate xmlTemplate;
            generatedDoc = new XmlDocument();

            if (string.IsNullOrWhiteSpace(sampleResource))
            {
                return false;
            }

            schemaToTest = new TSchema();

            if (string.IsNullOrWhiteSpace(targetSchemaNamespace))
            {
                targetSchemaNamespace = schemaToTest.Schema.TargetNamespace;
            }

            xmlTemplate = new XmlTemplate(typeof(TSchema), targetSchemaNamespace);

            xmlTemplate.LoadFromResource(assembly, sampleResource);
            if (arguments == null)
            {
                xmlTemplate.Execute();
            }
            else
            {
                xmlTemplate.Execute(arguments);
            }
            var tempFile = new TempFile(xmlTemplate);

            var xmlValidationResult = new XmlValidationResult(schemaToTest,
                tempFile.FilePath);

            if (testContext != null)
            {
                testContext.WriteLine(
                    xmlValidationResult.IsValid
                        ? "Test successful for {0} good sample message. {1}"
                        : "Test failed for {0} good sample message. {1}",
                    schemaToTest.ToString(),
                    xmlValidationResult.Message);
            }

            generatedDoc.Load(tempFile.FilePath);

            return xmlValidationResult.IsValid;
        }


        /// <summary>
        /// ExecuteSchemaTestJson
        /// </summary>
        /// <typeparam name="TSchema"></typeparam>
        /// <param name="assembly"></param>
        /// <param name="sampleResource"></param>
        /// <param name="rootNodeName"></param>
        /// <param name="targetSchemaNamespace"></param>
        /// <param name="namespacePrefix"></param>
        /// <param name="testContext"></param>
        /// <returns></returns>
        public bool ExecuteSchemaTestJson<TSchema>(Assembly assembly, string sampleResource, string rootNodeName,
                             string targetSchemaNamespace, string namespacePrefix = "ns0",
                                TestContext testContext = null) where TSchema : TestableSchemaBase, new()
        {
            XmlDocument xmlDocument = null;

            return ExecuteSchemaTestJson<TSchema>(assembly, sampleResource, rootNodeName, targetSchemaNamespace, out xmlDocument, namespacePrefix, testContext);
        }

        public bool ExecuteSchemaTestJson<TSchema>(Assembly assembly, string sampleResource, string rootNodeName,
                             string targetSchemaNamespace, out XmlDocument generatedDoc, string namespacePrefix = "ns0",
                                TestContext testContext = null) where TSchema : TestableSchemaBase, new()
        {
            TestableSchemaBase schemaToTest;
            generatedDoc = null;

            if (string.IsNullOrWhiteSpace(sampleResource))
            {
                return false;
            }

            schemaToTest = new TSchema();

            generatedDoc =
                    JsonParser.ConvertJsonToXmlDocument(
                    ResourceHelper.LoadAsString(assembly, sampleResource),
                    rootNodeName, targetSchemaNamespace, namespacePrefix);

            var validationResult = new XmlValidationResult(new TSchema(), generatedDoc);

            if (testContext != null)
            {
                testContext.WriteLine(
                    validationResult.IsValid
                        ? "Test successful for {0} good sample message. {1}"
                        : "Test failed for {0} good sample message. {1}",
                    schemaToTest.ToString(),
                    validationResult.Message);
            }
            return validationResult.IsValid;
        }

        public bool TestDisassembleXml<TSchema>(Assembly assembly, string flatFile,
                           TestContext testContext = null, Action<XmlDocument> validationAction = null) where TSchema : TestableSchemaBase, new()
        {
            XmlDocument xmlDoc;

            return TestDisassembleXml<TSchema>(assembly, flatFile, out xmlDoc, testContext, validationAction);
        }

        /// <summary>
        /// TestDisassembleXml
        /// </summary>
        /// <typeparam name="TSchema"></typeparam>
        public bool TestDisassembleXml<TSchema>(Assembly assembly, string flatFile, out XmlDocument xmlDoc,
                           TestContext testContext = null, Action<XmlDocument> validationAction = null) where TSchema : TestableSchemaBase, new()
        {
            TestableSchemaBase schemaToTest;
            FlatFileDasm flatFileDasm;
            string message;
            TempFile disassembledFile;
            xmlDoc = null;

            if (string.IsNullOrWhiteSpace(flatFile))
            {
                return false;
            }

            schemaToTest = new TSchema();
            flatFileDasm = new FlatFileDasm(schemaToTest);
            message = ResourceHelper.LoadAsString(assembly, flatFile);
            disassembledFile = flatFileDasm.Disassemble(message);
            xmlDoc = new XmlDocument();

            xmlDoc.Load(disassembledFile.FilePath);

            if (testContext != null)
            {
                testContext.WriteLine("{0}", xmlDoc.OuterXml);
            }

            //schema validation
            XmlValidationResult validationResult = new XmlValidationResult(schemaToTest, disassembledFile.FilePath);
            if (testContext != null)
            {
                testContext.WriteLine(
                    validationResult.IsValid
                        ? "Disassembly Test successful for {0} sample message. {1}"
                        : "Disassembly Test failed for {0} sample message. {1}",
                    schemaToTest.ToString(),
                    validationResult.Message);
            }

            if (validationResult.IsValid && (validationAction != null))
            {
                validationAction(xmlDoc);
            }

            return validationResult.IsValid;
        }


        /// <summary>
        /// TestAssembleXml
        /// </summary>
        /// <typeparam name="TSchema"></typeparam>
        public bool TestAssembleXml<TSchema>(Assembly assembly, string xmlFile, string expectedFlatFile,
                           TestContext testContext = null, string wrapChar = null) where TSchema : TestableSchemaBase, new()
        {
            TestableSchemaBase flatFileSchema;
            FlatFileAsm flatFileAsm;
            string xmlMessage, expectedFlatFileContent, actualFlatFileContent;
            TempFile assembledFlatFile;

            if (string.IsNullOrWhiteSpace(xmlFile) || string.IsNullOrWhiteSpace(expectedFlatFile))
            {
                return false;
            }

            flatFileSchema = new TSchema();
            flatFileAsm = new FlatFileAsm(flatFileSchema);
            xmlMessage = ResourceHelper.LoadAsString(assembly, xmlFile);
            expectedFlatFileContent = ResourceHelper.LoadAsString(assembly, expectedFlatFile);
            assembledFlatFile = flatFileAsm.Assemble(xmlMessage);
            actualFlatFileContent = assembledFlatFile.ReadAllText();
            if (!string.IsNullOrWhiteSpace(wrapChar))
            {
                actualFlatFileContent = actualFlatFileContent.Replace(wrapChar, "");
            }

            var compareResult = String.Equals(expectedFlatFileContent, actualFlatFileContent);

            if (testContext != null)
            {
                testContext.WriteLine(
                    compareResult
                        ? "Assembly Test successful for {0} sample message."
                        : "Assembly Test failed for {0} sample message.",
                    flatFileSchema.ToString());
            }
            return compareResult;
        }
    }
}
