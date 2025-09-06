using System.ComponentModel.DataAnnotations;
using Kontecg.Application.Services.Dto;
using Kontecg.Auditing;

namespace Kontecg.MultiCompany.Dto
{
    public class CompanyEditDto : EntityDto
    {
        [Required]
        [StringLength(KontecgCompanyBase.MaxCompanyNameLength)]
        public string CompanyName { get; set; }

        [Required]
        [StringLength(KontecgCompanyBase.MaxNameLength)]
        public string Name { get; set; }

        [StringLength(Company.MaxReupLength)]
        public string Reup { get; set; }

        [DisableAuditing] public string ConnectionString { get; set; }

        public bool IsActive { get; set; }
    }
}
