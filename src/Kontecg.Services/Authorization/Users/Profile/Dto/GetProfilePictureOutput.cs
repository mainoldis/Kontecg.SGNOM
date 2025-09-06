namespace Kontecg.Authorization.Users.Profile.Dto
{
    public class GetProfilePictureOutput
    {
        public GetProfilePictureOutput(string profilePicture)
        {
            ProfilePicture = profilePicture;
        }

        public string ProfilePicture { get; set; }
    }
}
