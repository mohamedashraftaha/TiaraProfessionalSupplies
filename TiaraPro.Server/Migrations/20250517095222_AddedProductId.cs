using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TiaraPro.Server.Migrations
{
    /// <inheritdoc />
    public partial class AddedProductId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Products_Products_ParentProductId",
                table: "Products");

            migrationBuilder.RenameColumn(
                name: "ParentProductId",
                table: "Products",
                newName: "ProductId");

            migrationBuilder.RenameIndex(
                name: "IX_Products_ParentProductId",
                table: "Products",
                newName: "IX_Products_ProductId");

            migrationBuilder.AddForeignKey(
                name: "FK_Products_Products_ProductId",
                table: "Products",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Products_Products_ProductId",
                table: "Products");

            migrationBuilder.RenameColumn(
                name: "ProductId",
                table: "Products",
                newName: "ParentProductId");

            migrationBuilder.RenameIndex(
                name: "IX_Products_ProductId",
                table: "Products",
                newName: "IX_Products_ParentProductId");

            migrationBuilder.AddForeignKey(
                name: "FK_Products_Products_ParentProductId",
                table: "Products",
                column: "ParentProductId",
                principalTable: "Products",
                principalColumn: "Id");
        }
    }
}
