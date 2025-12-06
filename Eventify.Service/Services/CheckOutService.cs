using AutoMapper;
using Eventify.Core.Entities;
using Eventify.Core.Enums;
using Eventify.Repository.Interfaces;
using Eventify.Service.DTOs.Bookings;
using Eventify.Service.DTOs.Payments;
using Eventify.Service.Helpers;
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

        public async Task<ServiceResult<CheckoutResponseDto>> CreateCheckoutAsync(CheckOutRequestDto dto)
        {
            int quantity = _unitOfWork._ticketRepository.CountNotBookedTickets(dto.EventId , dto.CategoryName);

            if (dto.TicketsNum > quantity) {
                return ServiceResult<CheckoutResponseDto>.Fail("TicketNum", "The number of tickets you are trying to book is not availabe");
            }

            // 1. Calculate total
            var total = dto.TotalPrice;
            var isFreeEvent = total == 0;
            var TicketsToBook = await _unitOfWork._ticketRepository.GetNotBookedTickets(dto.EventId, dto.CategoryName, dto.TicketsNum);
            
            // 2. Create Stripe PaymentIntent (skip for free events)
            PaymentIntent? intent = null;
            PaymentIntentService? paymentIntentService = null;

            if (!isFreeEvent)
            {
                var amountInCents = (long)(total * 100m);
                paymentIntentService = new PaymentIntentService();
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

                try
                {
                    intent = await paymentIntentService.CreateAsync(piOptions);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to create Stripe PaymentIntent");
                    throw;
                }
            }

            CheckoutResponseDto? response = null;
            using var transaction = await _unitOfWork.BeginTransactionAsync();
            try 
            {
                // 3. Create booking entity
                var user = await _unitOfWork._userRepository.GetUserByEmail(dto.EmailAddress);
                if (user == null)
                {
                    return ServiceResult<CheckoutResponseDto>.Fail("Email Address", "No such user with this email address");
                }

                var booking = new Booking
                {
                    UserId = user.Id,
                    TicketsNum = dto.TicketsNum,
                    CategoryName = dto.CategoryName,
                    Status = isFreeEvent ? BookingStatus.Booked : BookingStatus.Pending,
                    CreatedDate = DateTime.UtcNow,
                    EventId = dto.EventId
                };

                var createdBooking = await _unitOfWork._bookingRepository.AddAsync(booking);
                await _unitOfWork.SaveChangesAsync();

                // 4. Create payment entity (only for paid events)
                Payment? createdPayment = null;
                
                if (!isFreeEvent && intent != null)
                {
                    var payment = new Payment
                    {
                        BookingId = createdBooking.Id,
                        TotalPrice = total,
                        PaymentMethod = "Card",
                        StripePaymentIntentId = intent.Id,
                        Status = PaymentStatus.Pending,
                        DateTime = DateTime.UtcNow
                    };

                    createdPayment = await _unitOfWork._paymentRepository.AddAsync(payment);
                    await _unitOfWork.SaveChangesAsync();
                }

                // 5. Update tickets
                foreach (var ticket in TicketsToBook)
                {
                    ticket.BookingId = createdBooking.Id;
                }

                var category = await _unitOfWork._categoryRepository.GetByIdAsync(TicketsToBook.ElementAt(0).CategoryId);
                category.Booked += dto.TicketsNum;

                _unitOfWork._ticketRepository.UpdateRange(TicketsToBook);
                await _unitOfWork.SaveChangesAsync();
                await transaction.CommitAsync();

                // 6. Prepare response
                response = new CheckoutResponseDto
                {
                    BookingId = createdBooking.Id,
                    PaymentId = createdPayment?.BookingId ?? createdBooking.Id,
                    ClientSecret = intent?.ClientSecret ?? string.Empty
                };

                return ServiceResult<CheckoutResponseDto>.Ok(response);
            }
            catch (Exception dbEx)
            {
                _logger.LogError(dbEx, "Failed to save booking/payment to DB");
                await transaction.RollbackAsync();

                // Attempt to cancel the payment intent (only for paid events)
                if (!isFreeEvent && intent != null && paymentIntentService != null)
                {
                    try
                    {
                        await paymentIntentService.CancelAsync(intent.Id);
                    }
                    catch (Exception cancelEx)
                    {
                        _logger.LogError(cancelEx, "Failed to cancel PaymentIntent after DB save failure");
                    }
                }

                throw;
            }
        }
    }
}
