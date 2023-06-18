using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace KycViewer.App.HandlerProxies
{
    public class RpcBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    {
        private readonly HttpClient _httpClient;

        public RpcBehavior(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            RequestBuilder rpcRequestBuilder = new();
            return rpcRequestBuilder
                .UsingMethod(HttpMethod.Post)
                .ToUrl(HttpApiEndpoints.BaseUrl)
                .WithContent(request)
                .ExecuteOn<TResponse>(_httpClient, cancellationToken);
        }
    }
}