using System.ComponentModel.DataAnnotations;
using Kontecg.Application.Services.Dto;

namespace Kontecg.Dto
{
    public class PagedInputDto : IPagedResultRequest
    {
        public PagedInputDto()
        {
            MaxResultCount = KontecgCoreConsts.DefaultPageSize;
        }

        [Range(1, int.MaxValue)]
        public int MaxResultCount { get; set; }

        [Range(0, int.MaxValue)] public int SkipCount { get; set; }
    }
}
