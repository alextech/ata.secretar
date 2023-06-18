using Microsoft.Extensions.Hosting;

namespace Ata.Investment.Api
{
    public static class HostingEnvironmentExtensions
    {
        public const string Testing = "Testing";

        public static bool IsTesting(this IHostEnvironment hostingEnvironment)
        {
            return hostingEnvironment.IsEnvironment(Testing);
        }
    }
}