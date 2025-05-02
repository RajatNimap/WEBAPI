using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Crudoperation.Migrations
{
    /// <inheritdoc />
    public partial class dsjksd : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Salary",
                table: "emp",
                newName: "salary");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "salary",
                table: "emp",
                newName: "Salary");
        }
    }
}
