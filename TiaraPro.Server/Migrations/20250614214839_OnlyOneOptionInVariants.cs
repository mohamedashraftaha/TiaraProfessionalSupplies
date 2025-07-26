using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TiaraPro.Server.Migrations
{
    /// <inheritdoc />
    public partial class OnlyOneOptionInVariants : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "OnlyOneOption",
                table: "ProductVariants",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OnlyOneOption",
                table: "ProductVariants");
        }
    }
}
