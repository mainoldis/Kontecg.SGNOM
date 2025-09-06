using Kontecg.Application.Services.Dto;

namespace Kontecg.Identity.Dto
{
    public class SignDto : EntityDto
    {
        public string FullName { get; set; }

        public string Code { get; set; }

        public string Occupation { get; set; }

        public string IdentityCard { get; set; }

        public bool Owner { get; set; }
    }
}
