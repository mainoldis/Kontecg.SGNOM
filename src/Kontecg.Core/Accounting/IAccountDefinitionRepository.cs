using System.Threading.Tasks;
using Kontecg.Domain.Repositories;

namespace Kontecg.Accounting
{
    public interface IAccountDefinitionRepository : IRepository<AccountDefinition>
    {
        AccountDefinition GetByReference(string reference);

        Task<AccountDefinition> GetByReferenceAsync(string reference);
    }
}