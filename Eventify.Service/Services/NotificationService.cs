using Eventify.Service.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SendGrid.Helpers.Mail;
using SendGrid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace Eventify.Service.Services
{
    public class NotificationService : INotificationService
    {
        private readonly IEmailService _emailService;
        private readonly ISmsService _smsService;
        private readonly IPaymentService _paymentService;
        private readonly ILogger<NotificationService> _logger;

        public NotificationService(
            IEmailService emailService,
            ISmsService smsService,
            IPaymentService paymentService,
            ILogger<NotificationService> logger)
        {
            _emailService = emailService;
            _smsService = smsService;
            _paymentService = paymentService;
            _logger = logger;
        }

        public async Task SendPaymentSuccessAsyncByIntentId(string paymentIntentId)
        {
            var payment = await _paymentService.GetByIntentIdAsync(paymentIntentId);
            if (payment == null)
            {
                _logger.LogWarning($"No payment found for PaymentIntentId: {paymentIntentId}");
                return;
            }

            await SendPaymentSuccessAsync(payment.BookingId, payment.TotalPrice, payment.CustomerEmail, payment.CustomerPhone);
        }

        public async Task SendPaymentFailureAsyncByIntentId(string paymentIntentId)
        {
            var payment = await _paymentService.GetByIntentIdAsync(paymentIntentId);
            if (payment == null)
            {
                _logger.LogWarning($"No payment found for PaymentIntentId: {paymentIntentId}");
                return;
            }

            await SendPaymentFailureAsync(payment.BookingId, payment.CustomerEmail, payment.CustomerPhone);
        }

        public async Task SendRefundNotificationAsyncByIntentId(string paymentIntentId)
        {
            var payment = await _paymentService.GetByIntentIdAsync(paymentIntentId);
            if (payment == null)
            {
                _logger.LogWarning($"No payment found for PaymentIntentId: {paymentIntentId}");
                return;
            }

            await SendRefundNotificationAsync(payment.BookingId, payment.TotalPrice, payment.CustomerEmail, payment.CustomerPhone);
        }

    public async Task SendPaymentSuccessAsync(int bookingId, decimal amount, string email, string phoneNumber = null)
        {
            await _emailService.SendEmailAsync(email, $"Payment Successful for Booking #{bookingId}", $"Your payment of ${amount} was successful!");
            if (!string.IsNullOrEmpty(phoneNumber))
                await _smsService.SendSmsAsync(phoneNumber, $"Payment of ${amount} for Booking #{bookingId} successful!");
        }

        public async Task SendPaymentFailureAsync(int bookingId, string email, string phoneNumber = null)
        {
            await _emailService.SendEmailAsync(email, $"Payment Failed for Booking #{bookingId}", $"Your payment could not be processed.");
            if (!string.IsNullOrEmpty(phoneNumber))
                await _smsService.SendSmsAsync(phoneNumber, $"Payment for Booking #{bookingId} failed.");
        }

        public async Task SendRefundNotificationAsync(int bookingId, decimal amount, string email, string phoneNumber = null)
        {
            await _emailService.SendEmailAsync(email, $"Refund Processed for Booking #{bookingId}", $"A refund of ${amount} has been processed.");
            if (!string.IsNullOrEmpty(phoneNumber))
                await _smsService.SendSmsAsync(phoneNumber, $"Refund of ${amount} for Booking #{bookingId} processed.");
        }
    }
}
