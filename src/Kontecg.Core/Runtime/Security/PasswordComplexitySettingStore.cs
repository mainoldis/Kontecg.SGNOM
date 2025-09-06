using System.Threading.Tasks;
using Kontecg.Baseline.Configuration;
using Kontecg.Configuration;
using Kontecg.Dependency;

namespace Kontecg.Runtime.Security
{
    public class PasswordComplexitySettingStore : IPasswordComplexitySettingStore, ITransientDependency
    {
        private readonly ISettingManager _settingManager;

        public PasswordComplexitySettingStore(ISettingManager settingManager)
        {
            _settingManager = settingManager;
        }

        public async Task<PasswordComplexitySetting> GetSettingsAsync()
        {
            return new()
            {
                RequireDigit = await _settingManager.GetSettingValueAsync<bool>(KontecgBaselineSettingNames.UserManagement.PasswordComplexity.RequireDigit),
                RequireLowercase = await _settingManager.GetSettingValueAsync<bool>(KontecgBaselineSettingNames.UserManagement.PasswordComplexity.RequireLowercase),
                RequireNonAlphanumeric = await _settingManager.GetSettingValueAsync<bool>(KontecgBaselineSettingNames.UserManagement.PasswordComplexity.RequireNonAlphanumeric),
                RequireUppercase = await _settingManager.GetSettingValueAsync<bool>(KontecgBaselineSettingNames.UserManagement.PasswordComplexity.RequireUppercase),
                RequiredLength = await _settingManager.GetSettingValueAsync<int>(KontecgBaselineSettingNames.UserManagement.PasswordComplexity.RequiredLength)
            };
        }
    }
}
