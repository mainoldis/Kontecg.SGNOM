using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Kontecg.Accounting;
using Kontecg.Adjustments;
using Kontecg.Claims;
using Kontecg.EFCore.Utils;
using Kontecg.EFCore.ValueConverters;
using Kontecg.EFCore.ValueGenerators;
using Kontecg.EntityHistory;
using Kontecg.HistoricalData;
using Kontecg.Holidays;
using Kontecg.HumanResources;
using Kontecg.Identity;
using Kontecg.Model;
using Kontecg.Organizations;
using Kontecg.Retentions;
using Kontecg.Salary;
using Kontecg.SocialSecurity;
using Kontecg.Timing;
using Kontecg.WorkRelations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using NMoneys;

namespace Kontecg.EFCore
{
    public class SGNOMDbContext : KontecgDbContext
    {
        private static readonly MethodInfo ConfigureCommonValueConverterMethodInfo =
            typeof(SGNOMDbContext).GetMethod(nameof(ConfigureCommonValueConverter),
                BindingFlags.Instance | BindingFlags.NonPublic);

        #region Tables

        public virtual DbSet<SignOnDocument> SignOnDocuments { get; set; }

        public virtual DbSet<AdjustmentDefinition> AdjustmentDefinitions { get; set; }

        public virtual DbSet<RetentionDefinition> RetentionDefinitions { get; set; }

        public virtual DbSet<RetentionDocument> RetentionDocuments { get; set; }

        public virtual DbSet<AdjustmentDocument> AdjustmentDocuments { get; set; }

        public virtual DbSet<AdjustmentDocumentTransition> AdjustmentDocumentTransitions { get; set; }

        public virtual DbSet<AccountingDocumentSummary> AccountingDocumentSummaries { get; set; }

        public virtual DbSet<AccountingTaxSummary> AccountingTaxSummaries { get; set; }

        public virtual DbSet<QualificationRequirementDefinition> QualificationRequirementDefinition { get; set; }

        public virtual DbSet<ScholarshipLevelDefinition> ScholarshipLevelDefinitions { get; set; }

        public virtual DbSet<DriverLicenseDefinition> DriverLicenseDefinitions { get; set; }

        public virtual DbSet<LawPenaltyCauseDefinition> LawPenaltyCauseDefinitions { get; set; }

        public virtual DbSet<LawPenaltyDefinition> LawPenaltyDefinitions { get; set; }

        public virtual DbSet<PersonAccount> PersonAccounts { get; set; }

        public virtual DbSet<PersonBackgroundTime> PersonBackgroundTimes { get; set; }

        public virtual DbSet<PersonScholarship> PersonScholarships { get; set; }

        public virtual DbSet<PersonDriverLicense> PersonDriverLicenses { get; set; }

        public virtual DbSet<PersonFamilyRelationship> PersonFamilyRelationships { get; set; }

        public virtual DbSet<PersonHolidaySchedule> PersonHolidaySchedules { get; set; }

        public virtual DbSet<PersonMilitaryRecord> PersonMilitaryRecords { get; set; }

        public virtual DbSet<PersonLawPenalty> PersonLawPenalties { get; set; }

        public virtual DbSet<PersonTax> PersonTaxes { get; set; }

        public virtual DbSet<MilitaryLocationDefinition> MilitaryLocationDefinitions { get; set; }

        public virtual DbSet<DegreeDefinition> DegreeDefinitions { get; set; }

        public virtual DbSet<Responsibility> Responsibilities { get; set; }

        public virtual DbSet<TemplateDocument> TemplateDocuments { get; set; }

        public virtual DbSet<Template> Templates { get; set; }

        public virtual DbSet<TemplateJobPosition> TemplateJobPositions { get; set; }

        public virtual DbSet<ComplexityGroup> ComplexityGroups { get; set; }

        public virtual DbSet<EmploymentSummary> EmploymentSummaries { get; set; }

        public virtual DbSet<EmploymentDocument> Employments { get; set; }

        public virtual DbSet<EmploymentDocumentToGenerate> EmploymentDocumentsToGenerate { get; set; }
        
        public virtual DbSet<EmploymentPlus> EmploymentPluses { get; set; }

        public virtual DbSet<EmploymentIndex> EmploymentIndexes { get; set; }

        public virtual DbSet<Occupation> Occupations { get; set; }

        public virtual DbSet<OccupationalCategory> OccupationalCategories { get; set; }

        public virtual DbSet<WorkRegime> WorkRegimens { get; set; }

        public virtual DbSet<WorkShift> WorkShifts { get; set; }

        public virtual DbSet<PerformanceSummary> PerformanceSummaries { get; set; }

        public virtual DbSet<PerformanceDocument> PerformanceDocuments { get; set; }

        public virtual DbSet<PerformanceEvaluation> PerformanceEvaluations { get; set; }

        public virtual DbSet<PerformanceEvaluationLawPenalty> PerformanceEvaluationLawPenalties { get; set; }

        public virtual DbSet<TimeDistributionDocument> TimeDistributionDocuments { get; set; }

        public virtual DbSet<TimeDistribution> TimeDistributions { get; set; }

        public virtual DbSet<TimeDistributionPlus> TimeDistributionPluses { get; set; }

        public virtual DbSet<Incident> Incidents { get; set; }

        public virtual DbSet<PaymentDefinition> PaymentDefinitions { get; set; }

        public virtual DbSet<PlusDefinition> PlusDefinitions { get; set; }

        public virtual DbSet<HolidayDocument> HolidayDocuments { get; set; }

        public virtual DbSet<HolidayNote> HolidayNotes { get; set; }

        public virtual DbSet<HolidayNoteTransition> HolidayNoteTransitions { get; set; }

        public virtual DbSet<SubsidyDocument> SubsidyDocuments { get; set; }

        public virtual DbSet<SubsidyPaymentDefinition> SubsidyPaymentDefinitions { get; set; }

        public virtual DbSet<PartialDisability> PartialDisabilities { get; set; }

        public virtual DbSet<TemporalDisability> TemporalDisabilities { get; set; }

        public virtual DbSet<Maternity> Maternity { get; set; }

        public virtual DbSet<SubsidyNote> SubsidyNotes { get; set; }

        public virtual DbSet<SubsidyNoteTransition> SubsidyNoteTransitions { get; set; }
        
        public virtual DbSet<HolidayHistogram> HolidayHistograms { get; set; }

        public virtual DbSet<PaymentHistogram> PaymentHistograms { get; set; }

        #endregion

        #region DBFunctions

        public IQueryable<OrganizationUnitAncestor> GetOrganizationUnitAncestor(long id, int level)
            => FromExpression(() => GetOrganizationUnitAncestor(id, level));

        public IQueryable<EmploymentProvisionalAncestor> GetEmploymentProvisionalAncestor(long id)
            => FromExpression(() => GetEmploymentProvisionalAncestor(id));

        #endregion

        public SGNOMDbContext(DbContextOptions<SGNOMDbContext> options)
            : base(options)
        {
        }

        public IEntityHistoryHelper EntityHistoryHelper { get; set; }

        public override int SaveChanges()
        {
            var changeSet = EntityHistoryHelper?.CreateEntityChangeSet(ChangeTracker.Entries().ToList());
            var result = base.SaveChanges();

            EntityHistoryHelper?.Save(changeSet);
            return result;
        }
        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var changeSet = EntityHistoryHelper?.CreateEntityChangeSet(ChangeTracker.Entries().ToList());

            var result = await base.SaveChangesAsync(cancellationToken);

            if (EntityHistoryHelper != null) await EntityHistoryHelper.SaveAsync(changeSet);

            return result;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.HasDefaultSchema("sgnom");

            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                ConfigureCommonValueConverterMethodInfo
                    .MakeGenericMethod(entityType.ClrType)
                    .Invoke(this, [modelBuilder, entityType]);
            }

            modelBuilder.Entity<PersonAccountView>()
                        .HasNoKey() // Indica que es una vista sin clave primaria
                        .ToView("vw_persons_accounts", "ext");


            modelBuilder.Entity<OrganizationUnitAncestor>()
                        .HasNoKey()
                        .ToView(null);

            modelBuilder.Entity<EmploymentProvisionalAncestor>(b =>
                        {
                            b.Property(e => e.Type).HasConversion<string>().HasMaxLength(1);
                            b.Property(e => e.SubType).HasConversion<string>().HasMaxLength(3);
                            b.HasNoKey().ToView(null);
                        });

            modelBuilder.HasDbFunction(typeof(SGNOMDbContext).GetMethod(nameof(GetOrganizationUnitAncestor)))
                        .HasSchema("dbo")
                        .HasName("GetOrganizationUnitAncestor");

            modelBuilder.HasDbFunction(typeof(SGNOMDbContext).GetMethod(nameof(GetEmploymentProvisionalAncestor)))
                        .HasSchema("dbo")
                        .HasName("GetEmploymentProvisionalAncestor");

            modelBuilder.Entity<SignOnDocument>(b =>
            {
                b.HasIndex(e => new { e.CompanyId, e.Code, e.IdentityCard });
            });

            modelBuilder.Entity<DriverLicenseDefinition>(b =>
            {
                b.HasIndex(e => new { e.DisplayName }).IsUnique();
            });

            modelBuilder.Entity<MilitaryLocationDefinition>(b =>
            {
                b.HasIndex(e => new { e.DisplayName }).IsUnique();
            });

            modelBuilder.Entity<DegreeDefinition>(b =>
            {
                b.HasIndex(e => new { e.DisplayName }).IsUnique();
            });

            modelBuilder.Entity<LawPenaltyCauseDefinition>(b =>
            {
                b.HasIndex(e => new { e.DisplayName }).IsUnique();
            });

            modelBuilder.Entity<LawPenaltyDefinition>(b =>
            {
                b.HasIndex(e => new { e.DisplayName }).IsUnique();
            });

            modelBuilder.Entity<AdjustmentDefinition>(b =>
            {
                b.Property(e => e.Currency).HasDefaultValue(CurrencyIsoCode.CUP).HasSentinel(CurrencyIsoCode.XXX).HasConversion<string>().HasMaxLength(3);
                b.HasIndex(e => new { e.CompanyId, e.Code });
            });

            modelBuilder.Entity<AdjustmentDocument>(b =>
            {
                b.Property(e => e.Code).HasValueGenerator<AdjustmentDocumentCodeGenerator>().ValueGeneratedOnAdd();
                b.Property(e => e.Status).HasConversion<string>().HasMaxLength(10);
                b.Property(e => e.Currency).HasDefaultValue(CurrencyIsoCode.CUP).HasSentinel(CurrencyIsoCode.XXX).HasConversion<string>().HasMaxLength(3);
                b.HasIndex(e => new { e.PersonId, e.CompanyId, e.DocumentDefinitionId, e.AdjustmentDefinitionId });
                b.HasIndex(e => new { e.Code });
            });

            modelBuilder.Entity<AdjustmentDocumentTransition>(b =>
            {
                b.Property(e => e.Status).HasConversion<string>().HasMaxLength(10);
                b.HasIndex(e => new { e.CompanyId, e.AdjustmentDocumentId, e.DocumentId });
            });

            modelBuilder.Entity<RetentionDefinition>(b =>
            {
                b.Property(e => e.Currency).HasDefaultValue(CurrencyIsoCode.CUP).HasSentinel(CurrencyIsoCode.XXX).HasConversion<string>().HasMaxLength(3);
                b.Property(e => e.MathType).HasConversion<string>().HasMaxLength(11);
                b.HasIndex(e => new { e.Code, e.CompanyId}).IsUnique();
            });

            modelBuilder.Entity<ClaimDocument>(b =>
            {
                b.Property(e => e.Code).HasValueGenerator<ClaimDocumentCodeGenerator>().ValueGeneratedOnAdd();
                b.HasIndex(e => new { e.PersonId, e.CompanyId, e.DocumentDefinitionId });
                b.HasIndex(e => new { e.Code });
            });

            modelBuilder.Entity<RetentionDocument>(b =>
            {
                b.Property(e => e.Currency).HasDefaultValue(CurrencyIsoCode.CUP).HasSentinel(CurrencyIsoCode.XXX).HasConversion<string>().HasMaxLength(3);
                b.HasIndex(e => new { e.PersonId, e.CompanyId, e.DocumentDefinitionId, e.RetentionDefinitionId });
                b.HasIndex(e => new { e.Code });
            });

            modelBuilder.Entity<AccountingDocumentSummary>(b =>
            {
                b.Property(e => e.Currency).HasDefaultValue(CurrencyIsoCode.CUP).HasSentinel(CurrencyIsoCode.XXX).HasConversion<string>().HasMaxLength(3);
                b.HasIndex(e => new { e.DocumentId, e.PersonId, e.CompanyId, e.GroupId});
            });

            modelBuilder.Entity<AccountingTaxSummary>(b =>
            {
                b.Property(e => e.Currency).HasDefaultValue(CurrencyIsoCode.CUP).HasSentinel(CurrencyIsoCode.XXX).HasConversion<string>().HasMaxLength(3);
                b.Property(e => e.TaxType).HasConversion<string>().HasMaxLength(25);
                b.HasIndex(e => new { e.DocumentId, e.PersonId, e.CompanyId, e.GroupId });
            });

            modelBuilder.Entity<PersonAccount>(b =>
            {
                b.HasIndex(e => new { e.PersonId });
                b.HasIndex(e => new { e.AccountNumber }).IsUnique();
                b.Property(e => e.Currency).HasDefaultValue(CurrencyIsoCode.CUP).HasSentinel(CurrencyIsoCode.XXX).HasConversion<string>().HasMaxLength(3);
            });

            modelBuilder.Entity<PersonBackgroundTime>(b =>
            {
                b.HasIndex(e => new { e.CompanyId, e.PersonId, e.GroupId, e.Year, e.Key });
                b.Property(e => e.Hours).HasPrecision(10, 2);
            });

            modelBuilder.Entity<PersonFamilyRelationship>(b =>
            {
                b.Property(e => e.Kind).HasConversion<string>().HasMaxLength(15);
            });

            modelBuilder.Entity<PersonTax>(b =>
            {
                b.HasIndex(e => new {e.CompanyId, e.PersonId, e.GroupId});
                b.Property(e => e.TaxType).HasConversion<string>().HasMaxLength(25);
                b.Property(e => e.MathType).HasConversion<string>().HasMaxLength(11);
            });

            modelBuilder.Entity<PersonHolidaySchedule>(b =>
            {
                b.HasIndex(e => new { e.PersonId, e.Year}).IsUnique();
            });

            modelBuilder.Entity<TemplateDocument>(b =>
            {
                b.HasIndex(e => new {e.DocumentDefinitionId, e.CompanyId});
            });

            modelBuilder.Entity<Template>(b =>
            {
                b.HasIndex(e => new { e.CompanyId, e.DocumentId, e.OccupationId, 
                    EducationalLevelId = e.ScholarshipLevelId, e.OrganizationUnitId });
            });

            modelBuilder.Entity<TemplateJobPosition>(b =>
            {
                b.HasIndex(e => new { e.CompanyId, e.TemplateId });
                b.HasIndex(e => new { e.CompanyId, e.TemplateId, e.OccupationId, EducationalLevelId = e.ScholarshipLevelId, e.OrganizationUnitId });
                b.Property(e => e.ByOfficial).HasDefaultValue(false);
                b.Property(e => e.ByAssignment).HasDefaultValue(false);
            });

            modelBuilder.Entity<WorkRegime>(b =>
            {
                b.Property(e => e.Type).HasConversion<string>().HasMaxLength(8);
            });

            modelBuilder.Entity<EmploymentDocument>(b =>
            {
                b.Property(e => e.Code).HasValueGenerator<EmploymentDocumentCodeGenerator>().ValueGeneratedOnAdd();
                b.Property(e => e.Contract).HasConversion<string>().HasMaxLength(1);
                b.Property(e => e.ContractSubType).HasConversion<string>().HasMaxLength(1);
                b.Property(e => e.Type).HasConversion<string>().HasMaxLength(1);
                b.Property(e => e.SubType).HasConversion<string>().HasMaxLength(3);
                b.HasIndex(e => new { e.PersonId, e.CompanyId, e.DocumentDefinitionId });
            });

            modelBuilder.Entity<EmploymentDocumentToGenerate>(b =>
            {
                b.Property(e => e.OccupationCode).HasMaxLength(Occupation.MaxCodeMaxLength);
                b.Property(e => e.Type).HasConversion<string>().HasMaxLength(1);
                b.Property(e => e.SubType).HasConversion<string>().HasMaxLength(3);
                b.Property(e => e.ExtraSummary).HasMaxLength(EmploymentDocument.MaxExtraSummaryLength);
                b.HasIndex(e => new { e.EmploymentDocumentId, e.CompanyId });
            });

            modelBuilder.Entity<EmploymentSummary>(b =>
            {
                b.Property(e => e.Type).HasConversion<string>().HasMaxLength(1);
            });

            modelBuilder.Entity<EmploymentIndex>(b =>
            {
                b.HasIndex(e => new {e.CompanyId, e.Exp}).IsUnique();
                b.Property(e => e.Contract).HasConversion<string>().HasMaxLength(1);
                b.Property(e => e.Group).HasConversion<string>().HasMaxLength(1);
            });

            modelBuilder.Entity<PaymentDefinition>(b =>
            {
                b.Property(e => e.MathType).HasConversion<string>().HasMaxLength(11);
                b.Property(e => e.WageAdjuster).HasConversion<string>().HasMaxLength(7);
                b.Property(e => e.Operation).HasConversion<string>().HasMaxLength(6);
            });

            modelBuilder.Entity<PlusDefinition>(b =>
            {
                b.Property(e => e.Scope).HasConversion<string>().HasMaxLength(10);
            });

            modelBuilder.Entity<PerformanceDocument>(b =>
            {
                b.HasIndex(e => new { e.DocumentDefinitionId, e.CompanyId, e.PeriodId });
            });

            modelBuilder.Entity<PerformanceEvaluation>(b =>
            {
                b.Property(e => e.Kind).HasConversion<string>().HasMaxLength(10);
                b.HasIndex(e => new { e.DocumentId, e.PersonId, e.EmploymentId, e.CompanyId });
            });

            modelBuilder.Entity<PerformanceEvaluationLawPenalty>(b =>
            {
                b.HasIndex(e => new { e.CompanyId, e.EvaluationId, e.LawPenaltyId });
            });

            modelBuilder.Entity<TimeDistributionDocument>(b =>
            {
                b.Property(e => e.Code).HasValueGenerator<TimeDistributionDocumentCodeGenerator>().ValueGeneratedOnAdd();
                b.HasIndex(e => new { e.DocumentDefinitionId, e.CompanyId, e.PeriodId });
            });

            modelBuilder.Entity<TimeDistribution>(b =>
            {
                b.Property(e => e.Kind).HasConversion<string>().HasMaxLength(10);
                b.Property(e => e.Status).HasConversion<string>().HasMaxLength(10);
                b.Property(e => e.Currency).HasDefaultValue(CurrencyIsoCode.CUP).HasSentinel(CurrencyIsoCode.XXX).HasConversion<string>().HasMaxLength(3);
                b.Property(e => e.Hours).HasPrecision(10, 2);
                b.Property(e => e.ReservedForHoliday).HasPrecision(10, 2);
                b.HasIndex(e => new {e.DocumentId, e.PersonId, e.EmploymentId, e.CompanyId});
            });

            modelBuilder.Entity<TimeDistributionPlus>(b =>
            {
                b.Property(e => e.Status).HasConversion<string>().HasMaxLength(10);
                b.Property(e => e.Currency).HasDefaultValue(CurrencyIsoCode.CUP).HasSentinel(CurrencyIsoCode.XXX).HasConversion<string>().HasMaxLength(3);
                b.Property(e => e.ReservedForHoliday).HasPrecision(10, 2);
                b.HasIndex(e => new { e.TimeDistributionId, e.EmploymentPlusId, e.CompanyId });
            });

            modelBuilder.Entity<Incident>(b =>
            {
                b.HasKey(k => new { k.Id, k.Start, k.PersonId });
                b.Property(e => e.Hours).HasPrecision(10, 2);
            });

            modelBuilder.Entity<HolidayDocument>(b =>
            {
                b.Property(e => e.Currency).HasDefaultValue(CurrencyIsoCode.CUP).HasSentinel(CurrencyIsoCode.XXX).HasConversion<string>().HasMaxLength(3);
                b.HasIndex(e => new { e.DocumentDefinitionId, e.PersonId, e.EmploymentId, e.CompanyId });
            });

            modelBuilder.Entity<HolidayNote>(b =>
            {
                b.Property(e => e.Status).HasConversion<string>().HasMaxLength(10);
                b.Property(e => e.Currency).HasDefaultValue(CurrencyIsoCode.CUP).HasSentinel(CurrencyIsoCode.XXX).HasConversion<string>().HasMaxLength(3);
                b.Property(e => e.Hours).HasPrecision(10, 2);
                b.HasIndex(e => new { e.DocumentId, e.CompanyId, e.PeriodId });
            });

            modelBuilder.Entity<HolidayNoteTransition>(b =>
            {
                b.Property(e => e.Status).HasConversion<string>().HasMaxLength(10);
                b.HasIndex(e => new { e.CompanyId, e.HolidayNoteId, e.DocumentId });
            });

            modelBuilder.Entity<HolidayHistogram>(b =>
            {
                b.Property(e => e.Status).HasConversion<string>().HasMaxLength(10);
                b.Property(e => e.Currency).HasDefaultValue(CurrencyIsoCode.CUP).HasSentinel(CurrencyIsoCode.XXX).HasConversion<string>().HasMaxLength(3);
                b.Property(e => e.Hours).HasPrecision(10, 2);
                b.HasIndex(e => new { e.CompanyId, e.PersonId, e.GroupId });
            });

            modelBuilder.Entity<SubsidyDocument>(b =>
            {
                b.Property(e => e.Currency).HasDefaultValue(CurrencyIsoCode.CUP).HasSentinel(CurrencyIsoCode.XXX).HasConversion<string>().HasMaxLength(3);
                b.HasIndex(e => new { e.DocumentDefinitionId, e.PersonId, e.EmploymentId, e.CompanyId });
            });

            modelBuilder.Entity<PartialDisability>(b =>
            {
                b.Property(e => e.FixedAmountPerMonth).HasConversion<KontecgCurrencyValueConverter>();
            });

            modelBuilder.Entity<SubsidyNote>(b =>
            {
                b.Property(e => e.Status).HasConversion<string>().HasMaxLength(10);
                b.Property(e => e.Currency).HasDefaultValue(CurrencyIsoCode.CUP).HasSentinel(CurrencyIsoCode.XXX).HasConversion<string>().HasMaxLength(3);
                b.Property(e => e.Hours).HasPrecision(10, 2);
                b.HasIndex(e => new { e.DocumentId, e.CompanyId, e.PeriodId });
            });

            modelBuilder.Entity<SubsidyNoteTransition>(b =>
            {
                b.Property(e => e.Status).HasConversion<string>().HasMaxLength(10);
                b.HasIndex(e => new { e.CompanyId, e.SubsidyNoteId, e.DocumentId });
            });

            modelBuilder.Entity<PaymentHistogram>(b =>
            {
                b.Property(e => e.Status).HasConversion<string>().HasMaxLength(10);
                b.Property(e => e.Currency).HasDefaultValue(CurrencyIsoCode.CUP).HasSentinel(CurrencyIsoCode.XXX).HasConversion<string>().HasMaxLength(3);
                b.HasIndex(e => new { e.CompanyId, e.PersonId, e.Year, e.Month });
            });

            foreach (var relationship in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
                relationship.DeleteBehavior = DeleteBehavior.Restrict;

            modelBuilder.Entity<EmploymentPlus>().HasOne(r => r.Employment).WithMany(d => d.Plus).OnDelete(DeleteBehavior.Cascade);
        }

        protected void ConfigureCommonValueConverter<TEntity>(ModelBuilder modelBuilder, IMutableEntityType entityType)
            where TEntity : class
        {
            if (entityType.BaseType == null &&
                !typeof(TEntity).IsDefined(typeof(OwnedAttribute), true) &&
                !entityType.IsOwned())
            {
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
}
