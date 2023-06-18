using System;

namespace Ata.Investment.ClientsList.ViewModels
{
    public class ClientVM : ICloneable
    {
        public Guid Guid { get; set; }
        
        public string Name { get; set; }

        private string _details;
        
        public string Details
        {
            get => Guid != Guid.Empty ? "Other interesting details" : _details;
            set => _details = value;
        }

        public string Email { get; set; }

        public DateTimeOffset DateOfBirth { get; set; }

        public object Clone()
        {
            return new ClientVM()
            {
                Guid = Guid,
                Name = Name,
                Email = Email,
                DateOfBirth = DateOfBirth
            };
        }
    }
}
