using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Kontecg.Accounting;
using Kontecg.Domain.Entities;
using Kontecg.Extensions;
using Kontecg.MultiCompany;

namespace Kontecg.WorkRelations
{
    [Table("employment_summaries", Schema = "rel")]
    [MultiCompanySide(MultiCompanySides.Company)]
    public class EmploymentSummary : Entity
    {
        public const int MaxDisplayNameLength = 250;

        public const int MaxAcronymLength = 5;

        public const int MaxClassificationLength = 100;

        /// <summary>
        ///     Max length of the <see cref="Reference" /> property.
        /// </summary>
        public const int MaxReferenceLength = AccountingFunctionDefinition.MaxReferenceLength;

        [Required]
        [StringLength(MaxDisplayNameLength)]
        public virtual string DisplayName { get; set; }

        [StringLength(MaxAcronymLength)]
        public virtual string Acronym { get; set; }

        public virtual int? ParentId { get; set; }

        [ForeignKey("ParentId")]
        public virtual EmploymentSummary Parent { get; set; }

        [Required]
        public virtual EmploymentType Type { get; set; }

        [StringLength(MaxClassificationLength)]
        public virtual string Classification { get; set; }

        public virtual bool? Fluctuation { get; set; }

        [StringLength(MaxReferenceLength)]
        public virtual string Reference { get; set; }

        public EmploymentSummary()
        {
        }

        public EmploymentSummary(string description, EmploymentType type, string acronym = null, int? parentId = null, string classification = null, bool? fluctuation = null, string reference = null)
        {
            DisplayName = description ?? throw new ArgumentNullException(nameof(description));
            Acronym = acronym;
            Type = type;
            Classification = Type == EmploymentType.B && !classification.IsNullOrWhiteSpace() ? classification : null;
            Fluctuation = Type == EmploymentType.B ? fluctuation : null;
            ParentId = parentId;
            Reference = reference;
        }

        public override string ToString()
        {
            return $"{(Parent != null ? Parent + "/": "")}{DisplayName}";
        }
    }
}
