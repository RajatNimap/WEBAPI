using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Test.Migrations
{
    /// <inheritdoc />
    public partial class djfsd : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "productId",
                table: "Categories",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Categories_productId",
                table: "Categories",
                column: "productId");

            migrationBuilder.AddForeignKey(
                name: "FK_Categories_Products_productId",
                table: "Categories",
                column: "productId",
                principalTable: "Products",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Categories_Products_productId",
                table: "Categories");

            migrationBuilder.DropIndex(
                name: "IX_Categories_productId",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "productId",
                table: "Categories");
        }
    }
}
