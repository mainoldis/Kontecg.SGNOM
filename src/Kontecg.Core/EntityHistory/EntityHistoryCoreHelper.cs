using System;
using System.Linq;
using Kontecg.Accounting;
using Kontecg.MultiCompany;
using Kontecg.Organizations;
using Kontecg.Workflows;

namespace Kontecg.EntityHistory
{
    public static class EntityHistoryCoreHelper
    {
        public const string EntityHistoryConfigurationName = "EntityHistory";

        public static readonly Type[] HostSideTrackedTypes =
        {
            typeof(AccountDefinition), typeof(DocumentDefinition), typeof(AccountingFunctionDefinition),
            typeof(Company), typeof(OrganizationUnit), typeof(WorkPlaceUnit)
        };

        public static readonly Type[] CompanySideTrackedTypes =
        {
            typeof(CenterCostDefinition), typeof(ExpenseItemDefinition), typeof(OrganizationUnit), typeof(WorkPlaceUnit)
        };

        public static readonly Type[] TrackedTypes =
            HostSideTrackedTypes
                .Concat(CompanySideTrackedTypes)
                .GroupBy(type => type.FullName)
                .Select(types => types.First())
                .ToArray();
    }
}
