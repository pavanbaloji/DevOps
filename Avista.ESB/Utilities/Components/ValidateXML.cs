using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace Avista.ESB.Utilities
{
    [Serializable]
    public class ValidateDocument
    {
        public bool TryParseXml(XmlDocument xDoc)
        {
            try
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(xDoc.OuterXml);
                return IsValidXmlString(xmlDoc.InnerText);
            }
            catch (XmlException e)
            {
                return false;
            }
        }

        static bool IsValidXmlString(string text)
        {
            try
            {
                XmlConvert.VerifyXmlChars(text);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
