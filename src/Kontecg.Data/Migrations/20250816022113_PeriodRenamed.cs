using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kontecg.Migrations
{
    /// <inheritdoc />
    public partial class PeriodRenamed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_accounting_documents_periods_AccountingPeriodId",
                schema: "cnt",
                table: "accounting_documents");

            migrationBuilder.RenameColumn(
                name: "AccountingPeriodId",
                schema: "cnt",
                table: "accounting_documents",
                newName: "PeriodId");

            migrationBuilder.RenameIndex(
                name: "IX_accounting_documents_CompanyId_AccountingPeriodId_DocumentDefinitionId",
                schema: "cnt",
                table: "accounting_documents",
                newName: "IX_accounting_documents_CompanyId_PeriodId_DocumentDefinitionId");

            migrationBuilder.RenameIndex(
                name: "IX_accounting_documents_AccountingPeriodId",
                schema: "cnt",
                table: "accounting_documents",
                newName: "IX_accounting_documents_PeriodId");

            migrationBuilder.AddForeignKey(
                name: "FK_accounting_documents_periods_PeriodId",
                schema: "cnt",
                table: "accounting_documents",
                column: "PeriodId",
                principalSchema: "gen",
                principalTable: "periods",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_accounting_documents_periods_PeriodId",
                schema: "cnt",
                table: "accounting_documents");

            migrationBuilder.RenameColumn(
                name: "PeriodId",
                schema: "cnt",
                table: "accounting_documents",
                newName: "AccountingPeriodId");

            migrationBuilder.RenameIndex(
                name: "IX_accounting_documents_PeriodId",
                schema: "cnt",
                table: "accounting_documents",
                newName: "IX_accounting_documents_AccountingPeriodId");

            migrationBuilder.RenameIndex(
                name: "IX_accounting_documents_CompanyId_PeriodId_DocumentDefinitionId",
                schema: "cnt",
                table: "accounting_documents",
                newName: "IX_accounting_documents_CompanyId_AccountingPeriodId_DocumentDefinitionId");

            migrationBuilder.AddForeignKey(
                name: "FK_accounting_documents_periods_AccountingPeriodId",
                schema: "cnt",
                table: "accounting_documents",
                column: "AccountingPeriodId",
                principalSchema: "gen",
                principalTable: "periods",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
