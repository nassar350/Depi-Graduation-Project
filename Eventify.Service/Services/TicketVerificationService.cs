using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Eventify.Core.Entities;
using Eventify.Repository.Interfaces;
using Eventify.Service.DTOs.Tickets;
using Eventify.Service.Interfaces;

namespace Eventify.Service.Services
{
    public class TicketVerificationService : ITicketVerificationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITicketEncryptionService _encryptionService;

        public TicketVerificationService(
            IUnitOfWork unitOfWork,
            ITicketEncryptionService encryptionService)
        {
            _unitOfWork = unitOfWork;
            _encryptionService = encryptionService;
        }

        public async Task<(bool Success, string Message, TicketVerificationResponseDto Data, string[] Errors)> VerifyTicketAsync(string token)
        {
            try
            {
                // Step 1: Decrypt token
                TicketTokenData ticketData;
                try
                {
                    ticketData = _encryptionService.DecryptToken(token);
                }
                catch
                {
                    return (false, "Invalid ticket token", null, new[] { "Token validation failed or corrupted" });
                }

                // Step 2: Get ticket from database
                var ticket = await _unitOfWork._ticketRepository.GetByIdAsync(ticketData.TicketId);
                if (ticket == null)
                {
                    return (false, "Ticket not found", null, new[] { "Ticket does not exist in the system" });
                }

                // Step 3: Load related data
                var booking = await _unitOfWork._bookingRepository.GetByIdAsync(ticketData.BookingId);
                if (booking == null)
                {
                    return (false, "Booking not found", null, new[] { "Associated booking does not exist" });
                }

                var eventEntity = await _unitOfWork._eventRepository.GetByIdAsync(ticket.EventId);
                if (eventEntity == null)
                {
                    return (false, "Event not found", null, new[] { "Associated event does not exist" });
                }

                // Step 4: Check if event has ended
                var now = DateTime.UtcNow;
                var isValid = eventEntity.EndDate > now;
                var eventStatus = GetEventStatus(eventEntity.StartDate, eventEntity.EndDate, now);

                // Step 5: Create response
                var user = await _unitOfWork._userRepository.GetByIdAsync(booking.UserId);
                var response = new TicketVerificationResponseDto
                {
                    IsValid = isValid,
                    TicketId = ticket.ID,
                    BookingId = booking.Id,
                    AttendeeName = $"{user.Name}",
                    Email = user.Email,
                    EventName = eventEntity.Name,
                    EventDate = eventEntity.StartDate,
                    EventEndDate = eventEntity.EndDate,
                    CategoryName = booking.CategoryName,
                    EventStatus = eventStatus
                };

                if (!isValid)
                {
                    return (false, "Ticket is no longer valid - Event has ended", response, new[] { $"Event ended on {eventEntity.EndDate:yyyy-MM-dd HH:mm}" });
                }

                return (true, "Ticket is valid", response, Array.Empty<string>());
            }
            catch (Exception ex)
            {
                return (false, "Error verifying ticket", null, new[] { ex.Message });
            }
        }

        private string GetEventStatus(DateTime startDate, DateTime endDate, DateTime currentDate)
        {
            if (currentDate < startDate)
                return "Upcoming";

            if (currentDate >= startDate && currentDate <= endDate)
                return "Ongoing";

            return "Ended";
        }
    }
}