using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kontecg.Migrations
{
    /// <inheritdoc />
    public partial class NewAcronymProperty : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Description",
                schema: "est",
                table: "organization_units",
                newName: "Acronym");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Acronym",
                schema: "est",
                table: "organization_units",
                newName: "Description");
        }
    }
}
