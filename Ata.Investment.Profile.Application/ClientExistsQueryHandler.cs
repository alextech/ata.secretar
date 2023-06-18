using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Ata.Investment.Profile.Cmd;
using Ata.Investment.Profile.Data;

namespace Ata.Investment.Profile.Application
{
    public class ClientExistsQueryHandler : IRequestHandler<ClientExistsQuery, Guid>
    {
        private readonly ProfileContext _profileContext;

        public ClientExistsQueryHandler(ProfileContext profileContext)
        {
            _profileContext = profileContext;
        }

        public async Task<Guid> Handle(ClientExistsQuery clientExistsQuery, CancellationToken cancellationToken)
        {
            Guid clientGuid = await (
                from c in _profileContext.Clients
                where c.Name == clientExistsQuery.Name && c.Email == clientExistsQuery.Email
                select c.Guid
            ).SingleOrDefaultAsync(cancellationToken);

            if (clientGuid == null)
            {
                clientGuid = Guid.Empty;
            }

            return clientGuid;
        }
    }
}