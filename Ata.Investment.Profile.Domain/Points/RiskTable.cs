using System.Data;
using Range = SharedKernel.Range;

// ReSharper disable JoinDeclarationAndInitializer

namespace Ata.Investment.Profile.Domain.Points
{
    public class RiskTable : DataTable
    {

        public RiskTable()
        {
            InitColumns();
            InitData();
        }

        private void InitColumns()
        {
            DataColumn column;

            column = new DataColumn
            {
                DataType = typeof(int),
                ColumnName = "RiskLevel",
            };
            Columns.Add(column);

            column = new DataColumn
            {
                DataType = typeof(Range),
                ColumnName = "TimeHorizon",
                DefaultValue = new Range(-1, -1),
            };
            Columns.Add(column);

            column = new DataColumn
            {
                DataType = typeof(Range),
                ColumnName = "InvestmentKnowledge",
                DefaultValue = new Range(-1, -1),
            };
            Columns.Add(column);

            column = new DataColumn
            {
                DataType = typeof(int),
                ColumnName = "InvestmentObjectives",
                DefaultValue = -1,
            };
            Columns.Add(column);

            column = new DataColumn
            {
                DataType = typeof(Range),
                ColumnName = "RiskCapacity",
                DefaultValue = new Range(-1, -1),
            };
            Columns.Add(column);

            column = new DataColumn
            {
                DataType = typeof(Range),
                ColumnName = "RiskAttitude",
            };
            Columns.Add(column);
        }

        private void InitData()
        {
            DataRow row;

            row = NewRow();
            row["RiskLevel"] = 1;
            row["TimeHorizon"] = new Range(0, 0);
            // row["InvestmentKnowledge"] = NULL
            row["InvestmentObjectives"] = 1;
            // row["RiskCapacity"] = NULL
            row["RiskAttitude"] = new Range(0, 19);
            Rows.Add(row);

            row = NewRow();
            row["RiskLevel"] = 2;
            row["TimeHorizon"] = new Range(1, 3);
            // row["InvestmentKnowledge"] = NULL
            row["InvestmentObjectives"] = 2;
            row["RiskCapacity"] = new Range(0, 14);
            row["RiskAttitude"] = new Range(20, 24);
            Rows.Add(row);

            row = NewRow();
            row["RiskLevel"] = 3;
            // row["TimeHorizon"] = NULL
            row["InvestmentKnowledge"] = new Range(3, 3);
            row["InvestmentObjectives"] = 3;
            row["RiskCapacity"] = new Range(15, 25);
            row["RiskAttitude"] = new Range(25, 30);
            Rows.Add(row);

            row = NewRow();
            row["RiskLevel"] = 4;
            row["TimeHorizon"] = new Range(4, 7);
            row["InvestmentKnowledge"] = new Range(4, 5);
            // row["InvestmentObjectives"] = NULL;
            row["RiskCapacity"] = new Range(26, 39);
            row["RiskAttitude"] = new Range(31, 45);
            Rows.Add(row);

            row = NewRow();
            row["RiskLevel"] = 5;
            row["TimeHorizon"] = new Range(8, int.MaxValue);
            row["InvestmentKnowledge"] = new Range(6, 6);
            row["InvestmentObjectives"] = 4;
            row["RiskCapacity"] = new Range(40, int.MaxValue);
            row["RiskAttitude"] = new Range(46, int.MaxValue);
            Rows.Add(row);
        }
    }
}