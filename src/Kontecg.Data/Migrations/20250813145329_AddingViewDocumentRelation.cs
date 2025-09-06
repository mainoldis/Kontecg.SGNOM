using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kontecg.Migrations
{
    /// <inheritdoc />
    public partial class AddingViewDocumentRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "accounting_function_views",
                schema: "cnt");

            migrationBuilder.DropTable(
                name: "accounting_function_view_names",
                schema: "cnt");

            migrationBuilder.CreateTable(
                name: "view_names",
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
                    table.PrimaryKey("PK_view_names", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "document_views",
                schema: "cnt",
                columns: table => new
                {
                    DocumentsId = table.Column<int>(type: "int", nullable: false),
                    ViewsId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_document_views", x => new { x.DocumentsId, x.ViewsId });
                    table.ForeignKey(
                        name: "FK_document_views_document_definitions_DocumentsId",
                        column: x => x.DocumentsId,
                        principalSchema: "gen",
                        principalTable: "document_definitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_document_views_view_names_ViewsId",
                        column: x => x.ViewsId,
                        principalSchema: "cnt",
                        principalTable: "view_names",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_document_views_ViewsId",
                schema: "cnt",
                table: "document_views",
                column: "ViewsId");

            migrationBuilder.CreateIndex(
                name: "IX_view_names_Name_ReferenceGroup",
                schema: "cnt",
                table: "view_names",
                columns: new[] { "Name", "ReferenceGroup" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "document_views",
                schema: "cnt");

            migrationBuilder.DropTable(
                name: "view_names",
                schema: "cnt");

            migrationBuilder.CreateTable(
                name: "accounting_function_view_names",
                schema: "cnt",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorUserId = table.Column<long>(type: "bigint", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(1024)", maxLength: 1024, nullable: false),
                    ReferenceGroup = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_accounting_function_view_names", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "accounting_function_views",
                schema: "cnt",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AccountingFunctionViewNameId = table.Column<int>(type: "int", nullable: false),
                    DocumentDefinitionId = table.Column<int>(type: "int", nullable: false),
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
        }
    }
}
