using System;
using Kontecg.Localization;
using Kontecg.Views;
using Kontecg.Views.Dashboard;

namespace Kontecg.Navigation
{
    internal class DefaultModuleRegistrationProvider : ModuleRegistrationProvider
    {
        /// <inheritdoc />
        public override void Register(IModuleRegistrationContext context)
        {
            var modules = context.Manager.Modules;

            modules.Add(
                new ModuleDefinition(
                    KontecgWinFormsConsts.ModuleNames.Dashboard,
                    Guid.Parse(KontecgWinFormsConsts.ModuleNames.DashboardId),
                    displayName: new LocalizableString(KontecgWinFormsConsts.ModuleNames.Dashboard,
                        KontecgWinFormsConsts.LocalizationSourceName),
                    order: 1,
                    imageName: "Dashboard.svg",
                    smallImageUri: "resource://Kontecg.Resources.Modules.Dashboard.svg?Size=16x16",
                    largeImageUri: "resource://Kontecg.Resources.Modules.Dashboard.svg"
                ).AddView(
                    new ViewDefinition(KontecgWinFormsConsts.ModuleNames.Dashboard + ".MainView",
                        new LocalizableString(KontecgWinFormsConsts.ModuleNames.Dashboard,
                            KontecgWinFormsConsts.LocalizationSourceName), typeof(DashboardView))
                ).AddView(
                    new ViewDefinition(KontecgWinFormsConsts.ModuleNames.Dashboard + ".FilterPaneView",
                        new LocalizableString(KontecgWinFormsConsts.ModuleNames.Dashboard,
                            KontecgWinFormsConsts.LocalizationSourceName), typeof(DashboardFilterPaneView), category: ViewCategory.FilterPaneView)
                ).AddView(
                    new ViewDefinition(KontecgWinFormsConsts.ModuleNames.Dashboard + ".FilterPaneCollapsedView",
                        new LocalizableString(KontecgWinFormsConsts.ModuleNames.Dashboard,
                            KontecgWinFormsConsts.LocalizationSourceName), typeof(DashboardFilterPaneCollapsedView), category: ViewCategory.FilterPaneCollapsedView)
                ));

            /*
            modules.Add(
                new ModuleDefinition(
                    KontecgWinFormsConsts.ModuleNames.HumanResources,
                    Guid.Parse(KontecgWinFormsConsts.ModuleNames.HumanResourcesId),
                    displayName: new LocalizableString(KontecgWinFormsConsts.ModuleNames.HumanResources, KontecgWinFormsConsts.LocalizationSourceName),
                    order: 2,
                    imageName: "resource://Kontecg.Resources.Modules.HumanResources.svg?Size=16x16"
                ));

            modules.Add(
                new ModuleDefinition(
                    KontecgWinFormsConsts.ModuleNames.Accounting,
                    Guid.Parse(KontecgWinFormsConsts.ModuleNames.AccountingId),
                    displayName: new LocalizableString(KontecgWinFormsConsts.ModuleNames.Accounting, KontecgWinFormsConsts.LocalizationSourceName),
                    order: 3,
                    imageName: "resource://Kontecg.Resources.Modules.Accounting.svg?Size=16x16"
                ));

            modules.Add(
                new ModuleDefinition(
                    KontecgWinFormsConsts.ModuleNames.Administration,
                    Guid.Parse(KontecgWinFormsConsts.ModuleNames.AdministrationId),
                    displayName: new LocalizableString(KontecgWinFormsConsts.ModuleNames.Administration, KontecgWinFormsConsts.LocalizationSourceName),
                    order: 99,
                    imageName: "resource://Kontecg.Resources.Modules.Administration.svg?Size=16x16"
                ));
            */
        }
    }
}