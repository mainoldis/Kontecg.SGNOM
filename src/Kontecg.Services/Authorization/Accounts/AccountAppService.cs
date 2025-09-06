using System;
using System.Threading.Tasks;
using System.Web;
using Kontecg.Authorization.Accounts.Dto;
using Kontecg.Authorization.Delegation;
using Kontecg.Authorization.Impersonation;
using Kontecg.Authorization.Users;
using Kontecg.Extensions;
using Kontecg.MultiCompany;
using Kontecg.Runtime.Security;
using Kontecg.Runtime.Session;
using Kontecg.Timing;
using Kontecg.UI;

namespace Kontecg.Authorization.Accounts
{
    public class AccountAppService : KontecgAppServiceBase, IAccountAppService
    {
        private readonly IUserMailer _userMailer;
        private readonly IImpersonationManager _impersonationManager;
        private readonly IUserDelegationManager _userDelegationManager;
        private readonly IUserLinkManager _userLinkManager;

        public AccountAppService(
            IUserMailer userMailer,
            IImpersonationManager impersonationManager,
            IUserDelegationManager userDelegationManager,
            IUserLinkManager userLinkManager)
        {
            _userMailer = userMailer;
            _impersonationManager = impersonationManager;
            _userDelegationManager = userDelegationManager;
            _userLinkManager = userLinkManager;
        }

        public async Task<IsCompanyAvailableOutput> IsCompanyAvailableAsync(IsCompanyAvailableInput input)
        {
            var company = await CompanyManager.FindByCompanyNameAsync(input.CompanyName);
            if (company == null) return new IsCompanyAvailableOutput(CompanyAvailabilityState.NotFound);

            if (!company.IsActive) return new IsCompanyAvailableOutput(CompanyAvailabilityState.InActive);

            return new IsCompanyAvailableOutput(CompanyAvailabilityState.Available, company.Id);
        }

        public Task<int?> ResolveCompanyIdAsync(ResolveCompanyIdInput input)
        {
            if (string.IsNullOrEmpty(input.C)) return Task.FromResult(KontecgSession.CompanyId);

            var parameters = SimpleStringCipher.Instance.Decrypt(input.C);
            var query = HttpUtility.ParseQueryString(parameters);

            if (query["companyId"] == null) return Task.FromResult<int?>(null);

            var companyId = Convert.ToInt32(query["companyId"]) as int?;
            return Task.FromResult(companyId);
        }

        public async Task SendPasswordResetCodeAsync(SendPasswordResetCodeInput input)
        {
            var user = await UserManager.FindByEmailAsync(input.EmailAddress);
            if (user == null) return;
            user.SetNewPasswordResetCode();
            await _userMailer.SendPasswordResetLinkAsync(user);
        }

        public async Task<ResetPasswordOutput> ResetPasswordAsync(ResetPasswordInput input)
        {
            if (input.ExpireDate < Clock.Now)
                throw new UserFriendlyException(L("PasswordResetLinkExpired"));

            var user = await UserManager.GetUserByIdAsync(input.UserId);
            if (user == null || user.PasswordResetCode.IsNullOrEmpty() || user.PasswordResetCode != input.ResetCode)
                throw new UserFriendlyException(L("InvalidPasswordResetCode"), L("InvalidPasswordResetCode_Detail"));

            await UserManager.InitializeOptionsAsync(KontecgSession.CompanyId);
            CheckErrors(await UserManager.ChangePasswordAsync(user, input.Password));
            user.PasswordResetCode = null;
            user.IsEmailConfirmed = true;
            user.ShouldChangePasswordOnNextLogin = false;

            await UserManager.UpdateAsync(user);

            return new ResetPasswordOutput
            {
                CanLogin = user.IsActive,
                UserName = user.UserName
            };
        }

        public async Task SendEmailActivationLinkAsync(SendEmailActivationLinkInput input)
        {
            var user = await UserManager.FindByEmailAsync(input.EmailAddress);
            if (user == null) return;
            user.SetNewEmailConfirmationCode();
            await _userMailer.SendEmailActivationLinkAsync(user);
        }

        public async Task ActivateEmailAsync(ActivateEmailInput input)
        {
            var user = await UserManager.GetUserByIdAsync(input.UserId);
            if (user is {IsEmailConfirmed: true}) return;

            if (user == null || user.EmailConfirmationCode.IsNullOrEmpty() ||
                user.EmailConfirmationCode != input.ConfirmationCode)
                throw new UserFriendlyException(L("InvalidEmailConfirmationCode"),
                    L("InvalidEmailConfirmationCode_Detail"));

            user.IsEmailConfirmed = true;
            user.EmailConfirmationCode = null;

            await UserManager.UpdateAsync(user);
        }

        [KontecgAuthorize(PermissionNames.AdministrationUsersImpersonation)]
        public virtual async Task<ImpersonateOutput> ImpersonateUserAsync(ImpersonateUserInput input)
        {
            return new ImpersonateOutput
            {
                ImpersonationToken = await _impersonationManager.GetImpersonationTokenAsync(input.UserId, KontecgSession.CompanyId),
                CompanyName = await GetCompanyNameOrNullAsync(input.CompanyId)
            };
        }

        [KontecgAuthorize(PermissionNames.CompaniesImpersonation)]
        public virtual async Task<ImpersonateOutput> ImpersonateCompanyAsync(ImpersonateCompanyInput input)
        {
            return new ImpersonateOutput
            {
                ImpersonationToken = await _impersonationManager.GetImpersonationTokenAsync(input.UserId, input.CompanyId),
                CompanyName = await GetCompanyNameOrNullAsync(input.CompanyId)
            };
        }

        public virtual async Task<ImpersonateOutput> DelegatedImpersonateAsync(DelegatedImpersonateInput input)
        {
            var userDelegation = await _userDelegationManager.GetAsync(input.UserDelegationId);
            if (userDelegation.TargetUserId != KontecgSession.GetUserId())
                throw new UserFriendlyException("User delegation error.");

            return new ImpersonateOutput
            {
                ImpersonationToken = await _impersonationManager.GetImpersonationTokenAsync(userDelegation.SourceUserId, userDelegation.CompanyId),
                CompanyName = await GetCompanyNameOrNullAsync(userDelegation.CompanyId)
            };
        }

        public virtual async Task<ImpersonateOutput> BackToImpersonatorAsync()
        {
            return new ImpersonateOutput
            {
                ImpersonationToken = await _impersonationManager.GetBackToImpersonatorTokenAsync(),
                CompanyName = await GetCompanyNameOrNullAsync(KontecgSession.ImpersonatorCompanyId)
            };
        }

        public virtual async Task<SwitchToLinkedAccountOutput> SwitchToLinkedAccountAsync(SwitchToLinkedAccountInput input)
        {
            if (!await _userLinkManager.AreUsersLinkedAsync(KontecgSession.ToUserIdentifier(), input.ToUserIdentifier()))
            {
                throw new UserFriendlyException(L("This account is not linked to your account"));
            }

            return new SwitchToLinkedAccountOutput
            {
                SwitchAccountToken = await _userLinkManager.GetAccountSwitchTokenAsync(input.TargetUserId, input.TargetCompanyId),
                CompanyName = await GetCompanyNameOrNullAsync(input.TargetCompanyId)
            };
        }

        private async Task<Company> GetActiveCompanyAsync(int companyId)
        {
            var company = await CompanyManager.FindByIdAsync(companyId);
            if (company == null)
            {
                throw new UserFriendlyException(L("UnknownCompanyId{0}", companyId));
            }

            if (!company.IsActive)
            {
                throw new UserFriendlyException(L("CompanyIdIsNotActive{0}", companyId));
            }

            return company;
        }

        private async Task<string> GetCompanyNameOrNullAsync(int? companyId)
        {
            return companyId.HasValue ? (await GetActiveCompanyAsync(companyId.Value)).CompanyName : null;
        }
    }
}
