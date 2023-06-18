using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Ata.Investment.Allocation.Data.Migrations
{
    public partial class AllocationMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "FundsV1");

            migrationBuilder.CreateTable(
                name: "Allocations",
                schema: "FundsV1",
                columns: table => new
                {
                    RiskLevel = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Guid = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Allocations", x => x.RiskLevel);
                });

            migrationBuilder.CreateTable(
                name: "Funds",
                schema: "FundsV1",
                columns: table => new
                {
                    FundCode = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Funds", x => x.FundCode);
                });

            migrationBuilder.CreateTable(
                name: "Option",
                schema: "FundsV1",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(nullable: true),
                    OptionNumber = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Option", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "VersionDrafts",
                schema: "FundsV1",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Version = table.Column<int>(nullable: false),
                    Date = table.Column<DateTimeOffset>(nullable: false),
                    Draft = table.Column<string>(type: "xml", nullable: true),
                    Notes = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VersionDrafts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AllocationVersions",
                schema: "FundsV1",
                columns: table => new
                {
                    Version = table.Column<int>(nullable: false),
                    RiskLevel = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AllocationVersions", x => new { x.RiskLevel, x.Version });
                    table.ForeignKey(
                        name: "FK_AllocationVersions_Allocations_RiskLevel",
                        column: x => x.RiskLevel,
                        principalSchema: "FundsV1",
                        principalTable: "Allocations",
                        principalColumn: "RiskLevel",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AllocationOptions",
                schema: "FundsV1",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OptionId = table.Column<int>(nullable: true),
                    AllocationVersionRiskLevel = table.Column<int>(nullable: true),
                    AllocationVersionVersion = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AllocationOptions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AllocationOptions_Option_OptionId",
                        column: x => x.OptionId,
                        principalSchema: "FundsV1",
                        principalTable: "Option",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AllocationOptions_AllocationVersions_AllocationVersionRiskLevel_AllocationVersionVersion",
                        columns: x => new { x.AllocationVersionRiskLevel, x.AllocationVersionVersion },
                        principalSchema: "FundsV1",
                        principalTable: "AllocationVersions",
                        principalColumns: new[] { "RiskLevel", "Version" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CompositionPart",
                schema: "FundsV1",
                columns: table => new
                {
                    OptionId = table.Column<int>(nullable: false),
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Percent = table.Column<int>(nullable: false),
                    FundCode = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompositionPart", x => new { x.OptionId, x.Id });
                    table.ForeignKey(
                        name: "FK_CompositionPart_Funds_FundCode",
                        column: x => x.FundCode,
                        principalSchema: "FundsV1",
                        principalTable: "Funds",
                        principalColumn: "FundCode",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CompositionPart_AllocationOptions_OptionId",
                        column: x => x.OptionId,
                        principalSchema: "FundsV1",
                        principalTable: "AllocationOptions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                schema: "FundsV1",
                table: "Allocations",
                columns: new[] { "RiskLevel", "Guid", "Name" },
                values: new object[,]
                {
                    { 1, new Guid("6e7a5d25-1798-4331-84a1-bda4183bbdcd"), "Safety" },
                    { 2, new Guid("2571d584-8cc7-468c-9e08-a519a650a9ff"), "Conservative Income" },
                    { 3, new Guid("7ddb85b7-164e-452e-b894-9cb80d42ca6b"), "Balanced" },
                    { 4, new Guid("4db0f32b-a1ed-4093-a866-e576ac55cc2c"), "Growth" },
                    { 5, new Guid("6124468e-7540-4b10-838a-3d6e696d0683"), "Aggressive Growth" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_AllocationOptions_OptionId",
                schema: "FundsV1",
                table: "AllocationOptions",
                column: "OptionId");

            migrationBuilder.CreateIndex(
                name: "IX_AllocationOptions_AllocationVersionRiskLevel_AllocationVersionVersion",
                schema: "FundsV1",
                table: "AllocationOptions",
                columns: new[] { "AllocationVersionRiskLevel", "AllocationVersionVersion" });

            migrationBuilder.CreateIndex(
                name: "IX_CompositionPart_FundCode",
                schema: "FundsV1",
                table: "CompositionPart",
                column: "FundCode");

            migrationBuilder.CreateIndex(
                name: "IX_VersionDrafts_Version",
                schema: "FundsV1",
                table: "VersionDrafts",
                column: "Version");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CompositionPart",
                schema: "FundsV1");

            migrationBuilder.DropTable(
                name: "VersionDrafts",
                schema: "FundsV1");

            migrationBuilder.DropTable(
                name: "Funds",
                schema: "FundsV1");

            migrationBuilder.DropTable(
                name: "AllocationOptions",
                schema: "FundsV1");

            migrationBuilder.DropTable(
                name: "Option",
                schema: "FundsV1");

            migrationBuilder.DropTable(
                name: "AllocationVersions",
                schema: "FundsV1");

            migrationBuilder.DropTable(
                name: "Allocations",
                schema: "FundsV1");
        }
    }
}
