using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;

namespace Avista.ESB.DataAccess
{
    public static class DataHelper
    {
        public static IList<FieldRequestOrderReason> GetFieldRequestOrderReason()
        {
            using (AvistaESBLookupEntities context = new AvistaESBLookupEntities())
            {
                return context.FieldRequestOrderReasons.ToList();
            }
        }
        public static IList<FieldRequestResolution> GetFieldRequestResolution(Expression<Func<FieldRequestResolution, bool>> predicate)
        {
            using (AvistaESBLookupEntities context = new AvistaESBLookupEntities())
            {
                return context.FieldRequestResolutions.Where(predicate).ToList();
            }
        }

        public static IList<EsbFaultEvent> GetExceptionEvents()
        {
            using (AvistaESBLookupEntities context = new AvistaESBLookupEntities())
            {
                return context.EsbFaultEvents.ToList();
            }
        }
    }
}
