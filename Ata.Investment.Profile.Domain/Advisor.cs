using System;
using Newtonsoft.Json;

namespace Ata.Investment.Profile.Domain
{
    public class Advisor : IEquatable<Advisor>
    {
        public Advisor(Guid guid, string name, string credentials, string email)
        {
            Guid = guid;
            Name = name;
            Credentials = credentials;
            Email = email;
        }

        public Guid Guid { get; }
        public string Name { get; }
        public string Credentials { get; }
        public string Email { get; }

        public bool Equals(Advisor? other)
        {
            return other != null && other.Guid.Equals(Guid);
        }
    }
}