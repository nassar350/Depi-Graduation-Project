using AutoMapper;
using Eventify.Core.Entities;
using Eventify.Core.Enums;
using Eventify.Repository.Interfaces;
using Eventify.Service.DTOs.Payments;
using Eventify.Service.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Stripe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eventify.Service.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;
        private readonly ILogger<PaymentService> _logger;

        public PaymentService(IPaymentRepository paymentRepository, IUnitOfWork unitOfWork, IConfiguration configuration, IMapper mapper, ILogger<PaymentService> logger)
        {
            _paymentRepository = paymentRepository;
            _unitOfWork = unitOfWork;
            _configuration = configuration;
            _mapper = mapper;
            _logger = logger;

            var stripeSecretKey = Environment.GetEnvironmentVariable("SecretKey");

            if (string.IsNullOrEmpty(stripeSecretKey))
                throw new Exception("Stripe Secret Key not set!");

            StripeConfiguration.ApiKey = stripeSecretKey;

            //StripeConfiguration.ApiKey = _configuration["Stripe:SecretKey"];
        }

        public async Task<IEnumerable<PaymentDto>> GetAllAsync()
        {
            var payments = await _paymentRepository.GetAllAsync();

            return _mapper.Map<IEnumerable<PaymentDto>>(payments);
        }

        public async Task<PaymentDetailsDto?> GetByIdAsync(int bookingId)
        {
            var payment = await _paymentRepository.GetByIdAsync(bookingId);
            if (payment == null) return null;

            return _mapper.Map<PaymentDetailsDto>(payment);
        }

        public async Task<PaymentDto> CreateAsync(CreatePaymentDto dto)
        {
            // 1️⃣ Create Stripe Payment Intent
            var options = new PaymentIntentCreateOptions
            {
                Amount = (long)(dto.TotalPrice * 100), // Stripe uses cents
                Currency = "usd",
                PaymentMethodTypes = new List<string> { "card" },
                Metadata = new Dictionary<string, string>
                {
                    { "BookingId", dto.BookingId.ToString() }
                }
            };

            var service = new PaymentIntentService();
            var intent = await service.CreateAsync(options);

            // 2️⃣ Save payment to database
            var payment = _mapper.Map<Payment>(dto);
            payment.StripePaymentIntentId = intent.Id;
            payment.Status = PaymentStatus.Pending;

            await _paymentRepository.AddAsync(payment);
            await _paymentRepository.SaveChangesAsync();

            var response = _mapper.Map<PaymentDto>(payment);
            response.StripeClientSecret = intent.ClientSecret;

            return response;
        }

        public async Task<PaymentDto?> UpdateAsync(int bookingId, UpdatePaymentDto dto)
        {
            var payment = await _paymentRepository.GetByIdAsync(bookingId);
            if (payment == null) return null;

            if (dto.TotalPrice.HasValue) payment.TotalPrice = dto.TotalPrice.Value;
            if (!string.IsNullOrEmpty(dto.PaymentMethod)) payment.PaymentMethod = dto.PaymentMethod;
            if (dto.Status.HasValue) payment.Status = dto.Status.Value;
            if (dto.DateTime.HasValue) payment.DateTime = dto.DateTime.Value;

            await _paymentRepository.UpdateAsync(payment);
            await _paymentRepository.SaveChangesAsync();

            return _mapper.Map<PaymentDto>(payment);
        }

        public async Task<bool> DeleteAsync(int bookingId)
        {
            var payment = await _paymentRepository.GetByIdAsync(bookingId);
            if (payment == null) return false;

            await _paymentRepository.DeleteAsync(payment);
            await _paymentRepository.SaveChangesAsync();
            return true;
        }

        public async Task<PaymentDetailsDto?> GetByIntentIdAsync(string paymentIntentId)
        {
            var payment = await _paymentRepository.GetWithUserByIntentIdAsync(paymentIntentId);
            if (payment == null) return null;

            return _mapper.Map<PaymentDetailsDto>(payment);
        }

        public async Task<bool> RefundAsync(int bookingId)
        {
            try
            {
                _logger.LogInformation($"Starting refund process for BookingId: {bookingId}");

                // 1. Get payment and booking
                var payment = await _paymentRepository.GetByIdAsync(bookingId);
                if (payment == null)
                {
                    _logger.LogWarning($"Payment not found for BookingId: {bookingId}");
                    return false;
                }

                if (string.IsNullOrEmpty(payment.StripePaymentIntentId))
                {
                    _logger.LogWarning($"Missing Stripe PaymentIntent for BookingId: {bookingId}");
                    return false;
                }

                var booking = await _unitOfWork._bookingRepository.GetByIdAsync(bookingId);
                if (booking == null)
                {
                    _logger.LogWarning($"Booking not found for BookingId: {bookingId}");
                    return false;
                }

                // Check if already refunded
                if (payment.Status == PaymentStatus.Refunded)
                {
                    _logger.LogWarning($"Payment already refunded for BookingId: {bookingId}");
                    return false;
                }

                // Start transaction
                using var transaction = await _unitOfWork.BeginTransactionAsync();

                try
                {
                    // 2. Process Stripe refund
                    var refundService = new RefundService();
                    var refundOptions = new RefundCreateOptions
                    {
                        PaymentIntent = payment.StripePaymentIntentId,
                        Reason = RefundReasons.RequestedByCustomer
                    };

                    Refund refund;
                    try
                    {
                        refund = await refundService.CreateAsync(refundOptions);
                        _logger.LogInformation($"Stripe refund successful: {refund.Id} for PaymentIntent: {payment.StripePaymentIntentId}");
                    }
                    catch (StripeException stripeEx)
                    {
                        _logger.LogError(stripeEx, $"Stripe refund failed for PaymentIntent: {payment.StripePaymentIntentId}");
                        await transaction.RollbackAsync();
                        return false;
                    }

                    // 3. Update payment status
                    payment.Status = PaymentStatus.Refunded;
                    await _paymentRepository.UpdateAsync(payment);

                    // 4. Update booking status to Cancelled
                    booking.Status = BookingStatus.Cancelled;
                    await _unitOfWork._bookingRepository.UpdateAsync(bookingId, booking);

                    // 5. Release tickets - Remove BookingId from tickets
                    var allTickets = await _unitOfWork._ticketRepository.GetAllAsync();
                    var tickets = allTickets.Where(t => t.BookingId == bookingId).ToList();

                    if (tickets.Any())
                    {
                        foreach (var ticket in tickets)
                        {
                            ticket.BookingId = null; // Release the ticket back to available pool
                        }

                        // Use UpdateRange for efficiency
                        _unitOfWork._ticketRepository.UpdateRange(tickets);
                        _logger.LogInformation($"Released {tickets.Count} tickets for BookingId: {bookingId}");
                    }

                    var category = await _unitOfWork._categoryRepository.GetByIdAsync(tickets.ElementAt(0).CategoryId);

                    if (category is not null)
                        category.Booked -= tickets.Count;

                    // 6. Save all changes
                    await _unitOfWork.SaveChangesAsync();
                    await transaction.CommitAsync();

                    _logger.LogInformation($"✅ Complete refund successful for BookingId: {bookingId}");
                    return true;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Error during refund transaction for BookingId: {bookingId}");
                    await transaction.RollbackAsync();
                    throw;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Refund failed for BookingId: {bookingId}");
                return false;
            }
        }

        public async Task UpdatePaymentStatusAsync(string paymentIntentId, PaymentStatus status)
        {
            if (string.IsNullOrEmpty(paymentIntentId)) return;

            // Match by StripePaymentIntentId
            var allPayments = await _paymentRepository.GetAllAsync();
            var payment = allPayments.FirstOrDefault(p => p.StripePaymentIntentId == paymentIntentId);
            var booking = await _unitOfWork._bookingRepository.GetByIdAsync(payment.BookingId);

            if (payment != null)
            {
                payment.Status = status;

                switch (status)
                {
                    case PaymentStatus.Pending:
                        booking.Status = BookingStatus.Pending;
                        break;
                    case PaymentStatus.Paid:
                        booking.Status = BookingStatus.Booked;
                        break;
                    case PaymentStatus.Rejected:
                        booking.Status = BookingStatus.Cancelled;
                        break;
                    case PaymentStatus.Cancelled:
                        booking.Status = BookingStatus.Cancelled;
                        break;
                    case PaymentStatus.Refunded:
                        booking.Status = BookingStatus.Cancelled;
                        break;
                    default:
                        break;
                }

                await _unitOfWork._paymentRepository.UpdateAsync(payment);
                await _unitOfWork.SaveChangesAsync();

                Console.WriteLine($"✅ Payment with Intent {paymentIntentId} updated to {status}");
            }
            else
            {
                Console.WriteLine($"⚠️ No local payment found for Stripe PaymentIntent {paymentIntentId}");
            }
        }

        public async Task UpdateRefundStatusAsync(string paymentIntentId, PaymentStatus status)
        {
            if (string.IsNullOrEmpty(paymentIntentId)) return;

            var payments = await _paymentRepository.GetAllAsync();
            var payment = payments.FirstOrDefault(p => p.StripePaymentIntentId == paymentIntentId);
            var booking = await _unitOfWork._bookingRepository.GetByIdAsync(payment.BookingId);

            if (payment != null)
            {
                payment.Status = status;

                switch (status)
                {
                    case PaymentStatus.Pending:
                        booking.Status = BookingStatus.Pending;
                        break;
                    case PaymentStatus.Paid:
                        booking.Status = BookingStatus.Booked;
                        break;
                    case PaymentStatus.Rejected:
                        booking.Status = BookingStatus.Cancelled;
                        break;
                    case PaymentStatus.Cancelled:
                        booking.Status = BookingStatus.Cancelled;
                        break;
                    case PaymentStatus.Refunded:
                        booking.Status = BookingStatus.Cancelled;
                        break;
                    default:
                        break;
                }

                await _unitOfWork._paymentRepository.UpdateAsync(payment);
                await _unitOfWork.SaveChangesAsync();

                Console.WriteLine($"💸 Payment refunded: {paymentIntentId}");
            }
            else
            {
                Console.WriteLine($"⚠️ No local payment found for refund (Intent: {paymentIntentId})");
            }
        }
    }
}
