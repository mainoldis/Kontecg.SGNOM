using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Itenso.TimePeriod;
using Kontecg.Configuration.Startup;
using Kontecg.Domain.Repositories;
using Kontecg.Domain.Uow;
using Kontecg.Events.Bus;
using Kontecg.Extensions;
using Kontecg.Localization;
using Kontecg.MultiCompany;
using Kontecg.Notifications;
using Kontecg.Runtime.Session;
using Kontecg.Threading;

namespace Kontecg.Timing
{
    public class PeriodManager : KontecgCoreDomainServiceBase
    {
        private readonly IRepository<Period> _periodRepository;
        private readonly IAppNotifier _appNotifier;
        private readonly IPeriodPolicy _periodPolicy;
        private readonly CompanyManager _companyManager;
        private readonly IMultiCompanyConfig _multiCompanyConfig;
        private readonly KontecgPeriodResultTypeHelper _kontecgPeriodResultTypeHelper;

        public PeriodManager(
            ITimeCalendarProvider timeCalendarProvider,
            IRepository<Period> periodRepository, 
            CompanyManager companyManager, 
            IMultiCompanyConfig multiCompanyConfig,
            IAppNotifier appNotifier, 
            IPeriodPolicy periodPolicy, 
            KontecgPeriodResultTypeHelper kontecgPeriodResultTypeHelper)
        {
            _periodRepository = periodRepository;
            _appNotifier = appNotifier;
            _periodPolicy = periodPolicy;
            _kontecgPeriodResultTypeHelper = kontecgPeriodResultTypeHelper;
            _multiCompanyConfig = multiCompanyConfig;
            _companyManager = companyManager;

            Calendar = timeCalendarProvider.GetCalendar(false);
            KontecgSession = NullKontecgSession.Instance;
            EventBus = NullEventBus.Instance;
        }

        public IKontecgSession KontecgSession { get; set; }

        public IEventBus EventBus { get; set; }

        public ITimeCalendar Calendar { get; set; }

        public virtual async Task<KontecgPeriodResult<Company>> OpenAsync(string referenceGroup, int year, YearMonth month)
        {
            var result = await CreateOperationInternalAsync(PeriodInfo.Create(referenceGroup, year, month, Calendar));

            if (result.Result == KontecgPeriodResultType.Success)
            {
                await EventBus.TriggerAsync(this, new PeriodStatusChangedEventData(result.Period));
                await GetNotifiedAsync(result.Period);
            }

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="referenceGroup"></param>
        /// <param name="startingDate"></param>
        /// <param name="finishingDate"></param>
        /// <returns></returns>
        public virtual async Task<KontecgPeriodResult<Company>> OpenAsync(string referenceGroup, DateTime startingDate, DateTime finishingDate)
        {
            return await UnitOfWorkManager.WithUnitOfWorkAsync(async () =>
            {
                var result = await CreateOperationInternalAsync(PeriodInfo.Create(referenceGroup, startingDate, finishingDate, Calendar));

                if (result.Result == KontecgPeriodResultType.Success)
                {
                    await EventBus.TriggerAsync(this, new PeriodStatusChangedEventData(result.Period));
                    await GetNotifiedAsync(result.Period);
                }

                return result;
            });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="referenceGroup"></param>
        /// <returns></returns>
        public virtual async Task<PeriodInfo> GetCurrentPeriodAsync(string referenceGroup)
        {
            return await UnitOfWorkManager.WithUnitOfWorkAsync(async () =>
            {
                using (UnitOfWorkManager.Current.SetCompanyId(KontecgSession.CompanyId))
                {
                    var period = (await _periodRepository.GetAllAsync()).OrderByDescending(f => f.Until).FirstOrDefault(p => p.Status == PeriodStatus.Opened && p.ReferenceGroup == referenceGroup);
                    if (period == null)
                    {
                        var periodResult = await OpenAsync(referenceGroup, Clock.Now.Year, (YearMonth) Clock.Now.Month);
                        if (periodResult.Result != KontecgPeriodResultType.Success)
                            throw _kontecgPeriodResultTypeHelper.CreateExceptionForFailedOperationAttempt(periodResult);
                        return periodResult.Period;
                    }

                    return period.ToPeriodInfo();
                }
            });
        }

        public virtual PeriodInfo GetCurrentPeriod(string referenceGroup)
        {
            return AsyncHelper.RunSync(() => GetCurrentPeriodAsync(referenceGroup));
        }

        public virtual async Task<IReadOnlyList<PeriodInfo>> GetCurrentPeriodAsync()
        {
            return await UnitOfWorkManager.WithUnitOfWorkAsync(async () =>
            {
                using (UnitOfWorkManager.Current.SetCompanyId(KontecgSession.CompanyId))
                {
                    var periods = await _periodRepository.GetAllListAsync(p => p.Status == PeriodStatus.Opened);
                    return new List<PeriodInfo>(periods.Select(p => p.ToPeriodInfo()));
                }
            });
        }

        public virtual IReadOnlyList<PeriodInfo> GetCurrentPeriod()
        {
            return AsyncHelper.RunSync(GetCurrentPeriodAsync);
        }

        protected virtual async Task GetNotifiedAsync(PeriodInfo periodInfo)
        {
            await _appNotifier.SendPeriodStatusNotificationAsync(periodInfo);
        }

        protected void GetNotified(PeriodInfo periodInfo)
        {
            AsyncHelper.RunSync(() => GetNotifiedAsync(periodInfo));
        }

        protected virtual async Task<KontecgPeriodResult<Company>> CreateOperationInternalAsync(PeriodInfo periodInfo)
        {
            return await UnitOfWorkManager.WithUnitOfWorkAsync(async () =>
            {
                using (UnitOfWorkManager.Current.SetCompanyId(KontecgSession.CompanyId))
                {
                    if (periodInfo == null) throw new ArgumentException("periodInfo");

                    if (periodInfo.ReferenceGroup.IsNullOrWhiteSpace())
                        return new KontecgPeriodResult<Company>(KontecgPeriodResultType.InvalidReferenceGroup, period: periodInfo);

                    var company = await GetCompanyAsync();
                    var exactPeriodExpression = new ExactPeriodSpecification(periodInfo).ToExpression();
                    Period period = await _periodRepository.FirstOrDefaultAsync(exactPeriodExpression);
                    if (period == null)
                    {
                        period = new Period(periodInfo.ReferenceGroup, periodInfo.Calendar, periodInfo.Since, periodInfo.Until)
                        {
                            Status = PeriodStatus.Opened
                        };

                        if (await _periodPolicy.CheckOpenModeAsync(period.ToPeriodInfo()))
                        {
                            if (await _periodPolicy.CheckPendingDocumentsPolicyAsync(null))
                            {
                                return new KontecgPeriodResult<Company>(KontecgPeriodResultType.PendingOperations, company,
                                    periodInfo);
                            }

                            await _periodRepository.InsertAsync(period);
                        }
                        else
                        {
                            var failedResult =
                                new KontecgPeriodResult<Company>(KontecgPeriodResultType.FailedForOtherReason, company,
                                    periodInfo);
                            failedResult.SetFailReason(new LocalizableString("FaultedOpenMode", KontecgCoreConsts.LocalizationSourceName));
                            return failedResult;
                        }
                    }
                    else
                    {
                        if (period.Status == PeriodStatus.Closed && !await _periodPolicy.CheckOpenModeAsync(period.ToPeriodInfo()))
                        {
                            var failedResult =
                                new KontecgPeriodResult<Company>(KontecgPeriodResultType.FailedForOtherReason, company,
                                    periodInfo);
                            failedResult.SetFailReason(new LocalizableString("FaultedOpenMode", KontecgCoreConsts.LocalizationSourceName));
                            return failedResult;
                        }

                        if (period.Status == PeriodStatus.Opened &&
                            await _periodPolicy.CheckPendingDocumentsPolicyAsync(period.Id))
                        {
                            return new KontecgPeriodResult<Company>(KontecgPeriodResultType.PendingOperations, company,
                                periodInfo);
                        }

                        period.Status = period.Status == PeriodStatus.Opened ? PeriodStatus.Closed : PeriodStatus.Opened;
                        await _periodRepository.UpdateAsync(period);
                    }

                    await UnitOfWorkManager.Current.SaveChangesAsync();

                    return await CreateOperationPeriodResultAsync(company, period.ToPeriodInfo());
                }
            });
        }

        protected virtual Task<KontecgPeriodResult<Company>> CreateOperationPeriodResultAsync(Company company = null, PeriodInfo period = null)
        {
            return Task.FromResult(new KontecgPeriodResult<Company>(company, period));
        }

        protected virtual async Task<Company> GetCompanyAsync()
        {
            var companyId = _multiCompanyConfig.IsEnabled
                ? KontecgSession.CompanyId ?? MultiCompanyConsts.DefaultCompanyId
                : MultiCompanyConsts.DefaultCompanyId;
            return await _companyManager.FindByIdAsync(companyId);
        }
    }
}
