using System.Threading.Tasks;
using Kontecg.Domain.Policies;

namespace Kontecg.Timing
{
    public interface IPeriodPolicy : IPolicy
    {
        Task<bool> CheckOpenModeAsync(PeriodInfo periodInfo);

        bool CheckOpenMode(PeriodInfo periodInfo);

        Task<bool> CheckPendingDocumentsPolicyAsync(int? periodId);

        bool CheckPendingDocumentsPolicy(int? periodId);
    }
}