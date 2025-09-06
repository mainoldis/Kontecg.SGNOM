using System.Linq;
using Castle.Core.Logging;
using Kontecg.Authorization.Roles;
using Kontecg.Authorization.Users;
using Kontecg.EFCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Kontecg.Migrations.Seed.Companies
{
    public class CompanyRoleAndUserBuilder
    {
        private readonly KontecgCoreDbContext _context;
        private readonly int _companyId;
        private readonly ILogger _logger;

        public CompanyRoleAndUserBuilder(KontecgCoreDbContext context, int companyId, ILogger logger)
        {
            _context = context;
            _companyId = companyId;
            _logger = logger;
        }

        public void Create()
        {
            CreateRolesAndUsers();
            _logger.InfoFormat("Users and Roles for companyId {0} created.", _companyId);
        }

        private void CreateRolesAndUsers()
        {
            //Admin role
            var adminRole = _context.Roles.IgnoreQueryFilters().FirstOrDefault(r => r.CompanyId == _companyId && r.Name == StaticRoleNames.Admin);
            if (adminRole == null)
            {
                adminRole = _context.Roles.Add(new Role(_companyId, StaticRoleNames.Admin, StaticRoleNames.Admin) { IsStatic = true }).Entity;
                _context.SaveChanges();
            }

            //Public role
            var publicRole = _context.Roles.IgnoreQueryFilters().FirstOrDefault(r => r.CompanyId == _companyId && r.Name == StaticRoleNames.Public);
            if (publicRole == null)
            {
                publicRole = _context.Roles.Add(new Role(_companyId, StaticRoleNames.Public, StaticRoleNames.Public) { IsStatic = true, IsDefault = true }).Entity;
                _context.SaveChanges();
            }

            //admin user
            var adminUser = _context.Users.IgnoreQueryFilters().FirstOrDefault(u => u.CompanyId == _companyId && u.UserName == KontecgUserBase.AdminUserName);
            if (adminUser == null)
            {
                adminUser = User.CreateCompanyAdminUser(_companyId, "app@ecg.moa.minem.cu", "Soporte", "Técnico");
                adminUser.Password = new PasswordHasher<User>(new OptionsWrapper<PasswordHasherOptions>(new PasswordHasherOptions())).HashPassword(adminUser, User.DefaultPassword);
                adminUser.IsEmailConfirmed = true;
                adminUser.ShouldChangePasswordOnNextLogin = false;
                adminUser.IsActive = true;

                _context.Users.Add(adminUser);
                _context.SaveChanges();

                //Assign Admin role to admin user
                _context.UserRoles.Add(new UserRole(_companyId, adminUser.Id, adminRole.Id));
                _context.SaveChanges();

                //User account of admin user
                if (_companyId == 1)
                {
                    _context.UserAccounts.Add(new UserAccount
                    {
                        CompanyId = _companyId,
                        UserId = adminUser.Id,
                        UserName = KontecgUserBase.AdminUserName,
                        EmailAddress = adminUser.EmailAddress
                    });
                    _context.SaveChanges();
                }
            }
        }
    }
}
