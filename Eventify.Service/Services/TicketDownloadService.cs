using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Eventify.Core.Entities;
using Microsoft.Extensions.Configuration;
using Eventify.Repository.Interfaces;
using Eventify.Service.DTOs.Tickets;
using Eventify.Service.Interfaces;

namespace Eventify.Service.Services
{
    public class TicketDownloadService : ITicketDownloadService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITicketPdfService _pdfService;
        private readonly string _baseUrl;

        public TicketDownloadService(
            IUnitOfWork unitOfWork,
            ITicketPdfService pdfService,
            IConfiguration configuration)
        {
            _unitOfWork = unitOfWork;
            _pdfService = pdfService;
            _baseUrl = configuration["App:BaseUrl"] ?? "https://eventify.runasp.net";
        }

        public async Task<(bool Success, string Message, TicketDownloadResponseDto Data, string[] Errors)> GenerateTicketsPdfAsync(int bookingId)
        {
            try
            {
                // Validate booking exists
                var booking = await _unitOfWork._bookingRepository.GetByIdAsync(bookingId);
                if (booking == null)
                {
                    return (false, "Booking not found", null, new[] { "Invalid booking ID" });
                }

                // Get tickets
                var tickets = (await _unitOfWork._ticketRepository.GetAllAsync())
                    .Where(t => t.BookingId == bookingId)
                    .ToList();

                if (!tickets.Any())
                {
                    return (false, "No tickets found for this booking", null, new[] { "Tickets not generated yet" });
                }

                // Get event
                var eventEntity = await _unitOfWork._eventRepository.GetByIdAsync(tickets[0].EventId);
                if (eventEntity == null)
                {
                    return (false, "Event not found", null, new[] { "Associated event does not exist" });
                }

                // Generate PDF
                var pdfBytes = _pdfService.GenerateAllTicketsPdf(tickets, booking, eventEntity, _baseUrl);

                // Create response
                var response = new TicketDownloadResponseDto
                {
                    PdfBytes = pdfBytes,
                    FileName = $"Eventify-Tickets-{bookingId}.pdf",
                    TicketCount = tickets.Count
                };

                return (true, "PDF generated successfully", response, Array.Empty<string>());
            }
            catch (Exception ex)
            {
                return (false, "Error generating ticket PDF", null, new[] { ex.Message });
            }
        }
    }
}
