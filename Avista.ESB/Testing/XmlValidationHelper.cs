
using System;
using System.Text;
using Microsoft.BizTalk.TestTools.Schema;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Xml;
using System.Xml.Schema;
using System.Diagnostics;

namespace Avista.ESB.Testing
{
    /// <summary>
    /// Class for validating the Schema against the instance XML.
    /// </summary>
    public static class XmlValidationHelper
    {
        public static void ValidateXpath(string value, string xPath, string directory, string fileMask)
        {
            XmlDocument output = new XmlDocument();
            DirectoryInfo di = new DirectoryInfo(directory);
            FileInfo[] rgFiles = di.GetFiles(fileMask);
            foreach (FileInfo fi in rgFiles)
            {
                ValidateXpathFile(value, xPath, directory + fi.Name);
            }
        }

        public static void ValidateXpath(string value, string xPath, string directory, string fileMask, string description)
        {
            XmlDocument output = new XmlDocument();
            DirectoryInfo di = new DirectoryInfo(directory);
            FileInfo[] rgFiles = di.GetFiles(fileMask);
            foreach (FileInfo fi in rgFiles)
            {
                ValidateXpathFile(value, xPath, directory + fi.Name, description);
            }
        }

        public static void ValidateXpathFile(string value, string xPath, string fileName)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(fileName);
            Assert.AreEqual(value, xmlDoc.SelectSingleNode(xPath).InnerText);
        }

        public static void ValidateXpathFile(string value, string xPath, string fileName, string description)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(fileName);
            Assert.AreEqual(value, xmlDoc.SelectSingleNode(xPath).InnerText, description);
        }
    }
}
