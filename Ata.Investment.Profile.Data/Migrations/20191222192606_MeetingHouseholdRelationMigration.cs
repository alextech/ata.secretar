using Microsoft.EntityFrameworkCore.Migrations;

namespace Ata.Investment.Profile.Data.Migrations
{
    public partial class MeetingHouseholdRelationMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Meetings_Households_HouseholdId",
                schema: "Investments",
                table: "Meetings");

            migrationBuilder.AlterColumn<int>(
                name: "HouseholdId",
                schema: "Investments",
                table: "Meetings",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Meetings_Households_HouseholdId",
                schema: "Investments",
                table: "Meetings",
                column: "HouseholdId",
                principalSchema: "Investments",
                principalTable: "Households",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Meetings_Households_HouseholdId",
                schema: "Investments",
                table: "Meetings");

            migrationBuilder.AlterColumn<int>(
                name: "HouseholdId",
                schema: "Investments",
                table: "Meetings",
                type: "int",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddForeignKey(
                name: "FK_Meetings_Households_HouseholdId",
                schema: "Investments",
                table: "Meetings",
                column: "HouseholdId",
                principalSchema: "Investments",
                principalTable: "Households",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
