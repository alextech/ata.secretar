using Microsoft.EntityFrameworkCore.Migrations;

namespace Ata.Investment.Profile.Data.Migrations
{
    public partial class JsonRiskNames : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
update Investments.Meetings
set KycDocument = REPLACE(KycDocument, '""LossVsGainProfile"":""high""', '""LossVsGainProfile"":10');

update Investments.Meetings
set KycDocument = REPLACE(KycDocument, '""LossVsGainProfile"":""mediumHigh""', '""LossVsGainProfile"":6');

update Investments.Meetings
set KycDocument = REPLACE(KycDocument, '""LossVsGainProfile"":""medium""', '""LossVsGainProfile"":3');

update Investments.Meetings
set KycDocument = REPLACE(KycDocument, '""LossVsGainProfile"":""low""', '""LossVsGainProfile"":0');

update Investments.Meetings
set KycDocument = REPLACE(KycDocument, '""LossVsGainProfile"":""""', '""LossVsGainProfile"":-1');

update Investments.Meetings
set KycDocument = REPLACE(KycDocument, '""HypotheticalProfile"":""high""', '""HypotheticalProfile"":10');

update Investments.Meetings
set KycDocument = REPLACE(KycDocument, '""HypotheticalProfile"":""mediumHigh""', '""HypotheticalProfile"":6');

update Investments.Meetings
set KycDocument = REPLACE(KycDocument, '""HypotheticalProfile"":""medium""', '""HypotheticalProfile"":4');

update Investments.Meetings
set KycDocument = REPLACE(KycDocument, '""HypotheticalProfile"":""low""', '""HypotheticalProfile"":0');

update Investments.Meetings
set KycDocument = REPLACE(KycDocument, '""HypotheticalProfile"":""""', '""HypotheticalProfile"":-1');
");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
