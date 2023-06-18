using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;
using IronPdf;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Ata.Investment.Profile.Cmd;
using Ata.Investment.Profile.Domain;
using Ata.Investment.Profile.Domain.Household;
using Ata.Investment.ProfileV1.Pdf.Services;

namespace Ata.Investment.Api.Controllers
{
    [Authorize]
    public class PdfRequestController : Controller
    {
        private readonly PdfRenderer _pdfRenderer;
        private readonly IMediator _mediatR;

        public PdfRequestController(PdfRenderer pdfRenderer, IMediator mediatR)
        {
            _pdfRenderer = pdfRenderer;
            _mediatR = mediatR;
        }

        // GET
        public async Task<IActionResult> Index(Guid meetingId, Guid profileId)
        {
            Meeting meeting = await _mediatR.Send(new MeetingByIdQuery(meetingId));
            Profile.Domain.Profile.Profile profile = meeting.KycDocument.AllProfiles.Single(p => p.Guid == profileId);

            return await _downloadSinglePdf(meeting, profile);
        }

        public async Task<IActionResult> DownloadAllPdfForMeeting(Guid meetingId)
        {
            Meeting meeting = await _mediatR.Send(new MeetingByIdQuery(meetingId));

            if (meeting.KycDocument.AllProfiles.Count() == 1)
            {
                return await _downloadSinglePdf(meeting, meeting.KycDocument.AllProfiles.First());
            }
            else
            {
                return await _downloadZipPdf(meeting);
            }
        }

        public async Task<IActionResult> DownloadQuickViewPdf(Guid meetingId)
        {
            Meeting meeting = await _mediatR.Send(new MeetingByIdQuery(meetingId));

            PdfDocument pdf = await _pdfRenderer.RenderQuickViewForMeeting(meeting);

            string dateString = meeting.Date.ToString("dd-MM-yyyy");

            ContentDisposition contentDispositionHeader = new ContentDisposition
            {
                Inline = true,
                FileName = $"meeting_with_{meeting.KycDocument.PrimaryClient.Name}_on_{dateString}.pdf"
            };

            Response.Headers.Add("Content-Disposition", contentDispositionHeader.ToString());

            return new FileContentResult(pdf.BinaryData, MediaTypeNames.Application.Pdf);
        }

        private async Task<IActionResult> _downloadSinglePdf(Meeting meeting, Profile.Domain.Profile.Profile profile)
        {
            PdfDocument tstFile = await _pdfRenderer.RenderProfileFromDocument(meeting.KycDocument, profile.Guid);

            string preparedFor = profile.PrimaryClient.Name + (profile.IsJoint ? " and " + profile.JointClient?.Name : "");
            preparedFor = preparedFor.Replace(" ", "-");

            string dateString = meeting.Date.ToString("dd-MM-yyyy");

            ContentDisposition contentDispositionHeader = new ContentDisposition
            {
                Inline = true,
                FileName = $"{preparedFor}_{profile.Name.Replace(" ", "-")}_{dateString}.pdf"
            };
            Response.Headers.Add("Content-Disposition", contentDispositionHeader.ToString());

            return File(tstFile.Stream, MediaTypeNames.Application.Pdf, contentDispositionHeader.FileName);
        }

        private async Task<IActionResult> _downloadZipPdf(Meeting meeting)
        {
            string dateString = meeting.Date.ToString("dd-MM-yyyy");

            await using (MemoryStream compressedFileStream = new MemoryStream())
            using (ZipArchive zipArchive = new ZipArchive(compressedFileStream, ZipArchiveMode.Create, true))
            {
                foreach (Profile.Domain.Profile.Profile profile in meeting.KycDocument.AllProfiles)
                {
                    string preparedFor = profile.PrimaryClient.Name +
                                         (profile.IsJoint ? " and " + profile.JointClient?.Name : "");
                    preparedFor = preparedFor.Replace(" ", "-");

                    PdfDocument profilePdf =
                        await _pdfRenderer.RenderProfileFromDocument(meeting.KycDocument, profile.Guid);

                    string pdfFileName = $"{preparedFor}_{profile.Name.Replace(" ", "-")}_{dateString}.pdf";

                    ZipArchiveEntry zipEntry = zipArchive.CreateEntry(pdfFileName);
                    await using (MemoryStream originalFileStream = profilePdf.Stream)
                    await using (Stream zipEntryStream = zipEntry.Open())
                    {
                        await originalFileStream.CopyToAsync(zipEntryStream);
                    }
                }


                Household household = meeting.Household;
                string preparedForZip = household.PrimaryClient.Name +
                                        (household.IsJoint ? " and " + household.JointClient?.Name : "");
                preparedForZip = preparedForZip.Replace(" ", "-");

                ContentDisposition contentDispositionHeader = new ContentDisposition
                {
                    Inline = true,
                    FileName = $"{preparedForZip}_{dateString}.zip"
                };
                Response.Headers.Add("Content-Disposition", contentDispositionHeader.ToString());

                zipArchive.Dispose();
                compressedFileStream.Close();

                return File(compressedFileStream.ToArray(), MediaTypeNames.Application.Zip,
                    contentDispositionHeader.FileName);
            }
        }
    }
}