using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Newtonsoft.Json;
using SharedKernel;
using Ata.Investment.Profile.Domain.Household;
using Ata.Investment.Profile.Domain.Profile;

namespace Ata.Investment.Profile.Domain.KYC
{
    public class PClient
    {
        private PClient? _jointWith;
        private NetWorth _netWorth = new NetWorth(0, 0, 0);

        [JsonProperty("Profiles")]
        private readonly List<Profile.Profile> _profiles = new List<Profile.Profile>();

        [JsonConstructor]
        private PClient()
        {

        }

        // ReSharper disable once SuggestBaseTypeForParameter
        public PClient(Client client)
        {
            Guid = client.Guid;
            Name = client.Name;
            Email = client.Email;
            DateOfBirth = client.DateOfBirth;
        }

        [JsonProperty]
        public Guid Guid { get; private set; }

        [JsonProperty]
        public string Name { get; private set; }

        [JsonProperty]
        public string Email { get; private set; }

        [JsonProperty]
        public DateTimeOffset DateOfBirth { get; private set; }

        [JsonIgnore]
        public int Age => AgeFromBirthdate(DateOfBirth);

        [JsonIgnore]
        public PClient? JointWith
        {
            get => _jointWith;
            set
            {
                if (value == _jointWith || value == this) return;


                if (value != null)
                {
                    _jointWith = value;
                    _jointWith.NetWorth = NetWorth;
                    _jointWith.JointWith = this;
                }
                else
                {
                    PClient tmpClient = _jointWith;
                    _jointWith = null;

                    tmpClient.JointWith = null;
                    NetWorth = (NetWorth)NetWorth.Clone();
                }
            }
        }

        public Knowledge Knowledge { get; set; } = new Knowledge(-1, "");

        public Income Income { get; set; } = new Income(0, -1, "");

        // MFDA: Q7
        public NetWorth NetWorth
        {
            get => _netWorth;
            set
            {
                if (ReferenceEquals(_netWorth, value)) return;

                _netWorth = value;

                if (JointWith != null)
                {
                    JointWith.NetWorth = value;
                }
            }
        }

        // MFDA: Q6
        public int FinancialSituation { get; set; } = -1;

        public event EventHandler ProfileListChanged;

        public ReadOnlyCollection<Profile.Profile> Profiles => _profiles.AsReadOnly();

        public void AddProfile(Profile.Profile profile)
        {
            _profiles.Add(profile);
            ProfileListChanged?.Invoke(this, EventArgs.Empty);
        }

        public void ReplaceAllProfiles(IEnumerable<Profile.Profile> profiles)
        {
            _profiles.Clear();
            _profiles.AddRange(profiles);
            ProfileListChanged?.Invoke(this, EventArgs.Empty);
        }

        public bool RemoveProfile(Profile.Profile profile)
        {
            ProfileListChanged?.Invoke(this, EventArgs.Empty);
            return _profiles.Remove(profile);
        }

        public static int AgeFromBirthdate(DateTimeOffset birthdate)
        {
            DateTime today = TimeProvider.Current.UtcNow;
            int age = today.Year - birthdate.Year;
            if (birthdate.Date > today.AddYears(-age)) age--;

            return age;
        }
    }
}