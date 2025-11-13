using AutoMapper;
using Eventify.Core.Entities;
using Eventify.Core.Enums;
using Eventify.Repository.Interfaces;
using Eventify.Service.DTOs.Payments;
using Eventify.Service.Interfaces;
using Microsoft.Extensions.Configuration;
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
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;

        public PaymentService(IPaymentRepository paymentRepository, IConfiguration configuration, IMapper mapper)
        {
            _paymentRepository = paymentRepository;
            _configuration = configuration;
            _mapper = mapper;

            StripeConfiguration.ApiKey = _configuration["Stripe:SecretKey"];
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

            await _paymentRepository.AddAsync(payment);
            await _paymentRepository.SaveChangesAsync();

            return _mapper.Map<PaymentDto>(payment);
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
            var payment = await _paymentRepository.GetByIdAsync(bookingId);
            if (payment == null || string.IsNullOrEmpty(payment.StripePaymentIntentId))
                return false;

            var refundService = new RefundService();
            var refundOptions = new RefundCreateOptions
            {
                PaymentIntent = payment.StripePaymentIntentId
            };

            try
            {
                await refundService.CreateAsync(refundOptions);

                payment.Status = PaymentStatus.Refunded;
                await _paymentRepository.UpdateAsync(payment);
                await _paymentRepository.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Refund failed: {ex.Message}");
                return false;
            }
        }

        public async Task UpdatePaymentStatusAsync(string paymentIntentId, PaymentStatus status)
        {
            if (string.IsNullOrEmpty(paymentIntentId)) return;

            // Match by StripePaymentIntentId
            var allPayments = await _paymentRepository.GetAllAsync();
            var payment = allPayments.FirstOrDefault(p => p.StripePaymentIntentId == paymentIntentId);

            if (payment != null)
            {
                payment.Status = status;
                await _paymentRepository.UpdateAsync(payment);
                await _paymentRepository.SaveChangesAsync();

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

            if (payment != null)
            {
                payment.Status = status;
                await _paymentRepository.UpdateAsync(payment);
                await _paymentRepository.SaveChangesAsync();

                Console.WriteLine($"💸 Payment refunded: {paymentIntentId}");
            }
            else
            {
                Console.WriteLine($"⚠️ No local payment found for refund (Intent: {paymentIntentId})");
            }
        }
    }
}
