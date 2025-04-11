using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApimanyrelation.Migrations
{
    /// <inheritdoc />
    public partial class Initialmigraion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "coupons",
                columns: new[] { "Id", "code" },
                values: new object[] { 2, "save20" });

            migrationBuilder.InsertData(
                table: "userCoupons",
                columns: new[] { "CouponId", "UserId" },
                values: new object[] { 1, 1 });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "coupons",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "userCoupons",
                keyColumns: new[] { "CouponId", "UserId" },
                keyValues: new object[] { 1, 1 });
        }
    }
}
