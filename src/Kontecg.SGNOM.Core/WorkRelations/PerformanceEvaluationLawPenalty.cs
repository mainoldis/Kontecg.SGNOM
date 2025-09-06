using System.ComponentModel.DataAnnotations;
using Kontecg.Domain.Entities.Auditing;
using Kontecg.Domain.Entities;
using Kontecg.MultiCompany;
using System.ComponentModel.DataAnnotations.Schema;
using Kontecg.HumanResources;

namespace Kontecg.WorkRelations
{
    [Table("performance_evaluation_law_penalties", Schema = "rel")]
    [MultiCompanySide(MultiCompanySides.Company)]
    public class PerformanceEvaluationLawPenalty : AuditedEntity, IMustHaveCompany
    {
        [Required]
        public virtual int CompanyId { get; set; }

        public virtual int EvaluationId { get; set; }

        [Required]
        [ForeignKey("EvaluationId")]
        public virtual PerformanceEvaluation Evaluation { get; set; }

        public virtual int LawPenaltyId { get; set; }

        [Required]
        [ForeignKey("LawPenaltyId")]
        public virtual PersonLawPenalty LawPenalty { get; set; }

        public PerformanceEvaluationLawPenalty(int companyId, int evaluationId, int lawPenaltyId)
        {
            CompanyId = companyId;
            EvaluationId = evaluationId;
            LawPenaltyId = lawPenaltyId;
        }
    }
}
