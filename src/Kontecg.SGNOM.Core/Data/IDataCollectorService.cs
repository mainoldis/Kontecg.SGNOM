using System.Threading.Tasks;
using Kontecg.Domain.Services;

namespace Kontecg.Data
{
    public interface IDataCollectorService : IDomainService
    {
        Task ForcePersonToChangeTheirOrganizationUnitAsync(int companyId);

        Task ForcePersonToChangeTheirScholarshipDataAsync(int companyId);

        Task ForcePersonToChangeTheirExpNumberAsync(int companyId);

        Task ForcePersonToChangeTheirDataAsync();
    }
}
