using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EF_CORE.Migrations
{
    /// <inheritdoc />
    public partial class changeInporpertyname : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "StudentId",
                table: "Students",
                newName: "Students");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Students",
                table: "Students",
                newName: "StudentId");
        }
    }
}
