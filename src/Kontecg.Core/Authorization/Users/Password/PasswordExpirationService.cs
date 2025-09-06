using System;
using System.Linq;
using Kontecg.Configuration;
using Kontecg.Domain.Repositories;
using Kontecg.MultiCompany;
using Kontecg.Timing;

namespace Kontecg.Authorization.Users.Password
{
    public class PasswordExpirationService : KontecgCoreDomainServiceBase, IPasswordExpirationService
    {
        private readonly IRepository<RecentPassword, Guid> _recentPasswordRepository;
        private readonly IUserRepository _userRepository;
        private readonly IRepository<Company> _companyRepository;

        public PasswordExpirationService(
            IRepository<RecentPassword, Guid> recentPasswordRepository,
            IUserRepository userRepository,
            IRepository<Company> companyRepository)
        {
            _recentPasswordRepository = recentPasswordRepository;
            _userRepository = userRepository;
            _companyRepository = companyRepository;
        }

        public void ForcePasswordExpiredUsersToChangeTheirPassword()
        {
            var isEnabled = SettingManager.GetSettingValueForApplication<bool>(
                AppSettings.UserManagement.Password.EnablePasswordExpiration
            );

            if (!isEnabled)
            {
                return;
            }

            // check host users 
            ForcePasswordExpiredUsersToChangeTheirPasswordInternal(null);

            // check companies
            var companyIds = _companyRepository.GetAll().Select(company => company.Id).ToList();
            foreach (var companyId in companyIds)
            {
                ForcePasswordExpiredUsersToChangeTheirPasswordInternal(companyId);
            }
        }

        private void ForcePasswordExpiredUsersToChangeTheirPasswordInternal(int? companyId)
        {
            using (CurrentUnitOfWork.SetCompanyId(companyId))
            {
                var passwordExpirationDayCount = SettingManager.GetSettingValueForApplication<int>(
                    AppSettings.UserManagement.Password.PasswordExpirationDayCount
                );

                var passwordExpireDate = Clock.Now.AddDays(-passwordExpirationDayCount).ToUniversalTime();

                // TODO: Query seems wrong !
                var passwordExpiredUsers = _userRepository.GetPasswordExpiredUserIds(passwordExpireDate);

                var separationCount = 1000;
                var separationLoopCount = passwordExpiredUsers.Count / separationCount + 1;

                for (int i = 0; i < separationLoopCount; i++)
                {
                    var userIdsToUpdate = passwordExpiredUsers.Skip(i * separationCount).Take(separationCount).ToList();
                    if (userIdsToUpdate.Count > 0)
                    {
                        _userRepository.UpdateUsersToChangePasswordOnNextLogin(userIdsToUpdate);
                    }
                }
            }
        }
    }
}
