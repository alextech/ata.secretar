using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Ata.Investment.Profile.Data.Migrations
{
    public partial class ProfileMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "Investments");

            migrationBuilder.CreateTable(
                name: "Clients",
                schema: "Investments",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Guid = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    Email = table.Column<string>(nullable: false),
                    DateOfBirth = table.Column<DateTimeOffset>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clients", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Households",
                schema: "Investments",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Guid = table.Column<Guid>(nullable: false),
                    PrimaryClientId = table.Column<int>(nullable: false),
                    JointClientId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Households", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Households_Clients_JointClientId",
                        column: x => x.JointClientId,
                        principalSchema: "Investments",
                        principalTable: "Clients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Households_Clients_PrimaryClientId",
                        column: x => x.PrimaryClientId,
                        principalSchema: "Investments",
                        principalTable: "Clients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Meetings",
                schema: "Investments",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Guid = table.Column<Guid>(nullable: false),
                    KycDocument = table.Column<string>(nullable: false),
                    HouseholdId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Meetings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Meetings_Households_HouseholdId",
                        column: x => x.HouseholdId,
                        principalSchema: "Investments",
                        principalTable: "Households",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Households_JointClientId",
                schema: "Investments",
                table: "Households",
                column: "JointClientId");

            migrationBuilder.CreateIndex(
                name: "IX_Households_PrimaryClientId",
                schema: "Investments",
                table: "Households",
                column: "PrimaryClientId");

            migrationBuilder.CreateIndex(
                name: "IX_Meetings_HouseholdId",
                schema: "Investments",
                table: "Meetings",
                column: "HouseholdId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Meetings",
                schema: "Investments");

            migrationBuilder.DropTable(
                name: "Households",
                schema: "Investments");

            migrationBuilder.DropTable(
                name: "Clients",
                schema: "Investments");
        }
    }
}
