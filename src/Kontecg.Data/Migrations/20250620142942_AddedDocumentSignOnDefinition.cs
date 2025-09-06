using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kontecg.Migrations
{
    /// <inheritdoc />
    public partial class AddedDocumentSignOnDefinition : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "document_sign_on_definitions",
                schema: "gen",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DocumentDefinitionId = table.Column<int>(type: "int", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(8)", maxLength: 8, nullable: false),
                    Tag = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    VisualOrder = table.Column<int>(type: "int", nullable: false),
                    CompanyId = table.Column<int>(type: "int", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorUserId = table.Column<long>(type: "bigint", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifierUserId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_document_sign_on_definitions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_document_sign_on_definitions_document_definitions_DocumentDefinitionId",
                        column: x => x.DocumentDefinitionId,
                        principalSchema: "gen",
                        principalTable: "document_definitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_document_sign_on_definitions_DocumentDefinitionId_CompanyId_Code",
                schema: "gen",
                table: "document_sign_on_definitions",
                columns: new[] { "DocumentDefinitionId", "CompanyId", "Code" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "document_sign_on_definitions",
                schema: "gen");
        }
    }
}
