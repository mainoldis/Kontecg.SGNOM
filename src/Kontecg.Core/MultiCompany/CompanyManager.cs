using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using Kontecg.Application.Features;
using Kontecg.Authorization.Roles;
using Kontecg.Authorization.Users;
using Kontecg.Domain.Repositories;
using Kontecg.Domain.Uow;
using Kontecg.Extensions;
using Kontecg.HumanResources;
using Kontecg.IdentityFramework;
using Kontecg.Notifications;
using Kontecg.Runtime.Security;
using Kontecg.Runtime.Session;
using Microsoft.AspNetCore.Identity;

namespace Kontecg.MultiCompany
{
    /// <summary>
    ///     Company manager.
    /// </summary>
    public class CompanyManager : KontecgCompanyManager<Company, User>
    {
        private readonly IKontecgDbMigrator _kontecgDbMigrator;
        private readonly INotificationSubscriptionManager _notificationSubscriptionManager;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly RoleManager _roleManager;

        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly UserManager _userManager;

        public CompanyManager(
            IRepository<Company> companyRepository,
            IRepository<CompanyFeatureSetting, long> companyFeatureRepository,
            IUnitOfWorkManager unitOfWorkManager,
            RoleManager roleManager,
            UserManager userManager,
            INotificationSubscriptionManager notificationSubscriptionManager,
            IKontecgFeatureValueStore featureValueStore,
            IKontecgDbMigrator kontecgDbMigrator,
            IPasswordHasher<User> passwordHasher) : base(
            companyRepository,
            companyFeatureRepository,
            featureValueStore
        )
        {
            KontecgSession = NullKontecgSession.Instance;

            _unitOfWorkManager = unitOfWorkManager;
            _roleManager = roleManager;
            _userManager = userManager;
            _notificationSubscriptionManager = notificationSubscriptionManager;
            _kontecgDbMigrator = kontecgDbMigrator;
            _passwordHasher = passwordHasher;
        }

        public IKontecgSession KontecgSession { get; set; }

        public async Task<int> CreateWithAdminUserAsync(
            string companyName,
            string name,
            string reup,
            string organism,
            Address address,
            string adminPassword,
            string adminEmailAddress,
            string connectionString,
            bool isActive,
            bool shouldChangePasswordOnNextLogin)
        {
            int newCompanyId;
            long newAdminId;

            using (var uow = _unitOfWorkManager.Begin(TransactionScopeOption.RequiresNew))
            {
                //Create company
                var company = new Company(companyName, name, reup, address, organism)
                {
                    IsActive = isActive,
                    ConnectionString = connectionString.IsNullOrWhiteSpace()
                        ? null
                        : SimpleStringCipher.Instance.Encrypt(connectionString)
                };

                await CreateAsync(company);
                await _unitOfWorkManager.Current.SaveChangesAsync(); //To get new company's id.

                //Create company database
                _kontecgDbMigrator.CreateOrMigrateForCompany(company);

                //We are working entities of new company, so changing company filter
                using (_unitOfWorkManager.Current.SetCompanyId(company.Id))
                {
                    //Create static roles for new company
                    CheckErrors(await _roleManager.CreateStaticRolesAsync(company.Id));
                    await _unitOfWorkManager.Current.SaveChangesAsync(); //To get static role ids

                    //grant all permissions to admin role
                    var adminRole = _roleManager.Roles.Single(r => r.Name == StaticRoleNames.Admin);
                    await _roleManager.GrantAllPermissionsAsync(adminRole);

                    //User role should be default
                    var userRole = _roleManager.Roles.Single(r => r.Name == StaticRoleNames.Public);
                    userRole.IsDefault = true;
                    CheckErrors(await _roleManager.UpdateAsync(userRole));

                    //Create admin user for the company
                    var adminUser = User.CreateCompanyAdminUser(company.Id, adminEmailAddress);
                    adminUser.ShouldChangePasswordOnNextLogin = shouldChangePasswordOnNextLogin;
                    adminUser.IsActive = true;

                    if (adminPassword.IsNullOrEmpty())
                    {
                        adminPassword = await _userManager.CreateRandomPasswordAsync();
                    }
                    else
                    {
                        await _userManager.InitializeOptionsAsync(KontecgSession.CompanyId);
                        foreach (var validator in _userManager.PasswordValidators)
                            CheckErrors(await validator.ValidateAsync(_userManager, adminUser, adminPassword));
                    }

                    adminUser.Password = _passwordHasher.HashPassword(adminUser, adminPassword);

                    CheckErrors(await _userManager.CreateAsync(adminUser));
                    await _unitOfWorkManager.Current.SaveChangesAsync(); //To get admin user's id

                    //Assign admin user to admin role!
                    CheckErrors(await _userManager.AddToRoleAsync(adminUser, adminRole.Name));

                    await _unitOfWorkManager.Current.SaveChangesAsync();

                    newCompanyId = company.Id;
                    newAdminId = adminUser.Id;
                }

                await uow.CompleteAsync();
            }

            //Used a second UOW since UOW above sets some permissions and _notificationSubscriptionManager.SubscribeToAllAvailableNotificationsAsync needs these permissions to be saved.
            using (var uow = _unitOfWorkManager.Begin(TransactionScopeOption.RequiresNew))
            {
                using (_unitOfWorkManager.Current.SetCompanyId(newCompanyId))
                {
                    await _notificationSubscriptionManager.SubscribeToAllAvailableNotificationsAsync(
                        new UserIdentifier(newCompanyId, newAdminId));
                    await _unitOfWorkManager.Current.SaveChangesAsync();
                    await uow.CompleteAsync();
                }
            }

            return newCompanyId;
        }

        protected virtual void CheckErrors(IdentityResult identityResult)
        {
            identityResult.CheckErrors(LocalizationManager);
        }
    }
}
