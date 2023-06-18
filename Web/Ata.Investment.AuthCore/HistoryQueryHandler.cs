using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using History.Cmd;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Ata.Investment.AuthCore
{
    public class HistoryQueryHandler : IRequestHandler<HistoryQuery, IEnumerable<AccessLog>>
    {
        private readonly AuthDbContext _authDbContext;

        public HistoryQueryHandler(AuthDbContext authDbContext)
        {
            _authDbContext = authDbContext;
        }

        public async Task<IEnumerable<AccessLog>> Handle(HistoryQuery request, CancellationToken cancellationToken)
        {
            IQueryable<AccessLog> history = (
                from logs in _authDbContext.AccessLogs
                orderby logs.TimeStamp descending
                select logs
            );

            return await history.ToListAsync(cancellationToken: cancellationToken);
        }
    }
}