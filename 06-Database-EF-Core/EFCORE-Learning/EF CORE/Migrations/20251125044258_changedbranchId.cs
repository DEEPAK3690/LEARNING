using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EF_CORE.Migrations
{
    /// <inheritdoc />
    public partial class changedbranchId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_tb_Students_Branches_BranchId",
                table: "tb_Students");

            migrationBuilder.RenameColumn(
                name: "BranchId",
                table: "tb_Students",
                newName: "Branch_Id");

            migrationBuilder.RenameIndex(
                name: "IX_tb_Students_BranchId",
                table: "tb_Students",
                newName: "IX_tb_Students_Branch_Id");

            migrationBuilder.RenameColumn(
                name: "BranchId",
                table: "Branches",
                newName: "Branch_Id");

            migrationBuilder.AddForeignKey(
                name: "FK_tb_Students_Branches_Branch_Id",
                table: "tb_Students",
                column: "Branch_Id",
                principalTable: "Branches",
                principalColumn: "Branch_Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_tb_Students_Branches_Branch_Id",
                table: "tb_Students");

            migrationBuilder.RenameColumn(
                name: "Branch_Id",
                table: "tb_Students",
                newName: "BranchId");

            migrationBuilder.RenameIndex(
                name: "IX_tb_Students_Branch_Id",
                table: "tb_Students",
                newName: "IX_tb_Students_BranchId");

            migrationBuilder.RenameColumn(
                name: "Branch_Id",
                table: "Branches",
                newName: "BranchId");

            migrationBuilder.AddForeignKey(
                name: "FK_tb_Students_Branches_BranchId",
                table: "tb_Students",
                column: "BranchId",
                principalTable: "Branches",
                principalColumn: "BranchId");
        }
    }
}
