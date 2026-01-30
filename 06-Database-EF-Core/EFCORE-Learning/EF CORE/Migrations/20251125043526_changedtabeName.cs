using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EF_CORE.Migrations
{
    /// <inheritdoc />
    public partial class changedtabeName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Students_Branches_BranchId",
                table: "Students");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Students",
                table: "Students");

            migrationBuilder.RenameTable(
                name: "Students",
                newName: "tb_Students");

            migrationBuilder.RenameIndex(
                name: "IX_Students_BranchId",
                table: "tb_Students",
                newName: "IX_tb_Students_BranchId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_tb_Students",
                table: "tb_Students",
                column: "Students");

            migrationBuilder.AddForeignKey(
                name: "FK_tb_Students_Branches_BranchId",
                table: "tb_Students",
                column: "BranchId",
                principalTable: "Branches",
                principalColumn: "BranchId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_tb_Students_Branches_BranchId",
                table: "tb_Students");

            migrationBuilder.DropPrimaryKey(
                name: "PK_tb_Students",
                table: "tb_Students");

            migrationBuilder.RenameTable(
                name: "tb_Students",
                newName: "Students");

            migrationBuilder.RenameIndex(
                name: "IX_tb_Students_BranchId",
                table: "Students",
                newName: "IX_Students_BranchId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Students",
                table: "Students",
                column: "Students");

            migrationBuilder.AddForeignKey(
                name: "FK_Students_Branches_BranchId",
                table: "Students",
                column: "BranchId",
                principalTable: "Branches",
                principalColumn: "BranchId");
        }
    }
}
