using System;
using System.Linq;
using Kontecg.Auditing;
using Kontecg.Configuration;
using Kontecg.Dependency;
using Kontecg.Extensions;
using Kontecg.Text;
using static Kontecg.Text.FormattedStringValueExtracter;

namespace Kontecg.MultiCompany
{
    public class DomainCompanyResolveContributor : ICompanyResolveContributor, ITransientDependency
    {
        private readonly IKontecgCoreConfiguration _kontecgCoreConfiguration;
        private readonly ICompanyStore _companyStore;
        private readonly EnvironmentClientInfoProvider _clientInfoProvider;

        public DomainCompanyResolveContributor(
            IKontecgCoreConfiguration kontecgCoreConfiguration,
            ICompanyStore companyStore,
            EnvironmentClientInfoProvider clientInfoProvider)
        {
            _companyStore = companyStore;
            _clientInfoProvider = clientInfoProvider;
            _kontecgCoreConfiguration = kontecgCoreConfiguration;
        }

        public int? ResolveCompanyId()
        {
            if (_kontecgCoreConfiguration.DomainFormat.IsNullOrEmpty())
                return null;

            var domainName = _clientInfoProvider.DomainName;
            var computerName = _clientInfoProvider.ComputerName;
            if (domainName == computerName)
                return null;

            var domainFormats = _kontecgCoreConfiguration.DomainFormat.Split(";");
            var result = IsDomainFormatValid(domainFormats, domainName);

            if (result is null)
            {
                return null;
            }

            var companyName = result.Matches[0].Value.RemovePostFix(".");
            if (companyName.IsNullOrEmpty())
            {
                return null;
            }

            if (string.Equals(companyName, "www", StringComparison.OrdinalIgnoreCase))
            {
                return null;
            }

            if (string.Equals(companyName, _clientInfoProvider.ComputerName, StringComparison.OrdinalIgnoreCase))
            {
                return null;
            }

            var companyInfo = _companyStore.Find(companyName);
            if (companyInfo == null)
            {
                return null;
            }

            return companyInfo.Id;
        }

        private ExtractionResult IsDomainFormatValid(string[] domainFormats, string domainName)
        {
            foreach (var item in domainFormats)
            {
                var domainFormat = item.RemovePreFix("http://", "https://").Split(':')[0].RemovePostFix("/");
                var result = new FormattedStringValueExtracter().Extract(domainName, domainFormat, true, '.');

                if (result.IsMatch && result.Matches.Any())
                {
                    return result;
                }
            }

            return null;
        }
    }
}
