using System;
using System.Linq.Expressions;
using Kontecg.Specifications;

namespace Kontecg.Timing
{
    public class ExactPeriodSpecification : Specification<Period>
    {
        public PeriodInfo PeriodInfo { get; }

        public ExactPeriodSpecification(PeriodInfo periodInfo)
        {
            PeriodInfo = periodInfo;
        }

        public override Expression<Func<Period, bool>> ToExpression()
        {
            return period => period != null
                             && period.Since.Date == PeriodInfo.Since.Date
                             && period.Until.Date == PeriodInfo.Until.Date
                             && period.ReferenceGroup == PeriodInfo.ReferenceGroup;
        }
    }
}
