namespace Kontecg.Authorization.Users.Profile.Dto
{
    public class UploadProfilePictureOutput
    {
        public string FileName { get; set; }

        public string FileType { get; set; }

        public string FileToken { get; set; }

        public int Width { get; set; }

        public int Height { get; set; }
    }
}
