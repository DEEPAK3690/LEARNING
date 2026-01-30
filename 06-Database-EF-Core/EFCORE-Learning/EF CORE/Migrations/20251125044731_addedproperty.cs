using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EF_CORE.Migrations
{
    /// <inheritdoc />
    public partial class addedproperty : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "customid",
                table: "tb_Students",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "customid",
                table: "tb_Students");
        }
    }
}
