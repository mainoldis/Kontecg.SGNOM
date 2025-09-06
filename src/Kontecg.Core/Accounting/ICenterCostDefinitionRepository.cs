using Kontecg.Domain.Repositories;
using System.Threading.Tasks;

namespace Kontecg.Accounting
{
    public interface ICenterCostDefinitionRepository : IRepository<CenterCostDefinition>
    {
        CenterCostDefinition GetByReference(string reference);

        Task<CenterCostDefinition> GetByReferenceAsync(string reference);
    }
}