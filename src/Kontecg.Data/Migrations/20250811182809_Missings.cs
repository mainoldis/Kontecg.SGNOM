using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kontecg.Migrations
{
    /// <inheritdoc />
    public partial class Missings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "MadeOn",
                schema: "cnt",
                table: "accounting_voucher_documents",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "MadeOn",
                schema: "cnt",
                table: "accounting_documents",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MadeOn",
                schema: "cnt",
                table: "accounting_voucher_documents");

            migrationBuilder.DropColumn(
                name: "MadeOn",
                schema: "cnt",
                table: "accounting_documents");
        }
    }
}
