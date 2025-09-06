using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Kontecg.Domain.Repositories;

namespace Kontecg.WorkRelations
{
    public interface IEmploymentIndexRepository : IRepository<EmploymentIndex, Guid>
    {
        /// <summary>
        ///    Get all available exp numbers due Employment config
        /// </summary>
        /// <param name="contract">Contract type. <remarks>I - Indeterminate, D - Determinate</remarks></param>
        /// <param name="group">Grouping by config</param>
        /// <returns></returns>
        IReadOnlyList<int> GetAllAvailableExp(ContractType contract, ContractSubType group);

        /// <summary>
        ///    Get all available exp numbers due Employment config
        /// </summary>
        /// <param name="contract">Contract type. <remarks>I - Indeterminate, D - Determinate</remarks></param>
        /// <param name="group">Grouping by config</param>
        /// <returns></returns>
        Task<IReadOnlyList<int>> GetAllAvailableExpAsync(ContractType contract, ContractSubType group);
    }
}
