using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Kontecg.Configuration.Host.Dto;
using Kontecg.Runtime.Validation;
using Kontecg.Timing;

namespace Kontecg.Configuration.Companies.Dto
{
    public class CompanySettingsEditDto
    {
        public GeneralSettingsEditDto General { get; set; }

        [Required]
        public CompanyUserManagementSettingsEditDto UserManagement { get; set; }

        public CompanyEmailSettingsEditDto Email { get; set; }

        public LdapSettingsEditDto Ldap { get; set; }

        public ServicioAdminSettingsEditDto ServicioAdmin { get; set; }

        [Required]
        public SecuritySettingsEditDto Security { get; set; }

        public CompanyBillingSettingsEditDto Billing { get; set; }

        public CompanyOtherSettingsEditDto OtherSettings { get; set; }

        /// <summary>
        /// This validation is done for single-company applications.
        /// Because, these settings can only be set by company in a single-company application.
        /// </summary>
        public void ValidateHostSettings()
        {
            var validationErrors = new List<ValidationResult>();
            if (Clock.SupportsMultipleTimezone && General == null)
            {
                validationErrors.Add(new ValidationResult("General settings can not be null", new[] { "General" }));
            }

            if (Email == null)
            {
                validationErrors.Add(new ValidationResult("Email settings can not be null", new[] { "Email" }));
            }

            if (validationErrors.Count > 0)
            {
                throw new KontecgValidationException("Method arguments are not valid! See ValidationErrors for details.", validationErrors);
            }
        }
    }
}
