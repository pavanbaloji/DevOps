using Microsoft.BizTalk.XLANGs.BTXEngine;
using Microsoft.XLANGs.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Avista.ESB.Utilities
{
    [System.Serializable]
    public class AvistaEsbInternalMsg : BTXMessage
    {
        internal AvistaEsbInternalMsg(string msgName, Context ctx)
            : base(msgName, ctx)
        {
            ctx.RefMessage(this);
        }
    }
}
