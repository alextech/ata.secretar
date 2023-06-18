using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging.AzureAppServices;
using Newtonsoft.Json;
using SendGrid;
using Ata.Investment.Allocation.Application;
using Ata.Investment.Allocation.Data;
using Ata.Investment.Api.Controllers;
using Ata.Investment.AuthCore;
using Ata.Investment.Profile.Application;
using Ata.Investment.Profile.Data;
using Ata.Investment.ProfileV1.Pdf;
using Ata.Investment.Schedule.Application;

namespace Ata.Investment.Api
{
#pragma warning disable CS1591
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<AzureFileLoggerOptions>(options =>
            {
                options.FileName = "azure-diagnostics-";
                options.FileSizeLimit = 50 * 1024;
                options.RetainedFileCountLimit = 5;
            }).Configure<AzureBlobLoggerOptions>(options =>
            {
                options.BlobName = "log.txt";
            });
            
            services.AddControllers(o => o.InputFormatters.Insert(0, new RawJsonBodyInputFormatter()))
                .AddNewtonsoftJson(o =>
                {
                    o.SerializerSettings.PreserveReferencesHandling = PreserveReferencesHandling.Objects;
                    o.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                    o.SerializerSettings.TypeNameHandling = TypeNameHandling.All;
                })
                ;
            services.AddRazorPages();
            // services.AddServerSideBlazor()
                // .AddCircuitOptions(o => { o.DetailedErrors = true; });

            services.AddCors();

            services.AddScoped<SendGridClient>(sp =>
            {
                string key = Environment.GetEnvironmentVariable("SENDGRID_APIKEY");
                return new SendGridClient(key);
            });

            services.AddScoped<AllocationSeed>();

            services.AddDbContext<AllocationContext>(
                options => options
                    .UseSqlServer(
                        Configuration.GetConnectionString("InvestmentConnection"),
                        srv => srv.MigrationsHistoryTable("__InvestmentMigrationsHistory")
                    )
                    .EnableSensitiveDataLogging(false),
                ServiceLifetime.Scoped
            );

            services.AddDbContext<HistoryContext>(
                options => options
                    .UseSqlServer(
                        Configuration.GetConnectionString("InvestmentConnection"),
                        srv => srv.MigrationsHistoryTable("__InvestmentMigrationsHistory")
                    )
                    .EnableSensitiveDataLogging(false)
            );

            services.AddDbContext<ProfileContext>(
                options => options
                    // .UseLazyLoadingProxies()
                    .UseSqlServer(
                        Configuration.GetConnectionString("InvestmentConnection"),
                        srv => srv.MigrationsHistoryTable("__InvestmentMigrationsHistory")
                    )
                    .EnableSensitiveDataLogging(true),
                ServiceLifetime.Scoped
            );

            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
            services.AddMediatR(typeof(LoggingBehavior<,>).GetTypeInfo().Assembly);

            services.RegisterAllocationApplicationServices();
            services.RegisterProfileApplicationServices();
            services.RegisterScheduleApplicationServices();
            services.RegisterAuthServices(Configuration);
            services.RegisterPdfV1Services();

//            services.AddSwaggerGen(sg =>
//            {
//                sg.SwaggerDoc("v1", new OpenApiInfo {Title = "TSG Investment API", Version = "v1"});
//            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment() || env.IsTesting())
            {
                Console.WriteLine("ENVIRONMENT: development");
                app.UseDeveloperExceptionPage();
                app.UseWebAssemblyDebugging();
            }
            else
            {
                Console.WriteLine("ENVIRONMENT: production");
                app.UseHsts();
            }

            app.UseCors(
                builder =>
                    builder.WithOrigins(
                            "https://localhost:5001",
                            "http://localhost:5000",
                            "https://localhost:5003",
                            "http://localhost:5002"
                        )
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials()
            );

            app.UseHttpsRedirection();
            app.UseBlazorFrameworkFiles();
            app.UseStaticFiles();
            // access with /api-docs/
//            app.UseSwagger();
//            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1"));
//            app.UseReDoc(rd =>
//            {
//                rd.SpecUrl = "/swagger/v1/swagger.json";
//                rd.DocumentTitle = "TSG Investment API v1";
//            });
            
            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();


            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
                // endpoints.MapBlazorHub();
                endpoints.MapFallbackToPage("/_Host");
                endpoints.MapControllerRoute(
                    name: "profilePdf",
                    pattern: "/meeting/{meetingId}/profile/{profileId}/pdf",
                    defaults: new {controller = "PdfRequest", action = "Index"}
                );
                endpoints.MapControllerRoute(
                    "meetingFullPdf",
                    "/meeting/{meetingId}/pdf",
                    new {controller = "PdfRequest", action = "DownloadAllPdfForMeeting"}
                );

                endpoints.MapControllerRoute(
                    "meetingQuickPdf",
                    "/meeting/{meetingId}/quickPdf",
                    new {controller = "PdfRequest", action = "DownloadQuickViewPdf"}
                );

                // understand authenticaiton & authorization better here https://docs.microsoft.com/ru-ru/aspnet/core/web-api/route-to-code?view=aspnetcore-5.0
                endpoints.MapPost(
                    "/rpc",
                    async context =>
                    {
                        JsonSerializerSettings settings = new JsonSerializerSettings()
                        {
                            PreserveReferencesHandling = PreserveReferencesHandling.Objects,
                            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                            NullValueHandling = NullValueHandling.Include,
                            TypeNameHandling = TypeNameHandling.All
                        };

                        StreamReader reader = new StreamReader(context.Request.Body);
                        string requestJson = await reader.ReadToEndAsync();

                        var commandQueryRequest = JsonConvert.DeserializeObject(requestJson, settings);


                        IMediator mediatR = context.RequestServices.GetService<IMediator>();
                        Debug.Assert(mediatR != null, nameof(mediatR) + " != null");

                        object commandQueryResponse = await mediatR.Send(commandQueryRequest);
                        string responseJson = JsonConvert.SerializeObject(commandQueryResponse, settings);

                        await context.Response.WriteAsync(responseJson);
                    }).RequireAuthorization();
            });

            // TODO collect from all modules
            Mapper.Initialize(
                cfg =>
                {
                    cfg.AddProfile<AllocationMapperProfile>();
                    cfg.AddProfile<AllocationsEditorMapperProfile>();
                    cfg.AddProfile<HouseholdMapperProfile>();
                });
        }
    }
#pragma warning restore CS1591
}