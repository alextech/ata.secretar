using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Ata.Investment.Allocation.Data.Migrations
{
    public partial class ListedAllocations : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsListed",
                schema: "FundsV1",
                table: "AllocationVersions",
                nullable: false,
                defaultValue: true);

        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsListed",
                schema: "FundsV1",
                table: "AllocationVersions");

        }
    }
}
