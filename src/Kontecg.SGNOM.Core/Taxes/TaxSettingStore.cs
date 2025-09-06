using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using Kontecg.Configuration;
using Kontecg.Dependency;
using Kontecg.Domain.Repositories;
using Kontecg.Domain.Uow;
using Kontecg.HumanResources;
using Kontecg.Json;

namespace Kontecg.Taxes
{
    /// <summary>
    /// Implements <see cref="ITaxSettingStore"/> to get settings from <see cref="ISettingManager"/>.
    /// </summary>
    public class TaxSettingStore : ITaxSettingStore, ITransientDependency
    {
        private readonly ISettingManager _settingManager;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly IRepository<PersonTax> _taxPersonRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="TaxSettingStore"/> class.
        /// </summary>
        public TaxSettingStore(ISettingManager settingManager,
            IUnitOfWorkManager unitOfWorkManager,
            IRepository<PersonTax> taxPersonRepository)
        {
            _settingManager = settingManager;
            _unitOfWorkManager = unitOfWorkManager;
            _taxPersonRepository = taxPersonRepository;
        }

        /// <inheritdoc />
        public IReadOnlyDictionary<TaxType, TaxInfo> GetTaxesInfo(int? companyId)
        {
            List<TaxPersonInfo> taxPersons;

            using (var uow = _unitOfWorkManager.Begin(TransactionScopeOption.Suppress))
            using (_unitOfWorkManager.Current.SetCompanyId(companyId))
            {
                taxPersons = _taxPersonRepository.GetAll().Where(tax => tax.IsActive).Select(n => n.CreateTaxInfo()).ToList();
                uow.Complete();
            }

            var forSocialSecurity = (companyId.HasValue
                    ? _settingManager.GetSettingValueForCompany(SGNOMSettings.Taxes.ForSocialSecurity, companyId.Value)
                    : _settingManager.GetSettingValueForApplication(SGNOMSettings.Taxes.ForSocialSecurity))
                .FromJsonString<TaxInfo>();

            forSocialSecurity.Persons = taxPersons.FindAll(info => info.TaxType == TaxType.SocialSecurity);

            var forIncome = (companyId.HasValue
                    ? _settingManager.GetSettingValueForCompany(SGNOMSettings.Taxes.ForIncome, companyId.Value)
                    : _settingManager.GetSettingValueForApplication(SGNOMSettings.Taxes.ForIncome))
                .FromJsonString<TaxInfo>();

            forIncome.Persons = taxPersons.FindAll(info => info.TaxType == TaxType.Income);

            var forCompanySocialSecurity = (companyId.HasValue
                    ? _settingManager.GetSettingValueForCompany(SGNOMSettings.Taxes.ForCompanySocialSecurity, companyId.Value)
                    : _settingManager.GetSettingValueForApplication(SGNOMSettings.Taxes.ForCompanySocialSecurity))
                .FromJsonString<TaxInfo>();

            var forCompanyWorkforce = (companyId.HasValue
                    ? _settingManager.GetSettingValueForCompany(SGNOMSettings.Taxes.ForCompanyWorkforce, companyId.Value)
                    : _settingManager.GetSettingValueForApplication(SGNOMSettings.Taxes.ForCompanyWorkforce))
                .FromJsonString<TaxInfo>();

            var taxesInfoDictionary = new Dictionary<TaxType, TaxInfo>
            {
                {
                    TaxType.SocialSecurity, forSocialSecurity
                },
                {
                    TaxType.Income, forIncome
                },
                {
                    TaxType.CompanySocialSecurity, forCompanySocialSecurity
                },
                {
                    TaxType.CompanyWorkforce, forCompanyWorkforce
                }
            };

            return taxesInfoDictionary;
        }

        /// <inheritdoc />
        public async Task<IReadOnlyDictionary<TaxType, TaxInfo>> GetTaxesInfoAsync(int? companyId)
        {
            List<TaxPersonInfo> taxPersons;

            using (var uow = _unitOfWorkManager.Begin(TransactionScopeOption.Suppress))
            using (_unitOfWorkManager.Current.SetCompanyId(companyId))
            {
                taxPersons = (await _taxPersonRepository.GetAllAsync()).Where(tax => tax.IsActive).Select(n => n.CreateTaxInfo()).ToList();
                await uow.CompleteAsync();
            }

            var forSocialSecurity = (companyId.HasValue
                    ? await _settingManager.GetSettingValueForCompanyAsync(SGNOMSettings.Taxes.ForSocialSecurity, companyId.Value)
                    : await _settingManager.GetSettingValueForApplicationAsync(SGNOMSettings.Taxes.ForSocialSecurity))
                .FromJsonString<TaxInfo>();

            forSocialSecurity.Persons = taxPersons.FindAll(info => info.TaxType == TaxType.SocialSecurity);

            var forIncome = (companyId.HasValue
                    ? await _settingManager.GetSettingValueForCompanyAsync(SGNOMSettings.Taxes.ForIncome, companyId.Value)
                    : await _settingManager.GetSettingValueForApplicationAsync(SGNOMSettings.Taxes.ForIncome))
                .FromJsonString<TaxInfo>();

            forIncome.Persons = taxPersons.FindAll(info => info.TaxType == TaxType.Income);

            var forCompanySocialSecurity = (companyId.HasValue
                    ? await _settingManager.GetSettingValueForCompanyAsync(SGNOMSettings.Taxes.ForCompanySocialSecurity, companyId.Value)
                    : await _settingManager.GetSettingValueForApplicationAsync(SGNOMSettings.Taxes.ForCompanySocialSecurity))
                .FromJsonString<TaxInfo>();

            var forCompanyWorkforce = (companyId.HasValue
                    ? await _settingManager.GetSettingValueForCompanyAsync(SGNOMSettings.Taxes.ForCompanyWorkforce, companyId.Value)
                    : await _settingManager.GetSettingValueForApplicationAsync(SGNOMSettings.Taxes.ForCompanyWorkforce))
                .FromJsonString<TaxInfo>();

            var taxesInfoDictionary = new Dictionary<TaxType, TaxInfo>
            {
                {
                    TaxType.SocialSecurity, forSocialSecurity
                },
                {
                    TaxType.Income, forIncome
                },
                {
                    TaxType.CompanySocialSecurity, forCompanySocialSecurity
                },
                {
                    TaxType.CompanyWorkforce, forCompanyWorkforce
                }
            };

            return taxesInfoDictionary;
        }
    }
}
