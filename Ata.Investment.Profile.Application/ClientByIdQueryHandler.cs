using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Ata.Investment.Profile.Cmd;
using Ata.Investment.Profile.Data;
using Ata.Investment.Profile.Domain.Household;

namespace Ata.Investment.Profile.Application
{
    public class ClientByIdQueryHandler : IRequestHandler<ClientByIdQuery, Client>
    {
        private readonly ProfileContext _profileContext;

        public ClientByIdQueryHandler(ProfileContext profileContext)
        {
            _profileContext = profileContext;
        }

        public async Task<Client> Handle(ClientByIdQuery clientQuery, CancellationToken cancellationToken)
        {
            IQueryable<Client> client = from c in _profileContext.Clients
                where c.Guid == clientQuery.ClientId
                select c;

            return await client.SingleOrDefaultAsync(cancellationToken);
        }
    }
}