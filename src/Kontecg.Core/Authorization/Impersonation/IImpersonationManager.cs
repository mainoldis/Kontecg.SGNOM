using System.Threading.Tasks;
using Kontecg.Domain.Services;

namespace Kontecg.Authorization.Impersonation
{
    public interface IImpersonationManager : IDomainService
    {
        Task<UserAndIdentity> GetImpersonatedUserAndIdentityAsync(string impersonationToken);

        Task<string> GetImpersonationTokenAsync(long userId, int? companyId);

        Task<string> GetBackToImpersonatorTokenAsync();
    }
}
