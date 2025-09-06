using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Kontecg.Domain.Entities;
using Kontecg.Domain.Entities.Auditing;
using Kontecg.MultiCompany;

namespace Kontecg.Timing
{
    [Table("work_regimens", Schema = "gen")]
    [MultiCompanySide(MultiCompanySides.Company)]
    public class WorkRegime : AuditedEntity, IMustHaveCompany, IPassivable
    {
        public const int MaxLegalNameLength = 10;
        public const int MaxTimesLength = 500;

        public virtual WorkRegimeType Type { get; set; }

        /// <summary>
        ///     Store the day scheduling in the form of 5*2-6*1
        /// </summary>
        [Required]
        [StringLength(MaxTimesLength)]
        public virtual string DaysScheduling { get; set; }

        /// <summary>
        ///     Store the crazy day on scheduling in the form of 5*4
        /// </summary>
        [StringLength(MaxTimesLength)]
        public virtual string SpecialGroup { get; set; }

        /// <summary>
        ///     Store the legal form for day scheduling
        /// </summary>
        [StringLength(MaxLegalNameLength)]
        public virtual string LegalName { get; set; }

        /// <summary>
        /// Store the time scheduling in the form of 8:48*8:48*8:48*8:48*8:48
        /// </summary>
        [Required]
        [StringLength(MaxTimesLength)]
        public virtual string TimeScheduling { get; set; }

        [Required]
        public virtual int CompanyId { get; set; }

        [Required]
        public virtual bool IsActive { get; set; }

        public WorkRegime()
        {
            IsActive = true;
        }

        public WorkRegime(WorkRegimeType type, string daysScheduling, string timeScheduling, int companyId, string specialGroup = null, string legalName = null)
            : this()
        {
            Type = type;
            DaysScheduling = daysScheduling;
            TimeScheduling = timeScheduling;
            SpecialGroup = specialGroup;
            LegalName = legalName;
            CompanyId = companyId;
        }
    }
}
