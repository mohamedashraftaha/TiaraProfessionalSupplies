using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TiaraPro.Server.Migrations
{
    /// <inheritdoc />
    public partial class AddNotificationsNewModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Notifications",
                newName: "Title");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "Notifications",
                newName: "Message");

            migrationBuilder.AddColumn<bool>(
                name: "Active",
                table: "Notifications",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "Category",
                table: "Notifications",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Active",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "Category",
                table: "Notifications");

            migrationBuilder.RenameColumn(
                name: "Title",
                table: "Notifications",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "Message",
                table: "Notifications",
                newName: "Description");
        }
    }
}
