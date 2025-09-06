using System;
using System.ComponentModel.DataAnnotations.Schema;
using Kontecg.Domain.Entities;
using Kontecg.Domain.Entities.Auditing;
using Kontecg.Timing;

namespace Kontecg.Authorization.Delegation
{
    [Table("user_delegations", Schema = "seg")]
    public class UserDelegation : FullAuditedEntity<long>, IMayHaveCompany
    {
        /// <summary>
        /// Id of user who delegates the account
        /// </summary>
        public long SourceUserId { get; set; }

        /// <summary>
        /// Id of user who is delegated for the <see cref="SourceUserId"/> account
        /// </summary>
        public long TargetUserId { get; set; }

        /// <summary>
        /// CompanyId of delegation. Both users must be on same company.
        /// </summary>
        public int? CompanyId { get; set; }

        /// <summary>
        /// Start time of delegation
        /// </summary>
        public DateTime StartTime { get; set; }

        /// <summary>
        /// End time of delegation
        /// </summary>
        public DateTime EndTime { get; set; }

        public bool IsCreatedByUser(long userId){
            return SourceUserId == userId;
        }

        public bool IsExpired(){
            return EndTime <= Clock.Now;
        }

        public bool IsValid(){
            return StartTime <= Clock.Now && !IsExpired();
        }
    }
}
