using System.Threading.Tasks;
using Kontecg.Configuration;
using Kontecg.Dependency;

namespace Kontecg.HumanResources
{
    /// <summary>
    /// Implements <see cref="IPersonSettings"/> to get settings from <see cref="ISettingManager"/>.
    /// </summary>
    public class PersonSettings : IPersonSettings, ITransientDependency
    {
        private readonly ISettingManager _settingManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="PersonSettings"/> class.
        /// </summary>
        public PersonSettings(ISettingManager settingManager)
        {
            _settingManager = settingManager;
        }

        public bool MustHavePhoto(int? companyId)
        {
            return companyId.HasValue
                ? _settingManager.GetSettingValueForCompany<bool>(AppSettings.PersonManagement.MustHavePhoto,
                    companyId.Value)
                : _settingManager.GetSettingValueForApplication<bool>(AppSettings.PersonManagement.MustHavePhoto);
        }

        public async Task<bool> MustHavePhotoAsync(int? companyId)
        {
            return companyId.HasValue
                ? await _settingManager.GetSettingValueForCompanyAsync<bool>(AppSettings.PersonManagement.MustHavePhoto,
                    companyId.Value)
                : await _settingManager.GetSettingValueForApplicationAsync<bool>(AppSettings.PersonManagement.MustHavePhoto);
        }
         
        public bool MustHaveAddress(int? companyId)
        {
            return companyId.HasValue
                ? _settingManager.GetSettingValueForCompany<bool>(AppSettings.PersonManagement.MustHaveAddress, 
                    companyId.Value)
                : _settingManager.GetSettingValueForApplication<bool>(AppSettings.PersonManagement.MustHaveAddress);
        }

        public async Task<bool> MustHaveAddressAsync(int? companyId)
        {
            return companyId.HasValue
                ? await _settingManager.GetSettingValueForCompanyAsync<bool>(AppSettings.PersonManagement.MustHaveAddress,
                    companyId.Value)
                : await _settingManager.GetSettingValueForApplicationAsync<bool>(AppSettings.PersonManagement.MustHaveAddress);
        }

        public bool MustHaveEtnia(int? companyId)
        {
            return companyId.HasValue
                ? _settingManager.GetSettingValueForCompany<bool>(AppSettings.PersonManagement.MustHaveEtnia, 
                    companyId.Value)
                : _settingManager.GetSettingValueForApplication<bool>(AppSettings.PersonManagement.MustHaveEtnia);
        }

        public async Task<bool> MustHaveEtniaAsync(int? companyId)
        {
            return companyId.HasValue
                ? await _settingManager.GetSettingValueForCompanyAsync<bool>(AppSettings.PersonManagement.MustHaveEtnia,
                    companyId.Value)
                : await _settingManager.GetSettingValueForApplicationAsync<bool>(AppSettings.PersonManagement.MustHaveEtnia);
        }

        public bool MustHaveClothingSizes(int? companyId)
        {
            return companyId.HasValue
                ? _settingManager.GetSettingValueForCompany<bool>(AppSettings.PersonManagement.MustHaveClothingSizes, 
                    companyId.Value)
                : _settingManager.GetSettingValueForApplication<bool>(AppSettings.PersonManagement.MustHaveClothingSizes);
        }

        public async Task<bool> MustHaveClothingSizesAsync(int? companyId)
        {
            return companyId.HasValue
                ? await _settingManager.GetSettingValueForCompanyAsync<bool>(AppSettings.PersonManagement.MustHaveClothingSizes,
                    companyId.Value)
                : await _settingManager.GetSettingValueForApplicationAsync<bool>(AppSettings.PersonManagement.MustHaveClothingSizes);
        }
    }
}
