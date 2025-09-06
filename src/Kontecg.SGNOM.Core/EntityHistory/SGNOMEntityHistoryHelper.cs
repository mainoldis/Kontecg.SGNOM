using System;
using Kontecg.Adjustments;
using Kontecg.HumanResources;
using Kontecg.Organizations;
using Kontecg.Retentions;
using Kontecg.Salary;
using Kontecg.SocialSecurity;
using Kontecg.Timing;

namespace Kontecg.EntityHistory
{
    public static class SGNOMEntityHistoryHelper
    {
        public const string EntityHistoryConfigurationName = "SGNOMEntityHistory";

        public static readonly Type[] TrackedTypes =
        {
            typeof(Occupation),
            typeof(ComplexityGroup),
            typeof(Template),
            typeof(TemplateDocument),
            typeof(TemplateJobPosition),
            typeof(WorkShift),
            typeof(WorkRegime),
            typeof(PaymentDefinition),
            typeof(SubsidyPaymentDefinition),
            typeof(AdjustmentDefinition),
            typeof(RetentionDefinition),
            typeof(PersonTax),
        };
    }
}
