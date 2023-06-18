using System.Reflection;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Ata.Investment.Profile.Application.Advisors;
using Ata.Investment.Profile.Application.Households;
using Ata.Investment.Profile.Application.Profile;

namespace Ata.Investment.Profile.Application
{
    public static class ProfileApplicationModule
    {
        public static IServiceCollection RegisterProfileApplicationServices(this IServiceCollection services)
        {
            services.AddTransient<IMediator, Mediator>();
            services.AddMediatR(new[]
            {
                typeof(CloneProfileFromMeetingCommandHandler).GetTypeInfo().Assembly,
                typeof(AllAdvisorsQueryHandler).GetTypeInfo().Assembly
            });

            services.AddScoped<MeetingRepository>();

            return services;
        }
    }
}