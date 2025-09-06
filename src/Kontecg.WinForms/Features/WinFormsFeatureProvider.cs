using Kontecg.Application.Features;
using Kontecg.Localization;
using Kontecg.UI.Inputs;

namespace Kontecg.Features
{
    public class WinFormsFeatureProvider : FeatureProvider
    {
        public override void SetFeatures(IFeatureDefinitionContext context)
        {
            context.Create(
                WinFormsFeatureNames.ChangeThemeFeature,
                "false",
                L("ChangeThemeFeature"),
                L("ChangeThemeFeature_Description"),
                FeatureScopes.All,
                new CheckboxInputType()
            );

            context.Create(
                WinFormsFeatureNames.ReportingSystemDesignerFeature,
                "false",
                L("ReportingSystemDesignerFeature"),
                L("ReportingSystemDesignerFeature_Description"),
                FeatureScopes.Client,
                new CheckboxInputType()
            );

            context.Create(
                WinFormsFeatureNames.DashboardDesignerFeature,
                "false",
                L("DashboardDesignerFeature"),
                L("DashboardDesignerFeature_Description"),
                FeatureScopes.Client,
                new CheckboxInputType()
            );

            context.Create(
                WinFormsFeatureNames.OfficeApiFeature,
                "false",
                L("OfficeApiFeature"),
                L("OfficeApiFeature_Description"),
                FeatureScopes.Client,
                new CheckboxInputType()
            );
        }

        private static ILocalizableString L(string name)
        {
            return new LocalizableString(name, KontecgCoreConsts.LocalizationSourceName);
        }
    }
}