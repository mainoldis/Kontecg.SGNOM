using Kontecg.Application.Features;
using Kontecg.Localization;
using Kontecg.UI.Inputs;

namespace Kontecg.Features
{
    public class CoreFeatureProvider : FeatureProvider
    {
        public override void SetFeatures(IFeatureDefinitionContext context)
        {
            var exportingFeature = context.Create(
                CoreFeatureNames.ExportingFeature,
                "true",
                L("ExportingFeature"),
                L("ExportingFeature_Description"),
                FeatureScopes.All,
                new CheckboxInputType()
            );

            exportingFeature.CreateChildFeature(
                CoreFeatureNames.ExportingPdfFeature,
                "true",
                L("ExportingPdfFeature"),
                L("ExportingPdfFeature_Description"),
                FeatureScopes.All,
                new CheckboxInputType()
            );

            exportingFeature.CreateChildFeature(
                CoreFeatureNames.ExportingExcelFeature,
                "true",
                L("ExportingExcelFeature"),
                L("ExportingExcelFeature_Description"),
                FeatureScopes.All,
                new CheckboxInputType()
            );

            context.Create(
                CoreFeatureNames.CurrencyExchangeRateFeature,
                "false",
                L("ExchangeRateFeature"),
                L("ExchangeRateFeature_Description"),
                FeatureScopes.All,
                new CheckboxInputType()
            );
        }

        private static ILocalizableString L(string name)
        {
            return new LocalizableString(name, KontecgCoreConsts.LocalizationSourceName);
        }
    }
}
