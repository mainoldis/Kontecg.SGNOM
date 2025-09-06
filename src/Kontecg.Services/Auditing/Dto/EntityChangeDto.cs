using System;
using Kontecg.Application.Services.Dto;
using Kontecg.Events.Bus.Entities;

namespace Kontecg.Auditing.Dto
{
    public class EntityChangeDto : EntityDto<long>
    {
        public DateTime ChangeTime { get; set; }

        public EntityChangeType ChangeType { get; set; }

        public long EntityChangeSetId { get; set; }

        public string EntityId { get; set; }

        public string EntityTypeFullName { get; set; }

        public int? CompanyId { get; set; }

        public object EntityEntry { get; set; }
    }
}
