using System;
using System.Data;
using System.Diagnostics;
using System.Linq;
using SharedKernel;
using TimeZoneConverter;
using Ata.Investment.Profile.Domain;
using Ata.Investment.Profile.Domain.Household;
using Ata.Investment.Profile.Domain.KYC;
using Ata.Investment.Profile.Domain.Points;
using Ata.Investment.Profile.Domain.Profile;

namespace Ata.Investment.ProfileV1.Pdf.Views
{
    public class ProfileReportViewModel
    {
        private readonly KycDocument _document;
        public Profile.Domain.Profile.Profile Profile { get; }

        private readonly TimeZoneInfo _timeZoneInfo = TZConvert.GetTimeZoneInfo("Eastern Standard Time");

        public DateTimeOffset Date => TimeZoneInfo.ConvertTimeFromUtc(_document.Date.DateTime, _timeZoneInfo);

        public Advisor Advisor => _document.Advisor;

        public string AssetsDirectory =>
            _document.ServiceStandard switch
            {
                ServiceStandard.Safeguarding => _assetsDir + "/Safeguarding",
                ServiceStandard.CapitalPreservation => _assetsDir + "/CapitalPreservation",
                ServiceStandard.ExecutiveServices => _assetsDir + "/ExecutiveServices",
                ServiceStandard.WealthAccumulation => _assetsDir + "/WealthAccumulation",
                _ => "Safeguarding"
            };


        public string IncomeStability =>
            Profile.AnnualIncome switch
            {
                { Stability: 8 } => "stable",
                { Stability: 4 } => "somewhat stable",
                _ => "unstable" // Stability: = 1
            };

        public string Knowledge =>
            Profile.Knowledge.Answer switch
            {
                { Level: 6} => "in-depth amount of",
                { Level: 5} => "working level",
                { Level: 4 } => "some level of",
                _ => "very little" // Level: = 3
            };

        public string Situation =>
            Profile.FinancialSituation switch
            {
                10 => "significant savings and little or no debt",
                7 =>  "some savings and little or no debt",
                5 =>  "some savings and some debt",
                2 =>  "little savings and a fair amount of debt",
                _ =>  "no savings and significant debt"
            };

        public string OneYearDeclineAmount =>
            Profile.Decline switch
            {
                10 => "more than 20%",
                8 => "up to 20%",
                6 => "up to 10%",
                3 => "up to 3%",
                _ => "0%"
            };

        public string Goal =>
            Profile.Goal switch
            {
                1 => "<b>Safety</b>. You want to keep the money you have invested safe from short-term losses or readily available for short-term needs. (Investments that will satisfy this objective include GICs and money market funds).",
                2 => "<b>Income</b>. You want to generate a steady stream of income from your investments and you are less concerned about growing the value of my investments.  (Investments that will satisfy this objective include fixed income investments such as funds that invest in bonds).",
                3 => "<b>Balance</b>. You want to generate some income with some opportunity for the investments to grow in value. (A balanced fund or a portfolio that includes at least 40% in fixed income investments and no more than 60% in equity funds  will satisfy this objective).",
                4 => "<b>Growth</b>. You want to generate long-term growth from your investments.  (A portfolio with a relatively high proportion of funds that invest in equities will satisfy this objective if you also have a long time horizon and are willing and able to accept more risk).",
                _ => ""
            };

        public string PercentageOfSavings =>
            Profile.PercentageOfSavings switch
            {
                10 => "less than 25%",
                5  => "25%-50%",
                4  => "51%-75%",
                2  => "more than 75%",
                _  => ""
            };

        public string DecisionMaking =>
            Profile.DecisionMaking switch
            {
                0  => "<b>very conservative<b> and try to <b>minimize risk</b> and avoid the possibility of any loss.",
                4  => "<b>conservative</b> but willing to accept a <b>small amount of risk</b>.",
                6  => "willing to accept a <b>moderate level of risk</b> and tolerate losses to achieve potentially higher returns.",
                10 => "<b>aggressive</b> and typically take on significant risk and are willing to <b>tolerate large losses</b> for the potential of achieving higher returns.",
                _  => ""
            };

        public string Decline =>
            Profile.Decline switch
            {
                0  => "no losses.",
                3  => "-$300 (-3%).",
                6  => "-$1,000 (-10%).",
                8  => "-$2,000 (-20%).",
                10 => "more than -$2,000 (more than -20%).",
                _  => ""
            };

        public string LossesOrGains =>
            Profile.LossesOrGains switch
            {
                0  => "always concerted about possible losses.",
                3  => "usually concerned about possible losses.",
                6  => "usually concerned about possible gains.",
                10 => "always concerned about possible gains.",
                _  => ""
            };

        public string ActionOnLosses =>
            Profile.ActionOnLosses switch
            {
                0  => "sell all of the remaining investment to avoid further losses.",
                3  => "sell a portion of the remaining investment to protect some of your capital.",
                5  => "hold onto the investment and not sell any of the investment in the hopes of higher future returns.",
                10 => "buy more of the investment given that prices are lower.",
                _  => ""
            };

        public Objectives SuggestedObjectives { get; }

        public RiskTolerance SuggestedRiskTolerance { get; }

        public string PreparedFor => Profile.PrimaryClient.Name + (Profile.IsJoint ? " and " + Profile.JointClient?.Name : "");

        public OptionVM RecommendedOption { get; }

        public int RiskColumnIdx { get; }

        public DataTable PivotView { get; }

        private string _assetsDir;

        public ProfileReportViewModel(KycDocument document, Profile.Domain.Profile.Profile profile)
        {
            _document = document;
            Profile = profile;
            DecisionTable decisionTable = profile.CreateDecisionTable();
            SuggestedObjectives = decisionTable.SuggestedObjectives;
            SuggestedRiskTolerance = decisionTable.SuggestedRiskTolerance;

            Recommendation recommendation = profile.Recommendation;
            Debug.Assert(recommendation != null, nameof(recommendation) + " != null");

            RecommendationsVM recommendationsVM = new RecommendationsVM
            {
                Options =
                {
                    new OptionVM
                    {
                        OptionId = recommendation.AllocationOptionId,
                        Composition = recommendation.Composition
                            .Select(c => new CompositionVM {Portfolio = c.Key, Percent = c.Value}).ToList(),
                        Name = recommendation.Name,
                        AllocationName = recommendation.Allocation
                    }
                }
            };

            recommendationsVM.AssignColors();
            recommendationsVM.Oder();

            RecommendedOption = recommendationsVM.Options.First();
            RiskColumnIdx = decisionTable.SuggestedAllocation.RiskLevel - 1;
            PivotView = decisionTable.Pivot;

            // BaseDirectory already has leading slash /
            _assetsDir = System.AppDomain.CurrentDomain.BaseDirectory + "assets";
        }
    }
}