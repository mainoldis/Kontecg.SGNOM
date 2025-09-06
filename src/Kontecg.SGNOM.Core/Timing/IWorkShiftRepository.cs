using Kontecg.Domain.Repositories;

namespace Kontecg.Timing
{
    public interface IWorkShiftRepository : IRepository<WorkShift>
    {
        WorkShift GetWorkShiftByName(string name);
    }
}
