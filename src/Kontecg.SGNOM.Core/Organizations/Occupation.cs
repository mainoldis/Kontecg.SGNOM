using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Kontecg.Domain.Entities;
using Kontecg.Domain.Entities.Auditing;
using Kontecg.MultiCompany;

namespace Kontecg.Organizations
{
    [Table("occupations", Schema = "est")]
    [MultiCompanySide(MultiCompanySides.Company)]
    public class Occupation : CreationAuditedEntity, IMustHaveCompany, IPassivable
    {
        public const int MaxDisplayNameLength = 250;

        public const int MaxCodeMaxLength = 8;

        [Required]
        [StringLength(MaxDisplayNameLength)]
        public virtual string DisplayName { get; set; }

        [Required]
        [StringLength(MaxCodeMaxLength)]
        public virtual string Code { get; set; }

        public virtual int GroupId { get; set; }

        [Required]
        [ForeignKey("GroupId")]
        public virtual ComplexityGroup Group { get; set; }

        public virtual int CategoryId { get; set; }

        [Required]
        [ForeignKey("CategoryId")]
        public virtual OccupationalCategory Category { get; set; }

        public virtual int ResponsibilityId { get; set; }

        [Required]
        [ForeignKey("ResponsibilityId")]
        public virtual Responsibility Responsibility { get; set; }

        [NotMapped]
        public virtual string FullOccupationDescription
        {
            get
            {
                return
                    $"{DisplayName}{(Responsibility != null && Responsibility.NormalizedDescription != "DEL CARGO" ? " (" + Responsibility.DisplayName + ")" : "")}";
            }
        }

        public virtual IList<QualificationRequirementDefinition> Requirements { get; set; }

        [Required]
        public virtual int CompanyId { get; set; }

        [Required]
        public virtual bool IsActive { get; set; }

        public Occupation()
        {
            Code = "AUTO";
            IsActive = true;
            Requirements = new List<QualificationRequirementDefinition>();
        }

        public Occupation(int companyId, string description, ComplexityGroup complexityGroup, OccupationalCategory occupationalCategory, Responsibility responsibility)
            : this()
        {
            CompanyId = companyId;
            DisplayName = description;
            Group = complexityGroup;
            Category = occupationalCategory;
            Responsibility = responsibility;
        }

        public Occupation(int companyId, string description, int complexityGroupId, int occupationalCategoryId, int responsibilityId)
            : this()
        {
            CompanyId = companyId;
            DisplayName = description;
            GroupId = complexityGroupId;
            CategoryId = occupationalCategoryId;
            ResponsibilityId = responsibilityId;
        }

        public void AddQualificationRequirementDefinition(string displayName)
        {
            var qualificationRequirementDefinition = new QualificationRequirementDefinition(displayName)
                {
                    Occupation = this,
                    CompanyId = this.CompanyId
                };

            Requirements.Add(qualificationRequirementDefinition);
        }

        public void CreateCode()
        {

        }
    }
}
