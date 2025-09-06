using Kontecg.Domain.Services;

namespace Kontecg
{
    public abstract class KontecgCoreDomainServiceBase : DomainService
    {
        protected KontecgCoreDomainServiceBase()
        {
            LocalizationSourceName = KontecgCoreConsts.LocalizationSourceName;
        }
    }
}
