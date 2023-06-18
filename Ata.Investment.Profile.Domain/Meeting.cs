using System;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;
using SharedKernel;
using Ata.Investment.Profile.Domain.KYC;

[assembly: InternalsVisibleTo("Ata.Investment.Profile.Domain.Test")]
namespace Ata.Investment.Profile.Domain
{
    public class Meeting : Entity
    {

        private Advisor _advisor;
        protected Meeting() {}

        internal Meeting(Household.Household household, Advisor advisor, int allocationVersion)
        {
            _advisor = advisor;
            Household = household;
            AllocationVersion = allocationVersion;
            AdvisorGuid = advisor.Guid;
        }

        private KycDocument? _kycDocument;
        // deliberately not same name as XmlKycDocumentSource to avoid collision with efcore backing field convention
        // which would bypass getters that do encoding at correct time.
        private string _xml;

        [JsonIgnore]
        public KycDocument KycDocument => _kycDocument ??= KycDocumentEncoder.Decode(XmlKycDocumentSource);

        [JsonProperty]
        public string XmlKycDocumentSource
        {
            get => _kycDocument != null ? KycDocumentEncoder.Encode(_kycDocument) : _xml;
            set
            {
                _xml = value;
                _kycDocument = null;
            }
        }

        public string Purpose { get; set; } = "";

        public string CreatedFor { get; set; } = "";

        public DateTimeOffset Date { get; set; }

        [JsonProperty]
        public Guid AdvisorGuid { get; private set; }

        [JsonProperty]
        public virtual Household.Household Household { get; private set; }
        public int AllocationVersion { get; set; }

        [JsonProperty]
        public bool IsCompleted { get; private set; }

        public bool IsProcessed { get; private set; }

        public void Complete()
        {
            IsCompleted = true;
        }

        public void Process()
        {
            IsProcessed = true;
        }

        public void Begin()
        {
            _kycDocument = new KycDocument(Guid, _advisor, Household, AllocationVersion);
        }

        public void SwitchAdvisor(Advisor newAdvisor)
        {
            _advisor = newAdvisor;
            AdvisorGuid = newAdvisor.Guid;

            KycDocument.SwitchAdvisor(newAdvisor);
        }
    }
}