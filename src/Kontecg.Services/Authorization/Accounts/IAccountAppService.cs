using System.Threading.Tasks;
using Kontecg.Application.Services;
using Kontecg.Authorization.Accounts.Dto;

namespace Kontecg.Authorization.Accounts
{
    public interface IAccountAppService : IApplicationService
    {
        Task<IsCompanyAvailableOutput> IsCompanyAvailableAsync(IsCompanyAvailableInput input);

        Task<int?> ResolveCompanyIdAsync(ResolveCompanyIdInput input);

        Task SendPasswordResetCodeAsync(SendPasswordResetCodeInput input);

        Task<ResetPasswordOutput> ResetPasswordAsync(ResetPasswordInput input);

        Task SendEmailActivationLinkAsync(SendEmailActivationLinkInput input);

        Task ActivateEmailAsync(ActivateEmailInput input);

        Task<ImpersonateOutput> ImpersonateUserAsync(ImpersonateUserInput input);

        Task<ImpersonateOutput> ImpersonateCompanyAsync(ImpersonateCompanyInput input);

        Task<ImpersonateOutput> DelegatedImpersonateAsync(DelegatedImpersonateInput input);

        Task<ImpersonateOutput> BackToImpersonatorAsync();

        Task<SwitchToLinkedAccountOutput> SwitchToLinkedAccountAsync(SwitchToLinkedAccountInput input);
    }
}
