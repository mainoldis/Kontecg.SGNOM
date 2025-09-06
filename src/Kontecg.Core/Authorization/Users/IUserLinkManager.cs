using Kontecg.Authorization.Impersonation;
using System.Threading.Tasks;

namespace Kontecg.Authorization.Users
{
    public interface IUserLinkManager
    {
        Task LinkAsync(User firstUser, User secondUser);

        Task<bool> AreUsersLinkedAsync(UserIdentifier firstUserIdentifier, UserIdentifier secondUserIdentifier);

        Task UnlinkAsync(UserIdentifier userIdentifier);

        Task<UserAccount> GetUserAccountAsync(UserIdentifier userIdentifier);

        Task<string> GetAccountSwitchTokenAsync(long targetUserId, int? targetCompanyId);

        Task<UserAndIdentity> GetSwitchedUserAndIdentityAsync(string switchAccountToken);
    }
}
