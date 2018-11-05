using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading;
using System.Xml;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Avista.ESB.Admin.Utility
{
    public abstract class TestHelper : DiffPlexUtils
    {

        /// <summary>
        /// GetXmlFromTemplate
        /// </summary>
        /// <param name="assembly"></param>
        /// <param name="filename"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public string GetXmlFromTemplate(Assembly assembly, string filename, string[] args = null)
        {
            string result = string.Empty;
            try
            {
                XmlTemplate xmltemplate = new XmlTemplate();
                xmltemplate.LoadFromResource(assembly, filename);
                xmltemplate.Execute(args);

                XmlDocument xmlDocument = new XmlDocument();
                xmlDocument.LoadXml(xmltemplate.InstanceAsXmlString());
                if (xmlDocument.FirstChild.NodeType == XmlNodeType.XmlDeclaration)
                {
                    xmlDocument.RemoveChild(xmlDocument.FirstChild);
                }

                result = xmlDocument.OuterXml;
            }
            catch (SystemException contextualException)
            {
                Console.WriteLine("Unable to obtain {0}", filename);
                if (!contextualException.Message.Contains("Error loading XML template from resource"))
                {
                    throw contextualException;
                }
            }
            return result;
        }
    }
}