using Kontecg.Configuration.Dto;

namespace Kontecg.Configuration.Companies.Dto
{
    public class CompanyEmailSettingsEditDto : EmailSettingsEditDto
    {
        public bool UseHostDefaultEmailSettings { get; set; }
    }
}
