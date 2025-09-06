using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kontecg.Domain.Repositories;
using Kontecg.Domain.Uow;
using Kontecg.Linq;
using Kontecg.Runtime.Session;
using Kontecg.Threading;
using Kontecg.UI;
using Kontecg.Workflows;

namespace Kontecg.Accounting
{
    /// <summary>
    /// General purpose manager for control accounting documents
    /// </summary>
    public class AccountingManager : KontecgCoreDomainServiceBase
    {
        private readonly IAccountDefinitionRepository _accountDefinitionRepository;
        private readonly ICenterCostDefinitionRepository _centerCostDefinitionRepository;
        private readonly IExpenseItemDefinitionRepository _expenseItemDefinitionRepository;
        private readonly IDocumentDefinitionRepository _documentDefinitionRepository;
        private readonly IRepository<AccountingFunctionDefinition> _accountingFunctionDefinitionRepository;
        private readonly IRepository<AccountingClassifierDefinition> _accountingClassifierDefinitionRepository;

        public AccountingManager(
            IAccountDefinitionRepository accountDefinitionRepository,
            ICenterCostDefinitionRepository centerCostDefinitionRepository,
            IExpenseItemDefinitionRepository expenseItemDefinitionRepository,
            IDocumentDefinitionRepository documentDefinitionRepository,
            IRepository<AccountingFunctionDefinition> accountingFunctionDefinitionRepository, 
            IRepository<AccountingClassifierDefinition> accountingClassifierDefinitionRepository)
        {
            _accountDefinitionRepository = accountDefinitionRepository;
            _centerCostDefinitionRepository = centerCostDefinitionRepository;
            _expenseItemDefinitionRepository = expenseItemDefinitionRepository;
            _documentDefinitionRepository = documentDefinitionRepository;
            _accountingFunctionDefinitionRepository = accountingFunctionDefinitionRepository;
            _accountingClassifierDefinitionRepository = accountingClassifierDefinitionRepository;

            LocalizationSourceName = KontecgCoreConsts.LocalizationSourceName;
            AsyncQueryableExecuter = NullAsyncQueryableExecuter.Instance;
            KontecgSession = NullKontecgSession.Instance;
        }

        public IAsyncQueryableExecuter AsyncQueryableExecuter { get; set; }

        public IKontecgSession KontecgSession { get; set; }

        #region Get Methods

        public Task<List<AccountDefinition>> GetAllAccountDefinitionsAsync()
        {
            return UnitOfWorkManager.WithUnitOfWorkAsync(async () =>
            {
                using (UnitOfWorkManager.Current.SetCompanyId(KontecgSession.CompanyId))
                {
                    return await AsyncQueryableExecuter.ToListAsync(await _accountDefinitionRepository.GetAllAsync());
                }
            });
        }

        public List<AccountDefinition> GetAllAccountDefinitions()
        {
            return AsyncHelper.RunSync(GetAllAccountDefinitionsAsync);
        }

        public Task<List<CenterCostDefinition>> GetAllCenterCostDefinitionsAsync()
        {
            return UnitOfWorkManager.WithUnitOfWorkAsync(async () =>
            {
                using (UnitOfWorkManager.Current.SetCompanyId(KontecgSession.CompanyId))
                {
                    return await AsyncQueryableExecuter.ToListAsync(await _centerCostDefinitionRepository.GetAllIncludingAsync(c => c.AccountDefinition));
                }
            });
        }

        public List<CenterCostDefinition> GetAllCenterCostDefinitions()
        {
            return AsyncHelper.RunSync(GetAllCenterCostDefinitionsAsync);
        }

        public Task<List<ExpenseItemDefinition>> GetAllExpenseItemDefinitionsAsync()
        {
            return UnitOfWorkManager.WithUnitOfWorkAsync(async () =>
            {
                using (UnitOfWorkManager.Current.SetCompanyId(KontecgSession.CompanyId))
                {
                    return await AsyncQueryableExecuter.ToListAsync(await _expenseItemDefinitionRepository.GetAllIncludingAsync(c => c.CenterCostDefinition));
                }
            });
        }

        public List<ExpenseItemDefinition> GetAllExpenseItemDefinitions()
        {
            return AsyncHelper.RunSync(GetAllExpenseItemDefinitionsAsync);
        }

        public Task<List<AccountingClassifierDefinition>> GetAllClassifierDefinitionsAsync()
        {
            return UnitOfWorkManager.WithUnitOfWorkAsync(async () =>
            {
                using (UnitOfWorkManager.Current.SetCompanyId(KontecgSession.CompanyId))
                {
                    return await AsyncQueryableExecuter.ToListAsync(await _accountingClassifierDefinitionRepository.GetAllAsync());
                }
            });
        }

        public List<AccountingClassifierDefinition> GetAllClassifierDefinitions()
        {
            return AsyncHelper.RunSync(GetAllClassifierDefinitionsAsync);
        }

        public Task<List<AccountingFunctionDefinition>> GetAllFunctionDefinitionsAsync()
        {
            return UnitOfWorkManager.WithUnitOfWorkAsync(async () =>
            {
                using (UnitOfWorkManager.Current.SetCompanyId(KontecgSession.CompanyId))
                {
                    return await AsyncQueryableExecuter.ToListAsync(await _accountingFunctionDefinitionRepository.GetAllIncludingAsync(f => f.Storage));
                }
            });
        }

        public List<AccountingFunctionDefinition> GetAllFunctionDefinitions()
        {
            return AsyncHelper.RunSync(GetAllFunctionDefinitionsAsync);
        }

        public Task<List<DocumentDefinition>> GetAllDocumentDefinitionsAsync()
        {
            return UnitOfWorkManager.WithUnitOfWorkAsync(async () =>
            {
                using (UnitOfWorkManager.Current.SetCompanyId(KontecgSession.CompanyId))
                {
                    return await AsyncQueryableExecuter.ToListAsync(await _documentDefinitionRepository.GetAllIncludingAsync(f => f.Views, f => f.SignOnDefinitions));
                }
            });
        }

        public List<DocumentDefinition> GetAllDocumentDefinitions()
        {
            return AsyncHelper.RunSync(GetAllDocumentDefinitionsAsync);
        }

        #endregion

        #region Create Methods

        public async Task<int> InsertOrUpdateDefinitionAsync(AccountDefinition definition)
        {
            var accountId = 0;
            await UnitOfWorkManager.WithUnitOfWorkAsync(async () =>
            {
                using (UnitOfWorkManager.Current.SetCompanyId(KontecgSession.CompanyId))
                {
                    if (definition.IsTransient())
                    {
                        await ValidateAccountDefinitionAsync(definition);
                        accountId = await _accountDefinitionRepository.InsertAndGetIdAsync(definition);
                    }
                    else
                    {
                        await _accountDefinitionRepository.UpdateAsync(definition);
                        accountId = definition.Id;
                    }

                    await UnitOfWorkManager.Current.SaveChangesAsync();
                }
            });
            return accountId;
        }

        public async Task<int> InsertOrUpdateDefinitionAsync(CenterCostDefinition definition)
        {
            var centerCostId = 0;
            await UnitOfWorkManager.WithUnitOfWorkAsync(async () =>
            {
                using (UnitOfWorkManager.Current.SetCompanyId(KontecgSession.GetCompanyId()))
                {
                    if (definition.IsTransient())
                    {
                        await ValidateCenterCostDefinitionAsync(definition);
                        centerCostId = await _centerCostDefinitionRepository.InsertAndGetIdAsync(definition);
                    }
                    else
                    {
                        await _centerCostDefinitionRepository.UpdateAsync(definition);
                        centerCostId = definition.Id;
                    }
                    await UnitOfWorkManager.Current.SaveChangesAsync();
                }
            });
            return centerCostId;
        }

        public async Task<int> InsertOrUpdateDefinitionAsync(ExpenseItemDefinition definition)
        {
            var expenseItemId = 0;
            await UnitOfWorkManager.WithUnitOfWorkAsync(async () =>
            {
                using (UnitOfWorkManager.Current.SetCompanyId(KontecgSession.GetCompanyId()))
                {
                    if (definition.IsTransient())
                    {
                        await ValidateExpenseItemDefinitionAsync(definition);
                        expenseItemId = await _expenseItemDefinitionRepository.InsertAndGetIdAsync(definition);
                    }
                    else
                    {
                        await _expenseItemDefinitionRepository.UpdateAsync(definition);
                        expenseItemId = definition.Id;
                    }

                    await UnitOfWorkManager.Current.SaveChangesAsync();
                }
            });
            return expenseItemId;
        }

        public async Task<int> InsertOrUpdateDefinitionAsync(AccountingFunctionDefinition definition)
        {
            var functionId = 0;
            await UnitOfWorkManager.WithUnitOfWorkAsync(async () =>
            {
                using (UnitOfWorkManager.Current.SetCompanyId(KontecgSession.CompanyId))
                {
                    if (definition.IsTransient())
                    {
                        await ValidateAccountingFunctionDefinitionAsync(definition);
                        functionId = await _accountingFunctionDefinitionRepository.InsertAndGetIdAsync(definition);
                    }
                    else
                    {
                        await _accountingFunctionDefinitionRepository.UpdateAsync(definition);
                        functionId = definition.Id;
                    }
                    await UnitOfWorkManager.Current.SaveChangesAsync();
                }
            });
            return functionId;
        }

        #endregion

        #region Delete Methods

        public virtual async Task DeleteDefinitionAsync(AccountDefinition definition)
        {
            await UnitOfWorkManager.WithUnitOfWorkAsync(async () => await _accountDefinitionRepository.DeleteAsync(definition));
        }

        public virtual async Task DeleteDefinitionAsync(CenterCostDefinition definition)
        {
            await UnitOfWorkManager.WithUnitOfWorkAsync(async () => await _centerCostDefinitionRepository.DeleteAsync(definition));
        }

        public virtual async Task DeleteDefinitionAsync(ExpenseItemDefinition definition)
        {
            await UnitOfWorkManager.WithUnitOfWorkAsync(async () => await _expenseItemDefinitionRepository.DeleteAsync(definition));
        }

        public virtual async Task DeleteDefinitionAsync(AccountingFunctionDefinition definition)
        {
            await UnitOfWorkManager.WithUnitOfWorkAsync(async () => await _accountingFunctionDefinitionRepository.DeleteAsync(definition));
        }

        #endregion

        #region Validations

        protected virtual async Task ValidateAccountDefinitionAsync(AccountDefinition definition)
        {
            var def = await _accountDefinitionRepository.FirstOrDefaultAsync(
                    def => def.Account == definition.Account &&
                           def.SubAccount == definition.SubAccount &&
                           def.SubControl == definition.SubControl &&
                           def.Analysis == definition.Analysis);

            if (def != null)
            {
                if (def.IsActive)
                    throw new UserFriendlyException(string.Format(L("AccountIsAlreadyExists"), definition.ToString()));

                throw new UserFriendlyException(string.Format(L("AccountIsAlreadyExistsButIsInactive"),
                    definition.ToString()));
            }
        }

        protected virtual async Task ValidateCenterCostDefinitionAsync(CenterCostDefinition definition)
        {
            var def = await _centerCostDefinitionRepository.FirstOrDefaultAsync(
                    def => def.AccountDefinitionId == definition.AccountDefinitionId &&
                           def.Code == definition.Code)
                .ConfigureAwait(false);

            if (def != null)
            {
                if (def.IsActive)
                    throw new UserFriendlyException(string.Format(L("CenterCostIsAlreadyExists"), definition.ToString()));

                throw new UserFriendlyException(string.Format(L("CenterCostIsAlreadyExistsButIsInactive"),
                    definition.ToString()));
            }
        }

        protected virtual async Task ValidateExpenseItemDefinitionAsync(ExpenseItemDefinition definition)
        {
            var def = await _expenseItemDefinitionRepository.FirstOrDefaultAsync(
                    def => def.Code == definition.Code)
                .ConfigureAwait(false);

            if (def != null)
            {
                if (def.IsActive)
                    throw new UserFriendlyException(string.Format(L("ExpenseItemIsAlreadyExists"), definition.ToString()));

                throw new UserFriendlyException(string.Format(L("ExpenseItemIsAlreadyExistsButIsInactive"),
                    definition.ToString()));
            }
        }

        protected virtual async Task ValidateAccountingFunctionDefinitionAsync(AccountingFunctionDefinition definition)
        {
            var def = await _accountingFunctionDefinitionRepository.FirstOrDefaultAsync(
                    def => def.Name == definition.Name)
                .ConfigureAwait(false);

            if (def != null)
            {
                if (def.IsActive)
                    throw new UserFriendlyException(string.Format(L("AccountingFunctionIsAlreadyExists"), definition.ToString()));

                throw new UserFriendlyException(string.Format(L("AccountingFunctionIsAlreadyExistsButIsInactive"),
                    definition.ToString()));
            }
        }

        #endregion
    }
}
