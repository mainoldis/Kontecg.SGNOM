namespace Kontecg.Sessions.Dto
{
    public class UpdateUserSignInTokenOutput
    {
        public string SignInToken { get; set; }

        public string EncodedUserId { get; set; }

        public string EncodedCompanyId { get; set; }
    }
}
