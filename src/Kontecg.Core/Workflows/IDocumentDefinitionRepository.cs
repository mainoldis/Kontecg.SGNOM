using Kontecg.Domain.Repositories;
using System.Threading.Tasks;

namespace Kontecg.Workflows
{
    public interface IDocumentDefinitionRepository : IRepository<DocumentDefinition>
    {
        DocumentDefinition GetByReference(string reference);

        Task<DocumentDefinition> GetByReferenceAsync(string reference);
    }
}