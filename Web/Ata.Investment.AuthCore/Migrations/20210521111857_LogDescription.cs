using Microsoft.EntityFrameworkCore.Migrations;

namespace Ata.Investment.AuthCore.Migrations
{
    public partial class LogDescription : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ActionCommand",
                table: "AccessLogs");

            migrationBuilder.DropColumn(
                name: "FriendlyActionName",
                table: "AccessLogs");

            migrationBuilder.DropColumn(
                name: "IdentityEmail",
                table: "AccessLogs");

            migrationBuilder.AlterColumn<string>(
                name: "SerializedCommand",
                table: "AccessLogs",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "AccessLogs",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "LogDisplayName",
                table: "AccessLogs",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "User",
                table: "AccessLogs",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "AccessLogs");

            migrationBuilder.DropColumn(
                name: "LogDisplayName",
                table: "AccessLogs");

            migrationBuilder.DropColumn(
                name: "User",
                table: "AccessLogs");

            migrationBuilder.AlterColumn<string>(
                name: "SerializedCommand",
                table: "AccessLogs",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<string>(
                name: "ActionCommand",
                table: "AccessLogs",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FriendlyActionName",
                table: "AccessLogs",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "IdentityEmail",
                table: "AccessLogs",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
