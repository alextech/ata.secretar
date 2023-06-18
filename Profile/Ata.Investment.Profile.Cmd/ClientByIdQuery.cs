using System;
using MediatR;
using Ata.Investment.Profile.Domain.Household;

namespace Ata.Investment.Profile.Cmd
{
    public class ClientByIdQuery : IRequest<Client>
    {
        public Guid ClientId { get; }

        public ClientByIdQuery(Guid clientId)
        {
            ClientId = clientId;
        }
    }
}