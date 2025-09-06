using Kontecg.Application.Clients;
using Kontecg.Application.Features;
using Kontecg.Authorization.Users;
using Kontecg.Domain.Repositories;
using Kontecg.Domain.Uow;
using Kontecg.MultiCompany;
using Kontecg.Runtime.Caching;

namespace Kontecg.Features
{
    public class FeatureValueStore : KontecgFeatureValueStore<Company, User>
    {
        /// <inheritdoc />
        public FeatureValueStore(
            ICacheManager cacheManager, 
            IRepository<CompanyFeatureSetting, long> companyFeatureRepository, 
            IRepository<ClientFeatureSetting, long> clientFeatureRepository, 
            IRepository<ClientInfo, string> clientRepository, 
            IFeatureManager featureManager, IUnitOfWorkManager unitOfWorkManager) 
            : base(
                cacheManager, 
                companyFeatureRepository, 
                clientFeatureRepository, 
                clientRepository, 
                featureManager, 
                unitOfWorkManager)
        {
        }
    }
}
