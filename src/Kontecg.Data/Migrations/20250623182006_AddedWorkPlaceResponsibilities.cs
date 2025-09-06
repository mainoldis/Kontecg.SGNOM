using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kontecg.Migrations
{
    /// <inheritdoc />
    public partial class AddedWorkPlaceResponsibilities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "workplace_responsibilities",
                schema: "est",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClassificationId = table.Column<int>(type: "int", nullable: false),
                    OccupationCode = table.Column<string>(type: "nvarchar(8)", maxLength: 8, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_workplace_responsibilities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_workplace_responsibilities_workplace_classifications_ClassificationId",
                        column: x => x.ClassificationId,
                        principalSchema: "est",
                        principalTable: "workplace_classifications",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_workplace_responsibilities_ClassificationId",
                schema: "est",
                table: "workplace_responsibilities",
                column: "ClassificationId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "workplace_responsibilities",
                schema: "est");
        }
    }
}
