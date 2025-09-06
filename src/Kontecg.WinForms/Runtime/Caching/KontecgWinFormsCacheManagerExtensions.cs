using Kontecg.ExceptionHandling;

namespace Kontecg.Runtime.Caching
{
    public static class KontecgWinFormsCacheManagerExtensions
    {
        public static ITypedCache<string, SnapshotCacheItem> GetSnapshotCache(this ICacheManager cacheManager)
        {
            return cacheManager.GetCache<string, SnapshotCacheItem>(SnapshotCacheItem.CacheStoreName);
        }
    }
}