using System.Collections.Generic;
using System.Linq;
using Kontecg.Collections.Extensions;
using Kontecg.EFCore;
using Kontecg.EFCore.Repositories;
using Kontecg.Primitives;

namespace Kontecg.HumanResources
{
    public class PersonRepository : KontecgRepositoryBase<Person, long>,
        IPersonRepository
    {
        public PersonRepository(IDbContextProvider<KontecgCoreDbContext> dbContextProvider)
            : base(dbContextProvider)
        {
        }

        public IReadOnlyList<Person> GetAgedPeople(bool nextYear = false)
        {
            var query = GetAll().ToList();
            return query
                .WhereIf(!nextYear, p => (p.Gender == Gender.M && p.Age >= 65) || (p.Gender == Gender.F && p.Age >= 60))
                .WhereIf(nextYear,
                    p => (p.Gender == Gender.M && p.Age + 1 >= 65) || (p.Gender == Gender.F && p.Age + 1 >= 60))
                .ToList();
        }
    }
}
