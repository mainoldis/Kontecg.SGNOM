using System.Threading.Tasks;
using Kontecg.Configuration;
using Kontecg.Dependency;

namespace Kontecg.Authorization.Users.Profile
{
    public class ProfileImageServiceFactory : ITransientDependency
    {
        private readonly IIocResolver _iocResolver;
        private readonly ISettingManager _settingManager;

        public ProfileImageServiceFactory(
            ISettingManager settingManager,
            IIocResolver iocResolver)
        {
            _settingManager = settingManager;
            _iocResolver = iocResolver;
        }

        public async Task<IDisposableDependencyObjectWrapper<IProfileImageService>> GetAsync(UserIdentifier userIdentifier)
        {
            if (await _settingManager.GetSettingValueForUserAsync<bool>(
                AppSettings.UserManagement.UseBlobStorageProfilePicture, userIdentifier))
                return _iocResolver.ResolveAsDisposable<BlobProfileImageService>();

            return _iocResolver.ResolveAsDisposable<DbProfileImageService>();
        }
    }
}
