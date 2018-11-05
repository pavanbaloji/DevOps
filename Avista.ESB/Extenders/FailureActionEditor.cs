using System;
using System.ComponentModel;
using System.Windows.Forms;
using Microsoft.Practices.Modeling.Common.Design;

namespace Avista.ESB.Extenders
{
    public enum FailureAction
    {
        ThrowException = 0,
        SendException = 1,
        HandleException = 2,
    }

    public class FailureActionEditor : ListEditor
    {
        public FailureActionEditor()
        {
        }

        protected override void FillListWithData(ITypeDescriptorContext context, ListView values)
        {
            foreach (FailureAction failureAction in Enum.GetValues(typeof(FailureAction)))
            {
                values.Items.Add(failureAction.ToString());
            }
        }
    }
}


