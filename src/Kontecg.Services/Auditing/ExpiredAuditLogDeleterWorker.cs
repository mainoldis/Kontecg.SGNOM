using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Kontecg.Configuration;
using Kontecg.Dependency;
using Kontecg.Domain.Repositories;
using Kontecg.Domain.Uow;
using Kontecg.EFCore.EFPlus;
using Kontecg.Logging;
using Kontecg.MultiCompany;
using Kontecg.Threading;
using Kontecg.Threading.BackgroundWorkers;
using Kontecg.Threading.Timers;
using Kontecg.Timing;

namespace Kontecg.Auditing
{
    public class ExpiredAuditLogDeleterWorker : PeriodicBackgroundWorkerBase, ISingletonDependency
    {
        /// <summary>
        /// Set this field to true if you want to enable ExpiredAuditLogDeleterWorker.
        /// Be careful, If you enable this, all expired logs will be permanently deleted.
        /// </summary>
        public bool IsEnabled { get; }

        private const int CheckPeriodAsMilliseconds = 1 * 1000 * 60 * 3; // 3min
        private const int MaxDeletionCount = 10000;
        private readonly IRepository<AuditLog, long> _auditLogRepository;

        private readonly TimeSpan _logExpireTime = TimeSpan.FromDays(7);
        private readonly IRepository<Company> _companyRepository;
        //private readonly IExpiredAndDeletedAuditLogBackupService _expiredAndDeletedAuditLogBackupService;

        public ExpiredAuditLogDeleterWorker(
            KontecgTimer timer,
            IRepository<AuditLog, long> auditLogRepository,
            IRepository<Company> companyRepository,
            //IExpiredAndDeletedAuditLogBackupService expiredAndDeletedAuditLogBackupService,
            IAppConfigurationAccessor configurationAccessor
        )
            : base(timer)
        {
            _auditLogRepository = auditLogRepository;
            _companyRepository = companyRepository;
            //_expiredAndDeletedAuditLogBackupService = expiredAndDeletedAuditLogBackupService;

            LocalizationSourceName = KontecgCoreConsts.LocalizationSourceName;

            Timer.Period = CheckPeriodAsMilliseconds;
            Timer.RunOnStart = true;
            IsEnabled = configurationAccessor.Configuration["App:AuditLog:AutoDeleteExpiredLogs:IsEnabled"] == true.ToString();
        }

        protected override void DoWork()
        {
            if (!IsEnabled)
                return;

            var expireDate = Clock.Now - _logExpireTime;

            List<int> companyIds;
            using (var uow = UnitOfWorkManager.Begin())
            {
                companyIds = _companyRepository.GetAll()
                    .Where(t => !string.IsNullOrEmpty(t.ConnectionString))
                    .Select(t => t.Id)
                    .ToList();

                uow.Complete();
            }

            DeleteAuditLogsOnHostDatabase(expireDate);

            foreach (var companyId in companyIds) DeleteAuditLogsOnCompanyDatabase(companyId, expireDate);
        }

        protected virtual void DeleteAuditLogsOnHostDatabase(DateTime expireDate)
        {
            try
            {
                using var uow = UnitOfWorkManager.Begin();
                using (CurrentUnitOfWork.SetCompanyId(null))
                {
                    using (CurrentUnitOfWork.DisableFilter(KontecgDataFilters.MayHaveCompany))
                    {
                        DeleteAuditLogs(expireDate);
                        uow.Complete();
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Log(LogSeverity.Error, "An error occurred while deleting audit logs on host database", e);
            }
        }

        protected virtual void DeleteAuditLogsOnCompanyDatabase(int companyId, DateTime expireDate)
        {
            try
            {
                using var uow = UnitOfWorkManager.Begin();
                using (CurrentUnitOfWork.SetCompanyId(companyId))
                {
                    DeleteAuditLogs(expireDate);
                    uow.Complete();
                }
            }
            catch (Exception e)
            {
                Logger.Log(LogSeverity.Error,
                    $"An error occurred while deleting audit log for company. CompanyId: {companyId}", e);
            }
        }

        private void DeleteAuditLogs(DateTime expireDate)
        {
            var expiredEntryCount = _auditLogRepository.LongCount(l => l.ExecutionTime < expireDate);

            if (expiredEntryCount == 0) return;

            if (expiredEntryCount > MaxDeletionCount)
            {
                var deleteStartId = _auditLogRepository.GetAll().OrderBy(l => l.Id).Skip(MaxDeletionCount)
                    .Select(x => x.Id).First();

                BatchDelete(l => l.Id < deleteStartId);
            }
            else
            {
                BatchDelete(l => l.ExecutionTime < expireDate);
            }
        }

        private void BatchDelete(Expression<Func<AuditLog, bool>> expression)
        {
            //if (_expiredAndDeletedAuditLogBackupService.CanBackup())
            //{
            //    var auditLogs = _auditLogRepository.GetAll().AsNoTracking().Where(expression).ToList();
            //    _expiredAndDeletedAuditLogBackupService.Backup(auditLogs);
            //}

            //will not delete the logs from database if backup operation throws an exception
            AsyncHelper.RunSync(() => _auditLogRepository.BatchDeleteAsync(expression));
        }
    }
}
