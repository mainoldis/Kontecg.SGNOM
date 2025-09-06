using Kontecg.MultiCompany;

namespace Kontecg.Configuration.Dtos
{
    public class KontecgUserSessionConfigDto
    {
        public long? UserId { get; set; }

        public int? CompanyId { get; set; }

        public MultiCompanySides MultiCompanySide { get; set; }
    }
}
