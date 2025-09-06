using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kontecg.EFCore;
using Kontecg.EFCore.Repositories;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using static Kontecg.Domain.Uow.KontecgDataFilters;

namespace Kontecg.Accounting
{
    public class ViewRepository : KontecgRepositoryBase<ViewName>, IViewRepository
    {
        /// <inheritdoc />
        public ViewRepository(IDbContextProvider<KontecgCoreDbContext> dbContextProvider) 
            : base(dbContextProvider)
        {
        }

        /// <inheritdoc />  
        public IReadOnlyList<ViewNameResultRecord> ExecuteView(string viewName, string docCod)
        {
            if(!IsValidName(viewName))
                throw new System.ArgumentException("Invalid view name.", nameof(viewName));

            string query = $"SELECT * FROM {viewName}";
            var parameters = new List<SqlParameter>();

            if (!string.IsNullOrWhiteSpace(docCod))
            {
                query += " WHERE DocCod = @docCod";
                parameters.Add(new SqlParameter("@docCod", docCod));
            }

            return GetDbContext().Set<ViewNameResultRecord>().FromSqlRaw(query, parameters.Cast<object>().ToArray()).AsNoTracking().ToList();
        }

        /// <inheritdoc />  
        public async Task<IReadOnlyList<ViewNameResultRecord>> ExecuteViewAsync(string viewName, string docCod)
        {
            if (!IsValidName(viewName))
                throw new System.ArgumentException("Invalid view name.", nameof(viewName));

            string query = $"SELECT * FROM {viewName}";
            var parameters = new List<SqlParameter>();

            if (!string.IsNullOrWhiteSpace(docCod))
            {
                query += " WHERE DocCod = @docCod";
                parameters.Add(new SqlParameter("@docCod", docCod));
            }

            return await (await GetDbContextAsync()).Set<ViewNameResultRecord>().FromSqlRaw(query, parameters.Cast<object>().ToArray()).AsNoTracking().ToListAsync();
        }

        private bool IsValidName(string name)
        {
            return FirstOrDefault(v => v.Name == name) != null;
        }
    }
}