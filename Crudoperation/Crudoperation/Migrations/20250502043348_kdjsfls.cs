using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Crudoperation.Migrations
{
    /// <inheritdoc />
    public partial class kdjsfls : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_emps",
                table: "emps");

            migrationBuilder.RenameTable(
                name: "emps",
                newName: "emp");

            migrationBuilder.AddPrimaryKey(
                name: "PK_emp",
                table: "emp",
                column: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_emp",
                table: "emp");

            migrationBuilder.RenameTable(
                name: "emp",
                newName: "emps");

            migrationBuilder.AddPrimaryKey(
                name: "PK_emps",
                table: "emps",
                column: "Id");
        }
    }
}
