using Kontecg.Application.Services.Dto;

namespace Kontecg.Accounting.Dto
{
    public class ViewNameDto : EntityDto
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public string ReferenceGroup { get; set; }
    }
}