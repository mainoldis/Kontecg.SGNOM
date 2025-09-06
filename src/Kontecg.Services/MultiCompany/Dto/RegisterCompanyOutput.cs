namespace Kontecg.MultiCompany.Dto
{
    public class RegisterCompanyOutput
    {
        public int CompanyId { get; set; }

        public string CompanyName { get; set; }

        public string Name { get; set; }

        public string UserName { get; set; }

        public string EmailAddress { get; set; }

        public bool IsCompanyActive { get; set; }

        public bool IsActive { get; set; }

        public bool IsEmailConfirmationRequired { get; set; }
    }
}
