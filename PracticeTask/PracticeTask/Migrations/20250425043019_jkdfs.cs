using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PracticeTask.Migrations
{
    /// <inheritdoc />
    public partial class jkdfs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "UserDetailId",
                table: "orders",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "orders",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_orders_UserDetailId",
                table: "orders",
                column: "UserDetailId");

            migrationBuilder.AddForeignKey(
                name: "FK_orders_userDetails_UserDetailId",
                table: "orders",
                column: "UserDetailId",
                principalTable: "userDetails",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_orders_userDetails_UserDetailId",
                table: "orders");

            migrationBuilder.DropIndex(
                name: "IX_orders_UserDetailId",
                table: "orders");

            migrationBuilder.DropColumn(
                name: "UserDetailId",
                table: "orders");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "orders");
        }
    }
}
