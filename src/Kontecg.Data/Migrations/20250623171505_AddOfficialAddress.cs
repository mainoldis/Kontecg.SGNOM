using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kontecg.Migrations
{
    /// <inheritdoc />
    public partial class AddOfficialAddress : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "ScholarshipLevel",
                schema: "docs",
                table: "persons",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(10)",
                oldMaxLength: 10,
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OfficialAddress",
                schema: "docs",
                table: "persons",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OfficialAddress",
                schema: "docs",
                table: "persons");

            migrationBuilder.AlterColumn<string>(
                name: "ScholarshipLevel",
                schema: "docs",
                table: "persons",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(150)",
                oldMaxLength: 150,
                oldNullable: true);
        }
    }
}
