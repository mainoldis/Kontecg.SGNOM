using System;
using Kontecg.Application.Services.Dto;
using Kontecg.Domain.Entities.Auditing;

namespace Kontecg.Authorization.Roles.Dto
{
    public class RoleListDto : EntityDto, IHasCreationTime
    {
        public string Name { get; set; }

        public string DisplayName { get; set; }

        public bool IsStatic { get; set; }

        public bool IsDefault { get; set; }

        public DateTime CreationTime { get; set; }
    }
}
