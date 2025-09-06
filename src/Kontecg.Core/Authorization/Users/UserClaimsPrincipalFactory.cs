using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Kontecg.Authorization.Roles;
using Kontecg.Domain.Uow;
using Kontecg.Runtime.Security;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace Kontecg.Authorization.Users
{
    public class UserClaimsPrincipalFactory : KontecgUserClaimsPrincipalFactory<User, Role>
    {
        public UserClaimsPrincipalFactory(
            UserManager userManager,
            RoleManager roleManager,
            IOptions<IdentityOptions> options,
            IUnitOfWorkManager unitOfWorkManager)
            : base(
                userManager,
                roleManager,
                options,
                unitOfWorkManager)
        {
        }

        public override async Task<ClaimsPrincipal> CreateAsync(User user)
        {

            var claim = await base.CreateAsync(user);
            if (user.PersonId.HasValue)
            {
                claim.Identities.First().AddClaim(new Claim(KontecgClaimTypes.PersonId, user.PersonId.Value.ToString(CultureInfo.InvariantCulture)));
                claim.Identities.First().AddClaim(new Claim(ClaimTypes.DateOfBirth, user.Person.BirthDate.ToString(CultureInfo.InvariantCulture)));
                claim.Identities.First().AddClaim(new Claim(ClaimTypes.Gender, user.Person.Gender.ToString()));

                claim.Identities.First().AddClaim(new Claim(ClaimTypes.Country, user.Person.OfficialAddress?.Country ?? ""));
                claim.Identities.First().AddClaim(new Claim(ClaimTypes.StateOrProvince, user.Person.OfficialAddress?.State ?? ""));
                claim.Identities.First().AddClaim(new Claim(ClaimTypes.Locality, user.Person.OfficialAddress?.City ?? ""));
                claim.Identities.First().AddClaim(new Claim(ClaimTypes.StreetAddress, user.Person.OfficialAddress?.Street ?? ""));
                claim.Identities.First().AddClaim(new Claim(ClaimTypes.PostalCode, user.Person.OfficialAddress?.ZipCode ?? ""));

                return claim;
            }

            claim.Identities.First().AddClaim(new Claim(KontecgClaimTypes.PersonId, ""));
            return claim;
        }

        public override ClaimsPrincipal Create(User user)
        {
            var claim = base.Create(user);
            if (user.PersonId.HasValue)
            {
                claim.Identities.First().AddClaim(new Claim(KontecgClaimTypes.PersonId, user.PersonId.Value.ToString(CultureInfo.InvariantCulture)));
                claim.Identities.First().AddClaim(new Claim(ClaimTypes.DateOfBirth, user.Person.BirthDate.ToString(CultureInfo.InvariantCulture)));
                claim.Identities.First().AddClaim(new Claim(ClaimTypes.Gender, user.Person.Gender.ToString()));

                claim.Identities.First().AddClaim(new Claim(ClaimTypes.Country, user.Person.OfficialAddress?.Country ?? ""));
                claim.Identities.First().AddClaim(new Claim(ClaimTypes.StateOrProvince, user.Person.OfficialAddress?.State ?? ""));
                claim.Identities.First().AddClaim(new Claim(ClaimTypes.Locality, user.Person.OfficialAddress?.City ?? ""));
                claim.Identities.First().AddClaim(new Claim(ClaimTypes.StreetAddress, user.Person.OfficialAddress?.Street ?? ""));
                claim.Identities.First().AddClaim(new Claim(ClaimTypes.PostalCode, user.Person.OfficialAddress?.ZipCode ?? ""));
                return claim;
            }

            claim.Identities.First().AddClaim(new Claim(KontecgClaimTypes.PersonId, ""));
            return claim;
        }
    }
}
