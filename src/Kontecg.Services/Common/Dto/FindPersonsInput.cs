using Kontecg.Application.Services.Dto;
using Kontecg.Dto;
using Kontecg.Primitives;
using Kontecg.Runtime.Validation;

namespace Kontecg.Common.Dto
{
    public class FindPersonsInput : PagedSortedAndFilteredInputDto, IPagedAndSortedResultRequest, IShouldNormalize
    {
        public string IdentityCard { get; set;}

        public Gender? Gender { get; set; }

        public void Normalize()
        {
            if (string.IsNullOrEmpty(Sorting)) Sorting = "Name,Surname,Lastname";

            IdentityCard = IdentityCard?.Trim();
            Filter = Filter?.Trim();
        }
    }
}
