using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Kontecg.Domain.Entities;
using Kontecg.Domain.Entities.Auditing;
using Kontecg.MultiCompany;
using Kontecg.Salary;
using Kontecg.Taxes;

namespace Kontecg.HumanResources
{
    [Table("person_taxes", Schema = "docs")]
    [MultiCompanySide(MultiCompanySides.Company)]
    public class PersonTax : AuditedEntity, IMustHaveCompany, IPassivable
    {
        public const int MaxDisplayNameLength = 1000;

        public const int MaxScriptLength = 2000;

        [Required]
        public virtual long PersonId { get; set; }

        [Required]
        public virtual Guid GroupId { get; set; }

        [Required]
        public virtual TaxType TaxType { get; set; }

        [Required]
        public virtual MathType MathType { get; set; }

        [Required]
        public virtual decimal Factor { get; set; }

        [StringLength(MaxScriptLength)]
        public virtual string Formula { get; set; }

        /// <inheritdoc />
        [Required]
        public int CompanyId { get; set; }

        [StringLength(MaxDisplayNameLength)]
        public virtual string DisplayName { get; set; }

        /// <inheritdoc />
        public bool IsActive { get; set; }

        /// <inheritdoc />
        public PersonTax()
        {
            IsActive = true;
        }

        public PersonTax(long personId, Guid groupId, string displayName, TaxType taxType = TaxType.Income, MathType mathType = MathType.Percent, decimal factor = 50M, string formula = null)
            : this()
        {
            PersonId = personId;
            GroupId = groupId;
            DisplayName = displayName;
            TaxType = taxType;
            MathType = mathType;
            Factor = factor;
            Formula = formula;
        }
    }
}