using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EF_CORE.Migrations
{
    /// <inheritdoc />
    public partial class addedprop : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "customid",
                table: "tb_Students");

            migrationBuilder.AlterColumn<string>(
                name: "EmailAddress",
                table: "tb_Students",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "EmailAddress",
                table: "tb_Students",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "customid",
                table: "tb_Students",
                type: "int",
                nullable: true);
        }
    }
}
