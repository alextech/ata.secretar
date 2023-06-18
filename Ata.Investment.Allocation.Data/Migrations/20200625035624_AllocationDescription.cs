using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Ata.Investment.Allocation.Data.Migrations
{
    public partial class AllocationDescription : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Description",
                schema: "FundsV1",
                table: "VersionDrafts",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                schema: "FundsV1",
                table: "VersionDrafts");

        }
    }
}
