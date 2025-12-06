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

                // Step 4: Get payment information
                var payment = await _unitOfWork._paymentRepository.GetByIdAsync(booking.Id);
                if (payment == null)
                {
                    return (false, "Payment not found", null, new[] { "Associated payment does not exist" });
                }

                // Step 5: Validate ticket status
                var now = DateTime.UtcNow;
                var eventStatus = GetEventStatus(eventEntity.StartDate, eventEntity.EndDate, now);
                var user = await _unitOfWork._userRepository.GetByIdAsync(booking.UserId);

                // Determine ticket validity and status
                bool isValid = false;
                string ticketStatus = "";
                string invalidReason = "";
                string message = "";

                // Check payment status first
                if (payment.Status == Core.Enums.PaymentStatus.Rejected)
                {
                    ticketStatus = "PaymentRejected";
                    invalidReason = "Payment was rejected";
                    message = "Ticket is invalid - Payment was rejected";
                }
                else if (payment.Status == Core.Enums.PaymentStatus.Refunded)
                {
                    ticketStatus = "Refunded";
                    invalidReason = "Payment was refunded";
                    message = "Ticket is invalid - Payment was refunded";
                }
                else if (payment.Status == Core.Enums.PaymentStatus.Pending)
                {
                    ticketStatus = "PaymentPending";
                    invalidReason = "Payment is still pending";
                    message = "Ticket is invalid - Payment is pending";
                }
                // Check booking status
                else if (booking.Status == Core.Enums.BookingStatus.Cancelled)
                {
                    ticketStatus = "Cancelled";
                    invalidReason = "Booking was cancelled";
                    message = "Ticket is invalid - Booking was cancelled";
                }
                // Check if event has ended
                else if (eventEntity.EndDate < now)
                {
                    ticketStatus = "Expired";
                    invalidReason = $"Event ended on {eventEntity.EndDate:yyyy-MM-dd HH:mm}";
                    message = "Ticket has expired - Event has ended";
                }
                // Valid ticket: Payment is Paid and Booking is Booked
                else if (payment.Status == Core.Enums.PaymentStatus.Paid && 
                         booking.Status == Core.Enums.BookingStatus.Booked)
                {
                    isValid = true;
                    ticketStatus = "Valid";
                    message = "Ticket is valid";
                }
                else
                {
                    ticketStatus = "Invalid";
                    invalidReason = $"Payment status: {payment.Status}, Booking status: {booking.Status}";
                    message = "Ticket is invalid";
                }

                // Step 6: Create response
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
                    EventStatus = eventStatus,
                    TicketStatus = ticketStatus,
                    BookingStatus = booking.Status.ToString(),
                    PaymentStatus = payment.Status.ToString(),
                    InvalidReason = invalidReason
                };

                if (!isValid)
                {
                    return (false, message, response, new[] { invalidReason });
                }

                return (true, message, response, Array.Empty<string>());
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