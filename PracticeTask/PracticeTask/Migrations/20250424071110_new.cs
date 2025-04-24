using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PracticeTask.Migrations
{
    /// <inheritdoc />
    public partial class @new : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "age",
                table: "userDetails",
                newName: "Age");

            migrationBuilder.AddColumn<bool>(
                name: "SoftDelete",
                table: "userDetails",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SoftDelete",
                table: "userDetails");

            migrationBuilder.RenameColumn(
                name: "Age",
                table: "userDetails",
                newName: "age");
        }
    }
}
