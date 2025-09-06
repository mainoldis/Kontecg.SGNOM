using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kontecg.Authorization.Roles;
using Kontecg.Baseline.Configuration;
using Kontecg.Configuration;
using Kontecg.Domain.Repositories;
using Kontecg.Domain.Uow;
using Kontecg.Localization;
using Kontecg.Organizations;
using Kontecg.Runtime.Caching;
using Kontecg.Runtime.Security;
using Kontecg.Threading;
using Kontecg.UI;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Kontecg.Authorization.Users
{
    public class UserManager : KontecgUserManager<Role, User>
    {
        private readonly ILocalizationManager _localizationManager;
        private readonly ISettingManager _settingManager;
        private readonly IUnitOfWorkManager _unitOfWorkManager;

        public UserManager(
            KontecgRoleManager<Role, User> roleManager, 
            KontecgUserStore<Role, User> userStore, 
            IOptions<IdentityOptions> optionsAccessor, 
            IPasswordHasher<User> passwordHasher, 
            IEnumerable<IUserValidator<User>> userValidators, 
            IEnumerable<IPasswordValidator<User>> passwordValidators, 
            ILookupNormalizer keyNormalizer, 
            IdentityErrorDescriber errors, 
            IServiceProvider services, 
            ILogger<UserManager<User>> logger, 
            IPermissionManager permissionManager, 
            IUnitOfWorkManager unitOfWorkManager, 
            ICacheManager cacheManager, 
            IRepository<OrganizationUnit, long> organizationUnitRepository, 
            IRepository<UserOrganizationUnit, long> userOrganizationUnitRepository, 
            ISettingManager settingManager, IRepository<UserLogin, long> userLoginRepository, 
            ILocalizationManager localizationManager) 
            : base(
                roleManager, 
                userStore, 
                optionsAccessor, 
                passwordHasher, 
                userValidators, 
                passwordValidators, 
                keyNormalizer, 
                errors, 
                services, 
                logger, 
                permissionManager, 
                unitOfWorkManager, 
                cacheManager, 
                organizationUnitRepository, 
                userOrganizationUnitRepository, 
                settingManager, 
                userLoginRepository)
        {
            _localizationManager = localizationManager;
            _settingManager = settingManager;
            _unitOfWorkManager = unitOfWorkManager;
        }

        [UnitOfWork]
        public virtual async Task<User> GetUserOrNullAsync(UserIdentifier userIdentifier)
        {
            return await FindByIdAsync(userIdentifier.UserId.ToString());
        }

        public User GetUserOrNull(UserIdentifier userIdentifier)
        {
            return AsyncHelper.RunSync(() => GetUserOrNullAsync(userIdentifier));
        }

        public async Task<User> GetUserAsync(UserIdentifier userIdentifier)
        {
            var user = await GetUserOrNullAsync(userIdentifier);
            if (user == null) throw new Exception("There is no user: " + userIdentifier);

            return user;
        }
        public User GetUser(UserIdentifier userIdentifier)
        {
            return AsyncHelper.RunSync(() => GetUserAsync(userIdentifier));
        }

        public override Task<IdentityResult> SetRolesAsync(User user, string[] roleNames)
        {
            if (user.Name == KontecgUserBase.AdminUserName && !roleNames.Contains(StaticRoleNames.Admin))
                throw new UserFriendlyException(L("AdminRoleCannotRemoveFromAdminUser"));

            return base.SetRolesAsync(user, roleNames);
        }

        public override async Task SetGrantedPermissionsAsync(User user, IEnumerable<Permission> permissions)
        {
            var enumerable = permissions as Permission[] ?? permissions.ToArray();
            CheckPermissionsToUpdate(user, enumerable);

            await base.SetGrantedPermissionsAsync(user, enumerable);
        }

        public async Task<string> CreateRandomPasswordAsync()
        {
            var passwordComplexitySetting = new PasswordComplexitySetting
            {
                RequireDigit =
                    await _settingManager.GetSettingValueAsync<bool>(KontecgBaselineSettingNames.UserManagement
                        .PasswordComplexity.RequireDigit),
                RequireLowercase = await _settingManager.GetSettingValueAsync<bool>(KontecgBaselineSettingNames
                    .UserManagement.PasswordComplexity.RequireLowercase),
                RequireNonAlphanumeric = await _settingManager.GetSettingValueAsync<bool>(KontecgBaselineSettingNames
                    .UserManagement.PasswordComplexity.RequireNonAlphanumeric),
                RequireUppercase = await _settingManager.GetSettingValueAsync<bool>(KontecgBaselineSettingNames
                    .UserManagement.PasswordComplexity.RequireUppercase),
                RequiredLength = await _settingManager.GetSettingValueAsync<int>(KontecgBaselineSettingNames
                    .UserManagement.PasswordComplexity.RequiredLength)
            };

            var upperCaseLetters = "ABCDEFGHJKLMNOPQRSTUVWXYZ";
            var lowerCaseLetters = "abcdefghijkmnopqrstuvwxyz";
            var digits = "0123456789";
            var nonAlphanumerics = "!@$?_-";

            string[] randomChars =
            {
                upperCaseLetters,
                lowerCaseLetters,
                digits,
                nonAlphanumerics
            };

            var rand = new Random(Environment.TickCount);
            var chars = new List<char>();

            if (passwordComplexitySetting.RequireUppercase)
                chars.Insert(rand.Next(0, chars.Count),
                    upperCaseLetters[rand.Next(0, upperCaseLetters.Length)]);

            if (passwordComplexitySetting.RequireLowercase)
                chars.Insert(rand.Next(0, chars.Count),
                    lowerCaseLetters[rand.Next(0, lowerCaseLetters.Length)]);

            if (passwordComplexitySetting.RequireDigit)
                chars.Insert(rand.Next(0, chars.Count),
                    digits[rand.Next(0, digits.Length)]);

            if (passwordComplexitySetting.RequireNonAlphanumeric)
                chars.Insert(rand.Next(0, chars.Count),
                    nonAlphanumerics[rand.Next(0, nonAlphanumerics.Length)]);

            for (var i = chars.Count; i < passwordComplexitySetting.RequiredLength; i++)
            {
                var rcs = randomChars[rand.Next(0, randomChars.Length)];
                chars.Insert(rand.Next(0, chars.Count),
                    rcs[rand.Next(0, rcs.Length)]);
            }

            return new string(chars.ToArray());
        }

        private void CheckPermissionsToUpdate(User user, IEnumerable<Permission> permissions)
        {
            if (user.Name == KontecgUserBase.AdminUserName &&
                (!permissions.Any(p => p.Name == PermissionNames.AdministrationRolesEdit) ||
                 !permissions.Any(p => p.Name == PermissionNames.AdministrationUsersChangePermissions)))
                throw new UserFriendlyException(L("YouCannotRemoveUserRolePermissionsFromAdminUser"));
        }

        private new string L(string name)
        {
            return _localizationManager.GetString(KontecgCoreConsts.LocalizationSourceName, name);
        }
    }
}
