using System;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using Kontecg.Auditing;
using Kontecg.Authorization.Users.Dto;
using Kontecg.Authorization.Users.Profile.Dto;
using Kontecg.BackgroundJobs;
using Kontecg.Baseline.Configuration;
using Kontecg.Configuration;
using Kontecg.Extensions;
using Kontecg.Localization;
using Kontecg.Runtime.Caching;
using Kontecg.Runtime.Security;
using Kontecg.Runtime.Session;
using Kontecg.Storage;
using Kontecg.Timing;
using Kontecg.UI;
using Microsoft.AspNetCore.Identity;
using Microsoft.IO;

namespace Kontecg.Authorization.Users.Profile
{
    [KontecgAuthorize]
    public class ProfileAppService : KontecgAppServiceBase, IProfileAppService
    {
        private readonly IBackgroundJobManager _backgroundJobManager;
        private readonly IBinaryObjectManager _binaryObjectManager;
        private readonly ICacheManager _cacheManager;
        private readonly ProfileImageServiceFactory _profileImageServiceFactory;
        private readonly ITempFileCacheManager _tempFileCacheManager;
        private readonly ITimeZoneService _timeZoneService;

        public ProfileAppService(
            IBinaryObjectManager binaryObjectManager,
            ITimeZoneService timezoneService,
            ICacheManager cacheManager,
            ITempFileCacheManager tempFileCacheManager,
            IBackgroundJobManager backgroundJobManager,
            ProfileImageServiceFactory profileImageServiceFactory)
        {
            _binaryObjectManager = binaryObjectManager;
            _timeZoneService = timezoneService;
            _cacheManager = cacheManager;
            _tempFileCacheManager = tempFileCacheManager;
            _backgroundJobManager = backgroundJobManager;
            _profileImageServiceFactory = profileImageServiceFactory;
        }

        [DisableAuditing]
        public async Task<CurrentUserProfileEditDto> GetCurrentUserProfileForEdit()
        {
            var user = await GetCurrentUserAsync();
            var userProfileEditDto = ObjectMapper.Map<CurrentUserProfileEditDto>(user);

            if (Clock.SupportsMultipleTimezone)
            {
                userProfileEditDto.Timezone = await SettingManager.GetSettingValueAsync(TimingSettingNames.TimeZone);

                var defaultTimeZoneId =
                    await _timeZoneService.GetDefaultTimezoneAsync(SettingScopes.User, KontecgSession.CompanyId);
                if (userProfileEditDto.Timezone == defaultTimeZoneId) userProfileEditDto.Timezone = string.Empty;
            }

            return userProfileEditDto;
        }

        public async Task UpdateCurrentUserProfile(CurrentUserProfileEditDto input)
        {
            var user = await GetCurrentUserAsync();

            if (user.PhoneNumber != input.PhoneNumber)
                input.IsPhoneNumberConfirmed = false;
            else if (user.IsPhoneNumberConfirmed) input.IsPhoneNumberConfirmed = true;

            ObjectMapper.Map(input, user);
            CheckErrors(await UserManager.UpdateAsync(user));

            if (Clock.SupportsMultipleTimezone)
            {
                if (input.Timezone.IsNullOrEmpty())
                {
                    var defaultValue =
                        await _timeZoneService.GetDefaultTimezoneAsync(SettingScopes.User, KontecgSession.CompanyId);
                    await SettingManager.ChangeSettingForUserAsync(KontecgSession.ToUserIdentifier(),
                        TimingSettingNames.TimeZone, defaultValue);
                }
                else
                {
                    await SettingManager.ChangeSettingForUserAsync(KontecgSession.ToUserIdentifier(),
                        TimingSettingNames.TimeZone, input.Timezone);
                }
            }
        }

        public async Task ChangePassword(ChangePasswordInput input)
        {
            await UserManager.InitializeOptionsAsync(KontecgSession.CompanyId);

            var user = await GetCurrentUserAsync();
            if (await UserManager.CheckPasswordAsync(user, input.CurrentPassword))
                CheckErrors(await UserManager.ChangePasswordAsync(user, input.NewPassword));
            else
                CheckErrors(IdentityResult.Failed(new IdentityError
                {
                    Description = "Incorrect password."
                }));
        }

        public async Task UpdateProfilePicture(UpdateProfilePictureInput input)
        {
            byte[] byteArray;

            var imageBytes = _tempFileCacheManager.GetFile(input.FileToken);

            if (imageBytes == null)
                throw new UserFriendlyException("There is no such image file with the token: " + input.FileToken);

            using (var bmpImage = new Bitmap(new MemoryStream(imageBytes)))
            {
                var width = input.Width == 0 || input.Width > bmpImage.Width ? bmpImage.Width : input.Width;
                var height = input.Height == 0 || input.Height > bmpImage.Height ? bmpImage.Height : input.Height;
                var bmCrop = bmpImage.Clone(new Rectangle(input.X, input.Y, width, height), bmpImage.PixelFormat);

                using (var stream = new MemoryStream())
                {
                    bmCrop.Save(stream, bmpImage.RawFormat);
                    byteArray = stream.ToArray();
                }
            }

            if (byteArray.Length > KontecgCoreConsts.ProfilePictureBytesValue)
                throw new UserFriendlyException(L("ResizedProfilePicture_Warn_SizeLimit",
                    KontecgCoreConsts.ResizedMaxProfilePictureBytesValue));

            var user = await UserManager.GetUserByIdAsync(KontecgSession.GetUserId());

            if (user.ProfilePictureId.HasValue) await _binaryObjectManager.DeleteAsync(user.ProfilePictureId.Value);

            var storedFile = new BinaryObject(KontecgSession.CompanyId, byteArray,
                $"Profile picture of user {KontecgSession.UserId}. {DateTime.UtcNow}");
            await _binaryObjectManager.SaveAsync(storedFile);

            user.ProfilePictureId = storedFile.Id;
        }

        [KontecgAllowAnonymous]
        public async Task<GetPasswordComplexitySettingOutput> GetPasswordComplexitySetting()
        {
            var passwordComplexitySetting = new PasswordComplexitySetting
            {
                RequireDigit =
                    await SettingManager.GetSettingValueAsync<bool>(KontecgBaselineSettingNames.UserManagement
                        .PasswordComplexity.RequireDigit),
                RequireLowercase =
                    await SettingManager.GetSettingValueAsync<bool>(KontecgBaselineSettingNames.UserManagement
                        .PasswordComplexity.RequireLowercase),
                RequireNonAlphanumeric =
                    await SettingManager.GetSettingValueAsync<bool>(KontecgBaselineSettingNames.UserManagement
                        .PasswordComplexity.RequireNonAlphanumeric),
                RequireUppercase =
                    await SettingManager.GetSettingValueAsync<bool>(KontecgBaselineSettingNames.UserManagement
                        .PasswordComplexity.RequireUppercase),
                RequiredLength =
                    await SettingManager.GetSettingValueAsync<int>(KontecgBaselineSettingNames.UserManagement
                        .PasswordComplexity
                        .RequiredLength)
            };

            return new GetPasswordComplexitySettingOutput
            {
                Setting = passwordComplexitySetting
            };
        }

        [DisableAuditing]
        public async Task<GetProfilePictureOutput> GetProfilePicture()
        {
            using var profileImageService = await _profileImageServiceFactory.GetAsync(KontecgSession.ToUserIdentifier());
            var profilePictureContent = await profileImageService.Object.GetProfilePictureContentForUserAsync(
                KontecgSession.ToUserIdentifier()
            );

            return new GetProfilePictureOutput(profilePictureContent);
        }

        [KontecgAllowAnonymous]
        public async Task<GetProfilePictureOutput> GetProfilePictureByUserName(string username)
        {
            var user = await UserManager.FindByNameAsync(username);
            if (user == null) return new GetProfilePictureOutput(string.Empty);

            var userIdentifier = new UserIdentifier(KontecgSession.CompanyId, user.Id);
            using var profileImageService = await _profileImageServiceFactory.GetAsync(userIdentifier);
            var profileImage = await profileImageService.Object.GetProfilePictureContentForUserAsync(userIdentifier);
            return new GetProfilePictureOutput(profileImage);
        }

        [KontecgAllowAnonymous]
        public async Task<GetProfilePictureOutput> GetProfilePictureByUser(long userId)
        {
            var userIdentifier = new UserIdentifier(KontecgSession.CompanyId, userId);
            using var profileImageService = await _profileImageServiceFactory.GetAsync(userIdentifier);
            var profileImage = await profileImageService.Object.GetProfilePictureContentForUserAsync(userIdentifier);
            return new GetProfilePictureOutput(profileImage);
        }

        public async Task ChangeLanguage(ChangeUserLanguageDto input)
        {
            await SettingManager.ChangeSettingForUserAsync(
                KontecgSession.ToUserIdentifier(),
                LocalizationSettingNames.DefaultLanguage,
                input.LanguageName
            );
        }

        private async Task<byte[]> GetProfilePictureByIdOrNullAsync(Guid profilePictureId)
        {
            var file = await _binaryObjectManager.GetOrNullAsync(profilePictureId);

            return file?.Bytes;
        }

        private async Task<GetProfilePictureOutput> GetProfilePictureByIdInternalAsync(Guid profilePictureId)
        {
            var bytes = await GetProfilePictureByIdOrNullAsync(profilePictureId);
            if (bytes == null) return new GetProfilePictureOutput(string.Empty);

            return new GetProfilePictureOutput(Convert.ToBase64String(bytes));
        }
    }
}
