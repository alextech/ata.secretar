using System;

namespace Ata.Investment.Profile.Domain.Household
{
    public class Client : Person
    {
        protected Client() { }

        public Client(string name, string email, DateTimeOffset dateOfBirth) 
            : base(name, email, dateOfBirth)
        {
            
        }

        public Client(string name, string email, DateTimeOffset dateOfBirth, Guid guid)
            : this(name, email, dateOfBirth)
        {
            Guid = guid;
        }

        public static Client Create(string name, string email, DateTimeOffset dateOfBirth)
        {
            return Create(name, email, dateOfBirth, Guid.NewGuid());
        }

        public static Client Create(string name, string email, DateTimeOffset dateOfBirth, Guid guid)
        {
            Client client = new Client(name, email, dateOfBirth, guid);
            
            return client;
        }
    }
}