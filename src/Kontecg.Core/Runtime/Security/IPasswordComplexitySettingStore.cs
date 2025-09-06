using System.Threading.Tasks;

namespace Kontecg.Runtime.Security
{
    public interface IPasswordComplexitySettingStore
    {
        Task<PasswordComplexitySetting> GetSettingsAsync();
    }
}
