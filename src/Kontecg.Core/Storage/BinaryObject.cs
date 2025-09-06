using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Kontecg.Domain.Entities;

namespace Kontecg.Storage
{
    [Table("files", Schema = "docs")]
    public class BinaryObject : Entity<Guid>, IMayHaveCompany
    {
        public BinaryObject()
        {
            Id = UuidGenerator.Instance.Create();
        }

        public BinaryObject(int? companyId, byte[] bytes, string description = null)
            : this()
        {
            CompanyId = companyId;
            Bytes = bytes;
            Description = description;
        }

        public virtual string Description { get; set; }

        [Required]
        [MaxLength(BinaryObjectConsts.BytesMaxSize)]
        public virtual byte[] Bytes { get; set; }
        public virtual int? CompanyId { get; set; }
    }
}
