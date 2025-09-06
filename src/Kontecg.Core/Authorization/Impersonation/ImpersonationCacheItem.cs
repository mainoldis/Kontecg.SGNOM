using System;

namespace Kontecg.Authorization.Impersonation
{
    [Serializable]
    public class ImpersonationCacheItem
    {
        public const string CacheName = "KontecgImpersonationCache";

        public int? ImpersonatorCompanyId { get; set; }

        public long ImpersonatorUserId { get; set; }

        public int? TargetCompanyId { get; set; }

        public long TargetUserId { get; set; }

        public bool IsBackToImpersonator { get; set; }

        public ImpersonationCacheItem()
        {
        }

        public ImpersonationCacheItem(int? targetCompanyId, long targetUserId, bool isBackToImpersonator)
        {
            TargetCompanyId = targetCompanyId;
            TargetUserId = targetUserId;
            IsBackToImpersonator = isBackToImpersonator;
        }
    }
}
