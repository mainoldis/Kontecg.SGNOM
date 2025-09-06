using Kontecg.Application.Services.Dto;

namespace Kontecg.Dto
{
    public class PagedAndSortedInputDto : PagedInputDto, ISortedResultRequest
    {
        public PagedAndSortedInputDto()
        {
            MaxResultCount = KontecgCoreConsts.DefaultPageSize;
        }

        public string Sorting { get; set; }
    }
}
