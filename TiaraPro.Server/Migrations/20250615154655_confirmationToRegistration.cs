using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TiaraPro.Server.Migrations
{
    /// <inheritdoc />
    public partial class confirmationToRegistration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Confirmed",
                table: "DentalTrainingRegistrations",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Confirmed",
                table: "DentalTrainingRegistrations");
        }
    }
}
