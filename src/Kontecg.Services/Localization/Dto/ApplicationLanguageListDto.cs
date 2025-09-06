using Kontecg.Application.Services.Dto;

namespace Kontecg.Localization.Dto
{
    public class ApplicationLanguageListDto : FullAuditedEntityDto
    {
        public virtual int? CompanyId { get; set; }

        public virtual string Name { get; set; }

        public virtual string DisplayName { get; set; }

        public virtual string Icon { get; set; }

        public bool IsDisabled { get; set; }
    }
}
