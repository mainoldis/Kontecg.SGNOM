using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Kontecg.Data;
using Kontecg.Domain;
using Kontecg.Domain.Entities;
using Kontecg.Domain.Entities.Auditing;
using Kontecg.HumanResources;
using Kontecg.MultiCompany;

namespace Kontecg.WorkRelations
{
    [Table("performance_evaluations", Schema = "rel")]
    [MultiCompanySide(MultiCompanySides.Company)]
    public class PerformanceEvaluation : AuditedEntity, IMustHaveCompany
    {
        public const string LawPenaltyName = "SANCIÓN LABORAL";

        public virtual int DocumentId { get; set; }

        [Required]
        [ForeignKey("DocumentId")]
        public virtual PerformanceDocument Document { get; set; }

        [Required]
        public virtual decimal Evaluation { get; set; }

        public virtual int? SummaryId { get; set; }

        [ForeignKey("SummaryId")]
        public virtual PerformanceSummary Summary { get; set; }

        [Required]
        public virtual long PersonId { get; set; }

        [NotMapped]
        public virtual Person Person { get; set; }

        public virtual long EmploymentId { get; set; }

        [Required]
        [ForeignKey("EmploymentId")]
        public virtual EmploymentDocument Employment { get; set; }

        [Required]
        public virtual GenerationSystemData Kind { get; set; }

        public virtual int CompanyId { get; set; }

        public virtual IList<PerformanceEvaluationLawPenalty> LawPenalties { get; set; }

        [NotMapped]
        public virtual bool MatchPenaltyWithEvaluation
        {
            get
            {
                return LawPenalties.Any() && Evaluation == decimal.Zero && Summary is {DisplayName: LawPenaltyName};
            }
        }

        public PerformanceEvaluation()
        {
            Kind = GenerationSystemData.Unknown;
            LawPenalties = new List<PerformanceEvaluationLawPenalty>();
        }

        public PerformanceEvaluation(long personId, decimal evaluation)
            :this()
        {
            PersonId = personId;
            Evaluation = evaluation;
        }
    }
}
