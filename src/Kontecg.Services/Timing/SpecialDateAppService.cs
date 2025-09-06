using Kontecg.Application.Services;
using Kontecg.Authorization;
using Kontecg.Domain.Repositories;
using Kontecg.Timing.Dto;

namespace Kontecg.Timing
{
    public class SpecialDateAppService : AsyncCrudAppService<SpecialDate,SpecialDateDto>, ISpecialDateAppService
    {
        public SpecialDateAppService(IRepository<SpecialDate> repository) 
            : base(repository)
        {
        }

        protected override string GetPermissionName => PermissionNames.AdministrationSpecialDate;

        protected override string GetAllPermissionName => PermissionNames.AdministrationSpecialDateList;

        protected override string CreatePermissionName => PermissionNames.AdministrationSpecialDateCreate;

        protected override string UpdatePermissionName => PermissionNames.AdministrationSpecialDateUpdate;

        protected override string DeletePermissionName => PermissionNames.AdministrationSpecialDateDelete;

    }
}
