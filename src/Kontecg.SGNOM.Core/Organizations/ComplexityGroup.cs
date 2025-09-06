using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Kontecg.Domain.Entities;
using Kontecg.Domain.Entities.Auditing;
using Kontecg.MultiCompany;
using NMoneys;

namespace Kontecg.Organizations
{
    [Table("complexity_groups", Schema = "est")]
    [MultiCompanySide(MultiCompanySides.Company)]
    public class ComplexityGroup : CreationAuditedEntity, IMustHaveCompany, IPassivable
    {
        public const int MaxGroupLength = 8;

        public const int MaxDescriptionLength = 150;

        [Required]
        [StringLength(MaxGroupLength)]
        public virtual string Group { get; set; }

        [Required]
        public virtual Money BaseSalary { get; set; }

        [StringLength(MaxDescriptionLength)]
        public virtual string Description { get; set; }

        [Required]
        public virtual int CompanyId { get; set; }

        [Required]
        public virtual bool IsActive { get; set; }

        public ComplexityGroup()
        {
            IsActive = true;
        }

        public ComplexityGroup(int companyId, string group, string description, Money baseSalary)
            : this()
        {
            CompanyId = companyId;
            Group = group?.ToUpperInvariant();
            Description = description;
            BaseSalary = baseSalary;
        }

        public override string ToString()
        {
            return $"{Group}";
        }
    }
}
