using System.Threading.Tasks;
using Kontecg.Domain.Services;

namespace Kontecg.Authorization.Users.Profile
{
    public interface IProfileImageService : IDomainService
    {
        Task<string> GetProfilePictureContentForUserAsync(UserIdentifier userIdentifier);
    }
}
