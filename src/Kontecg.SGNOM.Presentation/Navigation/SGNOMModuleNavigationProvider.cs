using Kontecg.Application.Navigation;
using Kontecg.Localization;

namespace Kontecg.Navigation
{
    public class SGNOMModuleNavigationProvider : NavigationProvider
    {
        public const string MenuName = "MainMenu";

        public override void SetNavigation(INavigationProviderContext context)
        {
            //RULE: Siempre buscar una de las definiciones del menu principal existente y agregar el propio del módulo
            var modules = context.Manager.Menus[MenuName];

            ////RULE: Si es un módulo nuevo, crear la definición completa desde el primer nivel agregándola a "modules"
            //var humanResources = modules.GetItemByName(AppModuleNames.HumanResources);

            //humanResources.AddItem(
            //    new MenuItemDefinition(
            //        SGNOMNavigationNames.Organizations,
            //        L(SGNOMNavigationNames.Organizations),
            //        icon: "",
            //        order: 0
            //    ));

            //humanResources.AddItem(
            //    new MenuItemDefinition(
            //        SGNOMNavigationNames.WorkRelations,
            //        L(SGNOMNavigationNames.WorkRelations),
            //        icon: "",
            //        order: 1
            //    ));

            //humanResources.AddItem(
            //    new MenuItemDefinition(
            //        SGNOMNavigationNames.Salary,
            //        L(SGNOMNavigationNames.Salary),
            //        icon: "",
            //        order: 2
            //    ));

            //humanResources.AddItem(
            //    new MenuItemDefinition(
            //        SGNOMNavigationNames.Cpl,
            //        L(SGNOMNavigationNames.Cpl),
            //        icon: "",
            //        order: 3
            //    ));

            //humanResources.AddItem(
            //    new MenuItemDefinition(
            //        SGNOMNavigationNames.Holidays,
            //        L(SGNOMNavigationNames.Holidays),
            //        icon: "",
            //        order: 4
            //    ));

            //humanResources.AddItem(
            //    new MenuItemDefinition(
            //        SGNOMNavigationNames.SocialSecurity,
            //        L(SGNOMNavigationNames.SocialSecurity),
            //        icon: "",
            //        order: 5
            //    ));

            //humanResources.AddItem(
            //    new MenuItemDefinition(
            //        SGNOMNavigationNames.General,
            //        L(SGNOMNavigationNames.General),
            //        icon: "",
            //        order: 6
            //    ));

            //var accounting = modules.GetItemByName(AppModuleNames.Accounting);

            //accounting.AddItem(
            //    new MenuItemDefinition(
            //        SGNOMNavigationNames.Documents,
            //        L(SGNOMNavigationNames.Documents),
            //        icon: "",
            //        order: 0
            //    ));

            //accounting.AddItem(
            //    new MenuItemDefinition(
            //        SGNOMNavigationNames.Balance,
            //        L(SGNOMNavigationNames.Balance),
            //        icon: "",
            //        order: 1
            //    ));

            //accounting.AddItem(
            //    new MenuItemDefinition(
            //        SGNOMNavigationNames.Retentions,
            //        L(SGNOMNavigationNames.Retentions),
            //        icon: "",
            //        order: 2
            //    ));

            //accounting.AddItem(
            //    new MenuItemDefinition(
            //        SGNOMNavigationNames.Claims,
            //        L(SGNOMNavigationNames.Claims),
            //        icon: "",
            //        order: 3
            //    ));

            //accounting.AddItem(
            //    new MenuItemDefinition(
            //        SGNOMNavigationNames.General,
            //        L(SGNOMNavigationNames.General),
            //        icon: "",
            //        order: 4
            //    ));
        }

        private static ILocalizableString L(string name)
        {
            return new LocalizableString(name, SGNOMConsts.LocalizationSourceName);
        }
    }
}
