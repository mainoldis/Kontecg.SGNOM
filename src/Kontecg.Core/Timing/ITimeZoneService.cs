using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Kontecg.Application.Services.Dto;
using Kontecg.Configuration;

namespace Kontecg.Timing
{
    public interface ITimeZoneService
    {
        Task<string> GetDefaultTimezoneAsync(SettingScopes scope, int? companyId);

        TimeZoneInfo FindTimeZoneById(string timezoneId);

        List<NameValueDto> GetWindowsTimezones();
    }
}
