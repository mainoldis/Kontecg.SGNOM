using Kontecg.Domain.Repositories;

namespace Kontecg.Organizations
{
    public interface IOccupationRepository : IRepository<Occupation>
    {
        Occupation GetByCode(string code);
    }
}
