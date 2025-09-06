using Kontecg.Application.Services.Dto;
using Kontecg.Common;
using Kontecg.Runtime.Validation;

namespace Kontecg.Authorization.Users.Delegation.Dto
{
    public class GetUserDelegationsInput : IPagedResultRequest, ISortedResultRequest, IShouldNormalize
    {
        public int MaxResultCount { get; set; }

        public int SkipCount { get; set; }

        public string Sorting { get; set; }

        public void Normalize()
        {
            if (string.IsNullOrEmpty(Sorting) || Sorting == "userName ASC")
            {
                Sorting = "Username";
            }

            Sorting = DtoSortingHelper.ReplaceSorting(Sorting, s =>
            {
                if (s == "userName DESC")
                {
                    s = "UserName DESC";
                }

                return s;
            });
        }
    }
}
