using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kontecg.Migrations
{
    /// <inheritdoc />
    public partial class RemoveDatesOnVoucher : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_accounting_voucher_documents_CompanyId_StartingDate_FinishingDate",
                schema: "cnt",
                table: "accounting_voucher_documents");

            migrationBuilder.DropColumn(
                name: "FinishingDate",
                schema: "cnt",
                table: "accounting_voucher_documents");

            migrationBuilder.DropColumn(
                name: "StartingDate",
                schema: "cnt",
                table: "accounting_voucher_documents");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "FinishingDate",
                schema: "cnt",
                table: "accounting_voucher_documents",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "StartingDate",
                schema: "cnt",
                table: "accounting_voucher_documents",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateIndex(
                name: "IX_accounting_voucher_documents_CompanyId_StartingDate_FinishingDate",
                schema: "cnt",
                table: "accounting_voucher_documents",
                columns: new[] { "CompanyId", "StartingDate", "FinishingDate" });
        }
    }
}
