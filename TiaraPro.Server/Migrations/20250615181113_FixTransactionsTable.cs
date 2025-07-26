using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TiaraPro.Server.Migrations
{
    /// <inheritdoc />
    public partial class FixTransactionsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "dentalMeshResponseStlViewUrl",
                table: "Transactions",
                newName: "DentalMeshResponseStlViewUrl");

            migrationBuilder.RenameColumn(
                name: "dentalMeshResponseStlFolder",
                table: "Transactions",
                newName: "DentalMeshResponseStlFolder");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DentalMeshResponseStlViewUrl",
                table: "Transactions",
                newName: "dentalMeshResponseStlViewUrl");

            migrationBuilder.RenameColumn(
                name: "DentalMeshResponseStlFolder",
                table: "Transactions",
                newName: "dentalMeshResponseStlFolder");
        }
    }
}
