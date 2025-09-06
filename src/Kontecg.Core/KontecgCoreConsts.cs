using System;

namespace Kontecg
{
    public class KontecgCoreConsts
    {
        public const string DefaultReferenceGroup = "Kontecg";

        public const string LocalizationSourceName = "Core";

        public const string ConnectionStringName = "Default";

        public const bool MultiCompanyEnabled = true;

        public const bool AllowCompaniesToChangeEmailSettings = false;

        public const string DefaultCurrency = "CUP";

        public const string DefaultExchangeRateProvider = "BCC";

        public const int DefaultDecimalPrecision = 28;

        public const int DefaultDecimalScale = 24;

        public const string DefaultCountryCode = "192";

        public const string ExtensionsFolderName = "Extensions";

        public const string LogsFolderName = "Logs";

        public const string DefaultDataFolderName = "Data";

        /// <summary>
        ///     Default page size for paged requests.
        /// </summary>
        public const int DefaultPageSize = 500;

        /// <summary>
        ///     Default pass phrase for SimpleStringCipher decrypt/encrypt operations
        /// </summary>
        public const string DefaultPassPhrase = "gsKxGZ012HLL3MI5";

        public const int ResizedMaxProfilePictureBytesValue = 1024;

        public const int ProfilePictureBytesValue = 5242880; //5MB

        public const string TokenValidityKey = "token_validity_key";
        public const string RefreshTokenValidityKey = "refresh_token_validity_key";
        public const string SecurityStampKey = "AspNet.Identity.SecurityStamp";

        public const string TokenType = "token_type";

        public const string DateTimeOffsetFormat = "yyyy-MM-ddTHH:mm:sszzz";

        public static string UserIdentifier = "user_identifier";

        public static TimeSpan AccessTokenExpiration = TimeSpan.FromDays(1);
        public static TimeSpan RefreshTokenExpiration = TimeSpan.FromDays(365);
    }
}
