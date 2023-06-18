using System;
using System.Threading;
using System.Threading.Tasks;
using History.Cmd;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;
using SharedKernel;
using Ata.Investment.AuthCore;
using SessionAdvisor = Ata.Investment.AuthCore.Advisor;

namespace Ata.Investment.Api
{
    public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : ILoggable
    where TResponse : CommandResponse
    {
        private readonly AuthDbContext _dbContext;
        private readonly UserManager<Advisor> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly JsonSerializerSettings _jsonSettings = new JsonSerializerSettings()
        {
            PreserveReferencesHandling = PreserveReferencesHandling.Objects,
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            NullValueHandling = NullValueHandling.Include,
            TypeNameHandling = TypeNameHandling.All
        };

        public LoggingBehavior(
            AuthDbContext dbContext,
            UserManager<Advisor> userManager,
            IHttpContextAccessor httpContextAccessor
        )
        {
            _dbContext = dbContext;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            TResponse response = await next();

            await LogCommandAsync(request, response, cancellationToken);

            return response;
        }

        private async Task LogCommandAsync(TRequest request, TResponse response, CancellationToken cancellationToken)
        {
            SessionAdvisor userIdentity = await _userManager.GetUserAsync(_httpContextAccessor.HttpContext.User);

            string serializedCommand = JsonConvert.SerializeObject(request, _jsonSettings);

            AccessLog accessLog = new AccessLog()
            {
                LogDisplayName = request.LogDisplayName,
                Description = response.Description,
                User = userIdentity.Name,
                SerializedCommand = serializedCommand,
                TimeStamp = DateTimeOffset.UtcNow.DateTime,
            };
            await _dbContext.AccessLogs.AddAsync(accessLog, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}