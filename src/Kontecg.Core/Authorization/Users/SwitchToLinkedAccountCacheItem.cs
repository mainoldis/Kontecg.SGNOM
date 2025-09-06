using System;

namespace Kontecg.Authorization.Users
{
    [Serializable]
    public class SwitchToLinkedAccountCacheItem
    {
        public const string CacheName = "AppSwitchToLinkedAccountCache";

        public int? TargetCompanyId { get; set; }

        public long TargetUserId { get; set; }

        public int? ImpersonatorCompanyId { get; set; }

        public long? ImpersonatorUserId { get; set; }

        public SwitchToLinkedAccountCacheItem()
        {
        }

        public SwitchToLinkedAccountCacheItem(
            int? targetCompanyId,
            long targetUserId,
            int? impersonatorCompanyId,
            long? impersonatorUserId
        )
        {
            TargetCompanyId = targetCompanyId;
            TargetUserId = targetUserId;
            ImpersonatorCompanyId = impersonatorCompanyId;
            ImpersonatorUserId = impersonatorUserId;
        }
    }
}
