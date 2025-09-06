using System.Linq;
using Kontecg.EFCore;
using Kontecg.EFCore.Repositories;

namespace Kontecg.Timing
{
    public class WorkShiftRepository : SGNOMRepositoryBase<WorkShift>, IWorkShiftRepository
    {
        public WorkShiftRepository(IDbContextProvider<SGNOMDbContext> dbContextProvider)
            : base(dbContextProvider)
        {
        }

        public WorkShift GetWorkShiftByName(string name)
        {
            return GetAllIncluding(w => w.Regime)
                .FirstOrDefault(s => s.DisplayName == name && s.IsActive);
        }
    }
}
