using Kontecg.Configuration.Host.Dto;

namespace Kontecg.Configuration.Companies.Dto
{
    public class CompanyUserManagementSettingsEditDto
    {
        public bool AllowSelfRegistration { get; set; }

        public bool IsNewRegisteredUserActiveByDefault { get; set; }

        public bool IsEmailConfirmationRequiredForLogin { get; set; }

        public SessionTimeOutSettingsEditDto SessionTimeOutSettings { get; set; }

        public UserPasswordSettingsEditDto UserPasswordSettings { get; set; }
    }
}
