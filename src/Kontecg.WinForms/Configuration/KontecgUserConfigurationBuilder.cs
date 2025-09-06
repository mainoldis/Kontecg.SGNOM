using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Kontecg.Application.Features;
using Kontecg.Application.Navigation;
using Kontecg.Authorization;
using Kontecg.Configuration.Dtos;
using Kontecg.Configuration.Startup;
using Kontecg.Dependency;
using Kontecg.Extensions;
using Kontecg.Localization;
using Kontecg.Runtime.Session;
using Kontecg.Threading;
using Kontecg.Timing;
using Kontecg.Timing.Timezone;
using Kontecg.Views;

namespace Kontecg.Configuration
{
    public class KontecgUserConfigurationBuilder : ITransientDependency
    {
        private readonly IKontecgStartupConfiguration _startupConfiguration;
        protected IMultiCompanyConfig MultiCompanyConfig { get; }
        protected ILanguageManager LanguageManager { get; }
        protected ILocalizationManager LocalizationManager { get; }
        protected IFeatureManager FeatureManager { get; }
        protected IFeatureChecker FeatureChecker { get; }
        protected IPermissionManager PermissionManager { get; }
        protected IUserNavigationManager UserNavigationManager { get; }
        protected IUserModuleManager UserModuleManager { get; }
        protected ISettingDefinitionManager SettingDefinitionManager { get; }
        protected ISettingManager SettingManager { get; }
        protected IKontecgSession KontecgSession { get; }
        protected IPermissionChecker PermissionChecker { get; }
        protected Dictionary<string, object> CustomDataConfig { get; }

        private readonly IIocResolver _iocResolver;

        public KontecgUserConfigurationBuilder(
            IMultiCompanyConfig multiCompanyConfig,
            ILanguageManager languageManager,
            ILocalizationManager localizationManager,
            IFeatureManager featureManager,
            IFeatureChecker featureChecker,
            IPermissionManager permissionManager,
            IUserNavigationManager userNavigationManager,
            IUserModuleManager userModuleManager,
            ISettingDefinitionManager settingDefinitionManager,
            ISettingManager settingManager,
            IKontecgSession kontecgSession,
            IPermissionChecker permissionChecker,
            IIocResolver iocResolver,
            IKontecgStartupConfiguration startupConfiguration)
        {
            MultiCompanyConfig = multiCompanyConfig;
            LanguageManager = languageManager;
            LocalizationManager = localizationManager;
            FeatureManager = featureManager;
            FeatureChecker = featureChecker;
            PermissionManager = permissionManager;
            UserNavigationManager = userNavigationManager;
            UserModuleManager = userModuleManager;
            SettingDefinitionManager = settingDefinitionManager;
            SettingManager = settingManager;
            KontecgSession = kontecgSession;
            PermissionChecker = permissionChecker;
            _iocResolver = iocResolver;
            _startupConfiguration = startupConfiguration;

            CustomDataConfig = new Dictionary<string, object>();
        }

        public virtual KontecgUserConfigurationDto GetAll()
        {
            return new()
            {
                MultiCompany = GetUserMultiCompanyConfig(),
                Session = GetUserSessionConfig(),
                Localization = GetUserLocalizationConfig(),
                Features = GetUserFeaturesConfig(),
                Auth = GetUserAuthConfig(),
                Nav = GetUserNavConfig(),
                View = GetUserModuleConfig(),
                Setting = GetUserSettingConfig(),
                Clock = GetUserClockConfig(),
                Timing = GetUserTimingConfig(),
                Security = GetUserSecurityConfig(),
                Custom = _startupConfiguration.GetCustomConfig()
            };
        }

        protected virtual KontecgMultiCompanyConfigDto GetUserMultiCompanyConfig()
        {
            return new()
            {
                IsEnabled = MultiCompanyConfig.IsEnabled,
                IgnoreFeatureCheckForHostUsers = MultiCompanyConfig.IgnoreFeatureCheckForHostUsers
            };
        }

        protected virtual KontecgUserSessionConfigDto GetUserSessionConfig()
        {
            return new()
            {
                UserId = KontecgSession.UserId,
                CompanyId = KontecgSession.CompanyId,
                MultiCompanySide = KontecgSession.MultiCompanySide
            };
        }

        protected virtual KontecgUserLocalizationConfigDto GetUserLocalizationConfig()
        {
            var currentCulture = CultureInfo.CurrentUICulture;
            var languages = LanguageManager.GetActiveLanguages();

            var config = new KontecgUserLocalizationConfigDto
            {
                CurrentCulture = new KontecgUserCurrentCultureConfigDto
                {
                    Name = currentCulture.Name,
                    DisplayName = currentCulture.DisplayName
                },
                Languages = languages.ToList()
            };

            if (languages.Count > 0)
            {
                config.CurrentLanguage = LanguageManager.CurrentLanguage;
            }

            var sources = LocalizationManager.GetAllSources().OrderBy(s => s.Name).ToArray();
            config.Sources = sources.Select(s => new KontecgLocalizationSourceDto
            {
                Name = s.Name,
                Type = s.GetType().Name
            }).ToList();

            config.Values = new Dictionary<string, Dictionary<string, string>>();
            foreach (var source in sources)
            {
                var stringValues = source.GetAllStrings(currentCulture).OrderBy(s => s.Name).ToList();
                var stringDictionary = stringValues
                    .ToDictionary(_ => _.Name, _ => _.Value);
                config.Values.Add(source.Name, stringDictionary);
            }

            return config;
        }

        protected virtual KontecgUserFeatureConfigDto GetUserFeaturesConfig()
        {
            var config = new KontecgUserFeatureConfigDto()
            {
                AllFeatures = new Dictionary<string, KontecgStringValueDto>()
            };

            var allFeatures = FeatureManager.GetAll().ToList();

            if (KontecgSession.CompanyId.HasValue)
            {
                var currentCompanyId = KontecgSession.GetCompanyId();
                foreach (var feature in allFeatures)
                {
                    var value = FeatureChecker.GetValue(currentCompanyId, feature.Name);
                    config.AllFeatures.Add(feature.Name, new KontecgStringValueDto
                    {
                        Value = value
                    });
                }
            }
            else
            {
                foreach (var feature in allFeatures)
                {
                    config.AllFeatures.Add(feature.Name, new KontecgStringValueDto
                    {
                        Value = feature.DefaultValue
                    });
                }
            }

            return config;
        }

        protected virtual KontecgUserAuthConfigDto GetUserAuthConfig()
        {
            var config = new KontecgUserAuthConfigDto();

            var allPermissionNames = PermissionManager.GetAllPermissions(false).Select(p => p.Name).ToList();
            var grantedPermissionNames = new List<string>();

            if (KontecgSession.UserId.HasValue)
            {
                foreach (var permissionName in allPermissionNames)
                {
                    if (PermissionChecker.IsGranted(permissionName))
                    {
                        grantedPermissionNames.Add(permissionName);
                    }
                }
            }

            config.AllPermissions = allPermissionNames.ToDictionary(permissionName => permissionName, permissionName => "true");
            config.GrantedPermissions = grantedPermissionNames.ToDictionary(permissionName => permissionName, permissionName => "true");

            return config;
        }

        protected virtual KontecgUserNavConfigDto GetUserNavConfig()
        {
            var userMenus = AsyncHelper.RunSync(() => UserNavigationManager.GetMenusAsync(KontecgSession.ToUserIdentifier()));
            return new KontecgUserNavConfigDto
            {
                Menus = userMenus.ToDictionary(userMenu => userMenu.Name, userMenu => userMenu)
            };
        }

        protected virtual KontecgUserModuleConfigDto GetUserModuleConfig()
        {
            var modules = UserModuleManager.GetModules(KontecgSession.ToUserIdentifier());
            return new KontecgUserModuleConfigDto
            {
                Modules = modules.ToDictionary(m => m.Name, m => m)
            };
        }

        protected virtual KontecgUserSettingConfigDto GetUserSettingConfig()
        {
            var config = new KontecgUserSettingConfigDto
            {
                Values = new Dictionary<string, string>()
            };

            var settings = SettingManager.GetAllSettingValues(SettingScopes.All);

            using (var scope = _iocResolver.CreateScope())
            {
                foreach (var settingValue in settings)
                {
                    if (!AsyncHelper.RunSync(() => SettingDefinitionManager.GetSettingDefinition(settingValue.Name).ClientVisibilityProvider
                        .CheckVisibleAsync(scope)))
                    {
                        continue;
                    }

                    config.Values.Add(settingValue.Name, settingValue.Value);
                }
            }

            return config;
        }

        protected virtual KontecgUserClockConfigDto GetUserClockConfig()
        {
            return new()
            {
                Provider = Clock.Provider.GetType().Name.ToCamelCase()
            };
        }

        protected virtual KontecgUserTimingConfigDto GetUserTimingConfig()
        {
            var timezoneId = SettingManager.GetSettingValue(TimingSettingNames.TimeZone);
            var timezone = TimezoneHelper.FindTimeZoneInfo(timezoneId);

            return new KontecgUserTimingConfigDto
            {
                TimeZoneInfo = new KontecgUserTimeZoneConfigDto
                {
                    Windows = new KontecgUserWindowsTimeZoneConfigDto
                    {
                        TimeZoneId = timezoneId,
                        BaseUtcOffsetInMilliseconds = timezone.BaseUtcOffset.TotalMilliseconds,
                        CurrentUtcOffsetInMilliseconds = timezone.GetUtcOffset(Clock.Now).TotalMilliseconds,
                        IsDaylightSavingTimeNow = timezone.IsDaylightSavingTime(Clock.Now)
                    },
                    Iana = new KontecgUserIanaTimeZoneConfigDto
                    {
                        TimeZoneId = TimezoneHelper.WindowsToIana(timezoneId)
                    }
                }
            };
        }

        protected virtual KontecgUserSecurityConfigDto GetUserSecurityConfig()
        {
            return new();
        }
    }
}
