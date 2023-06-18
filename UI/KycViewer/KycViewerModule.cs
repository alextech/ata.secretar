using MediatR;
using Microsoft.Extensions.DependencyInjection;
using SharedKernel;

namespace KycViewer
{
    public static class KycViewerModule
    {
        public static IServiceCollection RegisterKycViewerServices(this IServiceCollection services)
        {
            services.AddTransient<TimerService>();
            services.AddTransient<IMediator, Mediator>();

            return services;
        }
    }
}