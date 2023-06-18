using System;
using System.Data;
using System.Linq;
using Ata.Investment.Profile.Domain.Profile;

// ReSharper disable JoinDeclarationAndInitializer

namespace Ata.Investment.Profile.Domain.Points
{
    public class DecisionTable : DataTable
    {
        private readonly Profile.Profile _profile;
        public DecisionBreakdown DecisionBreakdown { get; } = new DecisionBreakdown();

        public DecisionTable(Profile.Profile profile)
        {
            _profile = profile;
            ConstructDecisionTable();
            DecisionBreakdown.Profile = profile;
        }

        private void ConstructDecisionTable()
        {
            DataColumn column;
            DataRow row;


            column = new DataColumn
            {
                DataType = typeof(int),
                ColumnName = "RiskLevel",
            };
            Columns.Add(column);

            column = new DataColumn
            {
                DataType = typeof(string),
                ColumnName = "Allocation",
            };
            Columns.Add(column);

            column = new DataColumn
            {
                DataType = typeof(bool),
                ColumnName = "TimeHorizon",
                DefaultValue = false,
            };
            Columns.Add(column);

            column = new DataColumn
            {
                DataType = typeof(bool),
                ColumnName = "InvestmentKnowledge",
                DefaultValue = false,
            };
            Columns.Add(column);

            column = new DataColumn
            {
                DataType = typeof(bool),
                ColumnName = "InvestmentObjectives",
                DefaultValue = false,
            };
            Columns.Add(column);

            column = new DataColumn
            {
                DataType = typeof(bool),
                ColumnName = "RiskCapacity",
                DefaultValue = false,
            };
            Columns.Add(column);

            column = new DataColumn
            {
                DataType = typeof(bool),
                ColumnName = "RiskAttitude",
                DefaultValue = false,
            };
            Columns.Add(column);

            column = new DataColumn
            {
                DataType = typeof(bool),
                ColumnName = "HasSelection",
                Expression = "TimeHorizon = True OR InvestmentKnowledge = True OR InvestmentObjectives = True OR RiskCapacity = True OR RiskAttitude = True"
            };
            Columns.Add(column);

            row = NewRow();
            row["RiskLevel"] = 1;
            row["Allocation"] = "Safety";
            Rows.Add(row);

            row = NewRow();
            row["RiskLevel"] = 2;
            row["Allocation"] = "Conservative Income";
            Rows.Add(row);

            row = NewRow();
            row["RiskLevel"] = 3;
            row["Allocation"] = "Balanced";
            Rows.Add(row);

            row = NewRow();
            row["RiskLevel"] = 4;
            row["Allocation"] = "Growth";
            Rows.Add(row);

            row = NewRow();
            row["RiskLevel"] = 5;
            row["Allocation"] = "Aggressive Growth";
            Rows.Add(row);
        }
        
        public Allocation SuggestedAllocation => (
            from row in this.AsEnumerable()
            where row.Field<bool>("HasSelection") == true
            select new Allocation (
                row.Field<string>("Allocation"), 
                row.Field<int>("RiskLevel")
            )
        ).First();

        public Objectives SuggestedObjectives
        {
            get
            {
                int riskLevel = (int) this.AsEnumerable()
                    .Single(r => r.Field<bool>("InvestmentObjectives"))["RiskLevel"];

                return riskLevel switch
                {
                    1 => new Objectives(0, 0, 0, 0, 100),
                    2 => new Objectives(0, 30, 0, 70, 0),
                    3 => new Objectives(0, 60, 0, 40, 0),
                    4 => new Objectives(0, 100, 0, 0, 0),
                    5 => new Objectives(20, 80, 0, 0, 0),
                    _ => throw new ArgumentOutOfRangeException()
                };
            }
        }

        public RiskTolerance SuggestedRiskTolerance
        {
            get
            {
                int riskLevel = (int) this.AsEnumerable()
                    .Single(r => r.Field<bool>("RiskCapacity"))["RiskLevel"];

                return riskLevel switch
                {
                    1 => new RiskTolerance(0, 0, 0, 0, 100),
                    2 => new RiskTolerance(0, 0, 20, 80, 0),
                    3 => new RiskTolerance(0, 0, 50, 50, 0),
                    4 => new RiskTolerance(0, 20, 80, 0, 0),
                    5 => new RiskTolerance(40, 60, 0, 0, 0),
                    _ => throw new ArgumentOutOfRangeException()
                };
            }
        }
        
        public DataTable Pivot
        {
            get
            {
                DataTable pivot = new DataTable();
                EnumerableRowCollection<DataRow> rows = this.AsEnumerable();
                DataRow row;
                DataColumn column;
                
                column = new DataColumn
                {
                    ColumnName = "1",
                    DataType = typeof(bool),
                    DefaultValue = false
                };
                pivot.Columns.Add(column);
                
                column = new DataColumn
                {
                    ColumnName = "2",
                    DataType = typeof(bool),
                    DefaultValue = false
                };
                pivot.Columns.Add(column);
                
                column = new DataColumn
                {
                    ColumnName = "3",
                    DataType = typeof(bool),
                    DefaultValue = false
                };
                pivot.Columns.Add(column);
                
                column = new DataColumn
                {
                    ColumnName = "4",
                    DataType = typeof(bool),
                    DefaultValue = false
                };
                pivot.Columns.Add(column);
                
                column = new DataColumn
                {
                    ColumnName = "5",
                    DataType = typeof(bool),
                    DefaultValue = false
                };
                pivot.Columns.Add(column);
                
                row = pivot.NewRow(); 
                row[rows.Single(r => r.Field<bool>("TimeHorizon"))["RiskLevel"].ToString()]= true;
                pivot.Rows.Add(row);

                row = pivot.NewRow();
                row[rows.Single(r => r.Field<bool>("InvestmentKnowledge"))["RiskLevel"].ToString()]= true;
                pivot.Rows.Add(row);

                row = pivot.NewRow();
                row[rows.Single(r => r.Field<bool>("InvestmentObjectives"))["RiskLevel"].ToString()]= true;
                pivot.Rows.Add(row);

                row = pivot.NewRow();
                row[rows.Single(r => r.Field<bool>("RiskCapacity"))["RiskLevel"].ToString()]= true;
                pivot.Rows.Add(row);

                row = pivot.NewRow();
                row[rows.Single(r => r.Field<bool>("RiskAttitude"))["RiskLevel"].ToString()]= true;
                pivot.Rows.Add(row);

                return pivot;
            }
        }

    }
}