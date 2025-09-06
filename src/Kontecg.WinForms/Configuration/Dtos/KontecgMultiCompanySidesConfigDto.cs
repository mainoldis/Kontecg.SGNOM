using Kontecg.MultiCompany;

namespace Kontecg.Configuration.Dtos
{
    public class KontecgMultiCompanySidesConfigDto
    {
        public MultiCompanySides Host { get; private set; }

        public MultiCompanySides Company { get; private set; }

        public KontecgMultiCompanySidesConfigDto()
        {
            Host = MultiCompanySides.Host;
            Company = MultiCompanySides.Company;
        }
    }
}
