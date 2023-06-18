using System;
using MediatR;

namespace Ata.Investment.Profile.Cmd
{
    public class ClientExistsQuery : IRequest<Guid>
    {
        public string Name { get; }
        public string Email { get; }

        public ClientExistsQuery(string name, string email)
        {
            Name = name;
            Email = email;
        }
    }
}