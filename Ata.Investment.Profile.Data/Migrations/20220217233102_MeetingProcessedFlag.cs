using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ata.Investment.Profile.Data.Migrations
{
    public partial class MeetingProcessedFlag : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Schedules",
                schema: "Investments");

            migrationBuilder.AddColumn<bool>(
                name: "IsProcessed",
                schema: "Investments",
                table: "Meetings",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsProcessed",
                schema: "Investments",
                table: "Meetings");

            migrationBuilder.CreateTable(
                name: "Schedules",
                schema: "Investments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HouseholdId = table.Column<int>(type: "int", nullable: false),
                    AdvisorGuid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DateTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Schedules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Schedules_Households_HouseholdId",
                        column: x => x.HouseholdId,
                        principalSchema: "Investments",
                        principalTable: "Households",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Schedules_HouseholdId",
                schema: "Investments",
                table: "Schedules",
                column: "HouseholdId");
        }
    }
}
