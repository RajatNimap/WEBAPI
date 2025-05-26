using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Hospital_Management.Migrations
{
    /// <inheritdoc />
    public partial class newadded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Status",
                table: "appointments",
                newName: "StatusId");

            migrationBuilder.RenameColumn(
                name: "DayOfWeek",
                table: "appointments",
                newName: "Session");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "StatusId",
                table: "appointments",
                newName: "Status");

            migrationBuilder.RenameColumn(
                name: "Session",
                table: "appointments",
                newName: "DayOfWeek");
        }
    }
}
