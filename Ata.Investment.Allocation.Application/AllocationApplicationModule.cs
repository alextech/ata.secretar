using System.Reflection;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Ata.Investment.Allocation.Application.Draft;

namespace Ata.Investment.Allocation.Application
{
    public static class AllocationApplicationModule
    {
        public static IServiceCollection RegisterAllocationApplicationServices(this IServiceCollection services)
        {
            services.AddMediatR(typeof(PublishDraftCommandHandler).GetTypeInfo().Assembly);
            services.AddScoped<AllocationRepository>();

            return services;
        }
    }
}