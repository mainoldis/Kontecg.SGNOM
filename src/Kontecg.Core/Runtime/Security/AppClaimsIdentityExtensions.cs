using System;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using Kontecg.Extensions;

namespace Kontecg.Runtime.Security
{
    public static class AppClaimsIdentityExtensions
    {
        public static int? GetPersonId(this IIdentity identity)
        {
            Check.NotNull(identity, nameof(identity));

            var claimsIdentity = identity as ClaimsIdentity;

            var personIdOrNull = claimsIdentity?.Claims?.FirstOrDefault(c => c.Type == KontecgClaimTypes.PersonId);
            if (personIdOrNull == null || personIdOrNull.Value.IsNullOrWhiteSpace()) return null;

            return Convert.ToInt32(personIdOrNull.Value);
        }
    }
}
