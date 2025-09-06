using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kontecg.Application.Services.Dto;
using Kontecg.Auditing;
using Kontecg.Authorization.Users.Dto;
using Kontecg.Domain.Repositories;
using Kontecg.Runtime.Session;
using Microsoft.EntityFrameworkCore;

namespace Kontecg.Authorization.Users
{
    [KontecgAuthorize]
    public class UserLoginAppService : KontecgAppServiceBase, IUserLoginAppService
    {
        private readonly IRepository<UserLoginAttempt, long> _userLoginAttemptRepository;

        public UserLoginAppService(IRepository<UserLoginAttempt, long> userLoginAttemptRepository)
        {
            _userLoginAttemptRepository = userLoginAttemptRepository;
        }

        [DisableAuditing]
        public async Task<ListResultDto<UserLoginAttemptDto>> GetRecentUserLoginAttempts()
        {
            var userId = KontecgSession.GetUserId();

            var loginAttempts = await _userLoginAttemptRepository.GetAll()
                .Where(la => la.UserId == userId)
                .OrderByDescending(la => la.CreationTime)
                .Take(KontecgCoreConsts.DefaultPageSize)
                .ToListAsync();

            return new ListResultDto<UserLoginAttemptDto>(ObjectMapper.Map<List<UserLoginAttemptDto>>(loginAttempts));
        }
    }
}
