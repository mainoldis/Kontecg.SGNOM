using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Kontecg.Domain.Entities.Auditing;
using Kontecg.MultiCompany;

namespace Kontecg.HumanResources
{
    [Table("person_driver_licenses", Schema = "docs")]
    [MultiCompanySide(MultiCompanySides.Company)]
    public class PersonDriverLicense : AuditedEntity
    {
        public const int MaxLicenseNumber = 11;

        [Required]
        public virtual long PersonId { get; set; }

        [NotMapped]
        public virtual Person Person { get; set; }

        public virtual int DriverLicenseDefinitionId { get; set; }

        [Required]
        [ForeignKey("DriverLicenseDefinitionId")]
        public virtual DriverLicenseDefinition DriverLicense { get; set; }

        [Required]
        [StringLength(MaxLicenseNumber)]
        public virtual string LicenseNumber { get; set; }

        [Required]
        public virtual DateTime EffectiveSince { get; set; }

        [Required]
        public virtual DateTime EffectiveUntil { get; set; }

        public virtual DateTime? LastEvaluation { get; set; }

        public PersonDriverLicense(long personId, int driverLicenseDefinitionId, string licenseNumber, DateTime effectiveSince, DateTime effectiveUntil, DateTime? lastEvaluation = null)
        {
            PersonId = personId;
            DriverLicenseDefinitionId = driverLicenseDefinitionId;
            LicenseNumber = licenseNumber;
            EffectiveSince = effectiveSince;
            EffectiveUntil = effectiveUntil;
            LastEvaluation = lastEvaluation;
        }
    }
}
