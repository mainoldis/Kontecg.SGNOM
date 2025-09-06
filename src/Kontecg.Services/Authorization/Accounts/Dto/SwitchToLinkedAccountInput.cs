using System.ComponentModel.DataAnnotations;

namespace Kontecg.Authorization.Accounts.Dto
{
    public class SwitchToLinkedAccountInput
    {
        public int? TargetCompanyId { get; set; }

        [Range(1, long.MaxValue)]
        public long TargetUserId { get; set; }

        public UserIdentifier ToUserIdentifier()
        {
            return new UserIdentifier(TargetCompanyId, TargetUserId);
        }
    }
}
