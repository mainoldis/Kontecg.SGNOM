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
                name: "log");

            migrationBuilder.EnsureSchema(
                name: "job");

            migrationBuilder.EnsureSchema(
                name: "gen");

            migrationBuilder.EnsureSchema(
                name: "seg");

            migrationBuilder.EnsureSchema(
                name: "docs");

            migrationBuilder.EnsureSchema(
                name: "est");

            migrationBuilder.CreateTable(
                name: "account_definitions",
                schema: "cnt",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Account = table.Column<int>(type: "int", nullable: false),
                    SubAccount = table.Column<int>(type: "int", nullable: false),
                    SubControl = table.Column<int>(type: "int", nullable: false),
                    Analysis = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Reference = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Kind = table.Column<string>(type: "nvarchar(6)", maxLength: 6, nullable: false),
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
                    table.PrimaryKey("PK_account_definitions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "accounting_classifier_definitions",
                schema: "cnt",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Description = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_accounting_classifier_definitions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "accounting_function_definition_storage",
                schema: "cnt",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Description = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Script = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorUserId = table.Column<long>(type: "bigint", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifierUserId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_accounting_function_definition_storage", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "accounting_function_view_names",
                schema: "cnt",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(1024)", maxLength: 1024, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    ReferenceGroup = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorUserId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_accounting_function_view_names", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "audit_logs",
                schema: "log",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<long>(type: "bigint", nullable: true),
                    ServiceName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    MethodName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Parameters = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: true),
                    ReturnValue = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ExecutionTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ExecutionDuration = table.Column<int>(type: "int", nullable: false),
                    ClientIpAddress = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    ClientName = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    ClientInfo = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: true),
                    ExceptionMessage = table.Column<string>(type: "nvarchar(1024)", maxLength: 1024, nullable: true),
                    Exception = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    ImpersonatorUserId = table.Column<long>(type: "bigint", nullable: true),
                    ImpersonatorCompanyId = table.Column<int>(type: "int", nullable: true),
                    CustomData = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    CompanyId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_audit_logs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "background_jobs",
                schema: "job",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    JobType = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: false),
                    JobArgs = table.Column<string>(type: "nvarchar(max)", maxLength: 1048576, nullable: false),
                    TryCount = table.Column<short>(type: "smallint", nullable: false),
                    NextTryTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastTryTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsAbandoned = table.Column<bool>(type: "bit", nullable: false),
                    Priority = table.Column<byte>(type: "tinyint", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorUserId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_background_jobs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "banks",
                schema: "gen",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorUserId = table.Column<long>(type: "bigint", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifierUserId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_banks", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "bill_denominations",
                schema: "gen",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Currency = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    Bill = table.Column<decimal>(type: "decimal(28,24)", precision: 28, scale: 24, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorUserId = table.Column<long>(type: "bigint", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifierUserId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_bill_denominations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "clients",
                schema: "seg",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    IpAddress = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Info = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: false),
                    Version = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    CompanyId = table.Column<int>(type: "int", nullable: false),
                    ExtensionData = table.Column<string>(type: "nvarchar(max)", nullable: true),
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
                    table.PrimaryKey("PK_clients", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "company_notifications",
                schema: "log",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NotificationName = table.Column<string>(type: "nvarchar(96)", maxLength: 96, nullable: false),
                    Data = table.Column<string>(type: "nvarchar(max)", maxLength: 1048576, nullable: true),
                    DataTypeName = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
                    EntityTypeName = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    EntityTypeAssemblyQualifiedName = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
                    EntityId = table.Column<string>(type: "nvarchar(96)", maxLength: 96, nullable: true),
                    Severity = table.Column<byte>(type: "tinyint", nullable: false),
                    CompanyId = table.Column<int>(type: "int", nullable: true),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorUserId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_company_notifications", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "countries",
                schema: "gen",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RegionName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Acronym = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: true),
                    InternationalCode = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_countries", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "document_definitions",
                schema: "gen",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Reference = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    ReferenceGroup = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    Legal = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    LegalTypeAssemblyQualifiedName = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
                    WorkflowDefinitionDefinitionId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ExtensionData = table.Column<string>(type: "nvarchar(max)", nullable: true),
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
                    table.PrimaryKey("PK_document_definitions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "dynamic_properties",
                schema: "gen",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PropertyName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    DisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    InputType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Permission = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CompanyId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_dynamic_properties", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "entity_change_sets",
                schema: "log",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClientInfo = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
                    ClientIpAddress = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    ClientName = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    Reason = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    UserId = table.Column<long>(type: "bigint", nullable: true),
                    ImpersonatorCompanyId = table.Column<int>(type: "int", nullable: true),
                    ImpersonatorUserId = table.Column<long>(type: "bigint", nullable: true),
                    ExtensionData = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CompanyId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_entity_change_sets", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "files",
                schema: "docs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Bytes = table.Column<byte[]>(type: "varbinary(max)", maxLength: 204800, nullable: false),
                    CompanyId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_files", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "language_texts",
                schema: "gen",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LanguageName = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Source = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Key = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", maxLength: 67108864, nullable: false),
                    CompanyId = table.Column<int>(type: "int", nullable: true),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorUserId = table.Column<long>(type: "bigint", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifierUserId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_language_texts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "languages",
                schema: "gen",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    DisplayName = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    Icon = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    IsDisabled = table.Column<bool>(type: "bit", nullable: false),
                    CompanyId = table.Column<int>(type: "int", nullable: true),
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
                    table.PrimaryKey("PK_languages", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "notification_subscriptions",
                schema: "log",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    NotificationName = table.Column<string>(type: "nvarchar(96)", maxLength: 96, nullable: true),
                    EntityTypeName = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    EntityTypeAssemblyQualifiedName = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
                    EntityId = table.Column<string>(type: "nvarchar(96)", maxLength: 96, nullable: true),
                    CompanyId = table.Column<int>(type: "int", nullable: true),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorUserId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_notification_subscriptions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "notifications",
                schema: "log",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NotificationName = table.Column<string>(type: "nvarchar(96)", maxLength: 96, nullable: false),
                    Data = table.Column<string>(type: "nvarchar(max)", maxLength: 1048576, nullable: true),
                    DataTypeName = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
                    EntityTypeName = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    EntityTypeAssemblyQualifiedName = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
                    EntityId = table.Column<string>(type: "nvarchar(96)", maxLength: 96, nullable: true),
                    Severity = table.Column<byte>(type: "tinyint", nullable: false),
                    UserIds = table.Column<string>(type: "nvarchar(max)", maxLength: 131072, nullable: true),
                    ExcludedUserIds = table.Column<string>(type: "nvarchar(max)", maxLength: 131072, nullable: true),
                    CompanyIds = table.Column<string>(type: "nvarchar(max)", maxLength: 131072, nullable: true),
                    TargetNotifiers = table.Column<string>(type: "nvarchar(1024)", maxLength: 1024, nullable: true),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorUserId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_notifications", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "organization_unit_roles",
                schema: "seg",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<int>(type: "int", nullable: false),
                    OrganizationUnitId = table.Column<long>(type: "bigint", nullable: false),
                    CompanyId = table.Column<int>(type: "int", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorUserId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_organization_unit_roles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "organization_units_center_costs",
                schema: "est",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrganizationUnitId = table.Column<long>(type: "bigint", nullable: false),
                    CompanyId = table.Column<int>(type: "int", nullable: true),
                    CenterCostId = table.Column<int>(type: "int", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorUserId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_organization_units_center_costs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "periods",
                schema: "gen",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Year = table.Column<int>(type: "int", nullable: false),
                    Month = table.Column<int>(type: "int", nullable: false),
                    Quarter = table.Column<int>(type: "int", nullable: false),
                    Since = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Until = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(8)", maxLength: 8, nullable: false),
                    CompanyId = table.Column<int>(type: "int", nullable: false),
                    ReferenceGroup = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorUserId = table.Column<long>(type: "bigint", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifierUserId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_periods", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "persons",
                schema: "docs",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    Surname = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    Lastname = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    IdentityCard = table.Column<string>(type: "nvarchar(11)", maxLength: 11, nullable: false),
                    Gender = table.Column<string>(type: "nvarchar(1)", maxLength: 1, nullable: false),
                    BirthDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PhotoId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    PhotoFileType = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    ScholarshipLevel = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    Scholarship = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    ExtensionData = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorUserId = table.Column<long>(type: "bigint", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifierUserId = table.Column<long>(type: "bigint", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeleterUserId = table.Column<long>(type: "bigint", nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClothingSize = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Etnia = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_persons", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "recent_passwords",
                schema: "seg",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CompanyId = table.Column<int>(type: "int", nullable: true),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorUserId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_recent_passwords", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "signs",
                schema: "docs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
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
                    table.PrimaryKey("PK_signs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "special_dates",
                schema: "gen",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Cause = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_special_dates", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "user_accounts",
                schema: "seg",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CompanyId = table.Column<int>(type: "int", nullable: true),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    UserLinkId = table.Column<long>(type: "bigint", nullable: true),
                    UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    EmailAddress = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
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
                    table.PrimaryKey("PK_user_accounts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "user_delegations",
                schema: "seg",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SourceUserId = table.Column<long>(type: "bigint", nullable: false),
                    TargetUserId = table.Column<long>(type: "bigint", nullable: false),
                    CompanyId = table.Column<int>(type: "int", nullable: true),
                    StartTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndTime = table.Column<DateTime>(type: "datetime2", nullable: false),
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
                    table.PrimaryKey("PK_user_delegations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "user_login_attempts",
                schema: "log",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CompanyName = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    UserId = table.Column<long>(type: "bigint", nullable: true),
                    UserNameOrEmailAddress = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ClientIpAddress = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    ClientName = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    ClientInfo = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: true),
                    Result = table.Column<byte>(type: "tinyint", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FailReason = table.Column<string>(type: "nvarchar(1024)", maxLength: 1024, nullable: true),
                    CompanyId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_login_attempts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "user_notifications",
                schema: "log",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    CompanyNotificationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    State = table.Column<int>(type: "int", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CompanyId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_notifications", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "webhook_events",
                schema: "job",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    WebhookName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Data = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletionTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_webhook_events", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "webhook_subscriptions",
                schema: "job",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CompanyId = table.Column<int>(type: "int", nullable: true),
                    WebhookUri = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Secret = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Webhooks = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Headers = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorUserId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_webhook_subscriptions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "workplace_classifications",
                schema: "est",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Description = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    Level = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_workplace_classifications", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "workplace_payments",
                schema: "est",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(8)", maxLength: 8, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    CompanyId = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorUserId = table.Column<long>(type: "bigint", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifierUserId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_workplace_payments", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "center_cost_definitions",
                schema: "cnt",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Description = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Code = table.Column<int>(type: "int", nullable: false),
                    Reference = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    AccountDefinitionId = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CompanyId = table.Column<int>(type: "int", nullable: false),
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
                    table.PrimaryKey("PK_center_cost_definitions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_center_cost_definitions_account_definitions_AccountDefinitionId",
                        column: x => x.AccountDefinitionId,
                        principalSchema: "cnt",
                        principalTable: "account_definitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "accounting_function_definitions",
                schema: "cnt",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Reference = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    StorageId = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorUserId = table.Column<long>(type: "bigint", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifierUserId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_accounting_function_definitions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_accounting_function_definitions_accounting_function_definition_storage_StorageId",
                        column: x => x.StorageId,
                        principalSchema: "cnt",
                        principalTable: "accounting_function_definition_storage",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "bank_offices",
                schema: "gen",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    BankId = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorUserId = table.Column<long>(type: "bigint", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifierUserId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_bank_offices", x => x.Id);
                    table.ForeignKey(
                        name: "FK_bank_offices_banks_BankId",
                        column: x => x.BankId,
                        principalSchema: "gen",
                        principalTable: "banks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "exchange_rates",
                schema: "cnt",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BankId = table.Column<int>(type: "int", nullable: false),
                    From = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    To = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    Rate = table.Column<decimal>(type: "decimal(28,24)", precision: 28, scale: 24, nullable: false),
                    Scope = table.Column<int>(type: "int", nullable: false),
                    Since = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Until = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorUserId = table.Column<long>(type: "bigint", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifierUserId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_exchange_rates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_exchange_rates_banks_BankId",
                        column: x => x.BankId,
                        principalSchema: "gen",
                        principalTable: "banks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "features",
                schema: "gen",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Value = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    CompanyId = table.Column<int>(type: "int", nullable: true),
                    Discriminator = table.Column<string>(type: "nvarchar(21)", maxLength: 21, nullable: false),
                    ClientId = table.Column<string>(type: "nvarchar(128)", nullable: true),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorUserId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_features", x => x.Id);
                    table.ForeignKey(
                        name: "FK_features_clients_ClientId",
                        column: x => x.ClientId,
                        principalSchema: "seg",
                        principalTable: "clients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "states",
                schema: "gen",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    Code = table.Column<string>(type: "nvarchar(4)", maxLength: 4, nullable: false),
                    RegionName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CountryId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_states", x => x.Id);
                    table.ForeignKey(
                        name: "FK_states_countries_CountryId",
                        column: x => x.CountryId,
                        principalSchema: "gen",
                        principalTable: "countries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "accounting_function_views",
                schema: "cnt",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DocumentDefinitionId = table.Column<int>(type: "int", nullable: false),
                    AccountingFunctionViewNameId = table.Column<int>(type: "int", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorUserId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_accounting_function_views", x => x.Id);
                    table.ForeignKey(
                        name: "FK_accounting_function_views_accounting_function_view_names_AccountingFunctionViewNameId",
                        column: x => x.AccountingFunctionViewNameId,
                        principalSchema: "cnt",
                        principalTable: "accounting_function_view_names",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_accounting_function_views_document_definitions_DocumentDefinitionId",
                        column: x => x.DocumentDefinitionId,
                        principalSchema: "gen",
                        principalTable: "document_definitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "dynamic_entity_properties",
                schema: "gen",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EntityFullName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    DynamicPropertyId = table.Column<int>(type: "int", nullable: false),
                    CompanyId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_dynamic_entity_properties", x => x.Id);
                    table.ForeignKey(
                        name: "FK_dynamic_entity_properties_dynamic_properties_DynamicPropertyId",
                        column: x => x.DynamicPropertyId,
                        principalSchema: "gen",
                        principalTable: "dynamic_properties",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "dynamic_property_values",
                schema: "gen",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DynamicPropertyId = table.Column<int>(type: "int", nullable: false),
                    CompanyId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_dynamic_property_values", x => x.Id);
                    table.ForeignKey(
                        name: "FK_dynamic_property_values_dynamic_properties_DynamicPropertyId",
                        column: x => x.DynamicPropertyId,
                        principalSchema: "gen",
                        principalTable: "dynamic_properties",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "entity_changes",
                schema: "log",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ChangeTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ChangeType = table.Column<byte>(type: "tinyint", nullable: false),
                    EntityChangeSetId = table.Column<long>(type: "bigint", nullable: false),
                    EntityId = table.Column<string>(type: "nvarchar(48)", maxLength: 48, nullable: true),
                    EntityTypeFullName = table.Column<string>(type: "nvarchar(192)", maxLength: 192, nullable: true),
                    CompanyId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_entity_changes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_entity_changes_entity_change_sets_EntityChangeSetId",
                        column: x => x.EntityChangeSetId,
                        principalSchema: "log",
                        principalTable: "entity_change_sets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "accounting_documents",
                schema: "cnt",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    DocumentDefinitionId = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    AccountingPeriodId = table.Column<int>(type: "int", nullable: false),
                    CompanyId = table.Column<int>(type: "int", nullable: false),
                    Review = table.Column<int>(type: "int", nullable: false),
                    Exported = table.Column<bool>(type: "bit", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorUserId = table.Column<long>(type: "bigint", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifierUserId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_accounting_documents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_accounting_documents_document_definitions_DocumentDefinitionId",
                        column: x => x.DocumentDefinitionId,
                        principalSchema: "gen",
                        principalTable: "document_definitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_accounting_documents_periods_AccountingPeriodId",
                        column: x => x.AccountingPeriodId,
                        principalSchema: "gen",
                        principalTable: "periods",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "organization_units_persons",
                schema: "est",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PersonId = table.Column<long>(type: "bigint", nullable: false),
                    OrganizationUnitId = table.Column<long>(type: "bigint", nullable: false),
                    CompanyId = table.Column<int>(type: "int", nullable: true),
                    ResponsibilityLevel = table.Column<int>(type: "int", nullable: true),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorUserId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_organization_units_persons", x => x.Id);
                    table.ForeignKey(
                        name: "FK_organization_units_persons_persons_PersonId",
                        column: x => x.PersonId,
                        principalSchema: "docs",
                        principalTable: "persons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "users",
                schema: "seg",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProfilePictureId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    PersonId = table.Column<long>(type: "bigint", nullable: true),
                    ShouldChangePasswordOnNextLogin = table.Column<bool>(type: "bit", nullable: false),
                    SignInTokenExpireTimeUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    SignInToken = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastLoginTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorUserId = table.Column<long>(type: "bigint", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifierUserId = table.Column<long>(type: "bigint", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeleterUserId = table.Column<long>(type: "bigint", nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    AuthenticationSource = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    EmailAddress = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    Surname = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    Password = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    EmailConfirmationCode = table.Column<string>(type: "nvarchar(328)", maxLength: 328, nullable: true),
                    PasswordResetCode = table.Column<string>(type: "nvarchar(328)", maxLength: 328, nullable: true),
                    LockoutEndDateUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false),
                    IsLockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: true),
                    IsPhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    SecurityStamp = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    IsEmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    CompanyId = table.Column<int>(type: "int", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    NormalizedUserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    NormalizedEmailAddress = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_users", x => x.Id);
                    table.ForeignKey(
                        name: "FK_users_persons_PersonId",
                        column: x => x.PersonId,
                        principalSchema: "docs",
                        principalTable: "persons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_users_users_CreatorUserId",
                        column: x => x.CreatorUserId,
                        principalSchema: "seg",
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_users_users_DeleterUserId",
                        column: x => x.DeleterUserId,
                        principalSchema: "seg",
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_users_users_LastModifierUserId",
                        column: x => x.LastModifierUserId,
                        principalSchema: "seg",
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "webhook_send_attempts",
                schema: "job",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    WebhookEventId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    WebhookSubscriptionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Response = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ResponseStatusCode = table.Column<int>(type: "int", nullable: true),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_webhook_send_attempts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_webhook_send_attempts_webhook_events_WebhookEventId",
                        column: x => x.WebhookEventId,
                        principalSchema: "job",
                        principalTable: "webhook_events",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "organization_units",
                schema: "est",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ParentId = table.Column<long>(type: "bigint", nullable: true),
                    Code = table.Column<string>(type: "nvarchar(12)", maxLength: 12, nullable: false),
                    DisplayName = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Order = table.Column<int>(type: "int", nullable: false),
                    CompanyId = table.Column<int>(type: "int", nullable: true),
                    Discriminator = table.Column<string>(type: "nvarchar(21)", maxLength: 21, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    MaxMembersApproved = table.Column<int>(type: "int", nullable: true),
                    ClassificationId = table.Column<int>(type: "int", nullable: true),
                    WorkPlacePaymentId = table.Column<long>(type: "bigint", nullable: true),
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
                    table.PrimaryKey("PK_organization_units", x => x.Id);
                    table.ForeignKey(
                        name: "FK_organization_units_organization_units_ParentId",
                        column: x => x.ParentId,
                        principalSchema: "est",
                        principalTable: "organization_units",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_organization_units_workplace_classifications_ClassificationId",
                        column: x => x.ClassificationId,
                        principalSchema: "est",
                        principalTable: "workplace_classifications",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_organization_units_workplace_payments_WorkPlacePaymentId",
                        column: x => x.WorkPlacePaymentId,
                        principalSchema: "est",
                        principalTable: "workplace_payments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "expense_item_definitions",
                schema: "cnt",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Description = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Code = table.Column<int>(type: "int", nullable: false),
                    Reference = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CenterCostDefinitionId = table.Column<int>(type: "int", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CompanyId = table.Column<int>(type: "int", nullable: false),
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
                    table.PrimaryKey("PK_expense_item_definitions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_expense_item_definitions_center_cost_definitions_CenterCostDefinitionId",
                        column: x => x.CenterCostDefinitionId,
                        principalSchema: "cnt",
                        principalTable: "center_cost_definitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "cities",
                schema: "gen",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    Code = table.Column<string>(type: "nvarchar(4)", maxLength: 4, nullable: false),
                    StateId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_cities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_cities_states_StateId",
                        column: x => x.StateId,
                        principalSchema: "gen",
                        principalTable: "states",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "dynamic_entity_property_values",
                schema: "gen",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EntityId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DynamicEntityPropertyId = table.Column<int>(type: "int", nullable: false),
                    CompanyId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_dynamic_entity_property_values", x => x.Id);
                    table.ForeignKey(
                        name: "FK_dynamic_entity_property_values_dynamic_entity_properties_DynamicEntityPropertyId",
                        column: x => x.DynamicEntityPropertyId,
                        principalSchema: "gen",
                        principalTable: "dynamic_entity_properties",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "entity_property_changes",
                schema: "log",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EntityChangeId = table.Column<long>(type: "bigint", nullable: false),
                    NewValue = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
                    OriginalValue = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
                    PropertyName = table.Column<string>(type: "nvarchar(96)", maxLength: 96, nullable: true),
                    PropertyTypeFullName = table.Column<string>(type: "nvarchar(192)", maxLength: 192, nullable: true),
                    NewValueHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OriginalValueHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CompanyId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_entity_property_changes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_entity_property_changes_entity_changes_EntityChangeId",
                        column: x => x.EntityChangeId,
                        principalSchema: "log",
                        principalTable: "entity_changes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "accounting_voucher_documents",
                schema: "cnt",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DocumentDefinitionId = table.Column<int>(type: "int", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    StartingDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FinishingDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DocumentId = table.Column<int>(type: "int", nullable: false),
                    CompanyId = table.Column<int>(type: "int", nullable: false),
                    Review = table.Column<int>(type: "int", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorUserId = table.Column<long>(type: "bigint", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifierUserId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_accounting_voucher_documents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_accounting_voucher_documents_accounting_documents_DocumentId",
                        column: x => x.DocumentId,
                        principalSchema: "cnt",
                        principalTable: "accounting_documents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_accounting_voucher_documents_document_definitions_DocumentDefinitionId",
                        column: x => x.DocumentDefinitionId,
                        principalSchema: "gen",
                        principalTable: "document_definitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "companies",
                schema: "gen",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Reup = table.Column<string>(type: "nvarchar(11)", maxLength: 11, nullable: true),
                    Organism = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    LogoId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LogoFileType = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    LetterHeadId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LetterHeadFileType = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorUserId = table.Column<long>(type: "bigint", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifierUserId = table.Column<long>(type: "bigint", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeleterUserId = table.Column<long>(type: "bigint", nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyName = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    ConnectionString = table.Column<string>(type: "nvarchar(1024)", maxLength: 1024, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_companies", x => x.Id);
                    table.ForeignKey(
                        name: "FK_companies_users_CreatorUserId",
                        column: x => x.CreatorUserId,
                        principalSchema: "seg",
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_companies_users_DeleterUserId",
                        column: x => x.DeleterUserId,
                        principalSchema: "seg",
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_companies_users_LastModifierUserId",
                        column: x => x.LastModifierUserId,
                        principalSchema: "seg",
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "roles",
                schema: "seg",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Description = table.Column<string>(type: "nvarchar(max)", maxLength: 5000, nullable: true),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorUserId = table.Column<long>(type: "bigint", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifierUserId = table.Column<long>(type: "bigint", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeleterUserId = table.Column<long>(type: "bigint", nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    DisplayName = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    IsStatic = table.Column<bool>(type: "bit", nullable: false),
                    IsDefault = table.Column<bool>(type: "bit", nullable: false),
                    CompanyId = table.Column<int>(type: "int", nullable: true),
                    NormalizedName = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_roles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_roles_users_CreatorUserId",
                        column: x => x.CreatorUserId,
                        principalSchema: "seg",
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_roles_users_DeleterUserId",
                        column: x => x.DeleterUserId,
                        principalSchema: "seg",
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_roles_users_LastModifierUserId",
                        column: x => x.LastModifierUserId,
                        principalSchema: "seg",
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "settings",
                schema: "gen",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<long>(type: "bigint", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CompanyId = table.Column<int>(type: "int", nullable: true),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorUserId = table.Column<long>(type: "bigint", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifierUserId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_settings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_settings_users_UserId",
                        column: x => x.UserId,
                        principalSchema: "seg",
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "user_claims",
                schema: "seg",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CompanyId = table.Column<int>(type: "int", nullable: true),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorUserId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_claims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_user_claims_users_UserId",
                        column: x => x.UserId,
                        principalSchema: "seg",
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "user_logins",
                schema: "seg",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    LoginProvider = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    CompanyId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_logins", x => x.Id);
                    table.ForeignKey(
                        name: "FK_user_logins_users_UserId",
                        column: x => x.UserId,
                        principalSchema: "seg",
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "user_organization_units",
                schema: "seg",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    OrganizationUnitId = table.Column<long>(type: "bigint", nullable: false),
                    CompanyId = table.Column<int>(type: "int", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorUserId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_organization_units", x => x.Id);
                    table.ForeignKey(
                        name: "FK_user_organization_units_users_UserId",
                        column: x => x.UserId,
                        principalSchema: "seg",
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "user_roles",
                schema: "seg",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    RoleId = table.Column<int>(type: "int", nullable: false),
                    CompanyId = table.Column<int>(type: "int", nullable: true),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorUserId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_roles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_user_roles_users_UserId",
                        column: x => x.UserId,
                        principalSchema: "seg",
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "user_tokens",
                schema: "seg",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CompanyId = table.Column<int>(type: "int", nullable: true),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    LoginProvider = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    Name = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    Value = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
                    ExpireDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_tokens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_user_tokens_users_UserId",
                        column: x => x.UserId,
                        principalSchema: "seg",
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "accounting_voucher_notes",
                schema: "cnt",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DocumentId = table.Column<int>(type: "int", nullable: false),
                    Operation = table.Column<string>(type: "nvarchar(6)", maxLength: 6, nullable: false),
                    ScopeId = table.Column<int>(type: "int", nullable: false),
                    Account = table.Column<int>(type: "int", nullable: false),
                    SubAccount = table.Column<int>(type: "int", nullable: false),
                    SubControl = table.Column<int>(type: "int", nullable: false),
                    Analysis = table.Column<int>(type: "int", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    Currency = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false, defaultValue: "CUP"),
                    CompanyId = table.Column<int>(type: "int", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorUserId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_accounting_voucher_notes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_accounting_voucher_notes_accounting_classifier_definitions_ScopeId",
                        column: x => x.ScopeId,
                        principalSchema: "cnt",
                        principalTable: "accounting_classifier_definitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_accounting_voucher_notes_accounting_voucher_documents_DocumentId",
                        column: x => x.DocumentId,
                        principalSchema: "cnt",
                        principalTable: "accounting_voucher_documents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "permissions",
                schema: "seg",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    IsGranted = table.Column<bool>(type: "bit", nullable: false),
                    CompanyId = table.Column<int>(type: "int", nullable: true),
                    Discriminator = table.Column<string>(type: "nvarchar(21)", maxLength: 21, nullable: false),
                    RoleId = table.Column<int>(type: "int", nullable: true),
                    UserId = table.Column<long>(type: "bigint", nullable: true),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorUserId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_permissions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_permissions_roles_RoleId",
                        column: x => x.RoleId,
                        principalSchema: "seg",
                        principalTable: "roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_permissions_users_UserId",
                        column: x => x.UserId,
                        principalSchema: "seg",
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "role_claims",
                schema: "seg",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<int>(type: "int", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CompanyId = table.Column<int>(type: "int", nullable: true),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorUserId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_role_claims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_role_claims_roles_RoleId",
                        column: x => x.RoleId,
                        principalSchema: "seg",
                        principalTable: "roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_account_definitions_Account_SubAccount_SubControl_Analysis_Reference",
                schema: "cnt",
                table: "account_definitions",
                columns: new[] { "Account", "SubAccount", "SubControl", "Analysis", "Reference" },
                unique: true,
                filter: "[Reference] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_accounting_documents_AccountingPeriodId",
                schema: "cnt",
                table: "accounting_documents",
                column: "AccountingPeriodId");

            migrationBuilder.CreateIndex(
                name: "IX_accounting_documents_CompanyId_AccountingPeriodId_DocumentDefinitionId",
                schema: "cnt",
                table: "accounting_documents",
                columns: new[] { "CompanyId", "AccountingPeriodId", "DocumentDefinitionId" });

            migrationBuilder.CreateIndex(
                name: "IX_accounting_documents_DocumentDefinitionId",
                schema: "cnt",
                table: "accounting_documents",
                column: "DocumentDefinitionId");

            migrationBuilder.CreateIndex(
                name: "IX_accounting_function_definitions_Name",
                schema: "cnt",
                table: "accounting_function_definitions",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_accounting_function_definitions_StorageId",
                schema: "cnt",
                table: "accounting_function_definitions",
                column: "StorageId");

            migrationBuilder.CreateIndex(
                name: "IX_accounting_function_view_names_Name_ReferenceGroup",
                schema: "cnt",
                table: "accounting_function_view_names",
                columns: new[] { "Name", "ReferenceGroup" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_accounting_function_views_AccountingFunctionViewNameId",
                schema: "cnt",
                table: "accounting_function_views",
                column: "AccountingFunctionViewNameId");

            migrationBuilder.CreateIndex(
                name: "IX_accounting_function_views_DocumentDefinitionId_AccountingFunctionViewNameId",
                schema: "cnt",
                table: "accounting_function_views",
                columns: new[] { "DocumentDefinitionId", "AccountingFunctionViewNameId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_accounting_voucher_documents_CompanyId_DocumentId_DocumentDefinitionId",
                schema: "cnt",
                table: "accounting_voucher_documents",
                columns: new[] { "CompanyId", "DocumentId", "DocumentDefinitionId" });

            migrationBuilder.CreateIndex(
                name: "IX_accounting_voucher_documents_CompanyId_StartingDate_FinishingDate",
                schema: "cnt",
                table: "accounting_voucher_documents",
                columns: new[] { "CompanyId", "StartingDate", "FinishingDate" });

            migrationBuilder.CreateIndex(
                name: "IX_accounting_voucher_documents_DocumentDefinitionId",
                schema: "cnt",
                table: "accounting_voucher_documents",
                column: "DocumentDefinitionId");

            migrationBuilder.CreateIndex(
                name: "IX_accounting_voucher_documents_DocumentId",
                schema: "cnt",
                table: "accounting_voucher_documents",
                column: "DocumentId");

            migrationBuilder.CreateIndex(
                name: "IX_accounting_voucher_notes_CompanyId_DocumentId",
                schema: "cnt",
                table: "accounting_voucher_notes",
                columns: new[] { "CompanyId", "DocumentId" });

            migrationBuilder.CreateIndex(
                name: "IX_accounting_voucher_notes_DocumentId",
                schema: "cnt",
                table: "accounting_voucher_notes",
                column: "DocumentId");

            migrationBuilder.CreateIndex(
                name: "IX_accounting_voucher_notes_ScopeId",
                schema: "cnt",
                table: "accounting_voucher_notes",
                column: "ScopeId");

            migrationBuilder.CreateIndex(
                name: "IX_audit_logs_CompanyId_ExecutionDuration",
                schema: "log",
                table: "audit_logs",
                columns: new[] { "CompanyId", "ExecutionDuration" });

            migrationBuilder.CreateIndex(
                name: "IX_audit_logs_CompanyId_ExecutionTime",
                schema: "log",
                table: "audit_logs",
                columns: new[] { "CompanyId", "ExecutionTime" });

            migrationBuilder.CreateIndex(
                name: "IX_audit_logs_CompanyId_UserId",
                schema: "log",
                table: "audit_logs",
                columns: new[] { "CompanyId", "UserId" });

            migrationBuilder.CreateIndex(
                name: "IX_background_jobs_IsAbandoned_NextTryTime",
                schema: "job",
                table: "background_jobs",
                columns: new[] { "IsAbandoned", "NextTryTime" });

            migrationBuilder.CreateIndex(
                name: "IX_bank_offices_BankId",
                schema: "gen",
                table: "bank_offices",
                column: "BankId");

            migrationBuilder.CreateIndex(
                name: "IX_bank_offices_Name",
                schema: "gen",
                table: "bank_offices",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_banks_Name",
                schema: "gen",
                table: "banks",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_bill_denominations_Currency_Bill",
                schema: "gen",
                table: "bill_denominations",
                columns: new[] { "Currency", "Bill" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_center_cost_definitions_AccountDefinitionId",
                schema: "cnt",
                table: "center_cost_definitions",
                column: "AccountDefinitionId");

            migrationBuilder.CreateIndex(
                name: "IX_center_cost_definitions_CompanyId_Code",
                schema: "cnt",
                table: "center_cost_definitions",
                columns: new[] { "CompanyId", "Code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_cities_StateId",
                schema: "gen",
                table: "cities",
                column: "StateId");

            migrationBuilder.CreateIndex(
                name: "IX_clients_Id_CompanyId",
                schema: "seg",
                table: "clients",
                columns: new[] { "Id", "CompanyId" });

            migrationBuilder.CreateIndex(
                name: "IX_companies_CompanyName",
                schema: "gen",
                table: "companies",
                column: "CompanyName");

            migrationBuilder.CreateIndex(
                name: "IX_companies_CreationTime",
                schema: "gen",
                table: "companies",
                column: "CreationTime");

            migrationBuilder.CreateIndex(
                name: "IX_companies_CreatorUserId",
                schema: "gen",
                table: "companies",
                column: "CreatorUserId");

            migrationBuilder.CreateIndex(
                name: "IX_companies_DeleterUserId",
                schema: "gen",
                table: "companies",
                column: "DeleterUserId");

            migrationBuilder.CreateIndex(
                name: "IX_companies_LastModifierUserId",
                schema: "gen",
                table: "companies",
                column: "LastModifierUserId");

            migrationBuilder.CreateIndex(
                name: "IX_companies_Reup",
                schema: "gen",
                table: "companies",
                column: "Reup",
                unique: true,
                filter: "[Reup] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_company_notifications_CompanyId",
                schema: "log",
                table: "company_notifications",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_document_definitions_Code_Reference_ReferenceGroup",
                schema: "gen",
                table: "document_definitions",
                columns: new[] { "Code", "Reference", "ReferenceGroup" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_dynamic_entity_properties_DynamicPropertyId",
                schema: "gen",
                table: "dynamic_entity_properties",
                column: "DynamicPropertyId");

            migrationBuilder.CreateIndex(
                name: "IX_dynamic_entity_properties_EntityFullName_DynamicPropertyId_CompanyId",
                schema: "gen",
                table: "dynamic_entity_properties",
                columns: new[] { "EntityFullName", "DynamicPropertyId", "CompanyId" },
                unique: true,
                filter: "[EntityFullName] IS NOT NULL AND [CompanyId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_dynamic_entity_property_values_DynamicEntityPropertyId",
                schema: "gen",
                table: "dynamic_entity_property_values",
                column: "DynamicEntityPropertyId");

            migrationBuilder.CreateIndex(
                name: "IX_dynamic_properties_PropertyName_CompanyId",
                schema: "gen",
                table: "dynamic_properties",
                columns: new[] { "PropertyName", "CompanyId" },
                unique: true,
                filter: "[PropertyName] IS NOT NULL AND [CompanyId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_dynamic_property_values_DynamicPropertyId",
                schema: "gen",
                table: "dynamic_property_values",
                column: "DynamicPropertyId");

            migrationBuilder.CreateIndex(
                name: "IX_entity_change_sets_CompanyId_CreationTime",
                schema: "log",
                table: "entity_change_sets",
                columns: new[] { "CompanyId", "CreationTime" });

            migrationBuilder.CreateIndex(
                name: "IX_entity_change_sets_CompanyId_Reason",
                schema: "log",
                table: "entity_change_sets",
                columns: new[] { "CompanyId", "Reason" });

            migrationBuilder.CreateIndex(
                name: "IX_entity_change_sets_CompanyId_UserId",
                schema: "log",
                table: "entity_change_sets",
                columns: new[] { "CompanyId", "UserId" });

            migrationBuilder.CreateIndex(
                name: "IX_entity_changes_EntityChangeSetId",
                schema: "log",
                table: "entity_changes",
                column: "EntityChangeSetId");

            migrationBuilder.CreateIndex(
                name: "IX_entity_changes_EntityTypeFullName_EntityId",
                schema: "log",
                table: "entity_changes",
                columns: new[] { "EntityTypeFullName", "EntityId" });

            migrationBuilder.CreateIndex(
                name: "IX_entity_property_changes_EntityChangeId",
                schema: "log",
                table: "entity_property_changes",
                column: "EntityChangeId");

            migrationBuilder.CreateIndex(
                name: "IX_exchange_rates_BankId_From_To_Scope",
                schema: "cnt",
                table: "exchange_rates",
                columns: new[] { "BankId", "From", "To", "Scope" });

            migrationBuilder.CreateIndex(
                name: "IX_expense_item_definitions_CenterCostDefinitionId",
                schema: "cnt",
                table: "expense_item_definitions",
                column: "CenterCostDefinitionId");

            migrationBuilder.CreateIndex(
                name: "IX_expense_item_definitions_CompanyId_Code",
                schema: "cnt",
                table: "expense_item_definitions",
                columns: new[] { "CompanyId", "Code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_features_ClientId_Name",
                schema: "gen",
                table: "features",
                columns: new[] { "ClientId", "Name" });

            migrationBuilder.CreateIndex(
                name: "IX_features_CompanyId_Name",
                schema: "gen",
                table: "features",
                columns: new[] { "CompanyId", "Name" });

            migrationBuilder.CreateIndex(
                name: "IX_files_CompanyId",
                schema: "docs",
                table: "files",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_language_texts_CompanyId_Source_LanguageName_Key",
                schema: "gen",
                table: "language_texts",
                columns: new[] { "CompanyId", "Source", "LanguageName", "Key" });

            migrationBuilder.CreateIndex(
                name: "IX_languages_CompanyId_Name",
                schema: "gen",
                table: "languages",
                columns: new[] { "CompanyId", "Name" });

            migrationBuilder.CreateIndex(
                name: "IX_notification_subscriptions_CompanyId_NotificationName_EntityTypeName_EntityId_UserId",
                schema: "log",
                table: "notification_subscriptions",
                columns: new[] { "CompanyId", "NotificationName", "EntityTypeName", "EntityId", "UserId" });

            migrationBuilder.CreateIndex(
                name: "IX_notification_subscriptions_NotificationName_EntityTypeName_EntityId_UserId",
                schema: "log",
                table: "notification_subscriptions",
                columns: new[] { "NotificationName", "EntityTypeName", "EntityId", "UserId" });

            migrationBuilder.CreateIndex(
                name: "IX_organization_unit_roles_CompanyId_OrganizationUnitId",
                schema: "seg",
                table: "organization_unit_roles",
                columns: new[] { "CompanyId", "OrganizationUnitId" });

            migrationBuilder.CreateIndex(
                name: "IX_organization_unit_roles_CompanyId_RoleId",
                schema: "seg",
                table: "organization_unit_roles",
                columns: new[] { "CompanyId", "RoleId" });

            migrationBuilder.CreateIndex(
                name: "IX_organization_units_ClassificationId",
                schema: "est",
                table: "organization_units",
                column: "ClassificationId");

            migrationBuilder.CreateIndex(
                name: "IX_organization_units_CompanyId_Code",
                schema: "est",
                table: "organization_units",
                columns: new[] { "CompanyId", "Code" });

            migrationBuilder.CreateIndex(
                name: "IX_organization_units_ParentId",
                schema: "est",
                table: "organization_units",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_organization_units_WorkPlacePaymentId",
                schema: "est",
                table: "organization_units",
                column: "WorkPlacePaymentId");

            migrationBuilder.CreateIndex(
                name: "IX_organization_units_persons_PersonId",
                schema: "est",
                table: "organization_units_persons",
                column: "PersonId");

            migrationBuilder.CreateIndex(
                name: "IX_periods_CompanyId_ReferenceGroup_Year_Month",
                schema: "gen",
                table: "periods",
                columns: new[] { "CompanyId", "ReferenceGroup", "Year", "Month" });

            migrationBuilder.CreateIndex(
                name: "IX_permissions_CompanyId_Name",
                schema: "seg",
                table: "permissions",
                columns: new[] { "CompanyId", "Name" });

            migrationBuilder.CreateIndex(
                name: "IX_permissions_RoleId",
                schema: "seg",
                table: "permissions",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_permissions_UserId",
                schema: "seg",
                table: "permissions",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_persons_IdentityCard",
                schema: "docs",
                table: "persons",
                column: "IdentityCard",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_role_claims_CompanyId_ClaimType",
                schema: "seg",
                table: "role_claims",
                columns: new[] { "CompanyId", "ClaimType" });

            migrationBuilder.CreateIndex(
                name: "IX_role_claims_RoleId",
                schema: "seg",
                table: "role_claims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_roles_CompanyId_NormalizedName",
                schema: "seg",
                table: "roles",
                columns: new[] { "CompanyId", "NormalizedName" });

            migrationBuilder.CreateIndex(
                name: "IX_roles_CreatorUserId",
                schema: "seg",
                table: "roles",
                column: "CreatorUserId");

            migrationBuilder.CreateIndex(
                name: "IX_roles_DeleterUserId",
                schema: "seg",
                table: "roles",
                column: "DeleterUserId");

            migrationBuilder.CreateIndex(
                name: "IX_roles_LastModifierUserId",
                schema: "seg",
                table: "roles",
                column: "LastModifierUserId");

            migrationBuilder.CreateIndex(
                name: "IX_settings_CompanyId_Name_UserId",
                schema: "gen",
                table: "settings",
                columns: new[] { "CompanyId", "Name", "UserId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_settings_UserId",
                schema: "gen",
                table: "settings",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_signs_Code_Owner_CompanyId",
                schema: "docs",
                table: "signs",
                columns: new[] { "Code", "Owner", "CompanyId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_special_dates_Date_Cause",
                schema: "gen",
                table: "special_dates",
                columns: new[] { "Date", "Cause" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_states_CountryId",
                schema: "gen",
                table: "states",
                column: "CountryId");

            migrationBuilder.CreateIndex(
                name: "IX_user_accounts_CompanyId_EmailAddress",
                schema: "seg",
                table: "user_accounts",
                columns: new[] { "CompanyId", "EmailAddress" });

            migrationBuilder.CreateIndex(
                name: "IX_user_accounts_CompanyId_UserId",
                schema: "seg",
                table: "user_accounts",
                columns: new[] { "CompanyId", "UserId" });

            migrationBuilder.CreateIndex(
                name: "IX_user_accounts_CompanyId_UserName",
                schema: "seg",
                table: "user_accounts",
                columns: new[] { "CompanyId", "UserName" });

            migrationBuilder.CreateIndex(
                name: "IX_user_accounts_EmailAddress",
                schema: "seg",
                table: "user_accounts",
                column: "EmailAddress");

            migrationBuilder.CreateIndex(
                name: "IX_user_accounts_UserName",
                schema: "seg",
                table: "user_accounts",
                column: "UserName");

            migrationBuilder.CreateIndex(
                name: "IX_user_claims_CompanyId_ClaimType",
                schema: "seg",
                table: "user_claims",
                columns: new[] { "CompanyId", "ClaimType" });

            migrationBuilder.CreateIndex(
                name: "IX_user_claims_UserId",
                schema: "seg",
                table: "user_claims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_user_delegations_CompanyId_SourceUserId",
                schema: "seg",
                table: "user_delegations",
                columns: new[] { "CompanyId", "SourceUserId" });

            migrationBuilder.CreateIndex(
                name: "IX_user_delegations_CompanyId_TargetUserId",
                schema: "seg",
                table: "user_delegations",
                columns: new[] { "CompanyId", "TargetUserId" });

            migrationBuilder.CreateIndex(
                name: "IX_user_login_attempts_CompanyName_UserNameOrEmailAddress_Result",
                schema: "log",
                table: "user_login_attempts",
                columns: new[] { "CompanyName", "UserNameOrEmailAddress", "Result" });

            migrationBuilder.CreateIndex(
                name: "IX_user_login_attempts_UserId_CompanyId",
                schema: "log",
                table: "user_login_attempts",
                columns: new[] { "UserId", "CompanyId" });

            migrationBuilder.CreateIndex(
                name: "IX_user_logins_CompanyId_LoginProvider_ProviderKey",
                schema: "seg",
                table: "user_logins",
                columns: new[] { "CompanyId", "LoginProvider", "ProviderKey" });

            migrationBuilder.CreateIndex(
                name: "IX_user_logins_CompanyId_UserId",
                schema: "seg",
                table: "user_logins",
                columns: new[] { "CompanyId", "UserId" });

            migrationBuilder.CreateIndex(
                name: "IX_user_logins_ProviderKey_CompanyId",
                schema: "seg",
                table: "user_logins",
                columns: new[] { "ProviderKey", "CompanyId" },
                unique: true,
                filter: "[CompanyId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_user_logins_UserId",
                schema: "seg",
                table: "user_logins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_user_notifications_UserId_State_CreationTime",
                schema: "log",
                table: "user_notifications",
                columns: new[] { "UserId", "State", "CreationTime" });

            migrationBuilder.CreateIndex(
                name: "IX_user_organization_units_CompanyId_OrganizationUnitId",
                schema: "seg",
                table: "user_organization_units",
                columns: new[] { "CompanyId", "OrganizationUnitId" });

            migrationBuilder.CreateIndex(
                name: "IX_user_organization_units_CompanyId_UserId",
                schema: "seg",
                table: "user_organization_units",
                columns: new[] { "CompanyId", "UserId" });

            migrationBuilder.CreateIndex(
                name: "IX_user_organization_units_UserId",
                schema: "seg",
                table: "user_organization_units",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_user_roles_CompanyId_RoleId",
                schema: "seg",
                table: "user_roles",
                columns: new[] { "CompanyId", "RoleId" });

            migrationBuilder.CreateIndex(
                name: "IX_user_roles_CompanyId_UserId",
                schema: "seg",
                table: "user_roles",
                columns: new[] { "CompanyId", "UserId" });

            migrationBuilder.CreateIndex(
                name: "IX_user_roles_UserId",
                schema: "seg",
                table: "user_roles",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_user_tokens_CompanyId_UserId",
                schema: "seg",
                table: "user_tokens",
                columns: new[] { "CompanyId", "UserId" });

            migrationBuilder.CreateIndex(
                name: "IX_user_tokens_UserId",
                schema: "seg",
                table: "user_tokens",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_users_CompanyId_NormalizedEmailAddress",
                schema: "seg",
                table: "users",
                columns: new[] { "CompanyId", "NormalizedEmailAddress" });

            migrationBuilder.CreateIndex(
                name: "IX_users_CompanyId_NormalizedUserName",
                schema: "seg",
                table: "users",
                columns: new[] { "CompanyId", "NormalizedUserName" });

            migrationBuilder.CreateIndex(
                name: "IX_users_CreatorUserId",
                schema: "seg",
                table: "users",
                column: "CreatorUserId");

            migrationBuilder.CreateIndex(
                name: "IX_users_DeleterUserId",
                schema: "seg",
                table: "users",
                column: "DeleterUserId");

            migrationBuilder.CreateIndex(
                name: "IX_users_LastModifierUserId",
                schema: "seg",
                table: "users",
                column: "LastModifierUserId");

            migrationBuilder.CreateIndex(
                name: "IX_users_PersonId",
                schema: "seg",
                table: "users",
                column: "PersonId");

            migrationBuilder.CreateIndex(
                name: "IX_webhook_send_attempts_WebhookEventId",
                schema: "job",
                table: "webhook_send_attempts",
                column: "WebhookEventId");

            migrationBuilder.CreateIndex(
                name: "IX_workplace_payments_CompanyId_Description",
                schema: "est",
                table: "workplace_payments",
                columns: new[] { "CompanyId", "Description" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "accounting_function_definitions",
                schema: "cnt");

            migrationBuilder.DropTable(
                name: "accounting_function_views",
                schema: "cnt");

            migrationBuilder.DropTable(
                name: "accounting_voucher_notes",
                schema: "cnt");

            migrationBuilder.DropTable(
                name: "audit_logs",
                schema: "log");

            migrationBuilder.DropTable(
                name: "background_jobs",
                schema: "job");

            migrationBuilder.DropTable(
                name: "bank_offices",
                schema: "gen");

            migrationBuilder.DropTable(
                name: "bill_denominations",
                schema: "gen");

            migrationBuilder.DropTable(
                name: "cities",
                schema: "gen");

            migrationBuilder.DropTable(
                name: "companies",
                schema: "gen");

            migrationBuilder.DropTable(
                name: "company_notifications",
                schema: "log");

            migrationBuilder.DropTable(
                name: "dynamic_entity_property_values",
                schema: "gen");

            migrationBuilder.DropTable(
                name: "dynamic_property_values",
                schema: "gen");

            migrationBuilder.DropTable(
                name: "entity_property_changes",
                schema: "log");

            migrationBuilder.DropTable(
                name: "exchange_rates",
                schema: "cnt");

            migrationBuilder.DropTable(
                name: "expense_item_definitions",
                schema: "cnt");

            migrationBuilder.DropTable(
                name: "features",
                schema: "gen");

            migrationBuilder.DropTable(
                name: "files",
                schema: "docs");

            migrationBuilder.DropTable(
                name: "language_texts",
                schema: "gen");

            migrationBuilder.DropTable(
                name: "languages",
                schema: "gen");

            migrationBuilder.DropTable(
                name: "notification_subscriptions",
                schema: "log");

            migrationBuilder.DropTable(
                name: "notifications",
                schema: "log");

            migrationBuilder.DropTable(
                name: "organization_unit_roles",
                schema: "seg");

            migrationBuilder.DropTable(
                name: "organization_units",
                schema: "est");

            migrationBuilder.DropTable(
                name: "organization_units_center_costs",
                schema: "est");

            migrationBuilder.DropTable(
                name: "organization_units_persons",
                schema: "est");

            migrationBuilder.DropTable(
                name: "permissions",
                schema: "seg");

            migrationBuilder.DropTable(
                name: "recent_passwords",
                schema: "seg");

            migrationBuilder.DropTable(
                name: "role_claims",
                schema: "seg");

            migrationBuilder.DropTable(
                name: "settings",
                schema: "gen");

            migrationBuilder.DropTable(
                name: "signs",
                schema: "docs");

            migrationBuilder.DropTable(
                name: "special_dates",
                schema: "gen");

            migrationBuilder.DropTable(
                name: "user_accounts",
                schema: "seg");

            migrationBuilder.DropTable(
                name: "user_claims",
                schema: "seg");

            migrationBuilder.DropTable(
                name: "user_delegations",
                schema: "seg");

            migrationBuilder.DropTable(
                name: "user_login_attempts",
                schema: "log");

            migrationBuilder.DropTable(
                name: "user_logins",
                schema: "seg");

            migrationBuilder.DropTable(
                name: "user_notifications",
                schema: "log");

            migrationBuilder.DropTable(
                name: "user_organization_units",
                schema: "seg");

            migrationBuilder.DropTable(
                name: "user_roles",
                schema: "seg");

            migrationBuilder.DropTable(
                name: "user_tokens",
                schema: "seg");

            migrationBuilder.DropTable(
                name: "webhook_send_attempts",
                schema: "job");

            migrationBuilder.DropTable(
                name: "webhook_subscriptions",
                schema: "job");

            migrationBuilder.DropTable(
                name: "accounting_function_definition_storage",
                schema: "cnt");

            migrationBuilder.DropTable(
                name: "accounting_function_view_names",
                schema: "cnt");

            migrationBuilder.DropTable(
                name: "accounting_classifier_definitions",
                schema: "cnt");

            migrationBuilder.DropTable(
                name: "accounting_voucher_documents",
                schema: "cnt");

            migrationBuilder.DropTable(
                name: "states",
                schema: "gen");

            migrationBuilder.DropTable(
                name: "dynamic_entity_properties",
                schema: "gen");

            migrationBuilder.DropTable(
                name: "entity_changes",
                schema: "log");

            migrationBuilder.DropTable(
                name: "banks",
                schema: "gen");

            migrationBuilder.DropTable(
                name: "center_cost_definitions",
                schema: "cnt");

            migrationBuilder.DropTable(
                name: "clients",
                schema: "seg");

            migrationBuilder.DropTable(
                name: "workplace_classifications",
                schema: "est");

            migrationBuilder.DropTable(
                name: "workplace_payments",
                schema: "est");

            migrationBuilder.DropTable(
                name: "roles",
                schema: "seg");

            migrationBuilder.DropTable(
                name: "webhook_events",
                schema: "job");

            migrationBuilder.DropTable(
                name: "accounting_documents",
                schema: "cnt");

            migrationBuilder.DropTable(
                name: "countries",
                schema: "gen");

            migrationBuilder.DropTable(
                name: "dynamic_properties",
                schema: "gen");

            migrationBuilder.DropTable(
                name: "entity_change_sets",
                schema: "log");

            migrationBuilder.DropTable(
                name: "account_definitions",
                schema: "cnt");

            migrationBuilder.DropTable(
                name: "users",
                schema: "seg");

            migrationBuilder.DropTable(
                name: "document_definitions",
                schema: "gen");

            migrationBuilder.DropTable(
                name: "periods",
                schema: "gen");

            migrationBuilder.DropTable(
                name: "persons",
                schema: "docs");
        }
    }
}
