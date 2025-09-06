using System.Linq;
using Castle.Core.Logging;
using Kontecg.Authorization.Roles;
using Kontecg.Authorization.Users;
using Kontecg.EFCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Kontecg.Migrations.Seed.Host
{
    public class HostRoleAndUserBuilder
    {
        private readonly KontecgCoreDbContext _context;
        private readonly ILogger _logger;

        public HostRoleAndUserBuilder(KontecgCoreDbContext context, ILogger logger)
        {
            _context = context;
            _logger = logger;
        }

        public void Create()
        {
            CreateHostRoleAndUsers();
        }

        private void CreateHostRoleAndUsers()
        {
            //Admin role for host

            var adminRoleForHost = _context.Roles.IgnoreQueryFilters().FirstOrDefault(r => r.CompanyId == null && r.Name == StaticRoleNames.Admin);
            if (adminRoleForHost == null)
            {
                adminRoleForHost = _context.Roles.Add(new Role(null, StaticRoleNames.Admin, StaticRoleNames.Admin) { IsStatic = true, IsDefault = true }).Entity;
                _logger.InfoFormat("Role '{0}' added.", StaticRoleNames.Admin);
                _context.SaveChanges();
            }

            //admin user for host

            var adminUserForHost = _context.Users.IgnoreQueryFilters().FirstOrDefault(u => u.CompanyId == null && u.UserName == KontecgUserBase.AdminUserName);
            if (adminUserForHost == null)
            {
                var user = new User
                {
                    CompanyId = null,
                    UserName = KontecgUserBase.AdminUserName,
                    Name = "Soporte",
                    Surname = "Técnico",
                    EmailAddress = "app@ecg.moa.minem.cu",
                    IsEmailConfirmed = true,
                    ShouldChangePasswordOnNextLogin = false,
                    IsActive = true
                };

                user.Password =
                    new PasswordHasher<User>(new OptionsWrapper<PasswordHasherOptions>(new PasswordHasherOptions()))
                        .HashPassword(user, User.DefaultPassword);
                user.SetNormalizedNames();

                adminUserForHost = _context.Users.Add(user).Entity;
                _logger.InfoFormat("User '{0}' added.", user.UserName);
                _context.SaveChanges();

                //Assign Admin role to admin user
                _context.UserRoles.Add(new UserRole(null, adminUserForHost.Id, adminRoleForHost.Id));
                _logger.InfoFormat("Role '{0}' granted to '{1}'.", adminRoleForHost, user.UserName);
                _context.SaveChanges();

                //User account of admin user
                _context.UserAccounts.Add(new UserAccount
                {
                    CompanyId = null,
                    UserId = adminUserForHost.Id,
                    UserName = KontecgUserBase.AdminUserName,
                    EmailAddress = adminUserForHost.EmailAddress
                });

                _logger.InfoFormat("User account for '{0}' created.", user.UserName);

                _context.SaveChanges();
            }
        }
    }
}
