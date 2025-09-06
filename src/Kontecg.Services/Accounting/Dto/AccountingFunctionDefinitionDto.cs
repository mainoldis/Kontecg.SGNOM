using Kontecg.Application.Services.Dto;

namespace Kontecg.Accounting.Dto
{
    public class AccountingFunctionDefinitionDto : EntityDto
    {
        public virtual string Name { get; set; }

        public virtual string Description { get; set; }

        public virtual string Reference { get; set; }

        public virtual string Formula { get; set; }

        public virtual bool IsActive { get; set; }
    }
}