using System.Linq;
using Kontecg.EFCore;
using Kontecg.EFCore.Repositories;

namespace Kontecg.Organizations
{
    public class OccupationRepository : SGNOMRepositoryBase<Occupation>, IOccupationRepository
    {
        public OccupationRepository(IDbContextProvider<SGNOMDbContext> dbContextProvider)
            : base(dbContextProvider)
        {
        }

        public Occupation GetByCode(string code)
        {
            return
                GetAllIncluding(o => o.Category,
                        o => o.Group,
                        o => o.Responsibility)
                    .FirstOrDefault(o => o.Code == code);
        }
    }
}
