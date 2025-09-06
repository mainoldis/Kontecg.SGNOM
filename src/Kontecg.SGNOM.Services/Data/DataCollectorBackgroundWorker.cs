using System.Threading.Tasks;
using Kontecg.Dependency;
using Kontecg.Domain.Uow;
using Kontecg.Threading.BackgroundWorkers;
using Kontecg.Threading.Timers;

namespace Kontecg.Data
{
    public class DataCollectorBackgroundWorker : AsyncPeriodicBackgroundWorkerBase, ISingletonDependency
    {
        private const int CheckPeriodAsMilliseconds = 1 * 60 * 60 * 1000; //1 hour
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly IDataCollectorService _dataCollectorService;

        public DataCollectorBackgroundWorker(
            KontecgAsyncTimer timer,
            IUnitOfWorkManager unitOfWorkManager,
            IDataCollectorService dataCollectorService)
            : base(timer)
        {
            _unitOfWorkManager = unitOfWorkManager;
            _dataCollectorService = dataCollectorService;

            Timer.Period = CheckPeriodAsMilliseconds;
            Timer.RunOnStart = true;

            LocalizationSourceName = SGNOMConsts.LocalizationSourceName;
        }

        protected override async Task DoWorkAsync()
        {
            await _unitOfWorkManager.WithUnitOfWorkAsync(_dataCollectorService.ForcePersonToChangeTheirDataAsync);
        }
    }
}
