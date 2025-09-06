using System.Collections.Generic;
using Kontecg.Domain.Repositories;

namespace Kontecg.HumanResources
{
    public interface IPersonRepository : IRepository<Person, long>
    {
        /// <summary>
        ///     Search for men aged 60 or older and women 55 or older
        /// </summary>
        /// <returns>People list</returns>
        IReadOnlyList<Person> GetAgedPeople(bool nextYear = false);
    }
}
