using DevExpress.Mvvm.DataAnnotations;
using DevExpress.Mvvm.POCO;
using DevExpress.Mvvm;
using Kontecg.Authorization;
using Kontecg.Authorization.Users;
using Kontecg.Configuration;
using Kontecg.Logging;
using Kontecg.MultiCompany;
using Kontecg.Runtime.Validation;
using Kontecg.UI;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System;
using System.Collections.Generic;
using System.DirectoryServices.AccountManagement;
using System.Security.Claims;
using Kontecg.Extensions;
using Kontecg.Threading;
using Kontecg.Domain.Uow;
using Kontecg.Application.Services.Dto;
using System.Threading.Tasks;
using System.Windows.Forms;
using Kontecg.Configuration.Startup;
using Kontecg.Linq;
using Kontecg.Localization;
using Kontecg.Baseline;
using System.ComponentModel;

namespace Kontecg.ViewModels.Account
{
    public class LoginViewModel : KontecgViewModelBase, IDocumentContent
    {
        private readonly UserManager _userManager;
        private readonly CompanyManager _companyManager;
        private readonly IMultiCompanyConfig _multiCompanyConfig;
        private readonly KontecgLoginResultTypeHelper _kontecgLoginResultTypeHelper;
        private readonly LogInManager _logInManager;
        private readonly UserClaimsPrincipalFactory _claimsPrincipalFactory;
        private readonly LoginModel _model;

        /// <inheritdoc />
        public LoginViewModel(
            UserManager userManager, 
            CompanyManager companyManager,
            IMultiCompanyConfig multiCompanyConfig,
            KontecgLoginResultTypeHelper kontecgLoginResultTypeHelper, 
            LogInManager logInManager, 
            UserClaimsPrincipalFactory claimsPrincipalFactory)
        {
            _userManager = userManager;
            _companyManager = companyManager;
            _multiCompanyConfig = multiCompanyConfig;
            _kontecgLoginResultTypeHelper = kontecgLoginResultTypeHelper;
            _logInManager = logInManager;
            _claimsPrincipalFactory = claimsPrincipalFactory;

            _model = new();
            LocalizationSourceName = KontecgWinFormsConsts.LocalizationSourceName;
            AsyncQueryableExecuter = NullAsyncQueryableExecuter.Instance;

            IsLogin = false;
            DialogResult = DialogResult.Cancel;
        }

        public IAsyncQueryableExecuter AsyncQueryableExecuter { get; set; }

        public virtual ListResultDto<ComboboxItemDto> Companies => AsyncHelper.RunSync(GetCompaniesForComboboxAsync);

        private async Task<ListResultDto<ComboboxItemDto>> GetCompaniesForComboboxAsync()
        {
            using var uow = UnitOfWorkManager.Begin(new UnitOfWorkOptions() { IsTransactional = false });
            using (CurrentUnitOfWork.SetCompanyId(null))
            {
                var companies = await AsyncQueryableExecuter.ToListAsync(_companyManager.Companies.Where(a => a.IsActive).OrderBy(e => e.CompanyName));
                List<ComboboxItemDto> result = [new(string.Empty, string.Empty)];

                for (int i = 0; i < companies.Count; i++)
                {
                    if (i == 0)
                    {
                        result.Add(new ComboboxItemDto(companies[i].Id.ToString(), companies[i].CompanyName) {IsSelected = true});
                        continue;
                    }

                    result.Add(new ComboboxItemDto(companies[i].Id.ToString(), companies[i].CompanyName));
                }

                return new ListResultDto<ComboboxItemDto>(result.ToList());
            }
        }

        #region Login

        public bool IsLogin { get; set; }

        private sealed class LoginModel
        {
            public string UsernameOrEmail { get; set; }

            public string Password { get; set; }

            public string Company { get; set; }

            public bool RememberMe { get; set; }
        }

        [Required]
        public virtual string UsernameOrEmail
        {
            get => _model.UsernameOrEmail;
            set
            {
                if (_model.UsernameOrEmail == value) return;
                _model.UsernameOrEmail = value;
                this.RaisePropertyChanged(x => x.UsernameOrEmail);
                this.RaiseCanExecuteChanged(x => x.LoginAsync());
            }
        }

        [Required]
        public virtual string Password
        {
            get => _model.Password;
            set
            {
                if (_model.Password == value) return;
                _model.Password = value;
                this.RaisePropertyChanged(x => x.Password);
                this.RaiseCanExecuteChanged(x => x.LoginAsync());
            }
        }

        public virtual string Company
        {
            get => _model.Company;
            set
            {
                if (_model.Company == value) return;
                _model.Company = value;
                this.RaisePropertyChanged(x => x.Company);
                this.RaiseCanExecuteChanged(x => x.LoginAsync());
            }
        }

        public virtual bool RememberMe
        {
            get => _model.RememberMe;
            set
            {
                if (_model.RememberMe == value) return;
                _model.RememberMe = value;
                this.RaisePropertyChanged(x => x.RememberMe);
            }
        }

        protected void OnRememberMeChanged()
        {

        }

        [AsyncCommand]
        public async Task LoginAsync()
        {
            try
            {
                var asyncCommand = this.GetAsyncCommand(x => x.LoginAsync());
                if (!asyncCommand.IsCancellationRequested)
                {
                    using var uow = UnitOfWorkManager.Begin(new UnitOfWorkOptions());
                    var loginResult = await GetLoginResultAsync(UsernameOrEmail, Password, Company);

                    if (SettingManager.GetSettingValue<bool>(AppSettings.UserManagement.AllowOneConcurrentLoginPerUser))
                        await _userManager.UpdateSecurityStampAsync(loginResult.User);

                    if (loginResult.User.ShouldChangePasswordOnNextLogin)
                    {
                        loginResult.User.SetNewPasswordResetCode();
                        await _userManager.UpdateAsync(loginResult.User);
                    }

                    var userPrincipal = await _claimsPrincipalFactory.CreateAsync(loginResult.User);

                    userPrincipal.Identities.First()
                                 .AddClaim(new Claim(ClaimTypes.AuthenticationMethod, IdentityConstants.ApplicationScheme));

                    await UnitOfWorkManager.Current.SaveChangesAsync();
                    await uow.CompleteAsync();

                    await DispatcherService.BeginInvoke(() => AppDomain.CurrentDomain.SetThreadPrincipal(userPrincipal));
                    
                    DialogResult = DialogResult.OK;
                    IsLogin = true;
                    var session = KontecgSession;
                    await DispatcherService.BeginInvoke(() => Document?.Close());
                }
            }
            catch (Exception ex)
            {
                switch (ex)
                {
                    case IHasLogSeverity exceptionWithLogSeverity:
                        {
                            Logger.Log(exceptionWithLogSeverity.Severity, ex.Message, ex);
                            switch (exceptionWithLogSeverity)
                            {
                                case KontecgAuthorizationException kontecgAuthorizationException:
                                    MessageBoxService.ShowMessage(kontecgAuthorizationException.Message,
                                        GetMessageCaption(kontecgAuthorizationException.Severity),
                                        MessageButton.OK, GetMessageIcon(kontecgAuthorizationException.Severity));
                                    break;
                                case KontecgValidationException kontecgValidationException:
                                    break;
                                case UserFriendlyException userFriendlyException:
                                    MessageBoxService.ShowMessage(userFriendlyException.Message,
                                        GetMessageCaption(userFriendlyException.Severity),
                                        MessageButton.OK, GetMessageIcon(userFriendlyException.Severity));
                                    break;
                                default:
                                    throw new ArgumentOutOfRangeException(nameof(exceptionWithLogSeverity));
                            }
                            break;
                        }

                    case System.DirectoryServices.DirectoryServicesCOMException exception:
                        MessageBoxService.ShowMessage(exception.Message, GetMessageCaption(LogSeverity.Fatal),
                            MessageButton.OK, GetMessageIcon(LogSeverity.Fatal));
                        break;
                    case ArgumentNullException exception:
                        MessageBoxService.ShowMessage(exception.Message, GetMessageCaption(LogSeverity.Fatal),
                            MessageButton.OK, GetMessageIcon(LogSeverity.Fatal));
                        break;
                    default:
                        ex.ReThrow();
                        break;
                }

                IsLogin = false;
            }
        }

        public virtual bool CanLogin()
        {
            return !string.IsNullOrWhiteSpace(UsernameOrEmail) && !string.IsNullOrWhiteSpace(Password) && KontecgSession.UserId == null;
        }

        private async Task<KontecgLoginResult<Company, User>> GetLoginResultAsync(string usernameOrEmailAddress, string password,
            string companyName)
        {
            KontecgLoginResult<Company, User> loginResult = null;
            try
            {
                loginResult = await _logInManager.LoginAsync(usernameOrEmailAddress, password, companyName);
            }

            catch (PrincipalServerDownException ldapServerDownException)
            {
                loginResult = new KontecgLoginResult<Company, User>(KontecgLoginResultType.FailedForOtherReason);
                loginResult.SetFailReason(new LocalizableString("PrincipalServerDownExceptionMessage", KontecgBaselineConsts.LocalizationSourceName));
            }

            switch (loginResult.Result)
            {
                case KontecgLoginResultType.Success:
                    return loginResult;
                default:
                    throw _kontecgLoginResultTypeHelper.CreateExceptionForFailedLoginAttempt(loginResult,
                        usernameOrEmailAddress, companyName);
            }
        }

        #endregion

        public bool CheckRememberUsernameOrEmail()
        {
            return SettingManager.GetSettingValue<bool>(WinFormsSettings.AuthManagement.RememberLastLogin);
        }

        #region Services

        protected IMessageBoxService MessageBoxService => this.GetService<IMessageBoxService>();

        protected IDispatcherService DispatcherService => this.GetService<IDispatcherService>();

        private string GetMessageCaption(LogSeverity logSeverity)
        {
            switch (logSeverity)
            {
                case LogSeverity.Debug:
                case LogSeverity.Info:
                    return L("Information");
                case LogSeverity.Warn:
                    return L("Warning");
                case LogSeverity.Error:
                case LogSeverity.Fatal:
                    return L("Error");
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private MessageIcon GetMessageIcon(LogSeverity logSeverity)
        {
            switch (logSeverity)
            {
                case LogSeverity.Debug:
                case LogSeverity.Info:
                    return MessageIcon.Information;
                case LogSeverity.Warn:
                    return MessageIcon.Warning;
                case LogSeverity.Error:
                case LogSeverity.Fatal:
                    return MessageIcon.Error;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        #endregion

        public DialogResult DialogResult { get; private set; }

        public IDocument Document { get; set; }

        IDocumentOwner IDocumentContent.DocumentOwner { get; set; }

        object IDocumentContent.Title => Document?.Title ?? string.Empty;

        void IDocumentContent.OnClose(CancelEventArgs e) { }

        void IDocumentContent.OnDestroy() { }
    }
}