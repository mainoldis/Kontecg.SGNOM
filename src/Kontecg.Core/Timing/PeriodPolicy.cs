using System;
using System.Collections.Generic;
using System.Linq;
using Kontecg.Accounting;
using Kontecg.Domain.Repositories;
using Kontecg.Workflows;
using System.Threading.Tasks;
using Itenso.TimePeriod;
using Kontecg.Configuration;
using Kontecg.Threading;

namespace Kontecg.Timing
{
    public class PeriodPolicy : KontecgCoreServiceBase, IPeriodPolicy
    {
        private readonly IRepository<AccountingDocument> _accountingDocumentRepository;
        private readonly IRepository<Period> _periodRepository;
        private readonly ISettingManager _settingManager;

        /// <inheritdoc />
        public PeriodPolicy(
            IRepository<AccountingDocument> accountingDocumentRepository, 
            IRepository<Period> periodRepository, 
            ISettingManager settingManager)
        {
            _accountingDocumentRepository = accountingDocumentRepository;
            _settingManager = settingManager;
            _periodRepository = periodRepository;
        }

        /// <param name="periodInfo"></param>
        /// <inheritdoc />
        public async Task<bool> CheckOpenModeAsync(PeriodInfo periodInfo)
        {
            Check.NotNull(periodInfo, nameof(periodInfo));
            var openMode = Enum.Parse<PeriodOpenMode>(await _settingManager.GetSettingValueAsync(AppSettings.Timing.OpenModeManagement), true);
            List<Period> openedPeriods;

            switch (openMode)
            {
                case PeriodOpenMode.None:
                    if ((await _periodRepository.GetAllAsync()).Count(d => d.ReferenceGroup == periodInfo.ReferenceGroup && d.Status == PeriodStatus.Opened) > 0)
                        return false;
                    break;
                case PeriodOpenMode.CurrentMonth:
                    openedPeriods = (await _periodRepository.GetAllAsync())
                                  .Where(d => d.ReferenceGroup == periodInfo.ReferenceGroup &&
                                              d.Status == PeriodStatus.Opened)
                                  .ToList();

                    return periodInfo.Year == Clock.Now.Year && periodInfo.Month == (YearMonth) Clock.Now.Month &&
                           !openedPeriods.Exists(p =>
                                p.ToTimePeriod().GetRelation(periodInfo.ToTimePeriod()) == PeriodRelation.After ||
                                (p.Year == periodInfo.Year && p.Month == periodInfo.Month) &&
                                p.ReferenceGroup == periodInfo.ReferenceGroup);

                case PeriodOpenMode.LastMonth:
                    TimeTool.AddMonth(Clock.Now.Year, (YearMonth) Clock.Now.Month, -1, out var year, out var month);
                    openedPeriods = (await _periodRepository.GetAllAsync())
                                    .Where(d => d.ReferenceGroup == periodInfo.ReferenceGroup &&
                                                d.Status == PeriodStatus.Opened)
                                    .ToList();
                    return periodInfo.Year == year && periodInfo.Month == month &&
                           !openedPeriods.Exists(p =>
                                p.ToTimePeriod().GetRelation(periodInfo.ToTimePeriod()) == PeriodRelation.After ||
                                (p.Year == year && p.Month == month) &&
                                p.ReferenceGroup == periodInfo.ReferenceGroup);
                case PeriodOpenMode.Any:
                    return true;
                default:
                    throw new ArgumentOutOfRangeException(nameof(openMode));
            }

            return true;
        }

        /// <param name="periodInfo"></param>
        /// <inheritdoc />
        public bool CheckOpenMode(PeriodInfo periodInfo)
        {
            return AsyncHelper.RunSync(() => CheckOpenModeAsync(periodInfo));
        }

        /// <inheritdoc />
        public async Task<bool> CheckPendingDocumentsPolicyAsync(int? periodId)
        {
            int resultCount;
            if (periodId.HasValue)
                resultCount = await _accountingDocumentRepository.CountAsync(d =>
                    d.PeriodId == periodId && d.Review != ReviewStatus.Confirmed && d.Review != ReviewStatus.Canceled);
            else
                resultCount = await _accountingDocumentRepository.CountAsync(d =>
                    d.Review != ReviewStatus.Confirmed && d.Review != ReviewStatus.Canceled);

            return resultCount > 0;
        }

        /// <inheritdoc />
        public bool CheckPendingDocumentsPolicy(int? periodId)
        {
            return AsyncHelper.RunSync(() => CheckPendingDocumentsPolicyAsync(periodId));
        }
    }
}