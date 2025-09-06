using Kontecg.Application.Services.Dto;
using System.Collections.Generic;

namespace Kontecg.Accounting.Dto
{
    public class DocumentDefinitionDto : EntityDto
    {
        public int Code { get; set; }

        public string Description { get; set; }

        public string Reference { get; set; }

        public string Legal { get; set; }

        public string LegalTypeAssemblyQualifiedName { get; set; }

        public IReadOnlyList<ViewNameDto> Views { get; set; }

        public bool IsActive { get; set; }
    }
}
