using System;
using Kontecg.Localization;
using Kontecg.Views;

namespace Kontecg.Desktop.Views
{
    public class TestViewProvider : ModuleRegistrationProvider
    {
        /// <inheritdoc />
        public override void Register(IModuleRegistrationContext context)
        {
            context.Manager.Modules.Add(
                new ModuleDefinition("Test", Guid.NewGuid(), new FixedLocalizableString("Test Module")).AddView(
                    new ViewDefinition("Main", new FixedLocalizableString("Test View"), typeof(TestView),
                        ViewCategory.MainView)));
        }
    }
}
