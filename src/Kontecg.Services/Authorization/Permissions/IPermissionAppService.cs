using Kontecg.Application.Services;
using Kontecg.Application.Services.Dto;
using Kontecg.Authorization.Permissions.Dto;

namespace Kontecg.Authorization.Permissions
{
    public interface IPermissionAppService : IApplicationService
    {
        ListResultDto<FlatPermissionWithLevelDto> GetAllPermissions();
    }
}
