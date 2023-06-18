using Microsoft.EntityFrameworkCore.Migrations;

namespace Ata.Investment.Allocation.Data.Migrations
{
    public partial class DraftToJson : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Draft",
                schema: "FundsV1",
                table: "VersionDrafts",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "xml",
                oldNullable: true);

        }
    }
}
