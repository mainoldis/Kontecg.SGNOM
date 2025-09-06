using Kontecg.Auditing;

namespace Kontecg.Configuration.Companies.Dto
{
    public class LdapSettingsEditDto
    {
        public bool IsModuleEnabled { get; set; }

        public bool IsEnabled { get; set; }

        public string Domain { get; set; }

        public string UserName { get; set; }

        public bool UseSsl { get; set; }

        [DisableAuditing]
        public string Password { get; set; }
    }
}
