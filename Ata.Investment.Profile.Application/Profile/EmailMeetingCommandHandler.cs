using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SendGrid;
using SendGrid.Helpers.Mail;
using SharedKernel;
using Ata.Investment.Profile.Cmd.Profile;
using Ata.Investment.Profile.Data;
using Ata.Investment.Profile.Domain;
using Ata.Investment.Profile.Domain.KYC;
using Ata.Investment.ProfileV1.Pdf.Services;
using Response = SendGrid.Response;

namespace Ata.Investment.Profile.Application.Profile
{
    public class EmailMeetingCommandHandler : ICommandHandler<EmailMeetingCommand, CommandResponse>
    {
        private readonly ProfileContext _profileContext;
        private readonly PdfRenderer _pdfRenderer;
        private readonly SendGridClient _sendGridClient;

        public EmailMeetingCommandHandler(ProfileContext profileContext, PdfRenderer pdfRenderer, SendGridClient sendGridClient)
        {
            _profileContext = profileContext;
            _pdfRenderer = pdfRenderer;
            _sendGridClient = sendGridClient;
        }

        public async Task<CommandResponse> Handle(EmailMeetingCommand meetingRequest, CancellationToken cancellationToken)
        {
            KycDocument document = (
                from m in _profileContext.Meetings
                where m.Guid == meetingRequest.MeetingId
                select m
            ).First().KycDocument;

            List<EmailAddress> emails = new List<EmailAddress>()
            {
                new EmailAddress(document.PrimaryClient.Email)
            };

            if (document.IsJoint)
            {
                emails.Add(new EmailAddress(document.JointClient?.Email));
            }

            SendGridMessage clientEmail = MailHelper.CreateSingleEmailToMultipleRecipients(
                from: new EmailAddress("admin@example.com"),
                tos: emails,
                subject: "Your investment report",
                plainTextContent: "Attached is your investment questionnaire result.",
                htmlContent: "Attached is your investment questionnaire result."
            );

            SendGridMessage advisorEmail = MailHelper.CreateSingleEmailToMultipleRecipients(
                from: new EmailAddress("advisor@example.com"),
                tos:new List<EmailAddress>()
                {
                    new EmailAddress("admin@example.com"),
                    new EmailAddress(document.Advisor.Email)
                },
                subject: "Client investment meeting results",
                plainTextContent: "Client investment meeting results",
                htmlContent: "Client investment meeting results"
            );

            foreach (Domain.Profile.Profile profile in document.AllProfiles)
            {
                string pdf = Convert.ToBase64String((await _pdfRenderer.RenderProfileFromDocument(document, profile.Guid)).BinaryData);

                string preparedFor =
                    profile.PrimaryClient.Name + (profile.IsJoint ? " and " + profile.JointClient?.Name : "");
                preparedFor = preparedFor.Replace(" ", "_");
                string fileName = $"{preparedFor}-{profile.Name.Replace(" ", "_")}.pdf";

                clientEmail.AddAttachment(fileName, pdf, "application/pdf", "attachment", "Profile");
                advisorEmail.AddAttachment(fileName, pdf, "application/pdf", "attachment", "Profile");
            }

            Response clientEmailResponse = await _sendGridClient.SendEmailAsync(clientEmail, cancellationToken);
            Response advisorEmailResponse = await _sendGridClient.SendEmailAsync(advisorEmail, cancellationToken);

            return CommandResponse.Ok(
                $"Meeting for \"{document.Purpose}\" was emailed."
            );
        }
    }
}