using System.Collections.Generic;

namespace Kontecg.Configuration.Dtos
{
    public class KontecgUserConfigurationDto
    {
        public KontecgMultiCompanyConfigDto MultiCompany { get; set; }

        public KontecgUserSessionConfigDto Session { get; set; }

        public KontecgUserLocalizationConfigDto Localization { get; set; }

        public KontecgUserFeatureConfigDto Features { get; set; }

        public KontecgUserAuthConfigDto Auth { get; set; }

        public KontecgUserNavConfigDto Nav { get; set; }

        public KontecgUserModuleConfigDto View { get; set; }

        public KontecgUserSettingConfigDto Setting { get; set; }

        public KontecgUserClockConfigDto Clock { get; set; }

        public KontecgUserTimingConfigDto Timing { get; set; }

        public KontecgUserSecurityConfigDto Security { get; set; }

        public Dictionary<string, object> Custom { get; set; }
    }
}
