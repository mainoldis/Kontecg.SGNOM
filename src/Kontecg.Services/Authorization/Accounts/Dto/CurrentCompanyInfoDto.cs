using Kontecg.Application.Services.Dto;
using Kontecg.Domain.Entities.Auditing;
using System;

namespace Kontecg.Authorization.Accounts.Dto
{
    public class CurrentCompanyInfoDto : EntityDto, IHasCreationTime
    {
        public string CompanyName { get; set; }

        public string Organism { get; set; }

        public string Address { get; set; }

        public string Name { get; set; }

        public DateTime CreationTime { get; set; }
    }
}
