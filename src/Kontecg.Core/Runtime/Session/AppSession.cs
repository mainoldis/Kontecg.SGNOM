using System;
using System.Linq;
using Kontecg.Configuration.Startup;
using Kontecg.MultiCompany;
using Kontecg.Runtime.Security;

namespace Kontecg.Runtime.Session
{
    public class AppSession : ClaimsKontecgSession, IKontecgSession
    {
        public AppSession(
            IPrincipalAccessor principalAccessor,
            IMultiCompanyConfig multiCompany,
            ICompanyResolver companyResolver,
            IAmbientScopeProvider<SessionOverride> sessionOverrideScopeProvider)
            : base(principalAccessor, multiCompany, companyResolver, sessionOverrideScopeProvider)
        {
        }

        public virtual int? PersonId
        {
            get
            {
                var personIdClaim = PrincipalAccessor.Principal?.Claims.FirstOrDefault(c => c.Type == KontecgClaimTypes.PersonId);
                if (!string.IsNullOrEmpty(personIdClaim?.Value)) return Convert.ToInt32(personIdClaim.Value);

                return null;
            }
        }
    }
}
