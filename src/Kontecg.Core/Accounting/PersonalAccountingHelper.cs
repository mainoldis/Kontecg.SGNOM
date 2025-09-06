using Castle.Core.Logging;
using Kontecg.Dependency;
using Kontecg.Domain.Uow;
using Kontecg.Runtime.Session;
using System;
using System.Threading.Tasks;
using System.Transactions;

namespace Kontecg.Accounting
{
    public class PersonalAccountingHelper : IPersonalAccountingHelper, ITransientDependency
    {
        private readonly IPersonalAccountingInfoProvider _personalAccountingInfoProvider;
        private readonly IUnitOfWorkManager _unitOfWorkManager;

        public PersonalAccountingHelper(
            IPersonalAccountingInfoProvider personalAccountingInfoProvider,
            IUnitOfWorkManager unitOfWorkManager)
        {
            _personalAccountingInfoProvider = personalAccountingInfoProvider;
            _unitOfWorkManager = unitOfWorkManager;

            KontecgSession = NullKontecgSession.Instance;
            Logger = NullLogger.Instance;
            PersonalAccountingInfoStore = SimplePersonalAccountingInfoStore.Instance;
        }

        public ILogger Logger { get; set; }

        public IKontecgSession KontecgSession { get; set; }

        public IPersonalAccountingInfoStore PersonalAccountingInfoStore { get; set; }

        public PersonalAccountingInfo[] CreatePersonalAccountingInfo()
        {
            var accountingInfo = Array.Empty<PersonalAccountingInfo>();

            try
            {
                accountingInfo = _personalAccountingInfoProvider.GetPersonalAccountingInformation(KontecgSession.CompanyId);
            }
            catch (Exception ex)
            {
                Logger.Warn(ex.ToString(), ex);
            }

            return accountingInfo;
        }

        /// <inheritdoc />
        public async Task<PersonalAccountingInfo[]> CreatePersonalAccountingInfoAsync()
        {
            var accountingInfo = Array.Empty<PersonalAccountingInfo>();

            try
            {
                accountingInfo = await _personalAccountingInfoProvider.GetPersonalAccountingInformationAsync(KontecgSession.CompanyId);
            }
            catch (Exception ex)
            {
                Logger.Warn(ex.ToString(), ex);
            }

            return accountingInfo;
        }

        public void Save(PersonalAccountingInfo accountingInfo)
        {
            using var uow = _unitOfWorkManager.Begin(TransactionScopeOption.Suppress);
            PersonalAccountingInfoStore.Save(accountingInfo);
            uow.Complete();
        }

        public async Task SaveAsync(PersonalAccountingInfo accountingInfo)
        {
            using var uow = _unitOfWorkManager.Begin(TransactionScopeOption.Suppress);
            await PersonalAccountingInfoStore.SaveAsync(accountingInfo);
            await uow.CompleteAsync();
        }
    }
}
