using Kontecg.Organizations;
using Kontecg.Organizations.Dto;
using Shouldly;
using Xunit;

namespace Kontecg.Desktop.Tests
{
    public class OrganizationUnits_Tests : DesktopTestModuleTestBase
    {
        public OrganizationUnits_Tests()
        {
            var admin = GetDefaultCompanyAdmin();
            KontecgSession.CompanyId = admin.CompanyId;
            KontecgSession.UserId = admin.Id;
        }

        [Fact]
        public async void Create_organization_unit_Test()
        {
            var oum = LocalIocManager.Resolve<IOrganizationUnitAppService>();

            var ou = await oum.CreateOrganizationUnitAsync(new CreateOrganizationUnitInput{DisplayName = "Áreas"});

            ou.DisplayName.ShouldBe("Áreas");

            var second = await oum.CreateOrganizationUnitAsync(new CreateOrganizationUnitInput
                {DisplayName = "Dirección General", ParentId = ou.Id});

            second.ShouldNotBeNull();

        }

        [Fact]
        public async void Update_organization_unit_Test()
        {
            var oum = LocalIocManager.Resolve<IOrganizationUnitAppService>();

            var organizationUnitDto = (await oum.GetOrganizationUnitsAsync()).Items[0];

            organizationUnitDto.ShouldNotBeNull();

            var ouDto = await oum.UpdateOrganizationUnitAsync(new UpdateOrganizationUnitInput
                {DisplayName = "Dirección Económica", Id = organizationUnitDto.Id});

            ouDto.DisplayName.ShouldBe("Dirección Económica");
        }

        [Fact]
        public void Create_workplace_unit_Test()
        {
            var oum = LocalIocManager.Resolve<IWorkPlaceUnitAppService>();

            var ou = oum.CreateWorkPlaceUnit(new CreateWorkPlaceUnitInput { DisplayName = "Area de trabajo", ClassificationId = 13});

            ou.DisplayName.ShouldBe("Area de trabajo");

            var second = oum.CreateWorkPlaceUnit(new CreateWorkPlaceUnitInput { DisplayName = "Sub-área 1", ParentId = ou.Id, ClassificationId = 13 });

            second.ShouldNotBeNull();
        }

        [Fact]
        public async void Update_workplace_unit_Test()
        {
            var oum = LocalIocManager.Resolve<IWorkPlaceUnitAppService>();

            var ouDto = await oum.GetByCodeAsync("0201");

            ouDto.ShouldNotBeNull();

            var ouUpdatedDto = await oum.UpdateWorkPlaceUnitAsync(
                new UpdateWorkPlaceUnitInput
                {
                    DisplayName = "Dirección Económica", 
                    Acronym = "Esto es una descripción que no estaba",
                    ClassificationId = ouDto.ClassificationId,
                    Id = ouDto.Id
                });

            ouUpdatedDto.DisplayName.ShouldBe("Dirección Económica");
        }

        [Fact]
        public async void Delete_workplace_unit_Test()
        {
            var oum = LocalIocManager.Resolve<IWorkPlaceUnitAppService>();

            var ouDto = await oum.GetByCodeAsync("02");

            ouDto.ShouldNotBeNull();
            await oum.DeleteWorkPlaceUnitAsync(ouDto);
        }

        [Fact]
        public async void Get_persons_from_organization_unit_Test()
        {
            var oum = LocalIocManager.Resolve<IOrganizationUnitAppService>();
            var pagedResultDto = await oum.FindPersonsAsync(new FindOrganizationUnitPersonsInput() {OrganizationUnitId = 19});
            pagedResultDto.TotalCount.ShouldBeGreaterThan(0);
        }

        [Fact]
        public async void Get_all_organization_units_Test()
        {
            var oum = LocalIocManager.Resolve<IOrganizationUnitAppService>();
            var organizationUnits = await oum.GetOrganizationUnitsAsync();
            organizationUnits.Items.ShouldNotBeEmpty();
        }

        [Fact]
        public void Update_all_max_approved_organization_units_Test()
        {
            var oum = LocalIocManager.Resolve<WorkPlaceUnitManager>();
            oum.UpdateMaxMembersApproved();
        }
    }
}
