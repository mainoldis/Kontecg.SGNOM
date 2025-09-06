using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kontecg.Migrations
{
    /// <inheritdoc />
    public partial class Base : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "cnt");

            migrationBuilder.EnsureSchema(
                name: "aju");

            migrationBuilder.EnsureSchema(
                name: "rec");

            migrationBuilder.EnsureSchema(
                name: "est");

            migrationBuilder.EnsureSchema(
                name: "gen");

            migrationBuilder.EnsureSchema(
                name: "rel");

            migrationBuilder.EnsureSchema(
                name: "vac");

            migrationBuilder.EnsureSchema(
                name: "sal");

            migrationBuilder.EnsureSchema(
                name: "docs");

            migrationBuilder.EnsureSchema(
                name: "ret");

            migrationBuilder.EnsureSchema(
                name: "sub");

            migrationBuilder.CreateTable(
                name: "accounting_document_summaries",
                schema: "cnt",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DocumentId = table.Column<int>(type: "int", nullable: false),
                    CompanyId = table.Column<int>(type: "int", nullable: false),
                    PersonId = table.Column<long>(type: "bigint", nullable: false),
                    GroupId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Payment = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    PaymentPlus = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    ReservedForHoliday = table.Column<decimal>(type: "decimal(28,24)", precision: 28, scale: 24, nullable: false),
                    AmountReservedForHoliday = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    SocialSecurityTaxes = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    IncomeTaxes = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    Retentions = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    Currency = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false, defaultValue: "CUP"),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorUserId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_accounting_document_summaries", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "accounting_tax_summaries",
                schema: "cnt",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DocumentId = table.Column<int>(type: "int", nullable: false),
                    CompanyId = table.Column<int>(type: "int", nullable: false),
                    PersonId = table.Column<long>(type: "bigint", nullable: false),
                    GroupId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Tax = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    Currency = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false, defaultValue: "CUP"),
                    TaxType = table.Column<string>(type: "nvarchar(25)", maxLength: 25, nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorUserId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_accounting_tax_summaries", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "adjustment_definitions",
                schema: "aju",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    Reference = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ProcessOn = table.Column<int>(type: "int", nullable: false),
                    Currency = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false, defaultValue: "CUP"),
                    SumHoursForHolidayHistogram = table.Column<bool>(type: "bit", nullable: false),
                    SumAmountForHolidayHistogram = table.Column<bool>(type: "bit", nullable: false),
                    SumHoursForPaymentHistogram = table.Column<bool>(type: "bit", nullable: false),
                    SumAmountForPaymentHistogram = table.Column<bool>(type: "bit", nullable: false),
                    SumHoursForSocialSecurity = table.Column<bool>(type: "bit", nullable: false),
                    SumAmountForSocialSecurity = table.Column<bool>(type: "bit", nullable: false),
                    ContributeForCompanySocialSecurityTaxes = table.Column<int>(type: "int", nullable: false),
                    ContributeForIncomeTaxes = table.Column<int>(type: "int", nullable: false),
                    ContributeForCompanyWorkforceTaxes = table.Column<int>(type: "int", nullable: false),
                    CompanyId = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorUserId = table.Column<long>(type: "bigint", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifierUserId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_adjustment_definitions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "complexity_groups",
                schema: "est",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Group = table.Column<string>(type: "nvarchar(8)", maxLength: 8, nullable: false),
                    BaseSalary = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    CompanyId = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorUserId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_complexity_groups", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "degree_definitions",
                schema: "gen",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DisplayName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_degree_definitions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "driver_license_definitions",
                schema: "gen",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DisplayName = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    Category = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_driver_license_definitions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "employment_indexes",
                schema: "rel",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Exp = table.Column<int>(type: "int", nullable: false),
                    Contract = table.Column<string>(type: "nvarchar(1)", maxLength: 1, nullable: false),
                    Group = table.Column<string>(type: "nvarchar(1)", maxLength: 1, nullable: false),
                    Note = table.Column<string>(type: "nvarchar(195)", maxLength: 195, nullable: true),
                    CompanyId = table.Column<int>(type: "int", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorUserId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_employment_indexes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "employment_summaries",
                schema: "rel",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DisplayName = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    Acronym = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: true),
                    ParentId = table.Column<int>(type: "int", nullable: true),
                    Type = table.Column<string>(type: "nvarchar(1)", maxLength: 1, nullable: false),
                    Classification = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Fluctuation = table.Column<bool>(type: "bit", nullable: true),
                    Reference = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_employment_summaries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_employment_summaries_employment_summaries_ParentId",
                        column: x => x.ParentId,
                        principalSchema: "rel",
                        principalTable: "employment_summaries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "holiday_histogram",
                schema: "vac",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DocumentDefinitionId = table.Column<int>(type: "int", nullable: false),
                    DocumentId = table.Column<int>(type: "int", nullable: false),
                    PersonId = table.Column<long>(type: "bigint", nullable: false),
                    GroupId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Since = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Until = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Hours = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    Currency = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false, defaultValue: "CUP"),
                    Status = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    CompanyId = table.Column<int>(type: "int", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorUserId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_holiday_histogram", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "incidents",
                schema: "sal",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PersonId = table.Column<long>(type: "bigint", nullable: false),
                    Start = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EmploymentId = table.Column<long>(type: "bigint", nullable: false),
                    Key = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: false),
                    End = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Hours = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false),
                    CompanyId = table.Column<int>(type: "int", nullable: false),
                    Tight = table.Column<bool>(type: "bit", nullable: false),
                    Annotation = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorUserId = table.Column<long>(type: "bigint", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifierUserId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_incidents", x => new { x.Id, x.Start, x.PersonId });
                });

            migrationBuilder.CreateTable(
                name: "law_penalty_cause_definitions",
                schema: "gen",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DisplayName = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    Legal = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_law_penalty_cause_definitions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "law_penalty_definitions",
                schema: "gen",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DisplayName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Severe = table.Column<bool>(type: "bit", nullable: false),
                    Rehab = table.Column<TimeSpan>(type: "time", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_law_penalty_definitions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "military_location_definitions",
                schema: "gen",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DisplayName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_military_location_definitions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "occupational_categories",
                schema: "est",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DisplayName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Code = table.Column<string>(type: "nvarchar(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_occupational_categories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "payment_definitions",
                schema: "sal",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    Symbol = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: true),
                    MathType = table.Column<string>(type: "nvarchar(11)", maxLength: 11, nullable: false),
                    Factor = table.Column<decimal>(type: "decimal(28,24)", precision: 28, scale: 24, nullable: false),
                    AverageMonths = table.Column<int>(type: "int", nullable: false),
                    Formula = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    SalaryForm = table.Column<byte>(type: "tinyint", nullable: false),
                    PaymentSystem = table.Column<byte>(type: "tinyint", nullable: false),
                    WageAdjuster = table.Column<string>(type: "nvarchar(7)", maxLength: 7, nullable: false),
                    Operation = table.Column<string>(type: "nvarchar(6)", maxLength: 6, nullable: false),
                    Reference = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Observation = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    CompanyId = table.Column<int>(type: "int", nullable: false),
                    SumHoursForHolidayHistogram = table.Column<bool>(type: "bit", nullable: false),
                    SumAmountForHolidayHistogram = table.Column<bool>(type: "bit", nullable: false),
                    SumHoursForPaymentHistogram = table.Column<bool>(type: "bit", nullable: false),
                    SumAmountForPaymentHistogram = table.Column<bool>(type: "bit", nullable: false),
                    SumHoursForSocialSecurity = table.Column<bool>(type: "bit", nullable: false),
                    SumAmountForSocialSecurity = table.Column<bool>(type: "bit", nullable: false),
                    IsWageGuarantee = table.Column<bool>(type: "bit", nullable: false),
                    ContributeForCompanySocialSecurityTaxes = table.Column<int>(type: "int", nullable: false),
                    ContributeForIncomeTaxes = table.Column<int>(type: "int", nullable: false),
                    ContributeForCompanyWorkforceTaxes = table.Column<int>(type: "int", nullable: false),
                    Absence = table.Column<bool>(type: "bit", nullable: false),
                    LongTerm = table.Column<bool>(type: "bit", nullable: false),
                    IsIncident = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorUserId = table.Column<long>(type: "bigint", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifierUserId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_payment_definitions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "payment_histogram",
                schema: "sal",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DocumentDefinitionId = table.Column<int>(type: "int", nullable: false),
                    PersonId = table.Column<long>(type: "bigint", nullable: false),
                    GroupId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Year = table.Column<int>(type: "int", nullable: false),
                    Month = table.Column<int>(type: "int", nullable: false),
                    Since = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Until = table.Column<DateTime>(type: "datetime2", nullable: false),
                    WorkedDays = table.Column<int>(type: "int", nullable: false),
                    SalaryPaymentReceived = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    SickLeaveDays = table.Column<int>(type: "int", nullable: false),
                    SickLeavePaymentReceived = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    Holidays = table.Column<int>(type: "int", nullable: false),
                    HolidaysPaymentReceived = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    SalaryPlusPaymentReceived = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    BonusPaymentReceived = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    CertifiedDays = table.Column<int>(type: "int", nullable: false),
                    Currency = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false, defaultValue: "CUP"),
                    Observation = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    CompanyId = table.Column<int>(type: "int", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorUserId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_payment_histogram", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "performance_documents",
                schema: "rel",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DocumentDefinitionId = table.Column<int>(type: "int", nullable: false),
                    MadeOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: false),
                    DisplayName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    PeriodId = table.Column<int>(type: "int", nullable: false),
                    Since = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Until = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Review = table.Column<int>(type: "int", nullable: false),
                    CompanyId = table.Column<int>(type: "int", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorUserId = table.Column<long>(type: "bigint", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifierUserId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_performance_documents", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "performance_summaries",
                schema: "rel",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DisplayName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorUserId = table.Column<long>(type: "bigint", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifierUserId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_performance_summaries", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "person_accounts",
                schema: "docs",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PersonId = table.Column<long>(type: "bigint", nullable: false),
                    AccountNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Currency = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false, defaultValue: "CUP"),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorUserId = table.Column<long>(type: "bigint", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifierUserId = table.Column<long>(type: "bigint", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeleterUserId = table.Column<long>(type: "bigint", nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_person_accounts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "person_background_times",
                schema: "docs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CompanyId = table.Column<int>(type: "int", nullable: false),
                    PersonId = table.Column<long>(type: "bigint", nullable: false),
                    GroupId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Year = table.Column<int>(type: "int", nullable: false),
                    Month = table.Column<int>(type: "int", nullable: true),
                    Key = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: false),
                    Hours = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false),
                    Kind = table.Column<byte>(type: "tinyint", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorUserId = table.Column<long>(type: "bigint", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifierUserId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_person_background_times", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "person_family_relationships",
                schema: "docs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PersonId = table.Column<long>(type: "bigint", nullable: false),
                    PersonOnRelationId = table.Column<long>(type: "bigint", nullable: false),
                    Kind = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    Cohabits = table.Column<bool>(type: "bit", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorUserId = table.Column<long>(type: "bigint", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifierUserId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_person_family_relationships", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "person_holiday_schedules",
                schema: "docs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PersonId = table.Column<long>(type: "bigint", nullable: false),
                    Year = table.Column<int>(type: "int", nullable: false),
                    JanuaryFirstFortnightCalendarDays = table.Column<int>(type: "int", nullable: false),
                    JanuarySecondFortnightCalendarDays = table.Column<int>(type: "int", nullable: false),
                    JanuaryFirstFortnightDaysOff = table.Column<int>(type: "int", nullable: true),
                    JanuarySecondFortnightDaysOf = table.Column<int>(type: "int", nullable: true),
                    FebruaryFirstFortnightCalendarDays = table.Column<int>(type: "int", nullable: false),
                    FebruarySecondFortnightCalendarDays = table.Column<int>(type: "int", nullable: false),
                    FebruaryFirstFortnightDaysOff = table.Column<int>(type: "int", nullable: true),
                    FebruarySecondFortnightDaysOf = table.Column<int>(type: "int", nullable: true),
                    MarchFirstFortnightCalendarDays = table.Column<int>(type: "int", nullable: false),
                    MarchSecondFortnightCalendarDays = table.Column<int>(type: "int", nullable: false),
                    MarchFirstFortnightDaysOff = table.Column<int>(type: "int", nullable: true),
                    MarchSecondFortnightDaysOf = table.Column<int>(type: "int", nullable: true),
                    AprilFirstFortnightCalendarDays = table.Column<int>(type: "int", nullable: false),
                    AprilSecondFortnightCalendarDays = table.Column<int>(type: "int", nullable: false),
                    AprilFirstFortnightDaysOff = table.Column<int>(type: "int", nullable: true),
                    AprilSecondFortnightDaysOf = table.Column<int>(type: "int", nullable: true),
                    MayFirstFortnightCalendarDays = table.Column<int>(type: "int", nullable: false),
                    MaySecondFortnightCalendarDays = table.Column<int>(type: "int", nullable: false),
                    MayFirstFortnightDaysOff = table.Column<int>(type: "int", nullable: true),
                    MaySecondFortnightDaysOf = table.Column<int>(type: "int", nullable: true),
                    JuneFirstFortnightCalendarDays = table.Column<int>(type: "int", nullable: false),
                    JuneSecondFortnightCalendarDays = table.Column<int>(type: "int", nullable: false),
                    JuneFirstFortnightDaysOff = table.Column<int>(type: "int", nullable: true),
                    JuneSecondFortnightDaysOf = table.Column<int>(type: "int", nullable: true),
                    JulyFirstFortnightCalendarDays = table.Column<int>(type: "int", nullable: false),
                    JulySecondFortnightCalendarDays = table.Column<int>(type: "int", nullable: false),
                    JulyFirstFortnightDaysOff = table.Column<int>(type: "int", nullable: true),
                    JulySecondFortnightDaysOf = table.Column<int>(type: "int", nullable: true),
                    AugustFirstFortnightCalendarDays = table.Column<int>(type: "int", nullable: false),
                    AugustSecondFortnightCalendarDays = table.Column<int>(type: "int", nullable: false),
                    AugustFirstFortnightDaysOff = table.Column<int>(type: "int", nullable: true),
                    AugustSecondFortnightDaysOf = table.Column<int>(type: "int", nullable: true),
                    SeptemberFirstFortnightCalendarDays = table.Column<int>(type: "int", nullable: false),
                    SeptemberSecondFortnightCalendarDays = table.Column<int>(type: "int", nullable: false),
                    SeptemberFirstFortnightDaysOff = table.Column<int>(type: "int", nullable: true),
                    SeptemberSecondFortnightDaysOf = table.Column<int>(type: "int", nullable: true),
                    OctoberFirstFortnightCalendarDays = table.Column<int>(type: "int", nullable: false),
                    OctoberSecondFortnightCalendarDays = table.Column<int>(type: "int", nullable: false),
                    OctoberFirstFortnightDaysOff = table.Column<int>(type: "int", nullable: true),
                    OctoberSecondFortnightDaysOf = table.Column<int>(type: "int", nullable: true),
                    NovemberFirstFortnightCalendarDays = table.Column<int>(type: "int", nullable: false),
                    NovemberSecondFortnightCalendarDays = table.Column<int>(type: "int", nullable: false),
                    NovemberFirstFortnightDaysOff = table.Column<int>(type: "int", nullable: true),
                    NovemberSecondFortnightDaysOf = table.Column<int>(type: "int", nullable: true),
                    DecemberFirstFortnightCalendarDays = table.Column<int>(type: "int", nullable: false),
                    DecemberSecondFortnightCalendarDays = table.Column<int>(type: "int", nullable: false),
                    DecemberFirstFortnightDaysOff = table.Column<int>(type: "int", nullable: true),
                    DecemberSecondFortnightDaysOf = table.Column<int>(type: "int", nullable: true),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorUserId = table.Column<long>(type: "bigint", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifierUserId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_person_holiday_schedules", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "person_taxes",
                schema: "docs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PersonId = table.Column<long>(type: "bigint", nullable: false),
                    GroupId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TaxType = table.Column<string>(type: "nvarchar(25)", maxLength: 25, nullable: false),
                    MathType = table.Column<string>(type: "nvarchar(11)", maxLength: 11, nullable: false),
                    Factor = table.Column<decimal>(type: "decimal(28,24)", precision: 28, scale: 24, nullable: false),
                    Formula = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    CompanyId = table.Column<int>(type: "int", nullable: false),
                    DisplayName = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorUserId = table.Column<long>(type: "bigint", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifierUserId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_person_taxes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "plus_definitions",
                schema: "sal",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: false),
                    Scope = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    Reference = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CompanyId = table.Column<int>(type: "int", nullable: false),
                    SumHoursForHolidayHistogram = table.Column<bool>(type: "bit", nullable: false),
                    SumAmountForHolidayHistogram = table.Column<bool>(type: "bit", nullable: false),
                    SumHoursForPaymentHistogram = table.Column<bool>(type: "bit", nullable: false),
                    SumAmountForPaymentHistogram = table.Column<bool>(type: "bit", nullable: false),
                    SumHoursForSocialSecurity = table.Column<bool>(type: "bit", nullable: false),
                    SumAmountForSocialSecurity = table.Column<bool>(type: "bit", nullable: false),
                    ContributeForCompanySocialSecurityTaxes = table.Column<int>(type: "int", nullable: false),
                    ContributeForIncomeTaxes = table.Column<int>(type: "int", nullable: false),
                    ContributeForCompanyWorkforceTaxes = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorUserId = table.Column<long>(type: "bigint", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifierUserId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_plus_definitions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "responsibility_levels",
                schema: "est",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DisplayName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    NormalizedDescription = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_responsibility_levels", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "scholarship_level_definitions",
                schema: "gen",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Acronym = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    DisplayName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Scope = table.Column<int>(type: "int", nullable: false),
                    Weight = table.Column<decimal>(type: "decimal(28,24)", precision: 28, scale: 24, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_scholarship_level_definitions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "sign_on_documents",
                schema: "docs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Order = table.Column<int>(type: "int", nullable: false),
                    DocumentDefinitionId = table.Column<int>(type: "int", nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(192)", maxLength: 192, nullable: true),
                    Code = table.Column<string>(type: "nvarchar(8)", maxLength: 8, nullable: false),
                    Occupation = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    IdentityCard = table.Column<string>(type: "nvarchar(11)", maxLength: 11, nullable: true),
                    Owner = table.Column<bool>(type: "bit", nullable: false),
                    CompanyId = table.Column<int>(type: "int", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorUserId = table.Column<long>(type: "bigint", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifierUserId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sign_on_documents", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "time_distribution_documents",
                schema: "sal",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DocumentDefinitionId = table.Column<int>(type: "int", nullable: false),
                    MadeOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DisplayName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Code = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: false),
                    CompanyId = table.Column<int>(type: "int", nullable: false),
                    WorkPlacePaymentId = table.Column<long>(type: "bigint", nullable: false),
                    PeriodId = table.Column<int>(type: "int", nullable: false),
                    Since = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Until = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Review = table.Column<int>(type: "int", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorUserId = table.Column<long>(type: "bigint", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifierUserId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_time_distribution_documents", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "work_regimens",
                schema: "gen",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Type = table.Column<string>(type: "nvarchar(8)", maxLength: 8, nullable: false),
                    DaysScheduling = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    SpecialGroup = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    LegalName = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    TimeScheduling = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    CompanyId = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorUserId = table.Column<long>(type: "bigint", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifierUserId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_work_regimens", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "retention_definitions",
                schema: "ret",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    Reference = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Type = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Priority = table.Column<int>(type: "int", nullable: false),
                    MathType = table.Column<string>(type: "nvarchar(11)", maxLength: 11, nullable: true),
                    TaxAmortization = table.Column<decimal>(type: "decimal(28,24)", precision: 28, scale: 24, nullable: true),
                    Currency = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false, defaultValue: "CUP"),
                    Partial = table.Column<bool>(type: "bit", nullable: false),
                    Auto = table.Column<bool>(type: "bit", nullable: false),
                    Credit = table.Column<bool>(type: "bit", nullable: false),
                    Total = table.Column<bool>(type: "bit", nullable: false),
                    CompanyId = table.Column<int>(type: "int", nullable: false),
                    RefundDefinitionId = table.Column<int>(type: "int", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorUserId = table.Column<long>(type: "bigint", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifierUserId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_retention_definitions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_retention_definitions_adjustment_definitions_RefundDefinitionId",
                        column: x => x.RefundDefinitionId,
                        principalSchema: "aju",
                        principalTable: "adjustment_definitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "person_driver_licenses",
                schema: "docs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PersonId = table.Column<long>(type: "bigint", nullable: false),
                    DriverLicenseDefinitionId = table.Column<int>(type: "int", nullable: false),
                    LicenseNumber = table.Column<string>(type: "nvarchar(11)", maxLength: 11, nullable: false),
                    EffectiveSince = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EffectiveUntil = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastEvaluation = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorUserId = table.Column<long>(type: "bigint", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifierUserId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_person_driver_licenses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_person_driver_licenses_driver_license_definitions_DriverLicenseDefinitionId",
                        column: x => x.DriverLicenseDefinitionId,
                        principalSchema: "gen",
                        principalTable: "driver_license_definitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "person_law_penalties",
                schema: "docs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PersonId = table.Column<long>(type: "bigint", nullable: false),
                    CauseId = table.Column<int>(type: "int", nullable: false),
                    LawPenaltyDefinitionId = table.Column<int>(type: "int", nullable: false),
                    Notification = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Since = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Until = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Rehab = table.Column<bool>(type: "bit", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorUserId = table.Column<long>(type: "bigint", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifierUserId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_person_law_penalties", x => x.Id);
                    table.ForeignKey(
                        name: "FK_person_law_penalties_law_penalty_cause_definitions_CauseId",
                        column: x => x.CauseId,
                        principalSchema: "gen",
                        principalTable: "law_penalty_cause_definitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_person_law_penalties_law_penalty_definitions_LawPenaltyDefinitionId",
                        column: x => x.LawPenaltyDefinitionId,
                        principalSchema: "gen",
                        principalTable: "law_penalty_definitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "person_military_records",
                schema: "docs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PersonId = table.Column<long>(type: "bigint", nullable: false),
                    LocationId = table.Column<int>(type: "int", nullable: false),
                    LastUpdated = table.Column<DateTime>(type: "datetime2", nullable: true),
                    SuitableForMilitaryService = table.Column<bool>(type: "bit", nullable: false),
                    CompletedActiveMilitaryService = table.Column<bool>(type: "bit", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorUserId = table.Column<long>(type: "bigint", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifierUserId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_person_military_records", x => x.Id);
                    table.ForeignKey(
                        name: "FK_person_military_records_military_location_definitions_LocationId",
                        column: x => x.LocationId,
                        principalSchema: "gen",
                        principalTable: "military_location_definitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "subsidy_payment_definitions",
                schema: "sub",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DisplayName = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    Symbol = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: true),
                    BasePercent = table.Column<decimal>(type: "decimal(28,24)", precision: 28, scale: 24, nullable: false),
                    PaymentDefinitionId = table.Column<int>(type: "int", nullable: false),
                    Reference = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ProcessOn = table.Column<int>(type: "int", nullable: false),
                    CompanyId = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorUserId = table.Column<long>(type: "bigint", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifierUserId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_subsidy_payment_definitions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_subsidy_payment_definitions_payment_definitions_PaymentDefinitionId",
                        column: x => x.PaymentDefinitionId,
                        principalSchema: "sal",
                        principalTable: "payment_definitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "occupations",
                schema: "est",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DisplayName = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    Code = table.Column<string>(type: "nvarchar(8)", maxLength: 8, nullable: false),
                    GroupId = table.Column<int>(type: "int", nullable: false),
                    CategoryId = table.Column<int>(type: "int", nullable: false),
                    ResponsibilityId = table.Column<int>(type: "int", nullable: false),
                    CompanyId = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorUserId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_occupations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_occupations_complexity_groups_GroupId",
                        column: x => x.GroupId,
                        principalSchema: "est",
                        principalTable: "complexity_groups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_occupations_occupational_categories_CategoryId",
                        column: x => x.CategoryId,
                        principalSchema: "est",
                        principalTable: "occupational_categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_occupations_responsibility_levels_ResponsibilityId",
                        column: x => x.ResponsibilityId,
                        principalSchema: "est",
                        principalTable: "responsibility_levels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "person_scholarships",
                schema: "docs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PersonId = table.Column<long>(type: "bigint", nullable: false),
                    ScholarshipLevelId = table.Column<int>(type: "int", nullable: false),
                    DegreeId = table.Column<int>(type: "int", nullable: true),
                    Graduation = table.Column<DateTime>(type: "datetime2", nullable: true),
                    StudyCenter = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorUserId = table.Column<long>(type: "bigint", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifierUserId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_person_scholarships", x => x.Id);
                    table.ForeignKey(
                        name: "FK_person_scholarships_degree_definitions_DegreeId",
                        column: x => x.DegreeId,
                        principalSchema: "gen",
                        principalTable: "degree_definitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_person_scholarships_scholarship_level_definitions_ScholarshipLevelId",
                        column: x => x.ScholarshipLevelId,
                        principalSchema: "gen",
                        principalTable: "scholarship_level_definitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "template_documents",
                schema: "est",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DocumentDefinitionId = table.Column<int>(type: "int", nullable: false),
                    MadeOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MadeById = table.Column<int>(type: "int", nullable: false),
                    ApprovedById = table.Column<int>(type: "int", nullable: false),
                    CompanyId = table.Column<int>(type: "int", nullable: false),
                    Review = table.Column<int>(type: "int", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorUserId = table.Column<long>(type: "bigint", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifierUserId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_template_documents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_template_documents_sign_on_documents_ApprovedById",
                        column: x => x.ApprovedById,
                        principalSchema: "docs",
                        principalTable: "sign_on_documents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_template_documents_sign_on_documents_MadeById",
                        column: x => x.MadeById,
                        principalSchema: "docs",
                        principalTable: "sign_on_documents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "work_shifts",
                schema: "gen",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DisplayName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    RegimeId = table.Column<int>(type: "int", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    HoursWorking = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    RestingTimesPerShift = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    AverageHoursPerShift = table.Column<decimal>(type: "decimal(28,24)", precision: 28, scale: 24, nullable: false),
                    Legal = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CompanyId = table.Column<int>(type: "int", nullable: false),
                    VisualOrder = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorUserId = table.Column<long>(type: "bigint", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifierUserId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_work_shifts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_work_shifts_work_regimens_RegimeId",
                        column: x => x.RegimeId,
                        principalSchema: "gen",
                        principalTable: "work_regimens",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "retention_documents",
                schema: "ret",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DocumentDefinitionId = table.Column<int>(type: "int", nullable: false),
                    MadeOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RetentionDefinitionId = table.Column<int>(type: "int", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    PersonId = table.Column<long>(type: "bigint", nullable: false),
                    GroupId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CompanyId = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    Finished = table.Column<bool>(type: "bit", nullable: false),
                    Amortization = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    PaymentTerms = table.Column<int>(type: "int", nullable: false),
                    LastAmortization = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    Surcharge = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    Total = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    Currency = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false, defaultValue: "CUP"),
                    Review = table.Column<int>(type: "int", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorUserId = table.Column<long>(type: "bigint", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifierUserId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_retention_documents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_retention_documents_retention_definitions_RetentionDefinitionId",
                        column: x => x.RetentionDefinitionId,
                        principalSchema: "ret",
                        principalTable: "retention_definitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "qualification_requirement_definitions",
                schema: "est",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CompanyId = table.Column<int>(type: "int", nullable: false),
                    DisplayName = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    OccupationId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_qualification_requirement_definitions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_qualification_requirement_definitions_occupations_OccupationId",
                        column: x => x.OccupationId,
                        principalSchema: "est",
                        principalTable: "occupations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "templates",
                schema: "est",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DocumentId = table.Column<int>(type: "int", nullable: false),
                    CompanyId = table.Column<int>(type: "int", nullable: false),
                    OrganizationUnitId = table.Column<long>(type: "bigint", nullable: false),
                    OrganizationUnitCode = table.Column<string>(type: "nvarchar(12)", maxLength: 12, nullable: false),
                    CenterCost = table.Column<int>(type: "int", nullable: false),
                    EmployeeSalaryForm = table.Column<byte>(type: "tinyint", nullable: false),
                    OccupationId = table.Column<int>(type: "int", nullable: false),
                    ScholarshipLevelId = table.Column<int>(type: "int", nullable: true),
                    WorkShift = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    Proposals = table.Column<int>(type: "int", nullable: false),
                    Approved = table.Column<int>(type: "int", nullable: true),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorUserId = table.Column<long>(type: "bigint", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifierUserId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_templates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_templates_occupations_OccupationId",
                        column: x => x.OccupationId,
                        principalSchema: "est",
                        principalTable: "occupations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_templates_scholarship_level_definitions_ScholarshipLevelId",
                        column: x => x.ScholarshipLevelId,
                        principalSchema: "gen",
                        principalTable: "scholarship_level_definitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_templates_template_documents_DocumentId",
                        column: x => x.DocumentId,
                        principalSchema: "est",
                        principalTable: "template_documents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "employment_documents",
                schema: "rel",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DocumentDefinitionId = table.Column<int>(type: "int", nullable: false),
                    MadeOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EffectiveSince = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Exp = table.Column<int>(type: "int", nullable: false),
                    PersonId = table.Column<long>(type: "bigint", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(11)", maxLength: 11, nullable: false),
                    GroupId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EmployeeSalaryForm = table.Column<byte>(type: "tinyint", nullable: false),
                    CompanyId = table.Column<int>(type: "int", nullable: false),
                    Contract = table.Column<string>(type: "nvarchar(1)", maxLength: 1, nullable: false),
                    ContractSubType = table.Column<string>(type: "nvarchar(1)", maxLength: 1, nullable: true),
                    Type = table.Column<string>(type: "nvarchar(1)", maxLength: 1, nullable: false),
                    SubType = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    PreviousId = table.Column<long>(type: "bigint", nullable: true),
                    OrganizationUnitId = table.Column<long>(type: "bigint", nullable: false),
                    WorkPlacePaymentCode = table.Column<string>(type: "nvarchar(8)", maxLength: 8, nullable: false),
                    FirstLevelCode = table.Column<string>(type: "nvarchar(12)", maxLength: 12, nullable: false),
                    FirstLevelDisplayName = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    SecondLevelCode = table.Column<string>(type: "nvarchar(12)", maxLength: 12, nullable: false),
                    SecondLevelDisplayName = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    ThirdLevelCode = table.Column<string>(type: "nvarchar(12)", maxLength: 12, nullable: false),
                    ThirdLevelDisplayName = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    CenterCost = table.Column<int>(type: "int", nullable: false),
                    ComplexityGroup = table.Column<string>(type: "nvarchar(8)", maxLength: 8, nullable: false),
                    Occupation = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    OccupationCode = table.Column<string>(type: "nvarchar(8)", maxLength: 8, nullable: false),
                    Responsibility = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    OccupationCategory = table.Column<string>(type: "nvarchar(1)", nullable: false),
                    WorkShiftId = table.Column<int>(type: "int", nullable: false),
                    Salary = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    TotalSalary = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    RatePerHour = table.Column<decimal>(type: "decimal(28,24)", precision: 28, scale: 24, nullable: false),
                    SummaryId = table.Column<int>(type: "int", nullable: false),
                    ExtraSummary = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    LegalNumber = table.Column<int>(type: "int", nullable: true),
                    ExpirationDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    HasSalary = table.Column<bool>(type: "bit", nullable: false),
                    ByAssignment = table.Column<bool>(type: "bit", nullable: false),
                    ByOfficial = table.Column<bool>(type: "bit", nullable: false),
                    MadeById = table.Column<int>(type: "int", nullable: true),
                    ReviewedById = table.Column<int>(type: "int", nullable: true),
                    ApprovedById = table.Column<int>(type: "int", nullable: true),
                    Review = table.Column<int>(type: "int", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorUserId = table.Column<long>(type: "bigint", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifierUserId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_employment_documents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_employment_documents_employment_documents_PreviousId",
                        column: x => x.PreviousId,
                        principalSchema: "rel",
                        principalTable: "employment_documents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_employment_documents_employment_summaries_SummaryId",
                        column: x => x.SummaryId,
                        principalSchema: "rel",
                        principalTable: "employment_summaries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_employment_documents_sign_on_documents_ApprovedById",
                        column: x => x.ApprovedById,
                        principalSchema: "docs",
                        principalTable: "sign_on_documents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_employment_documents_sign_on_documents_MadeById",
                        column: x => x.MadeById,
                        principalSchema: "docs",
                        principalTable: "sign_on_documents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_employment_documents_sign_on_documents_ReviewedById",
                        column: x => x.ReviewedById,
                        principalSchema: "docs",
                        principalTable: "sign_on_documents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_employment_documents_work_shifts_WorkShiftId",
                        column: x => x.WorkShiftId,
                        principalSchema: "gen",
                        principalTable: "work_shifts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "adjustment_documents",
                schema: "aju",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DocumentDefinitionId = table.Column<int>(type: "int", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    PersonId = table.Column<long>(type: "bigint", nullable: false),
                    AdjustmentDefinitionId = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    HoursReservedForHoliday = table.Column<decimal>(type: "decimal(28,24)", precision: 28, scale: 24, nullable: true),
                    AmountReservedForHoliday = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    Currency = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false, defaultValue: "CUP"),
                    MadeOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EmploymentId = table.Column<long>(type: "bigint", nullable: false),
                    GroupId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CenterCost = table.Column<int>(type: "int", nullable: false),
                    CompanyId = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Review = table.Column<int>(type: "int", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorUserId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_adjustment_documents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_adjustment_documents_adjustment_definitions_AdjustmentDefinitionId",
                        column: x => x.AdjustmentDefinitionId,
                        principalSchema: "aju",
                        principalTable: "adjustment_definitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_adjustment_documents_employment_documents_EmploymentId",
                        column: x => x.EmploymentId,
                        principalSchema: "rel",
                        principalTable: "employment_documents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "claim_documents",
                schema: "rec",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DocumentDefinitionId = table.Column<int>(type: "int", nullable: false),
                    MadeOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    PersonId = table.Column<long>(type: "bigint", nullable: false),
                    EmploymentId = table.Column<long>(type: "bigint", nullable: false),
                    GroupId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CompanyId = table.Column<int>(type: "int", nullable: false),
                    Review = table.Column<int>(type: "int", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorUserId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_claim_documents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_claim_documents_employment_documents_EmploymentId",
                        column: x => x.EmploymentId,
                        principalSchema: "rel",
                        principalTable: "employment_documents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "employment_documents_to_generate",
                schema: "rel",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmploymentDocumentId = table.Column<long>(type: "bigint", nullable: false),
                    NextEmploymentDocumentId = table.Column<long>(type: "bigint", nullable: true),
                    LegalChangeType = table.Column<int>(type: "int", nullable: false),
                    EffectiveSince = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ExpirationDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EmployeeSalaryForm = table.Column<byte>(type: "tinyint", nullable: true),
                    Type = table.Column<string>(type: "nvarchar(1)", maxLength: 1, nullable: true),
                    SubType = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: true),
                    OrganizationUnitId = table.Column<long>(type: "bigint", nullable: true),
                    CenterCost = table.Column<int>(type: "int", nullable: true),
                    OccupationCode = table.Column<string>(type: "nvarchar(8)", maxLength: 8, nullable: true),
                    WorkShiftId = table.Column<int>(type: "int", nullable: true),
                    SummaryId = table.Column<int>(type: "int", nullable: true),
                    ExtraSummary = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Confirmed = table.Column<bool>(type: "bit", nullable: false),
                    CompanyId = table.Column<int>(type: "int", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorUserId = table.Column<long>(type: "bigint", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifierUserId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_employment_documents_to_generate", x => x.Id);
                    table.ForeignKey(
                        name: "FK_employment_documents_to_generate_employment_documents_EmploymentDocumentId",
                        column: x => x.EmploymentDocumentId,
                        principalSchema: "rel",
                        principalTable: "employment_documents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_employment_documents_to_generate_employment_documents_NextEmploymentDocumentId",
                        column: x => x.NextEmploymentDocumentId,
                        principalSchema: "rel",
                        principalTable: "employment_documents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "employment_pluses",
                schema: "rel",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmploymentId = table.Column<long>(type: "bigint", nullable: false),
                    PlusDefinitionId = table.Column<int>(type: "int", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    RatePerHour = table.Column<decimal>(type: "decimal(28,24)", precision: 28, scale: 24, nullable: false),
                    CompanyId = table.Column<int>(type: "int", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorUserId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_employment_pluses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_employment_pluses_employment_documents_EmploymentId",
                        column: x => x.EmploymentId,
                        principalSchema: "rel",
                        principalTable: "employment_documents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_employment_pluses_plus_definitions_PlusDefinitionId",
                        column: x => x.PlusDefinitionId,
                        principalSchema: "sal",
                        principalTable: "plus_definitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "holiday_documents",
                schema: "vac",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DocumentDefinitionId = table.Column<int>(type: "int", nullable: false),
                    MadeOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    PersonId = table.Column<long>(type: "bigint", nullable: false),
                    Since = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Until = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Return = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    EmploymentId = table.Column<long>(type: "bigint", nullable: false),
                    GroupId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Days = table.Column<int>(type: "int", nullable: false),
                    Hours = table.Column<decimal>(type: "decimal(28,24)", precision: 28, scale: 24, nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    Currency = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false, defaultValue: "CUP"),
                    RatePerDay = table.Column<decimal>(type: "decimal(28,24)", precision: 28, scale: 24, nullable: false),
                    CompanyId = table.Column<int>(type: "int", nullable: false),
                    Review = table.Column<int>(type: "int", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorUserId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_holiday_documents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_holiday_documents_employment_documents_EmploymentId",
                        column: x => x.EmploymentId,
                        principalSchema: "rel",
                        principalTable: "employment_documents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "performance_evaluations",
                schema: "rel",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DocumentId = table.Column<int>(type: "int", nullable: false),
                    Evaluation = table.Column<decimal>(type: "decimal(28,24)", precision: 28, scale: 24, nullable: false),
                    SummaryId = table.Column<int>(type: "int", nullable: true),
                    PersonId = table.Column<long>(type: "bigint", nullable: false),
                    EmploymentId = table.Column<long>(type: "bigint", nullable: false),
                    Kind = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    CompanyId = table.Column<int>(type: "int", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorUserId = table.Column<long>(type: "bigint", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifierUserId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_performance_evaluations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_performance_evaluations_employment_documents_EmploymentId",
                        column: x => x.EmploymentId,
                        principalSchema: "rel",
                        principalTable: "employment_documents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_performance_evaluations_performance_documents_DocumentId",
                        column: x => x.DocumentId,
                        principalSchema: "rel",
                        principalTable: "performance_documents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_performance_evaluations_performance_summaries_SummaryId",
                        column: x => x.SummaryId,
                        principalSchema: "rel",
                        principalTable: "performance_summaries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "subsidy_documents",
                schema: "sub",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DocumentDefinitionId = table.Column<int>(type: "int", nullable: false),
                    MadeOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    PersonId = table.Column<long>(type: "bigint", nullable: false),
                    SubsidyPaymentDefinitionId = table.Column<int>(type: "int", nullable: false),
                    Since = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Until = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EmploymentId = table.Column<long>(type: "bigint", nullable: false),
                    GroupId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PreviousId = table.Column<long>(type: "bigint", nullable: true),
                    SubsidizedDays = table.Column<int>(type: "int", nullable: false),
                    DeseaseDays = table.Column<int>(type: "int", nullable: false),
                    Percent = table.Column<decimal>(type: "decimal(28,24)", precision: 28, scale: 24, nullable: false),
                    AverageAmount = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    Currency = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false, defaultValue: "CUP"),
                    CompanyId = table.Column<int>(type: "int", nullable: false),
                    Review = table.Column<int>(type: "int", nullable: false),
                    Discriminator = table.Column<string>(type: "nvarchar(21)", maxLength: 21, nullable: false),
                    MultiplePregnancy = table.Column<bool>(type: "bit", nullable: true),
                    DueDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FixedAmountPerMonth = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    MedicalExpertise = table.Column<int>(type: "int", nullable: true),
                    Detail = table.Column<int>(type: "int", nullable: true),
                    WaitingPeriodDays = table.Column<int>(type: "int", nullable: true),
                    Hospitalized = table.Column<bool>(type: "bit", nullable: true),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorUserId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_subsidy_documents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_subsidy_documents_employment_documents_EmploymentId",
                        column: x => x.EmploymentId,
                        principalSchema: "rel",
                        principalTable: "employment_documents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_subsidy_documents_subsidy_documents_PreviousId",
                        column: x => x.PreviousId,
                        principalSchema: "sub",
                        principalTable: "subsidy_documents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_subsidy_documents_subsidy_payment_definitions_SubsidyPaymentDefinitionId",
                        column: x => x.SubsidyPaymentDefinitionId,
                        principalSchema: "sub",
                        principalTable: "subsidy_payment_definitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "template_job_positions",
                schema: "est",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TemplateId = table.Column<int>(type: "int", nullable: false),
                    CompanyId = table.Column<int>(type: "int", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(4)", maxLength: 4, nullable: false),
                    OrganizationUnitId = table.Column<long>(type: "bigint", nullable: false),
                    OrganizationUnitCode = table.Column<string>(type: "nvarchar(12)", maxLength: 12, nullable: false),
                    CenterCost = table.Column<int>(type: "int", nullable: false),
                    EmployeeSalaryForm = table.Column<byte>(type: "tinyint", nullable: false),
                    OccupationId = table.Column<int>(type: "int", nullable: false),
                    ScholarshipLevelId = table.Column<int>(type: "int", nullable: true),
                    WorkShiftId = table.Column<int>(type: "int", nullable: false),
                    ByAssignment = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    ByOfficial = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    DocumentId = table.Column<long>(type: "bigint", nullable: true),
                    TemporalDocumentId = table.Column<long>(type: "bigint", nullable: true),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorUserId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_template_job_positions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_template_job_positions_employment_documents_DocumentId",
                        column: x => x.DocumentId,
                        principalSchema: "rel",
                        principalTable: "employment_documents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_template_job_positions_employment_documents_TemporalDocumentId",
                        column: x => x.TemporalDocumentId,
                        principalSchema: "rel",
                        principalTable: "employment_documents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_template_job_positions_occupations_OccupationId",
                        column: x => x.OccupationId,
                        principalSchema: "est",
                        principalTable: "occupations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_template_job_positions_scholarship_level_definitions_ScholarshipLevelId",
                        column: x => x.ScholarshipLevelId,
                        principalSchema: "gen",
                        principalTable: "scholarship_level_definitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_template_job_positions_templates_TemplateId",
                        column: x => x.TemplateId,
                        principalSchema: "est",
                        principalTable: "templates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_template_job_positions_work_shifts_WorkShiftId",
                        column: x => x.WorkShiftId,
                        principalSchema: "gen",
                        principalTable: "work_shifts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "time_distributions",
                schema: "sal",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DocumentId = table.Column<int>(type: "int", nullable: false),
                    PersonId = table.Column<long>(type: "bigint", nullable: false),
                    EmploymentId = table.Column<long>(type: "bigint", nullable: false),
                    GroupId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CenterCost = table.Column<int>(type: "int", nullable: false),
                    Kind = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    PaymentDefinitionId = table.Column<int>(type: "int", nullable: false),
                    Hours = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    Currency = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: true, defaultValue: "CUP"),
                    ReservedForHoliday = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: true),
                    AmountReservedForHoliday = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    RatePerHour = table.Column<decimal>(type: "decimal(28,24)", precision: 28, scale: 24, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    CompanyId = table.Column<int>(type: "int", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorUserId = table.Column<long>(type: "bigint", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifierUserId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_time_distributions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_time_distributions_employment_documents_EmploymentId",
                        column: x => x.EmploymentId,
                        principalSchema: "rel",
                        principalTable: "employment_documents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_time_distributions_payment_definitions_PaymentDefinitionId",
                        column: x => x.PaymentDefinitionId,
                        principalSchema: "sal",
                        principalTable: "payment_definitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_time_distributions_time_distribution_documents_DocumentId",
                        column: x => x.DocumentId,
                        principalSchema: "sal",
                        principalTable: "time_distribution_documents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "adjustment_document_transitions",
                schema: "aju",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AdjustmentDocumentId = table.Column<long>(type: "bigint", nullable: false),
                    DocumentId = table.Column<long>(type: "bigint", nullable: false),
                    CompanyId = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorUserId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_adjustment_document_transitions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_adjustment_document_transitions_adjustment_documents_AdjustmentDocumentId",
                        column: x => x.AdjustmentDocumentId,
                        principalSchema: "aju",
                        principalTable: "adjustment_documents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "holiday_notes",
                schema: "vac",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DocumentId = table.Column<long>(type: "bigint", nullable: false),
                    CompanyId = table.Column<int>(type: "int", nullable: false),
                    PeriodId = table.Column<int>(type: "int", nullable: true),
                    Since = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Until = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Days = table.Column<int>(type: "int", nullable: false),
                    Hours = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    Currency = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false, defaultValue: "CUP"),
                    Status = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorUserId = table.Column<long>(type: "bigint", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifierUserId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_holiday_notes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_holiday_notes_holiday_documents_DocumentId",
                        column: x => x.DocumentId,
                        principalSchema: "vac",
                        principalTable: "holiday_documents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "performance_evaluation_law_penalties",
                schema: "rel",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CompanyId = table.Column<int>(type: "int", nullable: false),
                    EvaluationId = table.Column<int>(type: "int", nullable: false),
                    LawPenaltyId = table.Column<int>(type: "int", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorUserId = table.Column<long>(type: "bigint", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifierUserId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_performance_evaluation_law_penalties", x => x.Id);
                    table.ForeignKey(
                        name: "FK_performance_evaluation_law_penalties_performance_evaluations_EvaluationId",
                        column: x => x.EvaluationId,
                        principalSchema: "rel",
                        principalTable: "performance_evaluations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_performance_evaluation_law_penalties_person_law_penalties_LawPenaltyId",
                        column: x => x.LawPenaltyId,
                        principalSchema: "docs",
                        principalTable: "person_law_penalties",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "subsidy_notes",
                schema: "sub",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DocumentId = table.Column<long>(type: "bigint", nullable: false),
                    CompanyId = table.Column<int>(type: "int", nullable: false),
                    PeriodId = table.Column<int>(type: "int", nullable: true),
                    Since = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Until = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Days = table.Column<int>(type: "int", nullable: false),
                    Hours = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    Currency = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false, defaultValue: "CUP"),
                    ReservedForHoliday = table.Column<decimal>(type: "decimal(28,24)", precision: 28, scale: 24, nullable: true),
                    AmountReservedForHoliday = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorUserId = table.Column<long>(type: "bigint", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifierUserId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_subsidy_notes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_subsidy_notes_subsidy_documents_DocumentId",
                        column: x => x.DocumentId,
                        principalSchema: "sub",
                        principalTable: "subsidy_documents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "time_distribution_pluses",
                schema: "sal",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TimeDistributionId = table.Column<long>(type: "bigint", nullable: false),
                    EmploymentPlusId = table.Column<long>(type: "bigint", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    Currency = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: true, defaultValue: "CUP"),
                    ReservedForHoliday = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: true),
                    AmountReservedForHoliday = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    RatePerHour = table.Column<decimal>(type: "decimal(28,24)", precision: 28, scale: 24, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    CompanyId = table.Column<int>(type: "int", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorUserId = table.Column<long>(type: "bigint", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifierUserId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_time_distribution_pluses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_time_distribution_pluses_employment_pluses_EmploymentPlusId",
                        column: x => x.EmploymentPlusId,
                        principalSchema: "rel",
                        principalTable: "employment_pluses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_time_distribution_pluses_time_distributions_TimeDistributionId",
                        column: x => x.TimeDistributionId,
                        principalSchema: "sal",
                        principalTable: "time_distributions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "holiday_note_transitions",
                schema: "vac",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HolidayNoteId = table.Column<long>(type: "bigint", nullable: false),
                    DocumentId = table.Column<long>(type: "bigint", nullable: false),
                    CompanyId = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorUserId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_holiday_note_transitions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_holiday_note_transitions_holiday_notes_HolidayNoteId",
                        column: x => x.HolidayNoteId,
                        principalSchema: "vac",
                        principalTable: "holiday_notes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "subsidy_note_transitions",
                schema: "sub",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SubsidyNoteId = table.Column<long>(type: "bigint", nullable: false),
                    DocumentId = table.Column<long>(type: "bigint", nullable: false),
                    CompanyId = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorUserId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_subsidy_note_transitions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_subsidy_note_transitions_subsidy_notes_SubsidyNoteId",
                        column: x => x.SubsidyNoteId,
                        principalSchema: "sub",
                        principalTable: "subsidy_notes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_accounting_document_summaries_DocumentId_PersonId_CompanyId_GroupId",
                schema: "cnt",
                table: "accounting_document_summaries",
                columns: new[] { "DocumentId", "PersonId", "CompanyId", "GroupId" });

            migrationBuilder.CreateIndex(
                name: "IX_accounting_tax_summaries_DocumentId_PersonId_CompanyId_GroupId",
                schema: "cnt",
                table: "accounting_tax_summaries",
                columns: new[] { "DocumentId", "PersonId", "CompanyId", "GroupId" });

            migrationBuilder.CreateIndex(
                name: "IX_adjustment_definitions_CompanyId_Code",
                schema: "aju",
                table: "adjustment_definitions",
                columns: new[] { "CompanyId", "Code" });

            migrationBuilder.CreateIndex(
                name: "IX_adjustment_document_transitions_AdjustmentDocumentId",
                schema: "aju",
                table: "adjustment_document_transitions",
                column: "AdjustmentDocumentId");

            migrationBuilder.CreateIndex(
                name: "IX_adjustment_document_transitions_CompanyId_AdjustmentDocumentId_DocumentId",
                schema: "aju",
                table: "adjustment_document_transitions",
                columns: new[] { "CompanyId", "AdjustmentDocumentId", "DocumentId" });

            migrationBuilder.CreateIndex(
                name: "IX_adjustment_documents_AdjustmentDefinitionId",
                schema: "aju",
                table: "adjustment_documents",
                column: "AdjustmentDefinitionId");

            migrationBuilder.CreateIndex(
                name: "IX_adjustment_documents_Code",
                schema: "aju",
                table: "adjustment_documents",
                column: "Code");

            migrationBuilder.CreateIndex(
                name: "IX_adjustment_documents_EmploymentId",
                schema: "aju",
                table: "adjustment_documents",
                column: "EmploymentId");

            migrationBuilder.CreateIndex(
                name: "IX_adjustment_documents_PersonId_CompanyId_DocumentDefinitionId_AdjustmentDefinitionId",
                schema: "aju",
                table: "adjustment_documents",
                columns: new[] { "PersonId", "CompanyId", "DocumentDefinitionId", "AdjustmentDefinitionId" });

            migrationBuilder.CreateIndex(
                name: "IX_claim_documents_Code",
                schema: "rec",
                table: "claim_documents",
                column: "Code");

            migrationBuilder.CreateIndex(
                name: "IX_claim_documents_EmploymentId",
                schema: "rec",
                table: "claim_documents",
                column: "EmploymentId");

            migrationBuilder.CreateIndex(
                name: "IX_claim_documents_PersonId_CompanyId_DocumentDefinitionId",
                schema: "rec",
                table: "claim_documents",
                columns: new[] { "PersonId", "CompanyId", "DocumentDefinitionId" });

            migrationBuilder.CreateIndex(
                name: "IX_degree_definitions_DisplayName",
                schema: "gen",
                table: "degree_definitions",
                column: "DisplayName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_driver_license_definitions_DisplayName",
                schema: "gen",
                table: "driver_license_definitions",
                column: "DisplayName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_employment_documents_ApprovedById",
                schema: "rel",
                table: "employment_documents",
                column: "ApprovedById");

            migrationBuilder.CreateIndex(
                name: "IX_employment_documents_MadeById",
                schema: "rel",
                table: "employment_documents",
                column: "MadeById");

            migrationBuilder.CreateIndex(
                name: "IX_employment_documents_PersonId_CompanyId_DocumentDefinitionId",
                schema: "rel",
                table: "employment_documents",
                columns: new[] { "PersonId", "CompanyId", "DocumentDefinitionId" });

            migrationBuilder.CreateIndex(
                name: "IX_employment_documents_PreviousId",
                schema: "rel",
                table: "employment_documents",
                column: "PreviousId");

            migrationBuilder.CreateIndex(
                name: "IX_employment_documents_ReviewedById",
                schema: "rel",
                table: "employment_documents",
                column: "ReviewedById");

            migrationBuilder.CreateIndex(
                name: "IX_employment_documents_SummaryId",
                schema: "rel",
                table: "employment_documents",
                column: "SummaryId");

            migrationBuilder.CreateIndex(
                name: "IX_employment_documents_WorkShiftId",
                schema: "rel",
                table: "employment_documents",
                column: "WorkShiftId");

            migrationBuilder.CreateIndex(
                name: "IX_employment_documents_to_generate_EmploymentDocumentId_CompanyId",
                schema: "rel",
                table: "employment_documents_to_generate",
                columns: new[] { "EmploymentDocumentId", "CompanyId" });

            migrationBuilder.CreateIndex(
                name: "IX_employment_documents_to_generate_NextEmploymentDocumentId",
                schema: "rel",
                table: "employment_documents_to_generate",
                column: "NextEmploymentDocumentId");

            migrationBuilder.CreateIndex(
                name: "IX_employment_indexes_CompanyId_Exp",
                schema: "rel",
                table: "employment_indexes",
                columns: new[] { "CompanyId", "Exp" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_employment_pluses_EmploymentId",
                schema: "rel",
                table: "employment_pluses",
                column: "EmploymentId");

            migrationBuilder.CreateIndex(
                name: "IX_employment_pluses_PlusDefinitionId",
                schema: "rel",
                table: "employment_pluses",
                column: "PlusDefinitionId");

            migrationBuilder.CreateIndex(
                name: "IX_employment_summaries_ParentId",
                schema: "rel",
                table: "employment_summaries",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_holiday_documents_DocumentDefinitionId_PersonId_EmploymentId_CompanyId",
                schema: "vac",
                table: "holiday_documents",
                columns: new[] { "DocumentDefinitionId", "PersonId", "EmploymentId", "CompanyId" });

            migrationBuilder.CreateIndex(
                name: "IX_holiday_documents_EmploymentId",
                schema: "vac",
                table: "holiday_documents",
                column: "EmploymentId");

            migrationBuilder.CreateIndex(
                name: "IX_holiday_histogram_CompanyId_PersonId_GroupId",
                schema: "vac",
                table: "holiday_histogram",
                columns: new[] { "CompanyId", "PersonId", "GroupId" });

            migrationBuilder.CreateIndex(
                name: "IX_holiday_note_transitions_CompanyId_HolidayNoteId_DocumentId",
                schema: "vac",
                table: "holiday_note_transitions",
                columns: new[] { "CompanyId", "HolidayNoteId", "DocumentId" });

            migrationBuilder.CreateIndex(
                name: "IX_holiday_note_transitions_HolidayNoteId",
                schema: "vac",
                table: "holiday_note_transitions",
                column: "HolidayNoteId");

            migrationBuilder.CreateIndex(
                name: "IX_holiday_notes_DocumentId_CompanyId_PeriodId",
                schema: "vac",
                table: "holiday_notes",
                columns: new[] { "DocumentId", "CompanyId", "PeriodId" });

            migrationBuilder.CreateIndex(
                name: "IX_law_penalty_cause_definitions_DisplayName",
                schema: "gen",
                table: "law_penalty_cause_definitions",
                column: "DisplayName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_law_penalty_definitions_DisplayName",
                schema: "gen",
                table: "law_penalty_definitions",
                column: "DisplayName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_military_location_definitions_DisplayName",
                schema: "gen",
                table: "military_location_definitions",
                column: "DisplayName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_occupations_CategoryId",
                schema: "est",
                table: "occupations",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_occupations_GroupId",
                schema: "est",
                table: "occupations",
                column: "GroupId");

            migrationBuilder.CreateIndex(
                name: "IX_occupations_ResponsibilityId",
                schema: "est",
                table: "occupations",
                column: "ResponsibilityId");

            migrationBuilder.CreateIndex(
                name: "IX_payment_histogram_CompanyId_PersonId_Year_Month",
                schema: "sal",
                table: "payment_histogram",
                columns: new[] { "CompanyId", "PersonId", "Year", "Month" });

            migrationBuilder.CreateIndex(
                name: "IX_performance_documents_DocumentDefinitionId_CompanyId_PeriodId",
                schema: "rel",
                table: "performance_documents",
                columns: new[] { "DocumentDefinitionId", "CompanyId", "PeriodId" });

            migrationBuilder.CreateIndex(
                name: "IX_performance_evaluation_law_penalties_CompanyId_EvaluationId_LawPenaltyId",
                schema: "rel",
                table: "performance_evaluation_law_penalties",
                columns: new[] { "CompanyId", "EvaluationId", "LawPenaltyId" });

            migrationBuilder.CreateIndex(
                name: "IX_performance_evaluation_law_penalties_EvaluationId",
                schema: "rel",
                table: "performance_evaluation_law_penalties",
                column: "EvaluationId");

            migrationBuilder.CreateIndex(
                name: "IX_performance_evaluation_law_penalties_LawPenaltyId",
                schema: "rel",
                table: "performance_evaluation_law_penalties",
                column: "LawPenaltyId");

            migrationBuilder.CreateIndex(
                name: "IX_performance_evaluations_DocumentId_PersonId_EmploymentId_CompanyId",
                schema: "rel",
                table: "performance_evaluations",
                columns: new[] { "DocumentId", "PersonId", "EmploymentId", "CompanyId" });

            migrationBuilder.CreateIndex(
                name: "IX_performance_evaluations_EmploymentId",
                schema: "rel",
                table: "performance_evaluations",
                column: "EmploymentId");

            migrationBuilder.CreateIndex(
                name: "IX_performance_evaluations_SummaryId",
                schema: "rel",
                table: "performance_evaluations",
                column: "SummaryId");

            migrationBuilder.CreateIndex(
                name: "IX_person_accounts_AccountNumber",
                schema: "docs",
                table: "person_accounts",
                column: "AccountNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_person_accounts_PersonId",
                schema: "docs",
                table: "person_accounts",
                column: "PersonId");

            migrationBuilder.CreateIndex(
                name: "IX_person_background_times_CompanyId_PersonId_GroupId_Year_Key",
                schema: "docs",
                table: "person_background_times",
                columns: new[] { "CompanyId", "PersonId", "GroupId", "Year", "Key" });

            migrationBuilder.CreateIndex(
                name: "IX_person_driver_licenses_DriverLicenseDefinitionId",
                schema: "docs",
                table: "person_driver_licenses",
                column: "DriverLicenseDefinitionId");

            migrationBuilder.CreateIndex(
                name: "IX_person_holiday_schedules_PersonId_Year",
                schema: "docs",
                table: "person_holiday_schedules",
                columns: new[] { "PersonId", "Year" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_person_law_penalties_CauseId",
                schema: "docs",
                table: "person_law_penalties",
                column: "CauseId");

            migrationBuilder.CreateIndex(
                name: "IX_person_law_penalties_LawPenaltyDefinitionId",
                schema: "docs",
                table: "person_law_penalties",
                column: "LawPenaltyDefinitionId");

            migrationBuilder.CreateIndex(
                name: "IX_person_military_records_LocationId",
                schema: "docs",
                table: "person_military_records",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_person_scholarships_DegreeId",
                schema: "docs",
                table: "person_scholarships",
                column: "DegreeId");

            migrationBuilder.CreateIndex(
                name: "IX_person_scholarships_ScholarshipLevelId",
                schema: "docs",
                table: "person_scholarships",
                column: "ScholarshipLevelId");

            migrationBuilder.CreateIndex(
                name: "IX_person_taxes_CompanyId_PersonId_GroupId",
                schema: "docs",
                table: "person_taxes",
                columns: new[] { "CompanyId", "PersonId", "GroupId" });

            migrationBuilder.CreateIndex(
                name: "IX_qualification_requirement_definitions_OccupationId",
                schema: "est",
                table: "qualification_requirement_definitions",
                column: "OccupationId");

            migrationBuilder.CreateIndex(
                name: "IX_retention_definitions_Code_CompanyId",
                schema: "ret",
                table: "retention_definitions",
                columns: new[] { "Code", "CompanyId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_retention_definitions_RefundDefinitionId",
                schema: "ret",
                table: "retention_definitions",
                column: "RefundDefinitionId");

            migrationBuilder.CreateIndex(
                name: "IX_retention_documents_Code",
                schema: "ret",
                table: "retention_documents",
                column: "Code");

            migrationBuilder.CreateIndex(
                name: "IX_retention_documents_PersonId_CompanyId_DocumentDefinitionId_RetentionDefinitionId",
                schema: "ret",
                table: "retention_documents",
                columns: new[] { "PersonId", "CompanyId", "DocumentDefinitionId", "RetentionDefinitionId" });

            migrationBuilder.CreateIndex(
                name: "IX_retention_documents_RetentionDefinitionId",
                schema: "ret",
                table: "retention_documents",
                column: "RetentionDefinitionId");

            migrationBuilder.CreateIndex(
                name: "IX_sign_on_documents_CompanyId_Code_IdentityCard",
                schema: "docs",
                table: "sign_on_documents",
                columns: new[] { "CompanyId", "Code", "IdentityCard" });

            migrationBuilder.CreateIndex(
                name: "IX_subsidy_documents_DocumentDefinitionId_PersonId_EmploymentId_CompanyId",
                schema: "sub",
                table: "subsidy_documents",
                columns: new[] { "DocumentDefinitionId", "PersonId", "EmploymentId", "CompanyId" });

            migrationBuilder.CreateIndex(
                name: "IX_subsidy_documents_EmploymentId",
                schema: "sub",
                table: "subsidy_documents",
                column: "EmploymentId");

            migrationBuilder.CreateIndex(
                name: "IX_subsidy_documents_PreviousId",
                schema: "sub",
                table: "subsidy_documents",
                column: "PreviousId");

            migrationBuilder.CreateIndex(
                name: "IX_subsidy_documents_SubsidyPaymentDefinitionId",
                schema: "sub",
                table: "subsidy_documents",
                column: "SubsidyPaymentDefinitionId");

            migrationBuilder.CreateIndex(
                name: "IX_subsidy_note_transitions_CompanyId_SubsidyNoteId_DocumentId",
                schema: "sub",
                table: "subsidy_note_transitions",
                columns: new[] { "CompanyId", "SubsidyNoteId", "DocumentId" });

            migrationBuilder.CreateIndex(
                name: "IX_subsidy_note_transitions_SubsidyNoteId",
                schema: "sub",
                table: "subsidy_note_transitions",
                column: "SubsidyNoteId");

            migrationBuilder.CreateIndex(
                name: "IX_subsidy_notes_DocumentId_CompanyId_PeriodId",
                schema: "sub",
                table: "subsidy_notes",
                columns: new[] { "DocumentId", "CompanyId", "PeriodId" });

            migrationBuilder.CreateIndex(
                name: "IX_subsidy_payment_definitions_PaymentDefinitionId",
                schema: "sub",
                table: "subsidy_payment_definitions",
                column: "PaymentDefinitionId");

            migrationBuilder.CreateIndex(
                name: "IX_template_documents_ApprovedById",
                schema: "est",
                table: "template_documents",
                column: "ApprovedById");

            migrationBuilder.CreateIndex(
                name: "IX_template_documents_DocumentDefinitionId_CompanyId",
                schema: "est",
                table: "template_documents",
                columns: new[] { "DocumentDefinitionId", "CompanyId" });

            migrationBuilder.CreateIndex(
                name: "IX_template_documents_MadeById",
                schema: "est",
                table: "template_documents",
                column: "MadeById");

            migrationBuilder.CreateIndex(
                name: "IX_template_job_positions_CompanyId_TemplateId",
                schema: "est",
                table: "template_job_positions",
                columns: new[] { "CompanyId", "TemplateId" });

            migrationBuilder.CreateIndex(
                name: "IX_template_job_positions_CompanyId_TemplateId_OccupationId_ScholarshipLevelId_OrganizationUnitId",
                schema: "est",
                table: "template_job_positions",
                columns: new[] { "CompanyId", "TemplateId", "OccupationId", "ScholarshipLevelId", "OrganizationUnitId" });

            migrationBuilder.CreateIndex(
                name: "IX_template_job_positions_DocumentId",
                schema: "est",
                table: "template_job_positions",
                column: "DocumentId");

            migrationBuilder.CreateIndex(
                name: "IX_template_job_positions_OccupationId",
                schema: "est",
                table: "template_job_positions",
                column: "OccupationId");

            migrationBuilder.CreateIndex(
                name: "IX_template_job_positions_ScholarshipLevelId",
                schema: "est",
                table: "template_job_positions",
                column: "ScholarshipLevelId");

            migrationBuilder.CreateIndex(
                name: "IX_template_job_positions_TemplateId",
                schema: "est",
                table: "template_job_positions",
                column: "TemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_template_job_positions_TemporalDocumentId",
                schema: "est",
                table: "template_job_positions",
                column: "TemporalDocumentId");

            migrationBuilder.CreateIndex(
                name: "IX_template_job_positions_WorkShiftId",
                schema: "est",
                table: "template_job_positions",
                column: "WorkShiftId");

            migrationBuilder.CreateIndex(
                name: "IX_templates_CompanyId_DocumentId_OccupationId_ScholarshipLevelId_OrganizationUnitId",
                schema: "est",
                table: "templates",
                columns: new[] { "CompanyId", "DocumentId", "OccupationId", "ScholarshipLevelId", "OrganizationUnitId" });

            migrationBuilder.CreateIndex(
                name: "IX_templates_DocumentId",
                schema: "est",
                table: "templates",
                column: "DocumentId");

            migrationBuilder.CreateIndex(
                name: "IX_templates_OccupationId",
                schema: "est",
                table: "templates",
                column: "OccupationId");

            migrationBuilder.CreateIndex(
                name: "IX_templates_ScholarshipLevelId",
                schema: "est",
                table: "templates",
                column: "ScholarshipLevelId");

            migrationBuilder.CreateIndex(
                name: "IX_time_distribution_documents_DocumentDefinitionId_CompanyId_PeriodId",
                schema: "sal",
                table: "time_distribution_documents",
                columns: new[] { "DocumentDefinitionId", "CompanyId", "PeriodId" });

            migrationBuilder.CreateIndex(
                name: "IX_time_distribution_pluses_EmploymentPlusId",
                schema: "sal",
                table: "time_distribution_pluses",
                column: "EmploymentPlusId");

            migrationBuilder.CreateIndex(
                name: "IX_time_distribution_pluses_TimeDistributionId_EmploymentPlusId_CompanyId",
                schema: "sal",
                table: "time_distribution_pluses",
                columns: new[] { "TimeDistributionId", "EmploymentPlusId", "CompanyId" });

            migrationBuilder.CreateIndex(
                name: "IX_time_distributions_DocumentId_PersonId_EmploymentId_CompanyId",
                schema: "sal",
                table: "time_distributions",
                columns: new[] { "DocumentId", "PersonId", "EmploymentId", "CompanyId" });

            migrationBuilder.CreateIndex(
                name: "IX_time_distributions_EmploymentId",
                schema: "sal",
                table: "time_distributions",
                column: "EmploymentId");

            migrationBuilder.CreateIndex(
                name: "IX_time_distributions_PaymentDefinitionId",
                schema: "sal",
                table: "time_distributions",
                column: "PaymentDefinitionId");

            migrationBuilder.CreateIndex(
                name: "IX_work_shifts_RegimeId",
                schema: "gen",
                table: "work_shifts",
                column: "RegimeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "accounting_document_summaries",
                schema: "cnt");

            migrationBuilder.DropTable(
                name: "accounting_tax_summaries",
                schema: "cnt");

            migrationBuilder.DropTable(
                name: "adjustment_document_transitions",
                schema: "aju");

            migrationBuilder.DropTable(
                name: "claim_documents",
                schema: "rec");

            migrationBuilder.DropTable(
                name: "employment_documents_to_generate",
                schema: "rel");

            migrationBuilder.DropTable(
                name: "employment_indexes",
                schema: "rel");

            migrationBuilder.DropTable(
                name: "holiday_histogram",
                schema: "vac");

            migrationBuilder.DropTable(
                name: "holiday_note_transitions",
                schema: "vac");

            migrationBuilder.DropTable(
                name: "incidents",
                schema: "sal");

            migrationBuilder.DropTable(
                name: "payment_histogram",
                schema: "sal");

            migrationBuilder.DropTable(
                name: "performance_evaluation_law_penalties",
                schema: "rel");

            migrationBuilder.DropTable(
                name: "person_accounts",
                schema: "docs");

            migrationBuilder.DropTable(
                name: "person_background_times",
                schema: "docs");

            migrationBuilder.DropTable(
                name: "person_driver_licenses",
                schema: "docs");

            migrationBuilder.DropTable(
                name: "person_family_relationships",
                schema: "docs");

            migrationBuilder.DropTable(
                name: "person_holiday_schedules",
                schema: "docs");

            migrationBuilder.DropTable(
                name: "person_military_records",
                schema: "docs");

            migrationBuilder.DropTable(
                name: "person_scholarships",
                schema: "docs");

            migrationBuilder.DropTable(
                name: "person_taxes",
                schema: "docs");

            migrationBuilder.DropTable(
                name: "qualification_requirement_definitions",
                schema: "est");

            migrationBuilder.DropTable(
                name: "retention_documents",
                schema: "ret");

            migrationBuilder.DropTable(
                name: "subsidy_note_transitions",
                schema: "sub");

            migrationBuilder.DropTable(
                name: "template_job_positions",
                schema: "est");

            migrationBuilder.DropTable(
                name: "time_distribution_pluses",
                schema: "sal");

            migrationBuilder.DropTable(
                name: "adjustment_documents",
                schema: "aju");

            migrationBuilder.DropTable(
                name: "holiday_notes",
                schema: "vac");

            migrationBuilder.DropTable(
                name: "performance_evaluations",
                schema: "rel");

            migrationBuilder.DropTable(
                name: "person_law_penalties",
                schema: "docs");

            migrationBuilder.DropTable(
                name: "driver_license_definitions",
                schema: "gen");

            migrationBuilder.DropTable(
                name: "military_location_definitions",
                schema: "gen");

            migrationBuilder.DropTable(
                name: "degree_definitions",
                schema: "gen");

            migrationBuilder.DropTable(
                name: "retention_definitions",
                schema: "ret");

            migrationBuilder.DropTable(
                name: "subsidy_notes",
                schema: "sub");

            migrationBuilder.DropTable(
                name: "templates",
                schema: "est");

            migrationBuilder.DropTable(
                name: "employment_pluses",
                schema: "rel");

            migrationBuilder.DropTable(
                name: "time_distributions",
                schema: "sal");

            migrationBuilder.DropTable(
                name: "holiday_documents",
                schema: "vac");

            migrationBuilder.DropTable(
                name: "performance_documents",
                schema: "rel");

            migrationBuilder.DropTable(
                name: "performance_summaries",
                schema: "rel");

            migrationBuilder.DropTable(
                name: "law_penalty_cause_definitions",
                schema: "gen");

            migrationBuilder.DropTable(
                name: "law_penalty_definitions",
                schema: "gen");

            migrationBuilder.DropTable(
                name: "adjustment_definitions",
                schema: "aju");

            migrationBuilder.DropTable(
                name: "subsidy_documents",
                schema: "sub");

            migrationBuilder.DropTable(
                name: "occupations",
                schema: "est");

            migrationBuilder.DropTable(
                name: "scholarship_level_definitions",
                schema: "gen");

            migrationBuilder.DropTable(
                name: "template_documents",
                schema: "est");

            migrationBuilder.DropTable(
                name: "plus_definitions",
                schema: "sal");

            migrationBuilder.DropTable(
                name: "time_distribution_documents",
                schema: "sal");

            migrationBuilder.DropTable(
                name: "employment_documents",
                schema: "rel");

            migrationBuilder.DropTable(
                name: "subsidy_payment_definitions",
                schema: "sub");

            migrationBuilder.DropTable(
                name: "complexity_groups",
                schema: "est");

            migrationBuilder.DropTable(
                name: "occupational_categories",
                schema: "est");

            migrationBuilder.DropTable(
                name: "responsibility_levels",
                schema: "est");

            migrationBuilder.DropTable(
                name: "employment_summaries",
                schema: "rel");

            migrationBuilder.DropTable(
                name: "sign_on_documents",
                schema: "docs");

            migrationBuilder.DropTable(
                name: "work_shifts",
                schema: "gen");

            migrationBuilder.DropTable(
                name: "payment_definitions",
                schema: "sal");

            migrationBuilder.DropTable(
                name: "work_regimens",
                schema: "gen");
        }
    }
}
