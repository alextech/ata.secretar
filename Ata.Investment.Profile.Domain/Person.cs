using System;
using SharedKernel;

namespace Ata.Investment.Profile.Domain
{
    public abstract class Person : Entity
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public DateTimeOffset DateOfBirth { get; set; }
        
        protected Person () { }
        
        public Person(string name, string email, DateTimeOffset dateOfBirth)
        {
            Name = name;
            Email = email;
            DateOfBirth = dateOfBirth;
        }
    }
}