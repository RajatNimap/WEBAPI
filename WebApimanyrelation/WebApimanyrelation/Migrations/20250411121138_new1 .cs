using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace WebApimanyrelation.Migrations
{
    /// <inheritdoc />
    public partial class new1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_userCoupons",
                table: "userCoupons");

            migrationBuilder.DropIndex(
                name: "IX_userCoupons_UserId",
                table: "userCoupons");

            migrationBuilder.DeleteData(
                table: "userCoupons",
                keyColumns: new[] { "CouponId", "UserId" },
                keyValues: new object[] { 1, 1 });

            migrationBuilder.DeleteData(
                table: "userCoupons",
                keyColumns: new[] { "CouponId", "UserId" },
                keyValues: new object[] { 2, 1 });

            migrationBuilder.DeleteData(
                table: "coupons",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.AddPrimaryKey(
                name: "PK_userCoupons",
                table: "userCoupons",
                columns: new[] { "UserId", "CouponId" });

            migrationBuilder.UpdateData(
                table: "coupons",
                keyColumn: "Id",
                keyValue: 1,
                column: "code",
                value: "save20");

            migrationBuilder.InsertData(
                table: "userCoupons",
                columns: new[] { "CouponId", "UserId" },
                values: new object[] { 1, 1 });

            migrationBuilder.CreateIndex(
                name: "IX_userCoupons_CouponId",
                table: "userCoupons",
                column: "CouponId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_userCoupons",
                table: "userCoupons");

            migrationBuilder.DropIndex(
                name: "IX_userCoupons_CouponId",
                table: "userCoupons");

            migrationBuilder.DeleteData(
                table: "userCoupons",
                keyColumns: new[] { "CouponId", "UserId" },
                keyValues: new object[] { 1, 1 });

            migrationBuilder.AddPrimaryKey(
                name: "PK_userCoupons",
                table: "userCoupons",
                columns: new[] { "CouponId", "UserId" });

            migrationBuilder.UpdateData(
                table: "coupons",
                keyColumn: "Id",
                keyValue: 1,
                column: "code",
                value: "addf");

            migrationBuilder.InsertData(
                table: "coupons",
                columns: new[] { "Id", "code" },
                values: new object[] { 2, "save20" });

            migrationBuilder.InsertData(
                table: "userCoupons",
                columns: new[] { "CouponId", "UserId" },
                values: new object[,]
                {
                    { 1, 1 },
                    { 2, 1 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_userCoupons_UserId",
                table: "userCoupons",
                column: "UserId");
        }
    }
}
