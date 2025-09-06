using System.Linq;
using Castle.Core.Logging;
using Kontecg.EFCore;
using Kontecg.Features;
using Kontecg.MultiCompany;
using Microsoft.EntityFrameworkCore;

namespace Kontecg.Migrations.Seed.Host
{
    public class DefaultFeaturesBuilder
    {
        private readonly KontecgCoreDbContext _context;
        private readonly ILogger _logger;

        public DefaultFeaturesBuilder(KontecgCoreDbContext context, ILogger logger)
        {
            _context = context;
            _logger = logger;
        }

        public void Create()
        {
            CreateFeatureIfNotExists(MultiCompanyConsts.DefaultCompanyId, CoreFeatureNames.ExportingFeature, false);
            CreateFeatureIfNotExists(MultiCompanyConsts.DefaultCompanyId, CoreFeatureNames.ExportingPdfFeature, false);
            CreateFeatureIfNotExists(MultiCompanyConsts.DefaultCompanyId, CoreFeatureNames.ExportingExcelFeature, false);
            CreateFeatureIfNotExists(MultiCompanyConsts.DefaultCompanyId, CoreFeatureNames.CurrencyExchangeRateFeature, false);
            
            _logger.Info("Features created.");
        }

        private void CreateFeatureIfNotExists(int companyId, string featureName, bool isEnabled)
        {
            var feature = _context.FeatureSettings.IgnoreQueryFilters()
                .FirstOrDefault(ef => ef.CompanyId == companyId && ef.Name == featureName);

            if (feature != null)
                return;

            _context.FeatureSettings.Add(new CompanyFeatureSetting
            {
                Name = featureName,
                Value = isEnabled.ToString().ToLower(),
                CompanyId = companyId
            });

            _context.SaveChanges();
        }
    }
}
