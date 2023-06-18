using System.Reflection;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Ata.Investment.AuthCore
{
    public static class AuthModule
    {
        public static IServiceCollection RegisterAuthServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<AuthDbContext>(
                options =>options
                    .UseSqlServer(
                        configuration.GetConnectionString("InvestmentConnection"),
                    srv => srv.MigrationsHistoryTable("__InvestmentMigrationsHistory")
                    )
                    .EnableSensitiveDataLogging(true)
            );

            services.AddIdentity<Advisor, IdentityRole>(
                    options => options
                        .SignIn.RequireConfirmedAccount = false)
                .AddEntityFrameworkStores<AuthDbContext>();


            services.AddScoped<UserSeed>();
            services.AddMediatR(typeof(HistoryQueryHandler).GetTypeInfo().Assembly);

            return services;
        }
    }
}