using System.DirectoryServices.AccountManagement;
using System.Threading.Tasks;
using Castle.Core.Logging;
using Kontecg.Authorization.Users;
using Kontecg.Baseline.Ldap.Authentication;
using Kontecg.Baseline.Ldap.Configuration;
using Kontecg.MultiCompany;
using Kontecg.Threading;

namespace Kontecg.Authorization.Ldap
{
    public class LdapAuthenticationSource : LdapAuthenticationSource<Company, User>
    {
        public ILogger Logger { get; set; }

        public LdapAuthenticationSource(ILdapSettings settings, IKontecgLdapModuleConfig ldapModuleConfig)
            : base(settings, ldapModuleConfig)
        {
            Logger = NullLogger.Instance;
        }

        /// <inheritdoc />
        public override Task<bool> TryAuthenticateAsync(string userNameOrEmailAddress, string plainPassword, Company company)
        {
            try
            {
                return base.TryAuthenticateAsync(userNameOrEmailAddress, plainPassword, company);
            }
            catch (PrincipalServerDownException ex)
            {
                Logger.Warn(ex.Message, ex);
            }

            return Task.FromResult(false);
        }

        /// <inheritdoc />
        public override bool TryAuthenticate(string userNameOrEmailAddress, string plainPassword, Company company)
        {
            try
            {
                return AsyncHelper.RunSync(() => base.TryAuthenticateAsync(userNameOrEmailAddress, plainPassword, company));
            }
            catch (PrincipalServerDownException ex)
            {
                Logger.Warn(ex.Message, ex);
            }

            return false;
        }

        public override User CreateUser(string userNameOrEmailAddress, Company company)
        {
            return AsyncHelper.RunSync(() => CreateUserAsync(userNameOrEmailAddress, company));
        }

        public override void UpdateUser(User user, Company company)
        {
            AsyncHelper.RunSync(() => UpdateUserAsync(user, company));
        }
    }
}
