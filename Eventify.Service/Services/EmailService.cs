using Eventify.Service.Interfaces;
using Eventify.Repository.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SendGrid.Helpers.Mail;
using SendGrid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eventify.Service.Services
{
    public class EmailService : IEmailService
    {
        private readonly ILogger<EmailService> _logger;
        private readonly IConfiguration _config;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITicketPdfService _pdfService;
        private readonly string _baseUrl;

        public EmailService(
            ILogger<EmailService> logger, 
            IConfiguration config,
            IUnitOfWork unitOfWork,
            ITicketPdfService pdfService)
        {
            _logger = logger;
            _config = config;
            _unitOfWork = unitOfWork;
            _pdfService = pdfService;
            _baseUrl = config["App:BaseUrl"] ?? "https://eventiifyy.netlify.app";
        }

        public async Task SendEmailAsync(string toEmail, string subject, string content)
        {
            var apiKey = Environment.GetEnvironmentVariable("SendGridKey");
            var client = new SendGridClient(apiKey);
            var from = new EmailAddress(_config["Notification:FromEmail"], "Eventify");
            var to = new EmailAddress(toEmail);
            var msg = MailHelper.CreateSingleEmail(from, to, subject, content, content);
            var response = await client.SendEmailAsync(msg);

            _logger.LogInformation($"Email sent to {toEmail} with status {response.StatusCode}");
        }

        public async Task SendEmailWithAttachmentAsync(string toEmail, string subject, string htmlContent, byte[] attachmentData, string attachmentFileName, string attachmentContentType)
        {
            try
            {
                var apiKey = Environment.GetEnvironmentVariable("SendGridKey");
                var client = new SendGridClient(apiKey);
                var from = new EmailAddress(_config["Notification:FromEmail"], "Eventify");
                var to = new EmailAddress(toEmail);
                var msg = MailHelper.CreateSingleEmail(from, to, subject, null, htmlContent);

                // Add attachment
                msg.AddAttachment(
                    attachmentFileName,
                    Convert.ToBase64String(attachmentData),
                    attachmentContentType
                );

                var response = await client.SendEmailAsync(msg);
                _logger.LogInformation($"Email with attachment sent to {toEmail} with status {response.StatusCode}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to send email with attachment to {toEmail}");
                throw;
            }
        }

        public async Task SendPaymentSuccessEmailWithTicketsAsync(int bookingId, string customerEmail, string customerName)
        {
            try
            {
                // Get booking details
                var booking = await _unitOfWork._bookingRepository.GetByIdAsync(bookingId);
                if (booking == null)
                {
                    _logger.LogWarning($"Booking {bookingId} not found");
                    return;
                }

                // Get user details for proper name
                var user = await _unitOfWork._userRepository.GetByIdAsync(booking.UserId);
                if (user != null)
                {
                    customerName = user.Name ?? customerName;
                }

                // Get event details from booking's category
                var allCategories = await _unitOfWork._categoryRepository.GetAllAsync();
                var category = allCategories.FirstOrDefault(c => c.Title == booking.CategoryName);
                if (category == null)
                {
                    _logger.LogWarning($"Category {booking.CategoryName} not found");
                    return;
                }

                var eventEntity = await _unitOfWork._eventRepository.GetByIdAsync(category.EventId);
                if (eventEntity == null)
                {
                    _logger.LogWarning($"Event for booking {bookingId} not found");
                    return;
                }

                // Get tickets
                var allTickets = await _unitOfWork._ticketRepository.GetAllAsync();
                var tickets = allTickets.Where(t => t.BookingId == bookingId).ToList();
                if (!tickets.Any())
                {
                    _logger.LogWarning($"No tickets found for booking {bookingId}");
                    return;
                }

                // Generate PDF
                var pdfBytes = _pdfService.GenerateAllTicketsPdf(tickets, booking, eventEntity, _baseUrl);

                // Get payment details
                var allPayments = await _unitOfWork._paymentRepository.GetAllAsync();
                var payment = allPayments.FirstOrDefault(p => p.BookingId == bookingId);

                // Generate email HTML
                var emailHtml = GeneratePaymentSuccessEmailTemplate(
                    customerName,
                    booking,
                    eventEntity,
                    payment?.TotalPrice ?? 0,
                    payment?.StripePaymentIntentId ?? "N/A"
                );

                // Send email with PDF attachment
                await SendEmailWithAttachmentAsync(
                    customerEmail,
                    $"🎉 Payment Successful - Your Tickets for {eventEntity.Name}",
                    emailHtml,
                    pdfBytes,
                    $"Eventify-Tickets-{bookingId}.pdf",
                    "application/pdf"
                );

                _logger.LogInformation($"Payment success email with tickets sent for booking {bookingId}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to send payment success email for booking {bookingId}");
                throw;
            }
        }

        private string GeneratePaymentSuccessEmailTemplate(string customerName, dynamic booking, dynamic eventEntity, decimal totalAmount, string paymentIntentId)
        {
            var eventDate = ((DateTime)eventEntity.StartDate).ToString("dddd, MMMM dd, yyyy");
            var eventTime = ((DateTime)eventEntity.StartDate).ToString("hh:mm tt");

            return $@"
<!DOCTYPE html>
<html>
<head>
    <style>
        body {{ font-family: Arial, sans-serif; background-color: #f4f7fa; margin: 0; padding: 20px; }}
        .container {{ max-width: 600px; margin: 0 auto; background: white; border-radius: 12px; overflow: hidden; box-shadow: 0 4px 12px rgba(0,0,0,0.1); }}
        .header {{ background: linear-gradient(135deg, #7C5CFF 0%, #5B3FE5 100%); padding: 40px; text-align: center; color: white; }}
        .success-icon-wrapper {{ text-align: center; margin-bottom: 20px; }}
        .success-icon {{ width: 80px; height: 80px; background: #00d97a; border-radius: 50%; display: inline-block; line-height: 80px; text-align: center; font-size: 40px; color: white; }}
        .content {{ padding: 40px 30px; }}
        .greeting {{ font-size: 20px; color: #1e2329; margin-bottom: 15px; font-weight: 600; }}
        .message {{ font-size: 16px; color: #5a6b7c; line-height: 1.6; margin-bottom: 30px; }}
        .event-details {{ background: #f0f4ff; padding: 20px; border-radius: 8px; margin-bottom: 30px; }}
        .event-details h3 {{ color: #7C5CFF; margin-bottom: 15px; }}
        .event-info {{ color: #5a6b7c; margin-bottom: 10px; }}
        .event-info strong {{ color: #1e2329; }}
        .booking-details {{ background: #f8f9fa; border-left: 4px solid #7C5CFF; padding: 20px; border-radius: 8px; margin-bottom: 30px; }}
        .detail-row {{ display: flex; justify-content: space-between; padding: 10px 0; border-bottom: 1px solid #e4e7eb; }}
        .detail-row:last-child {{ border-bottom: none; }}
        .attachment-notice {{ background: #fff3cd; border: 1px solid #ffc107; border-radius: 8px; padding: 15px; margin-bottom: 30px; color: #856404; }}
        .footer {{ background: #1e2329; padding: 30px; text-align: center; color: #98a0ab; }}
    </style>
</head>
<body>
    <div class=""container"">
        <div class=""header"">
            <div class=""success-icon-wrapper"">
                <div class=""success-icon"">✓</div>
            </div>
            <h1>Payment Successful!</h1>
            <p>Your booking has been confirmed</p>
        </div>
        <div class=""content"">
            <div class=""greeting"">Hi {customerName},</div>
            <p class=""message"">
                Thank you for your booking! We're excited to confirm your purchase. 
                Your tickets are attached to this email as a PDF file.
            </p>
            <div class=""attachment-notice"">
                <strong>📎 Ticket Attachment</strong>
                Your tickets are attached to this email as a PDF file. Each ticket includes a unique QR code for verification.
            </div>
            <div class=""event-details"">
                <h3>🎉 {eventEntity.Name}</h3>
                <div class=""event-info""><strong>📅 Date:</strong> {eventDate}</div>
                <div class=""event-info""><strong>🕐 Time:</strong> {eventTime}</div>
                <div class=""event-info""><strong>📍 Location:</strong> {eventEntity.Address}</div>
                <div class=""event-info""><strong>🎫 Category:</strong> {booking.CategoryName}</div>
            </div>
            <div class=""booking-details"">
                <h2>Booking Summary</h2>
                <div class=""detail-row"">
                    <span><strong>Booking ID:</strong></span>
                    <span>#{booking.Id:D6}</span>
                </div>
                <div class=""detail-row"">
                    <span><strong>Number of Tickets:</strong></span>
                    <span>{booking.TicketsNum}</span>
                </div>
                <div class=""detail-row"">
                    <span><strong>Total Amount:</strong></span>
                    <span>${totalAmount:F2}</span>
                </div>
                <div class=""detail-row"">
                    <span><strong>Payment ID:</strong></span>
                    <span>{paymentIntentId}</span>
                </div>
            </div>
        </div>
        <div class=""footer"">
            <p>© 2025 Eventify. All rights reserved.</p>
        </div>
    </div>
</body>
</html>";
        }
    }
}
