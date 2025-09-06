using Castle.Core.Logging;
using Kontecg.Dependency;
using System.Threading.Tasks;
using Kontecg.Authorization;
using Kontecg.Authorization.Users;
using System;
using System.Linq;
using System.Security.Claims;
using Kontecg.MultiCompany;
using Microsoft.AspNetCore.Identity;

namespace Kontecg.SampleApp
{
    public class LoginExecuter : ITransientDependency
    {
        private readonly LogInManager _logInManager;
        private readonly KontecgLoginResultTypeHelper _typeHelper;
        private readonly UserClaimsPrincipalFactory _claimsPrincipalFactory;

        public LoginExecuter(
            LogInManager logInManager, 
            KontecgLoginResultTypeHelper typeHelper, 
            UserClaimsPrincipalFactory claimsPrincipalFactory)
        {
            _logInManager = logInManager;
            _typeHelper = typeHelper;
            _claimsPrincipalFactory = claimsPrincipalFactory;

            Logger = NullLogger.Instance;
        }

        public ILogger Logger { get; set; }

        public async Task<bool> RunAsync(bool withSpecificLogin)
        {
            KontecgLoginResult<Company, User> loginResult;

            if (!withSpecificLogin)
            {
                Logger.Info($"Login with '{KontecgUserBase.AdminUserName}' host admin user.");
                loginResult = await _logInManager.LoginAsync(KontecgUserBase.AdminUserName, "admin");

                if (loginResult.Result != KontecgLoginResultType.Success)
                {
                    Logger.Error(_typeHelper.CreateLocalizedMessageForFailedLoginAttempt(loginResult, KontecgUserBase.AdminUserName, null));
                    return false;
                }
            }
            else
            {
                Logger.Info("Login with concrete user.");
                Logger.Info("Username?");
                var username = Console.ReadLine();
                Logger.Info("Password?");
                var password = Console.ReadLine();
                Logger.Info("Company?");
                var company = Console.ReadLine();

                loginResult = await _logInManager.LoginAsync(username, password, company);

                if (loginResult.Result != KontecgLoginResultType.Success)
                {
                    Logger.Error(_typeHelper.CreateLocalizedMessageForFailedLoginAttempt(loginResult, username, company));
                    return false;
                }
            }

            var userPrincipal = await _claimsPrincipalFactory.CreateAsync(loginResult.User);

            userPrincipal.Identities.First()
                         .AddClaim(new Claim(ClaimTypes.AuthenticationMethod, IdentityConstants.ApplicationScheme));

            AppDomain.CurrentDomain.SetThreadPrincipal(userPrincipal);

            return true;
        }
    }
}