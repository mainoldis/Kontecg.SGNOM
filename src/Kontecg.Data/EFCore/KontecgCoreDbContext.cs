using System.Linq;
using System.Reflection;
using Kontecg.Accounting;
using Kontecg.Authorization.Delegation;
using Kontecg.Authorization.Roles;
using Kontecg.Authorization.Users;
using Kontecg.Baseline.EFCore;
using Kontecg.Currencies;
using Kontecg.EFCore.Utils;
using Kontecg.EFCore.ValueConverters;
using Kontecg.HumanResources;
using Kontecg.Identity;
using Kontecg.MultiCompany;
using Kontecg.Organizations;
using Kontecg.Storage;
using Kontecg.Timing;
using Kontecg.Workflows;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using NMoneys;

namespace Kontecg.EFCore
{
    [DefaultDbContext]
    public class KontecgCoreDbContext : KontecgBaselineDbContext<Company, Role, User, KontecgCoreDbContext>
    {
        private static readonly MethodInfo ConfigureCommonValueConverterMethodInfo =
            typeof(KontecgCoreDbContext).GetMethod(nameof(ConfigureCommonValueConverter),
                BindingFlags.Instance | BindingFlags.NonPublic);

        public KontecgCoreDbContext(DbContextOptions<KontecgCoreDbContext> options)
            : base(options)
        {
        }

        /// <summary>
        ///     ClientInfos.
        /// </summary>
        public virtual DbSet<SpecialDate> SpecialDates { get; set; }

        /// <summary>
        ///     BinaryObjects.
        /// </summary>
        public virtual DbSet<BinaryObject> BinaryObjects { get; set; }

        /// <summary>
        ///     Persons.
        /// </summary>
        public virtual DbSet<Person> Persons { get; set; }

        /// <summary>
        ///     Signs.
        /// </summary>
        public virtual DbSet<Sign> Signs { get; set; }

        /// <summary>
        ///     Countries.
        /// </summary>
        public virtual DbSet<Country> Countries { get; set; }

        /// <summary>
        ///     States.
        /// </summary>
        public virtual DbSet<State> States { get; set; }
        /// <summary>
        ///     Cities.
        /// </summary>
        public virtual DbSet<City> Cities { get; set; }

        /// <summary>
        ///     RecentPasswords.
        /// </summary>
        public virtual DbSet<RecentPassword> RecentPasswords { get; set; }

        /// <summary>
        ///     UserDelegations.
        /// </summary>
        public virtual DbSet<UserDelegation> UserDelegations { get; set; }

        /// <summary>
        ///     PersonOrganizationUnits.
        /// </summary>
        public virtual DbSet<PersonOrganizationUnit> PersonOrganizationUnits { get; set; }

        /// <summary>
        ///     AccountDefinitions.
        /// </summary>
        public virtual DbSet<AccountDefinition> AccountDefinitions { get; set; }

        /// <summary>
        ///     AccountingDocuments.
        /// </summary>
        public virtual DbSet<AccountingDocument> AccountingDocuments { get; set; }

        /// <summary>
        ///     AccountingVoucherDocuments.
        /// </summary>
        public virtual DbSet<AccountingClassifierDefinition> AccountingClassifierDefinitions { get; set; }

        /// <summary>
        ///     AccountingVoucherDocuments.
        /// </summary>
        public virtual DbSet<AccountingVoucherDocument> AccountingVoucherDocuments { get; set; }

        /// <summary>
        ///     AccountingVoucherNotes.
        /// </summary>
        public virtual DbSet<AccountingVoucherNote> AccountingVoucherNotes { get; set; }

        /// <summary>
        ///     AccountingFunctionDefinitions.
        /// </summary>
        public virtual DbSet<AccountingFunctionDefinition> AccountingFunctionDefinitions { get; set; }

        /// <summary>
        ///     AccountingFunctionDefinitionStorage.
        /// </summary>
        public virtual DbSet<AccountingFunctionDefinitionStorage> AccountingFunctionDefinitionStorage { get; set; }

        /// <summary>
        ///     ViewNames.
        /// </summary>
        public virtual DbSet<ViewName> ViewNames { get; set; }

        /// <summary>
        ///     AccountingPeriods.
        /// </summary>
        public virtual DbSet<Period> AccountingPeriods { get; set; }

        /// <summary>
        ///     CenterCostDefinitions.
        /// </summary>
        public virtual DbSet<CenterCostDefinition> CenterCostDefinitions { get; set; }

        /// <summary>
        ///     Banks.
        /// </summary>
        public virtual DbSet<Bank> Banks { get; set; }

        /// <summary>
        ///     Banks.
        /// </summary>
        public virtual DbSet<BankOffice> BankOffices { get; set; }

        /// <summary>
        ///     ExchangeRates.
        /// </summary>
        public virtual DbSet<ExchangeRateInfo> ExchangeRates { get; set; }

        /// <summary>
        ///     BillDenominations.
        /// </summary>
        public virtual DbSet<BillDenomination> BillDenominations { get; set; }

        /// <summary>
        ///     ExpenseItemDefinitions.
        /// </summary>
        public virtual DbSet<ExpenseItemDefinition> ExpenseItemDefinitions { get; set; }

        /// <summary>
        ///     WorkPlaceClassifications.
        /// </summary>
        public virtual DbSet<WorkPlaceClassification> WorkPlaceClassifications { get; set; }

        /// <summary>
        ///     WorkPlaceResponsibilities.
        /// </summary>
        public virtual DbSet<WorkPlaceResponsibility> WorkPlaceResponsibilities { get; set; }

        /// <summary>
        ///     WorkPlacePayments.
        /// </summary>
        public virtual DbSet<WorkPlacePayment> WorkPlacePayments { get; set; }

        /// <summary>
        ///     WorkPlaceUnits.
        /// </summary>
        public virtual DbSet<WorkPlaceUnit> WorkPlaceUnits { get; set; }

        /// <summary>
        ///     WorkPlaceUnitCenterCosts.
        /// </summary>
        public virtual DbSet<WorkPlaceUnitCenterCost> WorkPlaceUnitCenterCosts { get; set; }

        /// <summary>
        ///     DocumentDefinitions.
        /// </summary>
        public virtual DbSet<DocumentDefinition> DocumentDefinitions { get; set; }

        /// <summary>
        ///     DocumentSignOnDefinitions.
        /// </summary>
        public virtual DbSet<DocumentSignOnDefinition> DocumentSignOnDefinitions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ViewNameResultRecord>()
                        .HasNoKey()
                        .ToView(null);

            modelBuilder.Entity<SpecialDate>(b =>
            {
                b.Property(e => e.Cause).HasConversion<string>().HasMaxLength(50);
                b.HasIndex(e => new {e.Date, e.Cause}).IsUnique();
            });

            modelBuilder.Entity<BinaryObject>(b => b.HasIndex(e => new {e.CompanyId}));

            modelBuilder.Entity<Period>(b =>
            {
                b.Property(e => e.Status).HasConversion<string>().HasMaxLength(8);
                b.HasIndex(e => new { e.CompanyId, e.ReferenceGroup, e.Year, e.Month });
            });

            modelBuilder.Entity<Person>(b =>
            {
                b.HasIndex(e => new { e.IdentityCard }).IsUnique();
                b.Property(e => e.Gender).HasConversion<string>().HasMaxLength(1);
                b.OwnsOne<Etnia>(e => e.Etnia, ownedNavigationBuilder =>
                {
                    ownedNavigationBuilder.Property(e => e.Race).HasConversion<string>().HasMaxLength(1);
                    ownedNavigationBuilder.Property(e => e.EyeColor).HasConversion<string>().HasMaxLength(1);
                    ownedNavigationBuilder.Property(e => e.Height).HasPrecision(10, 2);
                    ownedNavigationBuilder.ToJson();
                });
                b.OwnsOne<Address>(e => e.OfficialAddress, ownedNavigationBuilder => ownedNavigationBuilder.ToJson());
                b.OwnsOne<Address>(e => e.Address, ownedNavigationBuilder => ownedNavigationBuilder.ToJson());
                b.OwnsOne<ClothingSize>(e => e.ClothingSize, ownedNavigationBuilder => ownedNavigationBuilder.ToJson());
            });

            modelBuilder.Entity<Sign>(b =>
            {
                b.HasIndex(e => new { e.Code, e.Owner, e.CompanyId }).IsUnique();
            });

            modelBuilder.Entity<UserDelegation>(b =>
            {
                b.HasIndex(e => new { e.CompanyId, e.SourceUserId });
                b.HasIndex(e => new { e.CompanyId, e.TargetUserId });
            });

            modelBuilder.Entity<AccountDefinition>(b =>
            {
                b.Property(e => e.Kind).HasConversion<string>().HasMaxLength(6);
                b.HasIndex(e => new {e.Account, e.SubAccount, e.SubControl, e.Analysis, e.Reference}).IsUnique();
            });

            modelBuilder.Entity<AccountingDocument>(b =>
            {
                b.HasIndex(e => new { e.CompanyId, AccountingPeriodId = e.PeriodId, e.DocumentDefinitionId });
            });

            modelBuilder.Entity<AccountingVoucherDocument>(b =>
            {
                b.HasIndex(e => new { e.CompanyId, e.DocumentId, e.DocumentDefinitionId });
            });

            modelBuilder.Entity<AccountingVoucherNote>(b =>
            {
                b.Property(e => e.Operation).HasConversion<string>().HasMaxLength(6);
                b.HasIndex(e => new { e.CompanyId, e.DocumentId });
                b.Property(e => e.Currency).HasDefaultValue(CurrencyIsoCode.CUP).HasSentinel(CurrencyIsoCode.XXX).HasConversion<string>().HasMaxLength(3);
            });

            modelBuilder.Entity<DocumentDefinition>(b =>
            {
                b.HasIndex(e => new { e.Code, e.Reference, e.ReferenceGroup }).IsUnique();
                b.HasMany(e => e.Views)
                 .WithMany(e => e.Documents)
                 .UsingEntity(e => e.ToTable("document_views", "cnt"));
            });

            modelBuilder.Entity<DocumentSignOnDefinition>(b =>
            {
                b.HasIndex(e => new { e.DocumentDefinitionId, e.CompanyId, e.Code });
            });

            modelBuilder.Entity<ViewName>(b =>
            {
                b.HasIndex(e => new { e.Name, e.ReferenceGroup }).IsUnique();
            });

            

            modelBuilder.Entity<Bank>(b =>
            {
                b.HasIndex(e => new { e.Name }).IsUnique();
            });

            modelBuilder.Entity<BankOffice>(b =>
            {
                b.HasIndex(e => new { e.Name }).IsUnique();
            });

            modelBuilder.Entity<ExchangeRateInfo>(b =>
            {
                b.HasIndex(e => new { e.BankId, e.From, e.To, e.Scope });
                b.Property(e => e.From).HasConversion<string>().HasMaxLength(3);
                b.Property(e => e.To).HasConversion<string>().HasMaxLength(3);
                b.Property(e => e.Rate).HasPrecision(10, 2);
            });

            modelBuilder.Entity<BillDenomination>(b =>
            {
                b.HasIndex(e => new { e.Currency, e.Bill }).IsUnique();
                b.Property(e => e.Currency).HasConversion<string>().HasMaxLength(3);
                b.Property(e => e.Bill).HasPrecision(10, 2);
            });

            modelBuilder.Entity<ExpenseItemDefinition>(b =>
            {
                b.HasIndex(e => new { e.CompanyId, Reference = e.Code}).IsUnique();
            });

            modelBuilder.Entity<CenterCostDefinition>(b =>
            {
                b.HasIndex(e => new { e.CompanyId, Reference = e.Code }).IsUnique();
            });

            modelBuilder.Entity<DocumentDefinition>(b =>
            {
                b.HasIndex(e => new { e.Code, e.Reference, e.ReferenceGroup }).IsUnique();
            });

            modelBuilder.Entity<DocumentSignOnDefinition>(b =>
            {
                b.HasIndex(e => new { e.DocumentDefinitionId, e.CompanyId, e.Code });
            });

            modelBuilder.Entity<AccountingFunctionDefinition>(b =>
            {
                b.HasIndex(e => new {e.Name}).IsUnique();
            });

            modelBuilder.Entity<Company>(b =>
            {
                b.OwnsOne<Address>(e => e.Address, ownedNavigationBuilder => ownedNavigationBuilder.ToJson());
                b.HasIndex(e => e.Reup).IsUnique();
                b.HasIndex(e => new { e.CreationTime });
            });

            modelBuilder.Entity<WorkPlacePayment>(b => b.HasIndex(e => new {e.CompanyId, e.Description}));

            foreach (var relationship in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
                relationship.DeleteBehavior = DeleteBehavior.Restrict;

            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                ConfigureCommonValueConverterMethodInfo
                    .MakeGenericMethod(entityType.ClrType)
                    .Invoke(this, [modelBuilder, entityType]);
            }
        }

        protected void ConfigureCommonValueConverter<TEntity>(ModelBuilder modelBuilder, IMutableEntityType entityType)
            where TEntity : class
        {
            if (entityType.BaseType != null ||
                typeof(TEntity).IsDefined(typeof(OwnedAttribute), true) ||
                entityType.IsOwned()) return;

            var currencyValueConverter = new KontecgCurrencyValueConverter();
            var currencyPropertyInfos = CurrencyPropertyInfoHelper.GetCurrencyPropertyInfos(typeof(TEntity));
            currencyPropertyInfos.CurrencyPropertyInfos.ForEach(property =>
            {
                modelBuilder
                    .Entity<TEntity>()
                    .Property(property.Name)
                    .HasConversion(currencyValueConverter);
            });

            var decimalProperties = entityType.GetProperties()
                                              .Where(property =>
                                                  property.ClrType == typeof(decimal) ||
                                                  property.ClrType == typeof(decimal?)
                                              ).ToList();

            decimalProperties.ForEach(property =>
            {
                property.SetPrecision(KontecgCoreConsts.DefaultDecimalPrecision);
                property.SetScale(KontecgCoreConsts.DefaultDecimalScale);
            });
        }
    }
}
