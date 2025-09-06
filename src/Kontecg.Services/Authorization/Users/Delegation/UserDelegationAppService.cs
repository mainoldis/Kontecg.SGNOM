using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Kontecg.Application.Services.Dto;
using Kontecg.Authorization.Delegation;
using Kontecg.Authorization.Users.Delegation.Dto;
using Kontecg.Domain.Repositories;
using Kontecg.Linq.Extensions;
using Kontecg.Runtime.Session;
using Kontecg.Timing;
using Kontecg.UI;
using Microsoft.EntityFrameworkCore;

namespace Kontecg.Authorization.Users.Delegation
{
    [KontecgAuthorize]
    public class UserDelegationAppService : KontecgAppServiceBase, IUserDelegationAppService
    {
        private readonly IRepository<UserDelegation, long> _userDelegationRepository;
        private readonly IRepository<User, long> _userRepository;
        private readonly IUserDelegationManager _userDelegationManager;
        private readonly IUserDelegationConfiguration _userDelegationConfiguration;

        public UserDelegationAppService(
            IRepository<UserDelegation, long> userDelegationRepository,
            IRepository<User, long> userRepository,
            IUserDelegationManager userDelegationManager,
            IUserDelegationConfiguration userDelegationConfiguration)
        {
            _userDelegationRepository = userDelegationRepository;
            _userRepository = userRepository;
            _userDelegationManager = userDelegationManager;
            _userDelegationConfiguration = userDelegationConfiguration;
        }

        public async Task<PagedResultDto<UserDelegationDto>> GetDelegatedUsersAsync(GetUserDelegationsInput input)
        {
            CheckUserDelegationOperation();

            var query = CreateDelegatedUsersQuery(sourceUserId: KontecgSession.GetUserId(), targetUserId: null, input.Sorting);
            var totalCount = await query.CountAsync();

            var userDelegations = await query
                .Skip(input.SkipCount)
                .Take(input.MaxResultCount)
                .ToListAsync();

            return new PagedResultDto<UserDelegationDto>(
                totalCount,
                userDelegations
            );
        }

        public async Task DelegateNewUserAsync(CreateUserDelegationDto input)
        {
            if (input.TargetUserId == KontecgSession.GetUserId())
            {
                throw new UserFriendlyException(L("SelfUserDelegationErrorMessage"));
            }

            CheckUserDelegationOperation();

            var delegation = ObjectMapper.Map<UserDelegation>(input);

            delegation.CompanyId = KontecgSession.CompanyId;
            delegation.SourceUserId = KontecgSession.GetUserId();

            await _userDelegationRepository.InsertAsync(delegation);
        }

        public async Task RemoveDelegationAsync(EntityDto<long> input)
        {
            CheckUserDelegationOperation();

            await _userDelegationManager.RemoveDelegationAsync(input.Id, KontecgSession.ToUserIdentifier());
        }

        /// <summary>
        /// Returns active user delegations for current user
        /// </summary>
        /// <returns></returns>
        public async Task<List<UserDelegationDto>> GetActiveUserDelegationsAsync()
        {
            var query = CreateActiveUserDelegationsQuery(KontecgSession.GetUserId(), "username");
            query = query.Where(e => e.EndTime >= Clock.Now);
            return await query.ToListAsync();
        }

        private void CheckUserDelegationOperation()
        {
            if (KontecgSession.ImpersonatorUserId.HasValue)
            {
                throw new Exception("Cannot execute user delegation operations with an impersonated user !");
            }

            if (!_userDelegationConfiguration.IsEnabled)
            {
                throw new Exception("User delegation configuration is not enabled !");
            }
        }

        private IQueryable<UserDelegationDto> CreateDelegatedUsersQuery(long? sourceUserId, long? targetUserId, string sorting)
        {
            var query = _userDelegationRepository.GetAll()
                .WhereIf(sourceUserId.HasValue, e => e.SourceUserId == sourceUserId)
                .WhereIf(targetUserId.HasValue, e => e.TargetUserId == targetUserId);

            return (from userDelegation in query
                    join targetUser in _userRepository.GetAll() on userDelegation.TargetUserId equals targetUser.Id into targetUserJoined
                    from targetUser in targetUserJoined.DefaultIfEmpty()
                    select new UserDelegationDto
                    {
                        Id = userDelegation.Id,
                        Username = targetUser.UserName,
                        StartTime = userDelegation.StartTime,
                        EndTime = userDelegation.EndTime
                    }).OrderBy(sorting);
        }

        private IQueryable<UserDelegationDto> CreateActiveUserDelegationsQuery(long targetUserId, string sorting)
        {
            var query = _userDelegationRepository.GetAll()
                .Where(e => e.TargetUserId == targetUserId);

            return (from userDelegation in query
                    join sourceUser in _userRepository.GetAll() on userDelegation.SourceUserId equals sourceUser.Id into sourceUserJoined
                    from sourceUser in sourceUserJoined.DefaultIfEmpty()
                    select new UserDelegationDto
                    {
                        Id = userDelegation.Id,
                        Username = sourceUser.UserName,
                        StartTime = userDelegation.StartTime,
                        EndTime = userDelegation.EndTime
                    }).OrderBy(sorting);
        }
    }
}
