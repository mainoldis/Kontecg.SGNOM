using System.Collections.Generic;

namespace Kontecg.Sessions.Dto
{
    public class ApplicationInfoDto
    {
        public string Currency { get; set; }

        public bool AllowCompaniesToChangeEmailSettings { get; set; }

        public bool UserDelegationIsEnabled { get; set; }

        public string Version { get; set; }

        public Dictionary<string, bool> Features { get; set; }
    }
}
