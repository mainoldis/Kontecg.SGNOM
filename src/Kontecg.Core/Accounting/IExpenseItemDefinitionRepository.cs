using Kontecg.Domain.Repositories;
using System.Threading.Tasks;

namespace Kontecg.Accounting
{
    public interface IExpenseItemDefinitionRepository : IRepository<ExpenseItemDefinition>
    {
        ExpenseItemDefinition GetByReference(string reference);

        Task<ExpenseItemDefinition> GetByReferenceAsync(string reference);
    }
}