using System.Collections.Generic;

namespace Kontecg.Configuration
{
    public class WinFormsSettingProvider : SettingProvider
    {
        private readonly HiddenSettingClientVisibilityProvider _hiddenSettingClientVisibilityProvider;
        private readonly VisibleSettingClientVisibilityProvider _visibleSettingClientVisibilityProvider;

        public WinFormsSettingProvider()
        {
            _hiddenSettingClientVisibilityProvider = new HiddenSettingClientVisibilityProvider();
            _visibleSettingClientVisibilityProvider = new VisibleSettingClientVisibilityProvider();
        }

        public override IEnumerable<SettingDefinition> GetSettingDefinitions(SettingDefinitionProviderContext context)
        {
            return new[]
            {
                new SettingDefinition(
                    WinFormsSettings.AuthManagement.LoginFirstRequired,
                    "true",
                    clientVisibilityProvider: _hiddenSettingClientVisibilityProvider,
                    scopes: SettingScopes.Application),

                new SettingDefinition(
                    WinFormsSettings.AuthManagement.RememberLastLogin,
                    "false",
                    clientVisibilityProvider: _hiddenSettingClientVisibilityProvider,
                    scopes: SettingScopes.Application),

                new SettingDefinition(
                    WinFormsSettings.Theme.AllowChangeTheme,
                    "false",
                clientVisibilityProvider: _visibleSettingClientVisibilityProvider,
                    scopes: SettingScopes.Application | SettingScopes.Company),

                new SettingDefinition(
                    WinFormsSettings.Theme.SkinName,
                    "Office 2019 Colorful",
                clientVisibilityProvider: _visibleSettingClientVisibilityProvider,
                    scopes: SettingScopes.All),

                new SettingDefinition(
                    WinFormsSettings.Theme.PaletteName,
                    "Amber",
                clientVisibilityProvider: _visibleSettingClientVisibilityProvider,
                    scopes: SettingScopes.All),

                new SettingDefinition(
                    WinFormsSettings.Theme.CompactUi,
                    "false",
                    clientVisibilityProvider: _visibleSettingClientVisibilityProvider,
                    scopes: SettingScopes.All),
            };
        }
    }
}
