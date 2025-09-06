using Kontecg.Domain.Repositories;
using Kontecg.HumanResources;
using Kontecg.Organizations;
using Xunit;

namespace Kontecg.SGNOM.Tests
{
    public class Occupation_Tests : SGNOMModuleTestBase
    {
        private readonly IOccupationRepository _occupationRepository;
        private readonly IRepository<DriverLicenseDefinition> _driverLicenseRepository;

        public Occupation_Tests()
        {
            var admin = GetDefaultCompanyAdmin();
            KontecgSession.UserId = admin.Id;
            KontecgSession.CompanyId = admin.CompanyId;

            _occupationRepository = LocalIocManager.Resolve<IOccupationRepository>();
            _driverLicenseRepository = LocalIocManager.Resolve<IRepository<DriverLicenseDefinition>>();
        }

        [Fact]
        public void Try_add_driver_license_to_specific_occupation_Test()
        {
            WithUnitOfWork(KontecgSession.CompanyId, () =>
            {
                var driverOccupation = _occupationRepository.GetByCode("605003");
                var driverLicense = _driverLicenseRepository.FirstOrDefault(d => d.Category == "C");
                driverOccupation.Requirements.Add(new QualificationRequirementDefinition(driverLicense.DisplayName));

                _occupationRepository.Update(driverOccupation);
            });
        }
    }
}
