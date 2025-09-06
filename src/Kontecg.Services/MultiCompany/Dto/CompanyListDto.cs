using System;
using Kontecg.Application.Services.Dto;
using Kontecg.Auditing;
using Kontecg.Domain.Entities;
using Kontecg.Domain.Entities.Auditing;

namespace Kontecg.MultiCompany.Dto
{
    public class CompanyListDto : EntityDto, IPassivable, IHasCreationTime
    {
        public string CompanyName { get; set; }

        public string Name { get; set; }

        public string Reup { get; set; }

        [DisableAuditing] public string ConnectionString { get; set; }

        public DateTime CreationTime { get; set; }

        public bool IsActive { get; set; }
    }
}
