using Microsoft.EntityFrameworkCore.Migrations;

namespace Ata.Investment.Profile.Data.Migrations
{
    public partial class ClientGuidIndex : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Clients_Guid",
                schema: "Investments",
                table: "Clients",
                column: "Guid",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Clients_Guid",
                schema: "Investments",
                table: "Clients");
        }
    }
}
