using Kontecg.Domain.Uow;
using Kontecg.MultiCompany;

namespace Kontecg.EFCore
{
    public class DbPerContextConnectionStringResolverArgs : ConnectionStringResolveArgs
    {
        public DbPerContextConnectionStringResolverArgs(int? companyId, MultiCompanySides? multiCompanySide = null)
            : base(multiCompanySide)
        {
            CompanyId = companyId;
        }

        public DbPerContextConnectionStringResolverArgs(int? companyId, ConnectionStringResolveArgs baseArgs)
        {
            CompanyId = companyId;
            MultiCompanySide = baseArgs.MultiCompanySide;

            foreach (var kvPair in baseArgs) Add(kvPair.Key, kvPair.Value);
        }

        public int? CompanyId { get; set; }
    }
}
