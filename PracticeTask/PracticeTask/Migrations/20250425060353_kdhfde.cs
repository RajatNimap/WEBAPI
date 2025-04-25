using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PracticeTask.Migrations
{
    /// <inheritdoc />
    public partial class kdhfde : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserId",
                table: "orders");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "orders",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
