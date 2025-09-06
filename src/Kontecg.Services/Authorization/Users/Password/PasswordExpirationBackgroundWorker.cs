using Kontecg.Dependency;
using Kontecg.Domain.Uow;
using Kontecg.Threading.BackgroundWorkers;
using Kontecg.Threading.Timers;

namespace Kontecg.Authorization.Users.Password
{
    public class PasswordExpirationBackgroundWorker : PeriodicBackgroundWorkerBase, ISingletonDependency
    {
        private const int CheckPeriodAsMilliseconds = 1 * 60 * 60 * 1000 * 24; //1 day

        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly IPasswordExpirationService _passwordExpirationService;

        public PasswordExpirationBackgroundWorker(
            KontecgTimer timer,
            IUnitOfWorkManager unitOfWorkManager,
            IPasswordExpirationService passwordExpirationService)
            : base(timer)
        {
            _unitOfWorkManager = unitOfWorkManager;
            _passwordExpirationService = passwordExpirationService;

            Timer.Period = CheckPeriodAsMilliseconds;
            Timer.RunOnStart = true;

            LocalizationSourceName = KontecgCoreConsts.LocalizationSourceName;
        }

        protected override void DoWork()
        {
            _unitOfWorkManager.WithUnitOfWork(_passwordExpirationService.ForcePasswordExpiredUsersToChangeTheirPassword);
        }
    }

}
