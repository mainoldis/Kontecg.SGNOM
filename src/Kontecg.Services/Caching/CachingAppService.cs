using System.Linq;
using System.Threading.Tasks;
using Kontecg.Application.Services.Dto;
using Kontecg.Authorization;
using Kontecg.Caching.Dto;
using Kontecg.Runtime.Caching;

namespace Kontecg.Caching
{
    [KontecgAuthorize(PermissionNames.AdministrationHostMaintenance)]
    public class CachingAppService : KontecgAppServiceBase, ICachingAppService
    {
        private readonly ICacheManager _cacheManager;

        public CachingAppService(ICacheManager cacheManager)
        {
            _cacheManager = cacheManager;
        }

        public ListResultDto<CacheDto> GetAllCaches()
        {
            var caches = _cacheManager.GetAllCaches()
                .Select(cache => new CacheDto
                {
                    Name = cache.Name
                })
                .ToList();

            return new ListResultDto<CacheDto>(caches);
        }

        public async Task ClearCache(EntityDto<string> input)
        {
            var cache = _cacheManager.GetCache(input.Id);
            await cache.ClearAsync();
        }

        public async Task ClearAllCaches()
        {
            var caches = _cacheManager.GetAllCaches();
            foreach (var cache in caches) await cache.ClearAsync();
        }
    }
}
