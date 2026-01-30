using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EF_CORE.Migrations
{
    /// <inheritdoc />
    public partial class changedcolumnname : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Email",
                table: "tb_Students",
                newName: "EmailAddress");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "EmailAddress",
                table: "tb_Students",
                newName: "Email");
        }
    }
}
