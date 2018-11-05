
using System;
using System.IO;
using System.Xml;
using System.Xml.Schema;
using Microsoft.BizTalk.TestTools.Schema;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Avista.ESB.Testing
{
    /// <summary>
    /// Container for XML Schema validation results.
    /// </summary>
    public class XmlValidationResult
    {
        private bool isValid = true;

        private string message = string.Empty;

        /// <summary>
        /// Constructs an xml validation result for the given XmlDocument against the given schema.
        /// </summary>
        /// <param name="schemaObject">The object of the schema based class.</param>
        /// <param name="document">The XmlDocument instance.</param>
        public XmlValidationResult(TestableSchemaBase schemaObject, XmlDocument document)
        {
            XmlReaderSettings settings = new XmlReaderSettings();
            XmlSchemaSet schemaSet = new XmlSchemaSet();
            schemaSet.Add(schemaObject.Schema);
            settings.Schemas = schemaSet;
            settings.ValidationType = ValidationType.Schema;
            settings.ValidationEventHandler += new ValidationEventHandler(SchemaReaderSettingsValidationEventHandler);
            StringReader stringReader = new StringReader(document.InnerXml);
            using (XmlReader reader = XmlReader.Create(stringReader, settings))
            {
                while (reader.Read())
                {
                    if (!isValid)
                    {
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Constructs an xml validation result for the given instance of an XML file against the given schema.
        /// </summary>
        /// <param name="schemaObject">The object of the schema based class.</param>
        /// <param name="xmlInstancePath">The file path of the XML Instance.</param>
        public XmlValidationResult(TestableSchemaBase schemaObject, string xmlInstancePath)
        {
            XmlReaderSettings settings = new XmlReaderSettings();
            XmlSchemaSet schemaSet = new XmlSchemaSet();
            schemaSet.Add(schemaObject.Schema);
            settings.Schemas = schemaSet;
            settings.ValidationType = ValidationType.Schema;
            settings.ValidationEventHandler += new ValidationEventHandler(SchemaReaderSettingsValidationEventHandler);
            using (XmlReader reader = XmlReader.Create(xmlInstancePath, settings))
            {
                while (reader.Read())
                {
                    if (!isValid)
                    {
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Constructs an xml validation result for the given instance of an XML Instance Stream against the given schema.
        /// </summary>
        /// <param name="schemaObject">The object of the schema based class.</param>
        /// <param name="xmlInstanceStream">The stream representation of the XML Instance.</param>
        public XmlValidationResult(TestableSchemaBase schemaObject, Stream xmlInstanceStream)
        {
            XmlReaderSettings settings = new XmlReaderSettings();
            XmlSchemaSet schemaSet = new XmlSchemaSet();
            schemaSet.Add(schemaObject.Schema);
            settings.Schemas = schemaSet;
            settings.ValidationType = ValidationType.Schema;
            settings.ValidationEventHandler += new ValidationEventHandler(SchemaReaderSettingsValidationEventHandler);
            using (XmlReader reader = XmlReader.Create(xmlInstanceStream, settings))
            {
                while (reader.Read())
                {
                    if (!isValid)
                    {
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Returns true if the schema is valid and false if the schema is not valid.
        /// </summary>
        public bool IsValid
        {
            get
            {
                return isValid;
            }
        }

        /// <summary>
        /// Returns the error message if the schema validation failed.
        /// </summary>
        public string Message
        {
            get
            {
                return message;
            }
        }

        /// <summary>
        /// Event handler for XML validation events.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="eventArgs">Event arguments.</param>
        private void SchemaReaderSettingsValidationEventHandler(object sender, ValidationEventArgs arguments)
        {
            // when deserializing json arrays, the Newtonsoft parser adds this, but the schema isn't available for validation. Ignore this message.
            if (arguments.Message != "The 'http://james.newtonking.com/projects/json:Array' attribute is not declared.")
            {
                if (arguments.Severity == XmlSeverityType.Error)
                {
                    isValid = false;
                    message = arguments.Message;
                }
            }
        }
    }
}
