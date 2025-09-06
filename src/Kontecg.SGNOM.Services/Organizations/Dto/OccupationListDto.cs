using Kontecg.Application.Services.Dto;

namespace Kontecg.Organizations.Dto
{
    public class OccupationListDto : EntityDto
    {
        public string DisplayName { get; set; }

        public string Code { get; set; }

        public string Group { get; set; }

        public string Category { get; set; }

        public string Responsibility { get; set; }

        public string FullOccupationDescription { get; set; }

        public bool IsActive { get; set; }
    }
}
