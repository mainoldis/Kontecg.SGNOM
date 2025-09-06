namespace Kontecg.Configuration
{
    /// <summary>
    ///     Defines string constants for setting names in the application.
    ///     See <see cref="AppSettingProvider" /> for setting definitions.
    /// </summary>
    public static class AppSettings
    {
        public static class HostManagement
        {
            public const string DefaultCountry = "App.HostManagement.DefaultCountry";
            public const string DefaultState = "App.HostManagement.DefaultState";
            public const string DefaultCity = "App.HostManagement.DefaultCity";
        }

        public static class CompanyManagement
        {
            public const string AllowSelfRegistration = "App.CompanyManagement.AllowSelfRegistration";

            public const string IsNewRegisteredCompanyActiveByDefault =
                "App.CompanyManagement.IsNewRegisteredCompanyActiveByDefault";

            public const string BillingLegalName = "App.CompanyManagement.BillingLegalName";
            public const string BillingAddress = "App.CompanyManagement.BillingAddress";
        }

        public static class UserManagement
        {
            public const string IsPersonRequiredForLogin = "App.UserManagement.IsPersonRequiredForLogin";

            public const string AllowSelfRegistration = "App.UserManagement.AllowSelfRegistration";

            public const string IsNewRegisteredUserActiveByDefault = "App.UserManagement.IsNewRegisteredUserActiveByDefault";

            public const string AllowOneConcurrentLoginPerUser = "App.UserManagement.AllowOneConcurrentLoginPerUser";

            public const string UseBlobStorageProfilePicture = "App.UserManagement.UseBlobStorageProfilePicture";

            public const string UserExpirationDayCount = "App.UserManagement.UserExpirationDayCount";

            public static class SessionTimeOut
            {
                public const string IsEnabled = "App.UserManagement.SessionTimeOut.IsEnabled";
                public const string TimeOutSecond = "App.UserManagement.SessionTimeOut.TimeOutSecond";

                public const string ShowTimeOutNotificationSecond =
                    "App.UserManagement.SessionTimeOut.ShowTimeOutNotificationSecond";

                public const string ShowLockScreenWhenTimedOut =
                    "App.UserManagement.SessionTimeOut.ShowLockScreenWhenTimedOut";
            }

            public static class Password
            {
                public const string EnableCheckingLastXPasswordWhenPasswordChange = "App.UserManagement.EnableCheckingLastXPasswordWhenPasswordChange";
                public const string CheckingLastXPasswordCount = "App.UserManagement.CheckingLastXPasswordCount";

                public const string EnablePasswordExpiration = "App.UserManagement.EnablePasswordExpiration";
                public const string PasswordExpirationDayCount = "App.UserManagement.PasswordExpirationDayCount";
                public const string PasswordResetCodeExpirationHours = "App.UserManagement.PasswordResetCodeExpirationHours";
            }
        }

        public static class PersonManagement
        {
            public const string MustHavePhoto = "App.PersonManagement.MustHavePhoto";
            public const string MustHaveAddress = "App.PersonManagement.MustHaveAddress";
            public const string MustHaveEtnia = "App.PersonManagement.MustHaveEtnia";
            public const string MustHaveClothingSizes = "App.PersonManagement.MustHaveClothingSizes";
        }

        public static class CurrencyManagement
        {
            public const string ScopeForCompanyOnExchangeRate = "App.CurrencyManagement.ScopeForCompanyOnExchangeRate";
            public const string BaseCurrency = "App.CurrencyManagement.BaseCurrency";
            public const string AllowedCurrencies = "App.CurrencyManagement.AllowedCurrencies";
        }

        public static class Email
        {
            public const string UseHostDefaultEmailSettings = "App.Email.UseHostDefaultEmailSettings";
        }

        public static class Timing
        {
            public const string OpenModeManagement = "App.Timing.OpenModeManagement";
        }
    }
}
