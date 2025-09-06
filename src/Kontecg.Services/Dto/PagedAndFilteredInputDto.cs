using System.ComponentModel.DataAnnotations;
using Kontecg.Application.Services.Dto;

namespace Kontecg.Dto
{
    public class PagedAndFilteredInputDto : IPagedResultRequest
    {
        public PagedAndFilteredInputDto()
        {
            MaxResultCount = KontecgCoreConsts.DefaultPageSize;
        }

        public string Filter { get; set; }

        [Range(1, int.MaxValue)]
        public int MaxResultCount { get; set; }

        [Range(0, int.MaxValue)] public int SkipCount { get; set; }
    }
}
