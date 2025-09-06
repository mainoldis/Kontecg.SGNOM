using Kontecg.Application.Services.Dto;

namespace Kontecg.DynamicEntityProperties.Dto
{
    public class DynamicEntityPropertyDto : EntityDto
    {
        public string EntityFullName { get; set; }

        public string DynamicPropertyName { get; set; }

        public int DynamicPropertyId { get; set; }

        public int? CompanyId { get; set; }
    }
}
