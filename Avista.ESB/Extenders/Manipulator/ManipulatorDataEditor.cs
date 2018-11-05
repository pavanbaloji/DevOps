using Microsoft.Practices.Modeling.Common.Design;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace Avista.ESB.Extenders.Manipulator
{
    class ManipulatorDataEditor
    {
    }

    [Serializable]
    public class XpathEditor
    {
        [Description("Specify the XPath."), DisplayName("XPath"), XmlElement]
        public string XPath { get; set; }
    }

    [Serializable]
    public class MessageContextEditor
    {
        [Description("Specify the message property name."), DisplayName("Property Name"), XmlElement]
        public string PropertyName { get; set; }

        [Description("Specify the message property namespace."), DisplayName("Property Namespace"), XmlElement]
        public string PropertyNamespace { get; set; }
    }

    [Serializable]
    public class HttpHeaderEditor
    {
        [Description("Specifies the http header name."), DisplayName("Http Header Name"), XmlElement]
        public string HttpHeaderName { get; set; }
    }

    [Serializable]
    public class ConstantEditor
    {
        [Description("Specifies the constant value."), DisplayName("Constant Value"), XmlElement]
        public string ConstantValue { get; set; }
    }

    [Serializable]
    public class BizTalkMacroEditor
    {
        [Description("Specifies the BizTalk macro."), DisplayName("BizTalk Macro"), XmlElement]
        public string BizTalkMacro { get; set; }
    }

    [Serializable]
    public class XmlDocumentStructure
    {
        [Description("Specifies the root node name."), DisplayName("Root Node Name"), XmlElement]
        public string RootNodeName { get; set; }

        [Description("Specifies the root node namespace."), DisplayName("Xml Namespace"), XmlElement]
        public string Namespace { get; set; }

        [Description("Specifies the prefix for namespace declaration."), DisplayName("Namespace Prefix"), XmlElement]
        public string NamespacePrefix { get; set; }

        [Description("Specifies the whether to apply prefix to root node or full xml document."), DisplayName("Namespace Prefix To"), Editor(typeof(NamespacePrefixEditorList), typeof(System.Drawing.Design.UITypeEditor)), TypeConverter(typeof(TypeConverter)), XmlElement]
        public string ApplyNamespacePrefixTo { get; set; }

        [Description("Specifies the action to perform on xml document."), DisplayName("Action"), Editor(typeof(XmlDocumentStructureEditor), typeof(System.Drawing.Design.UITypeEditor)), TypeConverter(typeof(TypeConverter)), XmlElement]
        public string Action { get; set; }
    }

    class XmlDocumentStructureEditor : ListEditor
    {
        protected override void FillListWithData(ITypeDescriptorContext context, ListView values)
        {
            values.Items.Add("Rename Root Node", 0);
            values.Items.Add("Wrap Root Node", 0);
            values.Items.Add("Remove Namespace Prefix", 0);
        }

        protected override void SetImageList(ImageList imageList)
        {
            imageList.Images.Add(SystemIcons.Information);
        }
    }

    class NamespacePrefixEditorList : ListEditor
    {
        protected override void FillListWithData(ITypeDescriptorContext context, ListView values)
        {
            values.Items.Add("FullDocument",0);
            values.Items.Add("OnlyToRootNode",0);
        }
        protected override void SetImageList(ImageList imageList)
        {
            imageList.Images.Add(SystemIcons.Information);
        }
    }
}
