using Kontecg.Configuration;
using Kontecg.Dependency;

namespace Kontecg.SocialSecurity
{
    /// <summary>
    /// Implements <see cref="ISocialSecuritySettingStore"/> to get settings from <see cref="ISettingManager"/>.
    /// </summary>
    public class SocialSecuritySettingStore : ISocialSecuritySettingStore, ITransientDependency
    {
        private readonly ISettingManager _settingManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="SocialSecuritySettingStore"/> class.
        /// </summary>
        public SocialSecuritySettingStore(ISettingManager settingManager)
        {
            _settingManager = settingManager;
        }
    }
}
