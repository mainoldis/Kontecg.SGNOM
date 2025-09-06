using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kontecg.Organizations
{
    public class WorkPlaceUnit : OrganizationUnit
    {
        /// <summary>
        ///     Max length of the <see cref="Acronym" /> property.
        /// </summary>
        public const int MaxAcronymLength = 150;

        [StringLength(MaxAcronymLength)]
        public virtual string Acronym { get; set; }

        [Required]
        public virtual int MaxMembersApproved { get; set; }

        public virtual int ClassificationId { get; set; }

        [Required]
        [ForeignKey("ClassificationId")]
        public virtual WorkPlaceClassification Classification { get; set; }

        public virtual long? WorkPlacePaymentId { get; set; }

        [ForeignKey("WorkPlacePaymentId")]
        public virtual WorkPlacePayment WorkPlacePayment { get; set; }

        public WorkPlaceUnit()
        {
        }

        public WorkPlaceUnit(int? companyId, string displayName, string acronym = null, int maxMembersApproved = 0, int order = 0, long? parentId = null)
            : base(companyId, displayName, order, parentId)
        {
            MaxMembersApproved = maxMembersApproved;
            Acronym = acronym;
        }
    }
}
