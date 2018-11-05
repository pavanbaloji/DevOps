using Microsoft.Practices.Modeling.ExtensionProvider.TypeDescription;
using Microsoft.Practices.Modeling.Services.Design;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Avista.ESB.Extenders.Cache
{
    public class CacheActionEditor : BiztalkListEditor
    {
        public CacheActionEditor()
            : base(false)
        {
        }

        protected override void FillListWithData(ITypeDescriptorContext context, ListView values)
        {
            values.Items.Add("Write to Cache", 0);
            values.Items.Add("Read from Cache", 0);
        }

        protected override object SelectedValue(object value, ITypeDescriptorContext context)
        {
            Dictionary<string, string> actionDictionaryList = new Dictionary<string, string>();
            actionDictionaryList.Add("Write to Cache", "Add");
            actionDictionaryList.Add("Read from Cache", "Get");

            object selectedValue = base.SelectedValue(value, context);

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
