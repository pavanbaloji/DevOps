using Microsoft.Practices.Modeling.Common.Design;
using Microsoft.Practices.Modeling.Common.Utility;
using Microsoft.Practices.Modeling.ExtensionProvider.TypeDescription;
using Microsoft.Practices.Modeling.Services.Design;
using System;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;
using System.Text;

namespace Avista.ESB.Extenders.Manipulator
{
    public class ManipulatorDataKeyEditor : PropertyGridEditor
    {
        protected override string Title
        {
            get
            {
                return "Manipulator Editor";
            }
        }

        protected override object FillProperties(ITypeDescriptorContext context, object value)
        {
            ExtendedPropertyDescriptor propertyDescriptor = context.PropertyDescriptor as ExtendedPropertyDescriptor;
            string readOrWriteAction = string.Empty;            

            if (propertyDescriptor.Name.Equals("ReadItem", System.StringComparison.OrdinalIgnoreCase))
            {
                readOrWriteAction = EditorUtility.GetInputProperty<string>(context, "ReadFrom");
                Guard.ArgumentNotNullOrEmpty(readOrWriteAction, "Read Action");
            }

            if (propertyDescriptor.Name.Equals("WriteItem", System.StringComparison.OrdinalIgnoreCase))
            {
                readOrWriteAction = EditorUtility.GetInputProperty<string>(context, "WriteTo");
                Guard.ArgumentNotNullOrEmpty(readOrWriteAction, "Write Action");
            }

            if (readOrWriteAction.Equals("HttpHeader"))
            {
                return Initialize(typeof(HttpHeaderEditor), value.ToString());
            }
            else if (readOrWriteAction.Equals("XPath"))
            {
                return Initialize(typeof(XpathEditor), value.ToString()); 
            }
            else if (readOrWriteAction.Equals("MessageContext") || readOrWriteAction.Equals("PromoteMessageContext"))
            {
                return Initialize(typeof(MessageContextEditor), value.ToString()); 
            }
            else if (readOrWriteAction.Equals("Constant"))
            {
                return Initialize(typeof(ConstantEditor), value.ToString()); 
            }
            else if (readOrWriteAction.Equals("BizTalkMacros"))
            {
                return Initialize(typeof(BizTalkMacroEditor), value.ToString()); 
            }
            else if (readOrWriteAction.Equals("XmlStructure"))
            {
                return Initialize(typeof(XmlDocumentStructure), value.ToString()); 
            }
            else
            {
                return null;
            }
        }

        protected override object SelectedValue(object instance, ITypeDescriptorContext context)
        {
            if (instance == null)
            {
                return null;
            }

            StringBuilder stringBuilder = new StringBuilder();
            PropertyInfo[] properties = instance.GetType().GetProperties();
            for (int i = 0; i < properties.Length; i++)
            {
                PropertyInfo propertyInfo = properties[i];
                object value = propertyInfo.GetValue(instance, null);

                if (!string.IsNullOrEmpty(CleanString(value as string)))
                {
                    if (stringBuilder.Length > 0)
                    {
                        stringBuilder.Append("&");
                    }
                    stringBuilder.Append(this.SerializeToStringAppendName(propertyInfo.Name));
                    stringBuilder.Append("=");
                    stringBuilder.Append(this.SerializeToStringAppendValue(value));
                }
            }

            if (properties.Length == 1)
            {
                string values = stringBuilder.ToString();
                return values.ToString().Substring(values.IndexOf('=')+1);
            }
            else
            {
                return "{"+ stringBuilder.ToString() + "}";
            }
        }

        protected override string SerializeToStringAppendName(string name)
        {
            return GenericSerializer.XmlEncodeName(name);
        }

        protected override string SerializeToStringAppendValue(object value)
        {
            return GenericSerializer.XmlEncodeValue(value);
        }

        protected object Initialize(Type type, string serializedValues)
        {
            object instance = Activator.CreateInstance(type);
            if (string.IsNullOrEmpty(serializedValues))
            {
                return instance;
            }

            if(serializedValues.StartsWith("{"))
            {
                serializedValues = serializedValues.Substring(1, serializedValues.LastIndexOf('}') - 1);
                string[] array = serializedValues.Split("&".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

                for (int i = 0; i < array.Length; i++)
                {
                    string[] propertyDetail = array[i].Split("=".ToCharArray(), 2, StringSplitOptions.RemoveEmptyEntries);
                    if (propertyDetail.Length == 2)
                    {
                        PropertyInfo property = type.GetProperty(propertyDetail[0]);
                        if (property != null)
                        {
                            property.SetValue(instance, Convert.ChangeType(propertyDetail[1], property.PropertyType, CultureInfo.CurrentCulture), BindingFlags.SetProperty, null, null, CultureInfo.CurrentCulture);
                        }
                    }
                }
            }
            else
            {
                PropertyInfo[] properties = type.GetProperties();
                if (properties[0] != null)
                {
                    properties[0].SetValue(instance, Convert.ChangeType(serializedValues, properties[0].PropertyType, CultureInfo.CurrentCulture), BindingFlags.SetProperty, null, null, CultureInfo.CurrentCulture);
                }
            }
            
            return instance;
        }

        private static string CleanString(string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                return value.Replace(Environment.NewLine, string.Empty).Trim();
            }
            return value;
        }
    }
}
