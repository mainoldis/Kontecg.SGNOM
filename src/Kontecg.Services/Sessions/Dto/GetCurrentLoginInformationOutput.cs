namespace Kontecg.Sessions.Dto
{
    public class GetCurrentLoginInformationOutput
    {
        public UserLoginInfoDto User { get; set; }

        public UserLoginInfoDto ImpersonatorUser { get; set; }

        public CompanyLoginInfoDto Company { get; set; }

        public CompanyLoginInfoDto ImpersonatorCompany { get; set; }

        public ApplicationInfoDto Application { get; set; }
    }
}
