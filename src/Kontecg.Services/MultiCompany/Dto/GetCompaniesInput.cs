using System;
using Kontecg.Dto;
using Kontecg.Runtime.Validation;

namespace Kontecg.MultiCompany.Dto
{
    public class GetCompaniesInput : PagedAndSortedInputDto, IShouldNormalize
    {
        public string Filter { get; set; }
        public DateTime? CreationDateStart { get; set; }
        public DateTime? CreationDateEnd { get; set; }

        public void Normalize()
        {
            if (string.IsNullOrEmpty(Sorting)) Sorting = "CompanyName";
        }
    }
}
