using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Ata.Investment.AuthCore.Migrations
{
    public partial class AccessLogging : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AccessLogs",
                columns: table => new
                {
                    TimeStamp = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IdentityEmail = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ActionCommand = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FriendlyActionName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SerializedCommand = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccessLogs", x => x.TimeStamp);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AccessLogs");
        }
    }
}
