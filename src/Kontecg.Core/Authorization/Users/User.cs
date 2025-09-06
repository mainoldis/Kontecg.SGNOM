using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Kontecg.Extensions;
using Kontecg.HumanResources;
using Kontecg.Timing;

namespace Kontecg.Authorization.Users
{
    public class User : KontecgUser<User>
    {
        public const string DefaultPassword = "admin";

        //Can add application specific user properties here

        public User()
        {
            IsLockoutEnabled = true;
        }

        public virtual Guid? ProfilePictureId { get; set; }

        public virtual long? PersonId { get; set; }

        [ForeignKey("PersonId")]
        public virtual Person Person { get; set; }

        public virtual bool ShouldChangePasswordOnNextLogin { get; set; }

        public virtual DateTime? SignInTokenExpireTimeUtc { get; set; }

        public virtual string SignInToken { get; set; }

        public virtual DateTime? LastLoginTime { get; set; }

        public virtual List<UserOrganizationUnit> OrganizationUnits { get; set; }

        /// <summary>
        ///     Creates admin <see cref="User" /> for a company.
        /// </summary>
        /// <param name="companyId">Company Id</param>
        /// <param name="emailAddress">Email address</param>
        /// <param name="name">Name of the user</param>
        /// <param name="surname">Surname of the user</param>
        /// <returns>Created <see cref="User" /> object</returns>
        public static User CreateCompanyAdminUser(int companyId, string emailAddress, string name = null, string surname = null)
        {
            var user = new User
            {
                CompanyId = companyId,
                UserName = AdminUserName,
                Name = string.IsNullOrWhiteSpace(name) ? AdminUserName : name,
                Surname = string.IsNullOrWhiteSpace(surname) ? AdminUserName : surname,
                EmailAddress = emailAddress,
                Roles = new List<UserRole>(),
                OrganizationUnits = new List<UserOrganizationUnit>()
            };

            user.SetNormalizedNames();

            return user;
        }

        public override void SetNewPasswordResetCode()
        {
            /* This reset code is intentionally kept short.
             * It should be short and easy to enter in a mobile application, where user can not click a link.
             */
            PasswordResetCode = Guid.NewGuid().ToString("N").Truncate(10).ToUpperInvariant();
        }

        public void Unlock()
        {
            AccessFailedCount = 0;
            LockoutEndDateUtc = null;
        }

        public void SetSignInToken()
        {
            SignInToken = Guid.NewGuid().ToString();
            SignInTokenExpireTimeUtc = Clock.Now.AddMinutes(1).ToUniversalTime();
        }
    }
}
