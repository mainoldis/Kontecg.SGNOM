namespace Kontecg.Authorization.Accounts.Dto
{
    public class IsCompanyAvailableOutput
    {
        public IsCompanyAvailableOutput()
        {
        }

        public IsCompanyAvailableOutput(CompanyAvailabilityState state, int? companyId = null)
        {
            State = state;
            CompanyId = companyId;
        }

        public CompanyAvailabilityState State { get; set; }

        public int? CompanyId { get; set; }
    }
}
