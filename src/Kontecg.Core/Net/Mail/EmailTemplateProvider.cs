using System;
using System.Collections.Concurrent;
using System.Text;
using Kontecg.Dependency;
using Kontecg.IO.Extensions;
using Kontecg.Reflection.Extensions;

namespace Kontecg.Net.Mail
{
    public class EmailTemplateProvider : IEmailTemplateProvider, ISingletonDependency
    {
        private readonly ConcurrentDictionary<string, string> _defaultTemplates;

        public EmailTemplateProvider()
        {
            _defaultTemplates = new ConcurrentDictionary<string, string>();
        }

        public string GetDefaultTemplate(int? companyId)
        {
            var companyKey = companyId.HasValue ? companyId.Value.ToString() : "host";

            return _defaultTemplates.GetOrAdd(companyKey, key =>
            {
                using var stream = typeof(EmailTemplateProvider).GetAssembly()
                    .GetManifestResourceStream("Kontecg.Net.Mail.EmailTemplates.default.html");
                var bytes = stream.GetAllBytes();
                var template = Encoding.UTF8.GetString(bytes, 3, bytes.Length - 3);
                template = template.Replace("{THIS_YEAR}", DateTime.Now.Year.ToString());
                return template.Replace("{EMAIL_LOGO_URL}", GetCompanyLogoUrl(companyId));
            });
        }

        private string GetCompanyLogoUrl(int? companyId)
        {
            //if (!companyId.HasValue)
            //{
            //    return _webUrlService.GetServerRootAddress().EnsureEndsWith('/') + "CompanyCustomization/GetCompanyLogo?skin=light";
            //}

            //return _webUrlService.GetServerRootAddress().EnsureEndsWith('/') + "CompanyCustomization/GetCompanyLogo?skin=light&companyId=" + companyId.Value;
            return null;
        }
    }
}
