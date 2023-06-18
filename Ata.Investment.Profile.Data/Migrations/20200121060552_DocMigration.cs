using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Ata.Investment.Profile.Data.Migrations
{
    public partial class DocMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CreatedFor",
                schema: "Investments",
                table: "Meetings",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "Date",
                schema: "Investments",
                table: "Meetings",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<string>(
                name: "Purpose",
                schema: "Investments",
                table: "Meetings",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedFor",
                schema: "Investments",
                table: "Meetings");

            migrationBuilder.DropColumn(
                name: "Date",
                schema: "Investments",
                table: "Meetings");

            migrationBuilder.DropColumn(
                name: "Purpose",
                schema: "Investments",
                table: "Meetings");
        }
    }
}
