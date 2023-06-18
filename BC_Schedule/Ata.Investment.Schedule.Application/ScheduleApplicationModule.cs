using System.Reflection;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Ata.Investment.Schedule.Application;

public static class ScheduleApplicationModule
{
    public static IServiceCollection RegisterScheduleApplicationServices(this IServiceCollection services)
    {
        services.AddTransient<IMediator, Mediator>();
        services.AddMediatR(new[]
        {
            typeof(ScheduleMeetingCommandHandler).GetTypeInfo().Assembly
        });

        return services;
    }
}