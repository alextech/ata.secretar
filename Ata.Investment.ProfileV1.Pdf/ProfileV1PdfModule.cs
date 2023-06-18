using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Ata.Investment.ProfileV1.Pdf.Services;

namespace Ata.Investment.ProfileV1.Pdf
{
    public static class ProfileV1PdfModule
    {
        public static IServiceCollection RegisterPdfV1Services(this IServiceCollection services)
        {

            services.AddScoped<PdfRenderer>();
            services.AddScoped<IRazorViewToStringRenderer, RazorViewToStringRenderer>();

            IronPdf.License.LicenseKey = "IRONPDF-172200AD80-675670-F335E3-F9AD33E274-3EFB2C5F-UEx8FD7BF9287F09D8-IRO200504.5759.59121.PRO.1DEV.1YR.SUPPORTED.UNTIL.05.MAY.2021";

            return services;
        }
    }
}