using System.Threading.Tasks;
using Kontecg.Application.Features;
using Kontecg.Domain.Repositories;

namespace Kontecg.Authorization.Users
{
    public class UserPolicy : KontecgCoreServiceBase, IUserPolicy
    {
        private readonly IFeatureChecker _featureChecker;
        private readonly IRepository<User, long> _userRepository;

        public UserPolicy(IFeatureChecker featureChecker, IRepository<User, long> userRepository)
        {
            _featureChecker = featureChecker;
            _userRepository = userRepository;
        }

        public async Task CheckUserPolicyAsync(int companyId)
        {
            //var registrationFeature = (await _featureChecker.GetValueAsync(companyId, WinFormsFeatureNames.RegistrationFeature)).To<bool>();
            //if (!registrationFeature)
            //    throw new UserFriendlyException(L("UserCannotRegisterByHimSelf_Error_Message"), L("UserCannotRegisterByHimSelf_Error_Detail"));
        }
    }
}
