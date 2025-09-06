using Kontecg.Dependency;
using Kontecg.Domain.Repositories;
using Kontecg.Domain.Uow;
using Kontecg.Threading.BackgroundWorkers;
using Kontecg.Threading.Timers;
using Kontecg.Timing;
using System;
using Kontecg.Configuration;

namespace Kontecg.Authorization.Users
{
    /// <summary>
    /// A sample background worker to make a user passive if he/she did not login in last 30 days.
    /// </summary>
    public class MakeInactiveUsersPassiveWorker : PeriodicBackgroundWorkerBase, ISingletonDependency
    {
        private const int CheckPeriodAsMilliseconds = 1 * 60 * 60 * 1000 * 24; //1 day
        private readonly IRepository<User, long> _userRepository;

        public MakeInactiveUsersPassiveWorker(
            KontecgTimer timer, 
            IRepository<User, long> userRepository) 
            : base(timer)
        {
            _userRepository = userRepository;
            Timer.Period = CheckPeriodAsMilliseconds;
            Timer.RunOnStart = true;

            LocalizationSourceName = KontecgCoreConsts.LocalizationSourceName;
        }

        protected override void DoWork()
        {
            using var uow = UnitOfWorkManager.Begin();
            using (CurrentUnitOfWork.DisableFilter(KontecgDataFilters.MayHaveCompany))
            {
                var oneMonthAgo = Clock.Now.Subtract(TimeSpan.FromDays(GetUserExpirationDayCount()));

                var inactiveUsers = _userRepository.GetAllList(u =>
                    u.IsActive &&
                    u.UserName != KontecgUserBase.AdminUserName &&
                    ((u.LastLoginTime < oneMonthAgo && u.LastLoginTime != null) || (u.CreationTime < oneMonthAgo && u.LastLoginTime == null))
                );

                foreach (var inactiveUser in inactiveUsers)
                {
                    inactiveUser.IsActive = false;
                    Logger.Info(inactiveUser + " made passive since he/she did not login in last 30 days.");
                }

                CurrentUnitOfWork.SaveChanges();
            }
            uow.Complete();
        }

        private int GetUserExpirationDayCount()
        {
            return SettingManager.GetSettingValue<int>(AppSettings.UserManagement.UserExpirationDayCount);
        }
    }
}
