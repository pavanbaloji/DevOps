using Avista.ESB.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Avista.ESB.Utilities.ExceptionManagement
{
    public sealed class ExceptionEventDetails
    {
        private static readonly Lazy<ExceptionEventDetails> lazy =
            new Lazy<ExceptionEventDetails>(() => new ExceptionEventDetails());

        public static ExceptionEventDetails Instance { get { return lazy.Value; } }

        private IList<EsbFaultEvent> exceptionEvents;
        private ExceptionEventDetails()
        {
            exceptionEvents = DataHelper.GetExceptionEvents();
        }

        public IList<EsbFaultEvent> ExceptionEvents
        {
            get
            {
                return exceptionEvents;
            }
        }
    }
}
