using System.Collections.Generic;
using System.Threading.Tasks;
using Kontecg.Domain.Repositories;

namespace Kontecg.Accounting
{
    public interface IViewRepository : IRepository<ViewName>
    {
        IReadOnlyList<ViewNameResultRecord> ExecuteView(string viewName, string docCod = null);

        Task<IReadOnlyList<ViewNameResultRecord>> ExecuteViewAsync(string viewName, string docCod = null);
    }
}