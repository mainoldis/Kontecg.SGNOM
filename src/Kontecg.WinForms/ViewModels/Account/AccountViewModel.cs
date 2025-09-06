using DevExpress.Mvvm.DataAnnotations;
using Kontecg.Authorization.Accounts;
using Kontecg.Authorization.Delegation;
using Kontecg.Authorization.Impersonation;
using Kontecg.Authorization.Users.Profile;
using Kontecg.Authorization.Users;
using Kontecg.Authorization;
using Kontecg.CachedUniqueKeys;
using Kontecg.Configuration;
using Kontecg.MultiCompany;
using Kontecg.Notifications;
using Kontecg.Runtime.Security;
using Kontecg.Runtime.Session;
using Kontecg.Sessions;
using Microsoft.AspNetCore.Identity;
using Kontecg.Configuration.Startup;
using Kontecg.Net.Mail;
using System;
using DevExpress.Mvvm.POCO;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using Kontecg.Threading;
using DevExpress.Mvvm;
using Kontecg.Extensions;
using Kontecg.Logging;
using Kontecg.Runtime.Validation;
using Kontecg.UI;

namespace Kontecg.ViewModels.Account
{
    public class AccountViewModel : KontecgViewModelBase
    {
        private readonly UserManager _userManager;
        private readonly CompanyManager _companyManager;
        private readonly IMultiCompanyConfig _multiCompanyConfig;
        private readonly IAppNotifier _appNotifier;
        private readonly KontecgLoginResultTypeHelper _kontecgLoginResultTypeHelper;
        private readonly IUserLinkManager _userLinkManager;
        private readonly LogInManager _logInManager;
        private readonly UserClaimsPrincipalFactory _claimsPrincipalFactory;
        private readonly IPerRequestSessionCache _sessionCache;
        private readonly ICompanyCache _companyCache;
        private readonly IAccountAppService _accountAppService;
        private readonly IImpersonationManager _impersonationManager;
        private readonly IEmailSender _emailSender;
        private readonly IPasswordComplexitySettingStore _passwordComplexitySettingStore;
        private readonly ISessionAppService _sessionAppService;
        private readonly ISettingManager _settingManager;
        private readonly IUserDelegationManager _userDelegationManager;
        private readonly ICachedUniqueKeyPerUser _cachedUniqueKeyPerUser;
        private readonly IProfileAppService _profileAppService;
        private readonly LoginViewModel _loginModel = new();

        public AccountViewModel(
            UserManager userManager, 
            CompanyManager companyManager, 
            IMultiCompanyConfig multiCompanyConfig, 
            IAppNotifier appNotifier, 
            KontecgLoginResultTypeHelper kontecgLoginResultTypeHelper, 
            IUserLinkManager userLinkManager, 
            LogInManager logInManager, 
            UserClaimsPrincipalFactory claimsPrincipalFactory, 
            IPerRequestSessionCache sessionCache, 
            ICompanyCache companyCache, 
            IAccountAppService accountAppService, 
            IImpersonationManager impersonationManager, 
            IEmailSender emailSender, 
            IPasswordComplexitySettingStore passwordComplexitySettingStore,
            ISessionAppService sessionAppService, 
            ISettingManager settingManager, 
            IUserDelegationManager userDelegationManager, 
            ICachedUniqueKeyPerUser cachedUniqueKeyPerUser, 
            IProfileAppService profileAppService)
        {
            _userManager = userManager;
            _companyManager = companyManager;
            _multiCompanyConfig = multiCompanyConfig;
            _appNotifier = appNotifier;
            _kontecgLoginResultTypeHelper = kontecgLoginResultTypeHelper;
            _userLinkManager = userLinkManager;
            _logInManager = logInManager;
            _sessionCache = sessionCache;
            _companyCache = companyCache;
            _accountAppService = accountAppService;
            _impersonationManager = impersonationManager;
            _emailSender = emailSender;
            _passwordComplexitySettingStore = passwordComplexitySettingStore;
            _sessionAppService = sessionAppService;
            _settingManager = settingManager;
            _userDelegationManager = userDelegationManager;
            _cachedUniqueKeyPerUser = cachedUniqueKeyPerUser;
            _profileAppService = profileAppService;
            _claimsPrincipalFactory = claimsPrincipalFactory;

            LocalizationSourceName = KontecgWinFormsConsts.LocalizationSourceName;
        }

        #region Login

        private sealed class LoginViewModel
        {
            public string UsernameOrEmail { get; set; }

            public string Password { get; set; }

            public string Company { get; set; }
        }

        [Required]
        public virtual string UsernameOrEmail
        {
            get => _loginModel.UsernameOrEmail;
            set
            {
                if (_loginModel.UsernameOrEmail == value) return;
                _loginModel.UsernameOrEmail = value;
                this.RaisePropertyChanged(x => x.UsernameOrEmail);
                this.RaiseCanExecuteChanged(x => x.Login());
            }
        }

        [Required]
        public virtual string Password
        {
            get => _loginModel.Password;
            set
            {
                if (_loginModel.Password == value) return;
                _loginModel.Password = value;
                this.RaisePropertyChanged(x => x.Password);
                this.RaiseCanExecuteChanged(x => x.Login());
            }
        }

        public virtual string Company
        {
            get => _loginModel.Company;
            set
            {
                if (_loginModel.Company == value) return;
                _loginModel.Company = value;
                this.RaisePropertyChanged(x => x.Company);
                this.RaiseCanExecuteChanged(x => x.Login());
            }
        }

        public event EventHandler LoginSuccess;

        [Command]
        public void Login()
        {
            try
            {
                var loginResult = GetLoginResult(UsernameOrEmail, Password, Company);
                
                if (SettingManager.GetSettingValue<bool>(AppSettings.UserManagement.AllowOneConcurrentLoginPerUser))
                    AsyncHelper.RunSync(() => _userManager.UpdateSecurityStampAsync(loginResult.User));

                if (loginResult.User.ShouldChangePasswordOnNextLogin)
                    loginResult.User.SetNewPasswordResetCode();

                var userPrincipal = _claimsPrincipalFactory.Create(loginResult.User);
                
                userPrincipal.Identities.First()
                             .AddClaim(new Claim(ClaimTypes.AuthenticationMethod, IdentityConstants.ApplicationScheme));

                Thread.CurrentPrincipal = userPrincipal;

                this.RaiseLoginSuccess();
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
                    default:
                        ex.ReThrow();
                        break;
                }
            }
        }

        public virtual bool CanLogin()
        {
            return !string.IsNullOrWhiteSpace(UsernameOrEmail) && !string.IsNullOrWhiteSpace(Password) && KontecgSession.UserId == null;
        }

        private void RaiseLoginSuccess()
        {
            LoginSuccess?.Invoke(this, EventArgs.Empty);
        }

        private KontecgLoginResult<Company, User> GetLoginResult(string usernameOrEmailAddress, string password,
            string companyName)
        {
            var loginResult = AsyncHelper.RunSync(() => _logInManager.LoginAsync(usernameOrEmailAddress, password, companyName));
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

        #region MessageBoxService

        protected IMessageBoxService MessageBoxService => this.GetService<IMessageBoxService>();

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
    }
}