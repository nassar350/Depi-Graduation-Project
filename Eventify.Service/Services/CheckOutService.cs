using AutoMapper;
using Eventify.Core.Entities;
using Eventify.Core.Enums;
using Eventify.Repository.Interfaces;
using Eventify.Service.DTOs.Bookings;
using Eventify.Service.DTOs.Payments;
using Eventify.Service.Interfaces;
using Microsoft.Extensions.Logging;
using Stripe;
using System.Threading.Tasks;

namespace Eventify.Service.Services
{
    public class CheckOutService : ICheckOutService
    {

        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<CheckoutService> _logger;

        public CheckOutService(
            IBookingRepository bookingRepo,
            IPaymentRepository paymentRepo,
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<CheckoutService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<CheckoutResponseDto> CreateCheckoutAsync(CheckOutRequestDto dto)
        {
            // 1. Calculate total
            var total = dto.TotalPrice;
            var amountInCents = (long)(total * 100m);

            // 2. Create Stripe PaymentIntent
            var paymentIntentService = new PaymentIntentService();
            var piOptions = new PaymentIntentCreateOptions
            {
                Amount = amountInCents,
                Currency = dto.Currency ?? "usd",
                PaymentMethodTypes = new List<string> { "card" },
                Metadata = new Dictionary<string, string>
                {
                    { "EventId", dto.EventId.ToString() },
                    { "Email", dto.EmailAddress }
                }
            };

            PaymentIntent intent = null;

            try
            {
                intent = await paymentIntentService.CreateAsync(piOptions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create Stripe PaymentIntent");
                throw;
            }


            CheckoutResponseDto? response = null;
            using var transaction = await _unitOfWork.BeginTransactionAsync();
            try 
            {
                // 3. Create booking entity

                var user = await _unitOfWork._userRepository.GetUserByEmail(dto.EmailAddress);

                var booking = new Booking
                {
                    UserId = user.Id,
                    TicketsNum = dto.TicketsNum,
                    CategoryName = dto.CategoryName,
                    Status = TicketStatus.Pending,
                    CreatedDate = DateTime.UtcNow
                };

                var createdBooking = await _unitOfWork._bookingRepository.AddAsync(booking);
                await _unitOfWork.SaveChangesAsync();

                // 4. Create payment entity referencing booking and stripe intent
                var payment = new Payment
                {
                    BookingId = createdBooking.Id,
                    TotalPrice = total,
                    PaymentMethod = "Card",
                    StripePaymentIntentId = intent.Id,
                    Status = PaymentStatus.Pending,
                    DateTime = DateTime.UtcNow
                };

                var createdPayment = await _unitOfWork._paymentRepository.AddAsync(payment);

                // 5. Persist both in one transaction (via unit of work)
                await _unitOfWork.SaveChangesAsync();
                await transaction.CommitAsync();

                // 6. Prepare response with client_secret for frontend to confirm payment

                response = new CheckoutResponseDto
                {
                    BookingId = createdBooking.Id,
                    PaymentId = createdPayment.BookingId,
                    ClientSecret = intent.ClientSecret ?? string.Empty
                };

                return response;
            }
            catch (Exception dbEx)
            {
                _logger.LogError(dbEx, "Failed to save booking/payment to DB, attempting to cancel PaymentIntent");
                await transaction.RollbackAsync();

                // Attempt to cancel the payment intent to avoid orphaned Stripe intents
                try
                {
                    await paymentIntentService.CancelAsync(intent.Id);
                }
                catch (Exception cancelEx)
                {
                    _logger.LogError(cancelEx, "Failed to cancel PaymentIntent after DB save failure");
                }

                throw;
            }
        }
    }
}
