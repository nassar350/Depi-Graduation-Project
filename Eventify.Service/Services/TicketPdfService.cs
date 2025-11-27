using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Eventify.Core.Entities;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using Eventify.Service.Interfaces;
using System.Reflection.Metadata;
using Eventify.Repository.Interfaces;

namespace Eventify.Service.Services
{
    public class TicketPdfService : ITicketPdfService
    {
        private readonly IQrCodeService _qrCodeService;
        private readonly ITicketEncryptionService _encryptionService;
        private readonly IUnitOfWork _unitOfWork;

        public TicketPdfService(
            IQrCodeService qrCodeService,
            ITicketEncryptionService encryptionService,
            IUnitOfWork unitOfWork)
        {
            _qrCodeService = qrCodeService;
            _encryptionService = encryptionService;
            _unitOfWork = unitOfWork;
            QuestPDF.Settings.License = LicenseType.Community;
        }

        public byte[] GenerateTicketPdf(Ticket ticket, Booking booking, Event eventEntity, string verificationUrl)
        {
            var document = QuestPDF.Fluent.Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(40);
                    page.PageColor(Colors.White);

                    page.Content().Column(column =>
                    {
                        // Header
                        BuildHeader(column, ticket);

                        // Event Information
                        BuildEventInfo(column, eventEntity, booking);

                        // Attendee Information
                        BuildAttendeeInfoAsync(column, booking);

                        // QR Code
                        BuildQrCodeSection(column, verificationUrl);

                        // Footer
                        BuildFooter(column);
                    });
                });
            });

            return document.GeneratePdf();
        }

        public byte[] GenerateAllTicketsPdf(List<Ticket> tickets, Booking booking, Event eventEntity, string verificationBaseUrl)
        {
            var document = QuestPDF.Fluent.Document.Create(container =>
            {
                foreach (var ticket in tickets)
                {
                    var token = _encryptionService.GenerateTicketToken(ticket.ID, booking.Id);
                    var verificationUrl = $"{verificationBaseUrl}/verify-ticket.html?token={token}";

                    container.Page(page =>
                    {
                        page.Size(PageSizes.A4);
                        page.Margin(40);
                        page.PageColor(Colors.White);

                        page.Content().Column(column =>
                        {
                            BuildHeader(column, ticket);
                            BuildEventInfo(column, eventEntity, booking);
                            BuildAttendeeInfoAsync(column, booking);
                            BuildQrCodeSection(column, verificationUrl);
                            BuildFooter(column);
                        });
                    });
                }
            });

            return document.GeneratePdf();
        }

        private void BuildHeader(ColumnDescriptor column, Ticket ticket)
        {
            column.Item().Row(row =>
            {
                row.RelativeItem().Column(col =>
                {
                    col.Item().Text("EVENTIFY").FontSize(24).Bold().FontColor("#7C5CFF");
                    col.Item().Text("Your Digital Ticket").FontSize(12).FontColor(Colors.Grey.Darken2);
                });
                row.ConstantItem(80).AlignRight().Text($"#{ticket.ID:D6}").FontSize(14).SemiBold();
            });

            column.Item().PaddingVertical(10).LineHorizontal(2).LineColor("#7C5CFF");
        }

        private void BuildEventInfo(ColumnDescriptor column, Event eventEntity, Booking booking)
        {
            column.Item().PaddingTop(20).Text(eventEntity.Name).FontSize(20).Bold();
            column.Item().PaddingTop(5).Text(eventEntity.Description ?? "").FontSize(10).FontColor(Colors.Grey.Darken1);

            column.Item().PaddingTop(15).Column(col =>
            {
                AddInfoRow(col, "📅 Date", eventEntity.StartDate.ToString("dddd, MMMM dd, yyyy"));
                AddInfoRow(col, "🕐 Time", eventEntity.StartDate.ToString("hh:mm tt"));
                AddInfoRow(col, "📍 Location", eventEntity.Address);
                AddInfoRow(col, "🎫 Category", booking.CategoryName);
            });

            column.Item().PaddingVertical(10).LineHorizontal(1).LineColor(Colors.Grey.Lighten2);
        }

        private async Task BuildAttendeeInfoAsync(ColumnDescriptor column, Booking booking)
        {
            var user = await _unitOfWork._userRepository.GetByIdAsync(booking.UserId);

            column.Item().PaddingTop(15).Text("Attendee Information").FontSize(14).SemiBold();
            column.Item().PaddingTop(10).Column(col =>
            {
                AddInfoRow(col, "Name", $"{user.Name}");
                AddInfoRow(col, "Email", user.Email);
                if (!string.IsNullOrEmpty(user.PhoneNumber))
                    AddInfoRow(col, "Phone", user.PhoneNumber);
                AddInfoRow(col, "Booking ID", $"#{booking.Id:D6}");
            });

            column.Item().PaddingVertical(10).LineHorizontal(1).LineColor(Colors.Grey.Lighten2);
        }

        private void BuildQrCodeSection(ColumnDescriptor column, string verificationUrl)
        {
            column.Item().PaddingTop(20).AlignCenter().Column(qrCol =>
            {
                qrCol.Item().Text("Scan to Verify Ticket").FontSize(12).SemiBold();
                qrCol.Item().PaddingTop(10).Width(200).Height(200).Image(_qrCodeService.GenerateQrCode(verificationUrl));
                qrCol.Item().PaddingTop(10).Hyperlink(verificationUrl).Text("Click here to verify online")
                    .FontSize(10).FontColor(Colors.Blue.Darken2).Underline();
                qrCol.Item().PaddingTop(5).Text("or scan the QR code above").FontSize(8).FontColor(Colors.Grey.Darken1);
            });
        }

        private void BuildFooter(ColumnDescriptor column)
        {
            column.Item().PaddingTop(30).AlignCenter().Column(footerCol =>
            {
                footerCol.Item().Text("Important Information").FontSize(10).SemiBold();
                footerCol.Item().PaddingTop(5).Text("• This ticket is valid for one person only").FontSize(8);
                footerCol.Item().Text("• Please arrive 30 minutes before the event starts").FontSize(8);
                footerCol.Item().Text("• Keep this ticket safe - it cannot be replaced if lost").FontSize(8);
                footerCol.Item().PaddingTop(10).Text($"Generated on {DateTime.Now:yyyy-MM-dd HH:mm}").FontSize(7).FontColor(Colors.Grey.Darken1);
            });
        }

        private void AddInfoRow(ColumnDescriptor column, string label, string value)
        {
            column.Item().PaddingVertical(3).Row(row =>
            {
                row.ConstantItem(100).Text(label).FontSize(10).SemiBold();
                row.RelativeItem().Text(value).FontSize(10);
            });
        }
    }
}
