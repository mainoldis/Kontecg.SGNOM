namespace Kontecg.Authorization.Users.Dto
{
    public class UnlinkUserInput
    {
        public int? CompanyId { get; set; }

        public long UserId { get; set; }

        public UserIdentifier ToUserIdentifier()
        {
            return new UserIdentifier(CompanyId, UserId);
        }
    }
}
