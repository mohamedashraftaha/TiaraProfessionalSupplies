using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TiaraPro.Server.Migrations
{
    /// <inheritdoc />
    public partial class AddPriceToDentalTraining : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "Price",
                table: "DentalTrainings",
                type: "numeric",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Price",
                table: "DentalTrainings");
        }
    }
}
