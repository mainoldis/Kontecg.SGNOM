using System.Collections.Generic;
using System.Linq;
using Castle.Core.Logging;
using Kontecg.EFCore;
using Kontecg.Localization;
using Kontecg.MultiCompany;
using Microsoft.EntityFrameworkCore;

namespace Kontecg.Migrations.Seed.Host
{
    public class DefaultLanguagesBuilder
    {
        public static List<ApplicationLanguage> InitialLanguages => GetInitialLanguages();

        private readonly KontecgCoreDbContext _context;
        private readonly ILogger _logger;

        public DefaultLanguagesBuilder(KontecgCoreDbContext context, ILogger logger)
        {
            _context = context;
            _logger = logger;
        }

        private static List<ApplicationLanguage> GetInitialLanguages()
        {
            var companyId = KontecgCoreConsts.MultiCompanyEnabled ? null : (int?)MultiCompanyConsts.DefaultCompanyId;
            return new List<ApplicationLanguage>
            {
                new ApplicationLanguage(companyId, "en", "English", "us"),
                new ApplicationLanguage(companyId, "es", "Español (Spanish)", "es"),
            };
        }

        public void Create()
        {
            CreateLanguages();
        }

        private void CreateLanguages()
        {
            foreach (var language in InitialLanguages)
            {
                AddLanguageIfNotExists(language);
            }
            _logger.Info("Languages created.");
        }

        private void AddLanguageIfNotExists(ApplicationLanguage language)
        {
            if (_context.Languages.IgnoreQueryFilters().Any(l => l.CompanyId == language.CompanyId && l.Name == language.Name))
            {
                return;
            }

            _context.Languages.Add(language);
            _context.SaveChanges();
        }
    }
}
