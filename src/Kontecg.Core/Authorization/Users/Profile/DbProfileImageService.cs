using System;
using System.Threading.Tasks;
using Kontecg.Dependency;
using Kontecg.Storage;

namespace Kontecg.Authorization.Users.Profile
{
    public class DbProfileImageService : IProfileImageService, ITransientDependency
    {
        private readonly IBinaryObjectManager _binaryObjectManager;
        private readonly UserManager _userManager;

        public DbProfileImageService(
            IBinaryObjectManager binaryObjectManager,
            UserManager userManager)
        {
            _binaryObjectManager = binaryObjectManager;
            _userManager = userManager;
        }

        public async Task<string> GetProfilePictureContentForUserAsync(UserIdentifier userIdentifier)
        {
            var user = await _userManager.GetUserOrNullAsync(userIdentifier);
            if (user?.ProfilePictureId == null) return "";

            var file = await _binaryObjectManager.GetOrNullAsync(user.ProfilePictureId.Value);
            return file == null ? "" : Convert.ToBase64String(file.Bytes);
        }
    }
}
