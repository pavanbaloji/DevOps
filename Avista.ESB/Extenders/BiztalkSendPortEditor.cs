using Microsoft.BizTalk.ExplorerOM;
using Microsoft.Practices.Modeling.Common.Utility;
using Microsoft.Practices.Modeling.Services.Biztalk;
using Microsoft.Practices.Modeling.Services.Design;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Avista.ESB.Extenders
{
    public class BiztalkSendPortEditor : Microsoft.Practices.Modeling.Services.Design.BiztalkSendPortEditor
    {
        protected override void FillListWithData(ITypeDescriptorContext context, ListView values)
        {
            base.FillListWithData(context, values);
            string inputProperty = EditorUtility.GetInputProperty<string>(context, "BiztalkApplication");

            foreach (SendPort sendPort in CatalogExplorer.GetSendPorts(base.BiztalkExplorer, inputProperty))
            {
                if (!sendPort.IsDynamic)
                {
                    base.AddItem(sendPort.Name, sendPort, new string[0]);
                }
            }
        }
    }
}
