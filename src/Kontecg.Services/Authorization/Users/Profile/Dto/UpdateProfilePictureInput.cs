using System;
using System.ComponentModel.DataAnnotations;
using Kontecg.Extensions;
using Kontecg.Runtime.Validation;

namespace Kontecg.Authorization.Users.Profile.Dto
{
    public class UpdateProfilePictureInput : ICustomValidate
    {
        [MaxLength(400)] public string FileToken { get; set; }

        public int X { get; set; }

        public int Y { get; set; }

        public int Width { get; set; }

        public int Height { get; set; }

        public void AddValidationErrors(CustomValidationContext context)
        {
            if (FileToken.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(FileToken));
        }
    }
}
