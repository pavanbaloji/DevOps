using Microsoft.Practices.Modeling.ExtensionProvider.TypeDescription;
using Microsoft.Practices.Modeling.Services.Design;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Avista.ESB.Extenders.Manipulator
{
    public class ManipulatorActionEditor : BiztalkListEditor
    {
        public ManipulatorActionEditor()
            : base(false)
        {
        }

        protected override void FillListWithData(ITypeDescriptorContext context, ListView values)
        {
            ExtendedPropertyDescriptor propertyDescriptor = context.PropertyDescriptor as ExtendedPropertyDescriptor;

            if (propertyDescriptor.Name.Equals("ReadFrom", System.StringComparison.OrdinalIgnoreCase))
            {
                values.Items.Add("Read from Http Header", 0);
                values.Items.Add("Read from XPath", 0);
                values.Items.Add("Read from Message Context", 0);
                values.Items.Add("Read Constant", 0);
                values.Items.Add("Read BizTalk Macros", 0);
                values.Items.Add("None", 0);
            }

            if (propertyDescriptor.Name.Equals("WriteTo", System.StringComparison.OrdinalIgnoreCase))
            {
                values.Items.Add("Write to Http Header", 0);
                values.Items.Add("Write to XPath", 0);
                values.Items.Add("Write to Message Context", 0);
                values.Items.Add("Write to Message Context & Promote", 0);
                values.Items.Add("Alter Xml Structure", 0);
            }
        }

        protected override object SelectedValue(object value, ITypeDescriptorContext context)
        {
            Dictionary<string, string> actionDictionaryList = new Dictionary<string, string>();
            actionDictionaryList.Add("Read from Http Header", "HttpHeader");
            actionDictionaryList.Add("Read from XPath", "XPath");
            actionDictionaryList.Add("Read from Message Context", "MessageContext");
            actionDictionaryList.Add("Read Constant", "Constant");
            actionDictionaryList.Add("Read BizTalk Macros", "BizTalkMacros");
            actionDictionaryList.Add("Write to Http Header", "HttpHeader");
            actionDictionaryList.Add("Write to XPath", "XPath");
            actionDictionaryList.Add("Write to Message Context", "MessageContext");
            actionDictionaryList.Add("Write to Message Context & Promote", "PromoteMessageContext");
            actionDictionaryList.Add("Alter Xml Structure", "XmlStructure");
            actionDictionaryList.Add("None", "None");

            object selectedValue = base.SelectedValue(value, context);

            ExtendedPropertyDescriptor propertyDescriptor = context.PropertyDescriptor as ExtendedPropertyDescriptor;

            if (propertyDescriptor.Name.Equals("ReadFrom", System.StringComparison.OrdinalIgnoreCase))
            {
                EditorUtility.SetOutputProperty<string>(context, "ReadItem", string.Empty);
            }

            if (propertyDescriptor.Name.Equals("WriteTo", System.StringComparison.OrdinalIgnoreCase))
            {
                EditorUtility.SetOutputProperty<string>(context, "WriteItem", string.Empty);
            }
            
            return actionDictionaryList[selectedValue.ToString()];
        }

        protected override void SetImageList(ImageList imageList)
        {
            imageList.Images.Add(SystemIcons.Application);
        }

        protected override bool CanUnselect()
        {
            return false;
        }
    }
}
