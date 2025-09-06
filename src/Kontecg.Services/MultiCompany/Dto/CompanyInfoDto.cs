using Kontecg.Application.Services.Dto;
using Kontecg.Domain.Entities.Auditing;
using Kontecg.Domain.Entities;
using System;
using Kontecg.Storage;

namespace Kontecg.MultiCompany.Dto
{
    public class CompanyInfoDto : EntityDto, IPassivable, IHasCreationTime
    {
        public string CompanyName { get; set; }

        public string Organism { get; set; }

        public string Address { get; set; }

        public string Name { get; set; }

        public string Reup { get; set; }

        public TempFileInfo LogoFile { get; set; }

        public TempFileInfo LetterHeadFile { get; set; }

        public DateTime CreationTime { get; set; }

        public bool IsActive { get; set; }
    }
}
