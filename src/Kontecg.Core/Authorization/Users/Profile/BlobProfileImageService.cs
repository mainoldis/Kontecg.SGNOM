using System;
using System.Threading.Tasks;
using Kontecg.BlobStoring;
using Kontecg.Dependency;
using Kontecg.Storage.Blobs;

namespace Kontecg.Authorization.Users.Profile
{
    public class BlobProfileImageService : IProfileImageService, ITransientDependency
    {
        private readonly IBlobContainer<ProfilePictureContainer> _blobContainer;
        private readonly UserManager _userManager;

        public BlobProfileImageService(
            IBlobContainer<ProfilePictureContainer> blobContainer,
            UserManager userManager)
        {
            _blobContainer = blobContainer;
            _userManager = userManager;
        }

        public async Task<string> GetProfilePictureContentForUserAsync(UserIdentifier userIdentifier)
        {
            var user = await _userManager.GetUserOrNullAsync(userIdentifier);
            if (user?.ProfilePictureId == null) return "";

            var blobName = user.ProfilePictureId.Value.ToString();
            var buffer = await _blobContainer.GetAllBytesOrNullAsync(blobName);
            return buffer == null ? "" : Convert.ToBase64String(buffer);
        }
    }
}
