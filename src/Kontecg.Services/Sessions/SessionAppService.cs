using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kontecg.Application.Services.Dto;
using Kontecg.Auditing;
using Kontecg.Authorization.Delegation;
using Kontecg.Authorization.Users;
using Kontecg.Configuration;
using Kontecg.Domain.Uow;
using Kontecg.Features;
using Kontecg.Localization;
using Kontecg.Runtime.Session;
using Kontecg.Sessions.Dto;
using Kontecg.Threading;

namespace Kontecg.Sessions
{
    public class SessionAppService : KontecgAppServiceBase, ISessionAppService
    {
        private readonly ILocalizationContext _localizationContext;
        private readonly IUserDelegationConfiguration _userDelegationConfiguration;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly AppVersion _appVersion;

        public SessionAppService(
            ILocalizationContext localizationContext,
            IUserDelegationConfiguration userDelegationConfiguration,
            IUnitOfWorkManager unitOfWorkManager, AppVersion appVersion)
        {
            _localizationContext = localizationContext;
            _userDelegationConfiguration = userDelegationConfiguration;
            _unitOfWorkManager = unitOfWorkManager;
            _appVersion = appVersion;
        }

        [DisableAuditing]
        public async Task<GetCurrentLoginInformationOutput> GetCurrentLoginInformationAsync()
        {
            return await _unitOfWorkManager.WithUnitOfWorkAsync(async () =>
            {
                var output = new GetCurrentLoginInformationOutput
                {
                    Application = new ApplicationInfoDto
                    {
                        Version = _appVersion.Version,
                        Features = new Dictionary<string, bool>(),
                        UserDelegationIsEnabled = _userDelegationConfiguration.IsEnabled,
                        Currency = await SettingManager.GetSettingValueForApplicationAsync(AppSettings.CurrencyManagement.BaseCurrency),
                        AllowCompaniesToChangeEmailSettings = KontecgCoreConsts.AllowCompaniesToChangeEmailSettings
                    }
                };

                if (KontecgSession.CompanyId.HasValue)
                {
                    output.Company = await GetCompanyLoginInfoAsync(KontecgSession.CompanyId.Value);
                }

                if (KontecgSession.ImpersonatorCompanyId.HasValue)
                {
                    output.ImpersonatorCompany = await GetCompanyLoginInfoAsync(KontecgSession.ImpersonatorCompanyId.Value);
                }

                if (KontecgSession.UserId.HasValue)
                {
                    output.User = ObjectMapper.Map<UserLoginInfoDto>(await GetCurrentUserAsync());
                }

                if (KontecgSession.ImpersonatorUserId.HasValue)
                {
                    output.ImpersonatorUser = ObjectMapper.Map<UserLoginInfoDto>(await GetImpersonatorUserAsync());
                }

                if (output.Company == null) return output;

                output.Company.CreationTimeString = output.Company.CreationTime.ToString("d");

                return output;
            });
        }

        [DisableAuditing]
        public GetCurrentLoginInformationOutput GetCurrentLoginInformation()
        {
            return AsyncHelper.RunSync(GetCurrentLoginInformationAsync);
        }

        public async Task<UpdateUserSignInTokenOutput> UpdateUserSignInTokenAsync()
        {
            if (KontecgSession.UserId <= 0) throw new KontecgException(L("ThereIsNoLoggedInUser"));

            var user = await UserManager.GetUserAsync(KontecgSession.ToUserIdentifier());
            user.SetSignInToken();
            return new UpdateUserSignInTokenOutput
            {
                SignInToken = user.SignInToken,
                EncodedUserId = Convert.ToBase64String(Encoding.UTF8.GetBytes(user.Id.ToString())),
                EncodedCompanyId = user.CompanyId.HasValue
                    ? Convert.ToBase64String(Encoding.UTF8.GetBytes(user.CompanyId.Value.ToString()))
                    : ""
            };
        }

        private async Task<CompanyLoginInfoDto> GetCompanyLoginInfoAsync(int companyId)
        {
            var company = CompanyManager
                .Companies
                .First(t => t.Id == companyId);

            var companyLoginInfo = ObjectMapper
                .Map<CompanyLoginInfoDto>(company);

            var features = FeatureManager
                .GetAll()
                .Where(feature => (feature[FeatureMetadata.CustomFeatureKey] as FeatureMetadata)?.IsVisibleOnInfoTable ?? false);

            var featureDictionary = features.ToDictionary(feature => feature.Name, f => f);

            companyLoginInfo.FeatureValues = (await CompanyManager.GetFeatureValuesAsync(company.Id))
                .Where(featureValue => featureDictionary.ContainsKey(featureValue.Name))
                .Select(fv => new NameValueDto(
                    featureDictionary[fv.Name].DisplayName.Localize(_localizationContext),
                    featureDictionary[fv.Name].GetValueText(fv.Value, _localizationContext))
                )
                .ToList();

            return companyLoginInfo;
        }

        protected virtual async Task<User> GetImpersonatorUserAsync()
        {
            using (CurrentUnitOfWork.SetCompanyId(KontecgSession.ImpersonatorCompanyId))
            {
                var user = await UserManager.FindByIdAsync(KontecgSession.ImpersonatorUserId.ToString());
                if (user == null)
                {
                    throw new KontecgException("User not found!");
                }

                return user;
            }
        }
    }
}
