using System.Text.RegularExpressions;
using Kontecg.Extensions;

namespace Kontecg.Validation
{
    public static class ValidationHelper
    {
        public const string EmailRegex = @"^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$";

        public const string IdentityCardRegex = @"^[0-9]{11,}$";

        public const string TimeRegex = @"^([01]?[0-9]|2[0-3]):[0-5][0-9]$";

        public const string AccountRegex = @"^[0-9]{20,}$";

        public static bool IsEmail(string value)
        {
            if (value.IsNullOrEmpty()) return false;

            var regex = new Regex(EmailRegex);
            return regex.IsMatch(value);
        }

        public static bool IsIdentityCard(string value)
        {
            if(value.IsNullOrEmpty()) return false;

            var regex = new Regex(IdentityCardRegex);
            return regex.IsMatch(value);
        }

        public static bool IsTime(string value)
        {
            if (value.IsNullOrEmpty()) return false;
            var regex = new Regex(TimeRegex);
            return regex.IsMatch(value);
        }

        public static bool IsAccount(string value)
        {
            if (value.IsNullOrEmpty()) return false;
            var regex = new Regex(AccountRegex);
            return regex.IsMatch(value);
        }
    }
}
