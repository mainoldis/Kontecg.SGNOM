using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kontecg.Application.Services.Dto;
using Kontecg.Configuration;
using Kontecg.Dependency;
using TimeZoneConverter;

namespace Kontecg.Timing
{
    public class TimeZoneService : ITimeZoneService, ITransientDependency
    {
        private readonly ISettingDefinitionManager _settingDefinitionManager;
        private readonly ISettingManager _settingManager;

        public TimeZoneService(
            ISettingManager settingManager,
            ISettingDefinitionManager settingDefinitionManager)
        {
            _settingManager = settingManager;
            _settingDefinitionManager = settingDefinitionManager;
        }

        public async Task<string> GetDefaultTimezoneAsync(SettingScopes scope, int? companyId)
        {
            if (scope == SettingScopes.User)
            {
                if (companyId.HasValue)
                    return await _settingManager.GetSettingValueForCompanyAsync(TimingSettingNames.TimeZone,
                        companyId.Value);

                return await _settingManager.GetSettingValueForApplicationAsync(TimingSettingNames.TimeZone);
            }

            if (scope == SettingScopes.Company)
                return await _settingManager.GetSettingValueForApplicationAsync(TimingSettingNames.TimeZone);

            if (scope == SettingScopes.Application)
            {
                var timezoneSettingDefinition =
                    _settingDefinitionManager.GetSettingDefinition(TimingSettingNames.TimeZone);
                return timezoneSettingDefinition.DefaultValue;
            }

            throw new Exception("Unknown scope for default timezone setting.");
        }

        public TimeZoneInfo FindTimeZoneById(string timezoneId)
        {
            return TZConvert.GetTimeZoneInfo(timezoneId);
        }

        public List<NameValueDto> GetWindowsTimezones()
        {
            return TZConvert.KnownWindowsTimeZoneIds.OrderBy(tz => tz)
                .Select(tz => new NameValueDto
                {
                    Value = tz,
                    Name = tz
                }).ToList();
        }
    }
}
