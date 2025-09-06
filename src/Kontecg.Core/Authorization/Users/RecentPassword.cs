using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Kontecg.Domain.Entities;
using Kontecg.Domain.Entities.Auditing;

namespace Kontecg.Authorization.Users
{
    [Table("recent_passwords", Schema = "seg")]
    public class RecentPassword : CreationAuditedEntity<Guid>, IMayHaveCompany
    {
        public virtual int? CompanyId { get; set; }

        [Required]
        public virtual long UserId { get; set; }

        [Required]
        public virtual string Password { get; set; }

        public RecentPassword()
        {
            Id = UuidGenerator.Instance.Create();
        }
    }
}
