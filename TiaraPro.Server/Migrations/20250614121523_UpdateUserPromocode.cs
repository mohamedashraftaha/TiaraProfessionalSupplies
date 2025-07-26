using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TiaraPro.Server.Migrations
{
    /// <inheritdoc />
    public partial class UpdateUserPromocode : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserPromoCodes_Orders_OrderId",
                table: "UserPromoCodes");

            migrationBuilder.DropForeignKey(
                name: "FK_UserPromoCodes_PromoCodes_PromoCodeId",
                table: "UserPromoCodes");

            migrationBuilder.DropForeignKey(
                name: "FK_UserPromoCodes_Users_UserId",
                table: "UserPromoCodes");

            migrationBuilder.DropIndex(
                name: "IX_UserPromoCodes_OrderId",
                table: "UserPromoCodes");

            migrationBuilder.DropIndex(
                name: "IX_UserPromoCodes_PromoCodeId",
                table: "UserPromoCodes");

            migrationBuilder.DropIndex(
                name: "IX_UserPromoCodes_UserId_PromoCodeId",
                table: "UserPromoCodes");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_UserPromoCodes_OrderId",
                table: "UserPromoCodes",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_UserPromoCodes_PromoCodeId",
                table: "UserPromoCodes",
                column: "PromoCodeId");

            migrationBuilder.CreateIndex(
                name: "IX_UserPromoCodes_UserId_PromoCodeId",
                table: "UserPromoCodes",
                columns: new[] { "UserId", "PromoCodeId" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_UserPromoCodes_Orders_OrderId",
                table: "UserPromoCodes",
                column: "OrderId",
                principalTable: "Orders",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_UserPromoCodes_PromoCodes_PromoCodeId",
                table: "UserPromoCodes",
                column: "PromoCodeId",
                principalTable: "PromoCodes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserPromoCodes_Users_UserId",
                table: "UserPromoCodes",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
