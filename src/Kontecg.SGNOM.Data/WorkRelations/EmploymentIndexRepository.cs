using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kontecg.EFCore;
using Kontecg.EFCore.Repositories;
using Kontecg.MultiCompany;

namespace Kontecg.WorkRelations
{
    public class EmploymentIndexRepository : SGNOMRepositoryBase<EmploymentIndex, Guid>, IEmploymentIndexRepository
    {
        private readonly IEmploymentSettingStore _employmentSettings;

        public EmploymentIndexRepository(
            IDbContextProvider<SGNOMDbContext> dbContextProvider,
            IEmploymentSettingStore settings)
            : base(dbContextProvider)
        {
            _employmentSettings = settings;
        }

        public IReadOnlyList<int> GetAllAvailableExp(ContractType contract, ContractSubType group)
        {
            var query = GetAll()
                .Where(index => index.Contract == contract && index.Group == group)
                .Select(s => s.Exp)
                .ToList();

            var validIndexes = GenerateValidIndexes(contract, group);
            return validIndexes.Except(query).ToList();
        }

        public async Task<IReadOnlyList<int>> GetAllAvailableExpAsync(ContractType contract, ContractSubType group)
        {
            var query = (await GetAllAsync())
                .Where(index => index.Contract == contract && index.Group == group)
                .Select(s => s.Exp)
                .ToList();

            var validIndexes = GenerateValidIndexes(contract, group);
            return validIndexes.Except(query).ToList();
        }

        private IReadOnlyList<int> GenerateValidIndexes(ContractType contract, ContractSubType group)
        {
            int? companyId = null;

            if (KontecgCoreConsts.MultiCompanyEnabled && MultiCompanySide == MultiCompanySides.Company)
                companyId = UnitOfWorkManager.Current.GetCompanyId();

            companyId ??= MultiCompanyConsts.DefaultCompanyId;
            var settings = _employmentSettings.GetIndexRanges(companyId.Value);

            int start = 0, end = 0;

            if (contract == ContractType.I)
            {
                start = settings.StartIndexForLifeTimeContracts;
                end = settings.EndIndexForLifeTimeContracts;
            }
            else
                switch (group)
                {
                    case ContractSubType.A:
                        start = settings.StartIndexForInitialLifeTimeContracts;
                        end = settings.EndIndexForInitialLifeTimeContracts;
                        break;
                    case ContractSubType.D:
                        start = settings.StartIndexForShortContracts;
                        end = settings.EndIndexForShortContracts;
                        break;
                    case ContractSubType.P:
                        start = settings.StartIndexForTemporallyContracts;
                        end = settings.EndIndexForTemporallyContracts;
                        break;
                }

            return Enumerable.Range(start, end).ToList();
        }
    }
}
