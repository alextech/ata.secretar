using System;
using System.Linq;
using System.Threading.Tasks;
using IronPdf;
using Microsoft.Extensions.Logging;
using Ata.Investment.Profile.Domain;
using Ata.Investment.Profile.Domain.KYC;
using Ata.Investment.ProfileV1.Pdf.Views;

namespace Ata.Investment.ProfileV1.Pdf.Services
{
    public class PdfRenderer
    {
        private readonly IRazorViewToStringRenderer _razorViewToStringRenderer;
        private readonly ILogger<PdfRenderer> _logger;

        public PdfRenderer(IRazorViewToStringRenderer razorViewToStringRenderer, ILogger<PdfRenderer> logger)
        {
            _razorViewToStringRenderer = razorViewToStringRenderer;
            _logger = logger;
        }

        public async Task<PdfDocument> RenderProfileFromDocument(KycDocument document, Guid profileId)
        {
            Profile.Domain.Profile.Profile profile = document.AllProfiles.Single(p => p.Guid == profileId);
            ProfileReportViewModel vm = new ProfileReportViewModel(document, profile);

            string reportBody = await _razorViewToStringRenderer.RenderViewToStringAsync("/Views/ProfileReport.cshtml", vm);

            // Does not work with IronPDF. Using base64 file content in template.
            // BaseDirectory already has leading slash /
            // string baseDir = System.AppDomain.CurrentDomain.BaseDirectory + "assets";

            // _logger.LogInformation("Pdf assets location: {fileLocation}", baseDir);

            // Uri assetsDir = new Uri(baseDir);

            HtmlToPdf renderer = new IronPdf.HtmlToPdf
            {
                PrintOptions =
                {
                    CssMediaType = PdfPrintOptions.PdfCssMediaType.Print,
                    MarginTop = 0,
                    MarginRight = 0,
                    MarginBottom = 0,
                    MarginLeft = 0,

                    EnableJavaScript = true,
                    RenderDelay = 2000
                }
            };
            PdfDocument pdf = renderer.RenderHtmlAsPdf(reportBody);

            // GetTempPath already has leading slash \
            // TODO eventually upload to azure blob storage
            // string tmpFileName = Path.GetTempPath() + "test.pdf";
            // _logger.LogInformation("Temporary pdf file location: {fileLocation}", tmpFileName);

            return pdf;
        }

        public async Task<PdfDocument> RenderQuickViewForMeeting(Meeting meeting)
        {
            MeetingQuickViewViewModel vm = new MeetingQuickViewViewModel(meeting.KycDocument);
            string reportBody = await _razorViewToStringRenderer.RenderViewToStringAsync("/Views/MeetingQuickView.cshtml", vm);

            HtmlToPdf renderer = new IronPdf.HtmlToPdf
            {
                PrintOptions =
                {
                    CssMediaType = PdfPrintOptions.PdfCssMediaType.Print,
                    MarginTop = 10,
                    MarginRight = 0,
                    MarginBottom = 0,
                    MarginLeft = 10,
                }
            };
            PdfDocument pdf = renderer.RenderHtmlAsPdf(reportBody);
            pdf.Flatten();

            return pdf;
        }
    }
}