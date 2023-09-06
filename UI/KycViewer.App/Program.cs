using System;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using AutoMapper;
using KycViewer.App.HandlerProxies;
using MediatR;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.FeatureManagement;
using SharedKernel;
using Ata.Investment.ClientsList;
using AtaUiToolkit;

namespace KycViewer.App
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            WebAssemblyHostBuilder builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("body");

            builder.Services.AddMediatR(new []
            {
                typeof(RpcBehavior<,>).GetTypeInfo().Assembly
            });

            builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(RpcBehavior<,>));
            builder.Services.AddSingleton<IApplicationState>(new ApplicationState());
            builder.Services.AddScoped<WorkItemTypeStore>();

            builder.Services.AddFeatureManagement();

            builder.Services.RegisterKycViewerServices();
            builder.Services.RegisterClientsListServices();

            builder.Services.AddScoped(
                sp => new HttpClient {BaseAddress = new Uri(builder.HostEnvironment.BaseAddress)});

            HttpApiEndpoints.BaseUrl = builder.Configuration.GetSection(ApiOptions.Api)["BaseUrl"];

            Mapper.Initialize(cfg =>
            {
                cfg.AddProfile<ClientsListMapperProfile>();
            });

            await builder.Build().RunAsync();
        }
    }
}