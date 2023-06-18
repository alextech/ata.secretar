using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using SharedKernel;
using Ata.Investment.Profile.Domain.KYC;
using Ata.Investment.Profile.Domain.Points;

namespace Ata.Investment.Profile.Domain.Profile
{
    // ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
    public sealed class Profile
    {
        private delegate int AgeFormula();
        private delegate (Knowledge, Dictionary<Holder, int>) KnowledgeFormula();
        private delegate Income AnnualIncomeFormula();
        private delegate int FinancialSituationFormula();

        private AgeFormula _age;
        private KnowledgeFormula _knowledge;
        private AnnualIncomeFormula _annualIncome;
        private FinancialSituationFormula _financialSituation;
        private TimeHorizon _timeHorizon = new TimeHorizon(TimeProvider.Current.UtcNow.Year);

        #region Constructors

        [JsonConstructor]
#pragma warning disable 8618
        private Profile()
#pragma warning restore 8618
        {
            // delegates assigned at [OnDeserialized]
        }

        public Profile(PClient primaryClient) : this()
        {
            PrimaryClient = primaryClient;

            AppointFormulaDelegates();

        }

        public Profile(PClient primaryClient, PClient? jointClient)
            : this(primaryClient)
        {
            if (jointClient == null)
            {
                return;
            }

            JointClient = jointClient;

            AppointFormulaDelegates();
        }

        [OnDeserialized]
        internal void OnDeserialized(StreamingContext context)
        {
            AppointFormulaDelegates();
        }

        private void AppointFormulaDelegates()
        {
            if (IsJoint)
            {
                _age = JointAge;
                _knowledge = JointKnowledge;
                _annualIncome = CombinedAnnualIncome;
                _financialSituation = JointSituation;
            }
            else
            {
                _age = SingleAge;
                _knowledge = SingleKnowledge;
                _annualIncome = SingleAnnualIncome;
                _financialSituation = SingleSituation;
            }
        }

        public static Profile CreateFromVO(NewProfileVO newProfileVO)
        {
            // NewProfileVO joint client is already nullable,
            // and since constructor is capable of detecting nullable parameter to switch to single constructor
            // doing a isJoint check here is duplicate logic maintenance
            Profile profile = new Profile(newProfileVO.Primary, newProfileVO.Joint)
            {
                Name = newProfileVO.Name,
                Accounts = newProfileVO.Accounts,
                TimeHorizon = (TimeHorizon) newProfileVO.TimeHorizon.Clone()
            };

            profile.Meta.Icon = newProfileVO.Icon;

            return profile;
        }

        #endregion

        public Guid Guid { get; init; } = Guid.NewGuid();

        public string Name { get; set; } = "";

        public PClient PrimaryClient { get; set; }

        public PClient? JointClient { get; set; }

        // delegator
        private int SingleAge()
        {
            return PrimaryClient.Age;
        }

        private int JointAge()
        {
            Debug.Assert(JointClient != null, nameof(JointClient) + " != null");

            return Math.Max(PrimaryClient.Age, JointClient.Age);
        }

        private (Knowledge, Dictionary<Holder, int>) SingleKnowledge()
        {
            return (
                PrimaryClient.Knowledge,
                new Dictionary<Holder, int>
                {
                    { Holder.Primary, PrimaryClient.Knowledge.Level }
                }
            );
        }

        private (Knowledge, Dictionary<Holder, int>) JointKnowledge()
        {
            Debug.Assert(JointClient != null, nameof(JointClient) + " != null");

            return (
                new Knowledge(
                    level: Math.Min(
                        PrimaryClient.Knowledge.Level,
                        JointClient.Knowledge.Level
                    ),
                    notes: PrimaryClient.Knowledge.Notes
                           + "<br /><br />" +
                           JointClient.Knowledge.Notes
                ),
                new Dictionary<Holder, int>
                {
                    { Holder.Primary, PrimaryClient.Knowledge.Level },
                    { Holder.Joint, JointClient.Knowledge.Level }
                }
            );
        }

        private Income SingleAnnualIncome()
        {
            return PrimaryClient.Income;
        }

        private Income CombinedAnnualIncome()
        {
            Debug.Assert(JointClient != null, nameof(JointClient) + " != null");

            Income income = new Income(

                amount: PrimaryClient.Income.Amount +
                        JointClient.Income.Amount,

                stability: Math.Min(
                    PrimaryClient.Income.Stability,
                    JointClient.Income.Stability
                ),

                notes: PrimaryClient.Income.Notes
                        + "<br /><br />" +
                        JointClient.Income.Notes
            );

            return income;
        }

        private int SingleSituation()
        {
            return PrimaryClient.FinancialSituation;
        }

        private int JointSituation()
        {
            Debug.Assert(JointClient != null, nameof(JointClient) + " != null");

            return Math.Min(
                PrimaryClient.FinancialSituation,
                JointClient.FinancialSituation
            );
        }

        [JsonIgnore]
        public bool IsJoint => JointClient != null;

        [JsonIgnore]
        public int Age => _age();

        [JsonIgnore]
        public (Knowledge Answer, Dictionary<Holder, int> Breakdown) Knowledge => _knowledge();

        [JsonIgnore]
        public Income AnnualIncome => _annualIncome();

        // MFDA Q6
        [JsonIgnore]
        public int FinancialSituation => _financialSituation();

        // networth is pointer between clients, so if joint will be same VO
        [JsonIgnore]
        public NetWorth NetWorth => PrimaryClient.NetWorth;

        // MFDA: Q3
        public int Goal { get; set; } = -1;

        // MFDA: Q8
        public int PercentageOfSavings { get; set; } = -1;

        // MFDA: Q10
        public int DecisionMaking { get; set; } = -1;

        // MFDA: Q12
        public int LossesOrGains { get; set; } = -1;

        // MFDA: Q14
        public int ActionOnLosses { get; set; } = -1;

        // MFDA: Q15
        public int HypotheticalProfile { get; set; } = -1;

        // MFDA: Q13
        public int LossVsGainProfile { get; set; } = -1;

        // MFDA: Q1
        public TimeHorizon TimeHorizon { get; set; } = new TimeHorizon(TimeProvider.Current.UtcNow.Year);

        // MFDA: Q11
        public int Decline { get; set; } = -1;

        // investment amounts
        public Accounts Accounts { get; set; } = new Accounts();
        public int InitialInvestment { get; set; }
        public int MonthlyCommitment { get; set; }
        public Recommendation? Recommendation { get; set; }
        public Meta Meta { get; private set; } = new Meta();
    }
}
