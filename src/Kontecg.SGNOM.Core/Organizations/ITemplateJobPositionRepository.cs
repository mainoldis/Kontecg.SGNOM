using System.Collections.Generic;
using System.Threading.Tasks;
using Kontecg.Domain.Repositories;

namespace Kontecg.Organizations
{
    public interface ITemplateJobPositionRepository : IRepository<TemplateJobPosition>
    {
        IReadOnlyList<TemplateJobPosition> GetJobPositions(int documentId, string occupationCode, params long[] ouIds);

        Task<IReadOnlyList<TemplateJobPosition>> GetJobPositionsAsync(int documentId, string occupationCode, params long[] ouIds);

        int TryToRemoveOrphans(int templateId, int countToRemove);

        Task<int> TryToRemoveOrphansAsync(int templateId, int countToRemove);

        void UpdateCodes();

        Task UpdateCodesAsync();
    }
}
