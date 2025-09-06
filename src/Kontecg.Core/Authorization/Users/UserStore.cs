using Kontecg.Authorization.Roles;
using Kontecg.Domain.Repositories;
using Kontecg.Domain.Uow;
using Kontecg.Organizations;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Threading;

namespace Kontecg.Authorization.Users
{
    public class UserStore : KontecgUserStore<Role, User>
    {
        private readonly IUnitOfWorkManager _unitOfWorkManager;

        public UserStore(
            IUnitOfWorkManager unitOfWorkManager, 
            IRepository<User, long> userRepository, 
            IRepository<Role> roleRepository, 
            IRepository<UserRole, long> userRoleRepository, 
            IRepository<UserLogin, long> userLoginRepository, 
            IRepository<UserClaim, long> userClaimRepository, 
            IRepository<UserPermissionSetting, long> userPermissionSettingRepository, 
            IRepository<UserOrganizationUnit, long> userOrganizationUnitRepository, 
            IRepository<OrganizationUnitRole, long> organizationUnitRoleRepository, 
            IRepository<UserToken, long> userTokenRepository) 
            : base(
                unitOfWorkManager, 
                userRepository, 
                roleRepository, 
                userRoleRepository, 
                userLoginRepository, 
                userClaimRepository,
                userPermissionSettingRepository, 
                userOrganizationUnitRepository, 
                organizationUnitRoleRepository, 
                userTokenRepository)
        {
            _unitOfWorkManager = unitOfWorkManager;
        }

        /// <summary>
        /// Get the claims associated with the specified <paramref name="user"/> as an asynchronous operation.
        /// </summary>
        /// <param name="user">The user whose claims should be retrieved.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>A <see cref="Task{TResult}"/> that contains the claims granted to a user.</returns>
        public override async Task<IList<Claim>> GetClaimsAsync(User user,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return await _unitOfWorkManager.WithUnitOfWorkAsync(async () =>
            {
                cancellationToken.ThrowIfCancellationRequested();

                Check.NotNull(user, nameof(user));

                await UserRepository.EnsureCollectionLoadedAsync(user, u => u.Claims, cancellationToken);
                await UserRepository.EnsurePropertyLoadedAsync(user, u => u.Person, cancellationToken);

                return user.Claims.Select(c => new Claim(c.ClaimType, c.ClaimValue)).ToList();
            });
        }

        /// <summary>
        /// Get the claims associated with the specified <paramref name="user"/> as an asynchronous operation.
        /// </summary>
        /// <param name="user">The user whose claims should be retrieved.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>A <see cref="Task{TResult}"/> that contains the claims granted to a user.</returns>
        public override IList<Claim> GetClaims(User user,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return _unitOfWorkManager.WithUnitOfWork(() =>
            {
                cancellationToken.ThrowIfCancellationRequested();

                Check.NotNull(user, nameof(user));

                UserRepository.EnsureCollectionLoaded(user, u => u.Claims, cancellationToken);
                UserRepository.EnsurePropertyLoaded(user, u => u.Person, cancellationToken);

                return user.Claims.Select(c => new Claim(c.ClaimType, c.ClaimValue)).ToList();
            });
        }
    }
}
