using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Kontecg.Domain.Entities.Auditing;
using Kontecg.MultiCompany;

namespace Kontecg.WorkRelations
{
    [Table("performance_summaries", Schema = "rel")]
    [MultiCompanySide(MultiCompanySides.Company)]
    public class PerformanceSummary : AuditedEntity
    {
        public const int MaxDisplayNameLength = 150;

        [Required]
        [StringLength(MaxDisplayNameLength)]
        public string DisplayName { get; set; }

        public PerformanceSummary()
        {
        }

        public PerformanceSummary(string description)
        {
            DisplayName = description;

            SetDescriptionNormalized();
        }

        protected virtual void SetDescriptionNormalized()
        {
            DisplayName = DisplayName?.ToUpperInvariant();
        }
    }
}
