using Kontecg.Application.Clients;
using Kontecg.Application.Features;
using Kontecg.Authorization.Users;
using Kontecg.Domain.Repositories;
using Kontecg.Domain.Uow;
using Kontecg.MultiCompany;

namespace Kontecg.RealTime
{
    public class ClientManager : KontecgClientManager<Company, User>
    {
        /// <inheritdoc />
        public ClientManager(
            IKontecgFeatureValueStore featureValueStore, 
            IUnitOfWorkManager unitOfWorkManager, 
            IRepository<ClientInfo, string> clientRepository, 
            IClientFactory clientFactory) 
            : base(featureValueStore, unitOfWorkManager, clientRepository, clientFactory)
        {
        }
    }
}