using System;
using Kontecg.Authorization.Users;
using Kontecg.Dependency;
using Kontecg.Localization;
using Kontecg.MultiCompany;

namespace Kontecg.Authorization
{
    public class KontecgLoginResultTypeHelper : KontecgCoreServiceBase, ITransientDependency
    {
        /// <inheritdoc />
        public KontecgLoginResultTypeHelper()
        {
            LocalizationSourceName = KontecgCoreConsts.LocalizationSourceName;
        }

        public Exception CreateExceptionForFailedLoginAttempt(KontecgLoginResult<Company, User> loginResult,
            string usernameOrEmailAddress, string companyName)
        {
            switch (loginResult.Result)
            {
                case KontecgLoginResultType.Success:
                    return new Exception("Don't call this method with a success result!");
                case KontecgLoginResultType.InvalidUserNameOrEmailAddress:
                case KontecgLoginResultType.InvalidPassword:
                    return new KontecgAuthorizationException(L("InvalidUserNameOrPassword"));
                case KontecgLoginResultType.InvalidCompanyName:
                    return new KontecgAuthorizationException(L("ThereIsNoCompanyDefinedWithName{0}", companyName));
                case KontecgLoginResultType.CompanyIsNotActive:
                    return new KontecgAuthorizationException(L("CompanyIsNotActive", companyName));
                case KontecgLoginResultType.UserIsNotActive:
                    return new KontecgAuthorizationException(L("UserIsNotActiveAndCanNotLogin",
                        usernameOrEmailAddress));
                case KontecgLoginResultType.UserEmailIsNotConfirmed:
                    return new KontecgAuthorizationException(L("UserEmailIsNotConfirmedAndCanNotLogin"));
                case KontecgLoginResultType.LockedOut:
                    return new KontecgAuthorizationException(L("UserLockedOutMessage"));
                case KontecgLoginResultType.FailedForOtherReason:
                    return new KontecgAuthorizationException(loginResult.FailReason.Localize(LocalizationManager));
                default
                    : //Can not fall to default actually. But other result types can be added in the future and we may forget to handle it
                    Logger.Warn("Unhandled login fail reason: " + loginResult);
                    return new KontecgAuthorizationException(L("LoginFailed"));
            }
        }

        public string CreateLocalizedMessageForFailedLoginAttempt(KontecgLoginResult<Company, User> loginResult,
            string usernameOrEmailAddress, string companyName)
        {
            switch (loginResult.Result)
            {
                case KontecgLoginResultType.Success:
                    throw new Exception("Don't call this method with a success result!");
                case KontecgLoginResultType.InvalidUserNameOrEmailAddress:
                case KontecgLoginResultType.InvalidPassword:
                    return L("InvalidUserNameOrPassword");
                case KontecgLoginResultType.InvalidCompanyName:
                    return L("ThereIsNoCompanyDefinedWithName{0}", companyName);
                case KontecgLoginResultType.CompanyIsNotActive:
                    return L("CompanyIsNotActive", companyName);
                case KontecgLoginResultType.UserIsNotActive:
                    return L("UserIsNotActiveAndCanNotLogin", usernameOrEmailAddress);
                case KontecgLoginResultType.UserEmailIsNotConfirmed:
                    return L("UserEmailIsNotConfirmedAndCanNotLogin");
                case KontecgLoginResultType.LockedOut:
                    return L("UserLockedOutMessage");
                case KontecgLoginResultType.FailedForOtherReason:
                    return loginResult.FailReason.Localize(LocalizationManager);
                default
                    : //Can not fall to default actually. But other result types can be added in the future and we may forget to handle it
                    Logger.Warn("Unhandled login fail reason: " + loginResult.Result);
                    return L("LoginFailed");
            }
        }
    }
}
