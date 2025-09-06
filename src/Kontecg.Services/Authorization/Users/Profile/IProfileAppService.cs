using System.Threading.Tasks;
using Kontecg.Application.Services;
using Kontecg.Authorization.Users.Dto;
using Kontecg.Authorization.Users.Profile.Dto;

namespace Kontecg.Authorization.Users.Profile
{
    public interface IProfileAppService : IApplicationService
    {
        Task<CurrentUserProfileEditDto> GetCurrentUserProfileForEdit();

        Task UpdateCurrentUserProfile(CurrentUserProfileEditDto input);

        Task ChangePassword(ChangePasswordInput input);

        Task UpdateProfilePicture(UpdateProfilePictureInput input);

        Task<GetPasswordComplexitySettingOutput> GetPasswordComplexitySetting();

        Task<GetProfilePictureOutput> GetProfilePicture();

        Task<GetProfilePictureOutput> GetProfilePictureByUser(long userId);

        Task<GetProfilePictureOutput> GetProfilePictureByUserName(string username);

        Task ChangeLanguage(ChangeUserLanguageDto input);
    }
}
