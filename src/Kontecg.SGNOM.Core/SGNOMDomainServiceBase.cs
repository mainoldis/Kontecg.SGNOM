using Kontecg.Domain.Services;

namespace Kontecg
{
    public abstract class SGNOMDomainServiceBase : DomainService
    {
        protected SGNOMDomainServiceBase()
        {
            LocalizationSourceName = SGNOMConsts.LocalizationSourceName;
        }
    }
}