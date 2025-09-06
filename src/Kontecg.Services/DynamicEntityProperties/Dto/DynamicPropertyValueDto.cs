using Kontecg.Application.Services.Dto;

namespace Kontecg.DynamicEntityProperties.Dto
{
    public class DynamicPropertyValueDto : EntityDto
    {
        public virtual string Value { get; set; }

        public int? CompanyId { get; set; }

        public int DynamicPropertyId { get; set; }
    }
}
