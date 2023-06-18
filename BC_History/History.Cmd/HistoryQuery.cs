using System.Collections.Generic;
using MediatR;

namespace History.Cmd
{
    public class HistoryQuery : IRequest<IEnumerable<AccessLog>>
    {
    }
}