using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Kontecg.Domain.Entities;
using Kontecg.Domain.Entities.Auditing;

namespace Kontecg.Organizations
{
    [Table("workplace_payments", Schema = "est")]
    public class WorkPlacePayment : AuditedEntity<long>, IMustHaveCompany, IPassivable
    {
        /// <summary>
        ///     Max length of the <see cref="Code" /> property.
        /// </summary>
        public const int MaxCodeLength = 8;

        /// <summary>
        ///     Max length of the <see cref="Description" /> property.
        /// </summary>
        public const int MaxDescriptionLength = 150;

        [Required]
        [StringLength(MaxCodeLength)]
        public virtual string Code { get; set; }

        [Required]
        [StringLength(MaxDescriptionLength)]
        public virtual string Description { get; set; }

        public virtual int CompanyId { get; set; }

        /// <inheritdoc />
        public virtual bool IsActive { get; set; }

        public virtual List<WorkPlaceUnit> WorkPlaces { get; set; }

        public WorkPlacePayment()
        {
            WorkPlaces = new List<WorkPlaceUnit>();
            IsActive = true;
        }

        public WorkPlacePayment(string code, string description)
            : this()
        {
            Code = code;
            Description = description;
        }
    }
}
