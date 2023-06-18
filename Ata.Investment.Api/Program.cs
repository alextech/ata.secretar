using System.IO;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Ata.Investment.Api
{
#pragma warning disable CS1591
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args)
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("hosting.json", optional: true)
                .AddEnvironmentVariables()
                .Build();

            return WebHost.CreateDefaultBuilder(args)
                .ConfigureLogging(ConfigLogging)
                .ConfigureKestrel(options => { options.AllowSynchronousIO = true; })
                .UseConfiguration(config)
                .UseStaticWebAssets()
                .UseStartup<Startup>();
        }

        private static void ConfigLogging(WebHostBuilderContext hostingContext, ILoggingBuilder logging)
        {
            logging.AddAzureWebAppDiagnostics();
            logging.AddFilter(DbLoggerCategory.Database.Connection.Name, LogLevel.Information);
        }
    }
#pragma warning restore CS1591
}