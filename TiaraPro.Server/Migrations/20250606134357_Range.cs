using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TiaraPro.Server.Migrations
{
    /// <inheritdoc />
    public partial class Range : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Range",
                table: "ProductVariants",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Range",
                table: "ProductVariants");
        }
    }
}
