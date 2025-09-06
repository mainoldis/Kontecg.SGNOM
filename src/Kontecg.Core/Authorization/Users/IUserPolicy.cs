using System.Threading.Tasks;
using Kontecg.Domain.Policies;

namespace Kontecg.Authorization.Users
{
    public interface IUserPolicy : IPolicy
    {
        Task CheckUserPolicyAsync(int companyId);
    }
}
