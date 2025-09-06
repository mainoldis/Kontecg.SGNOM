using System;
using Kontecg.Application.Services.Dto;
using Kontecg.Events.Bus.Entities;

namespace Kontecg.Auditing.Dto
{
    public class EntityChangeListDto : EntityDto<long>
    {
        public long? UserId { get; set; }

        public string UserName { get; set; }

        public DateTime ChangeTime { get; set; }

        public string EntityTypeFullName { get; set; }

        public EntityChangeType ChangeType { get; set; }

        public string ChangeTypeName => ChangeType.ToString();

        public long EntityChangeSetId { get; set; }
    }
}
