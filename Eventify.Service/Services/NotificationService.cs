using Eventify.Service.Interfaces;
using Microsoft.Extensions.Logging;


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
            // Send professional email with PDF tickets attached
            try
            {
                // Extract customer name from email or use a default
                var customerName = email.Split('@')[0];
                await _emailService.SendPaymentSuccessEmailWithTicketsAsync(bookingId, email, customerName);
                _logger.LogInformation($"Payment success email with tickets sent for booking {bookingId}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to send payment success email with tickets for booking {bookingId}");
                // Fallback to simple email
                await _emailService.SendEmailAsync(email, $"Payment Successful for Booking #{bookingId}", $"Your payment of ${amount} was successful!");
            }
            
            if (!string.IsNullOrEmpty(phoneNumber))
                await _smsService.SendSmsAsync(phoneNumber, $"Payment of ${amount} for Booking #{bookingId} successful!");
        }

        public async Task SendPaymentFailureAsync(int bookingId, string email, string phoneNumber = null)
        {
            try
            {
                var customerName = email.Split('@')[0];
                var emailHtml = GeneratePaymentFailureEmailTemplate(customerName, bookingId);
                await _emailService.SendEmailAsync(email, $"❌ Payment Failed - Booking #{bookingId}", emailHtml);
                _logger.LogInformation($"Payment failure email sent for booking {bookingId}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to send payment failure email for booking {bookingId}");
            }
            
            if (!string.IsNullOrEmpty(phoneNumber))
                await _smsService.SendSmsAsync(phoneNumber, $"Payment for Booking #{bookingId} failed.");
        }

        public async Task SendRefundNotificationAsync(int bookingId, decimal amount, string email, string phoneNumber = null)
        {
            try
            {
                var customerName = email.Split('@')[0];
                var emailHtml = GenerateRefundEmailTemplate(customerName, bookingId, amount);
                await _emailService.SendEmailAsync(email, $"💰 Refund Processed - Booking #{bookingId}", emailHtml);
                _logger.LogInformation($"Refund notification email sent for booking {bookingId}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to send refund notification email for booking {bookingId}");
            }
            
            if (!string.IsNullOrEmpty(phoneNumber))
                await _smsService.SendSmsAsync(phoneNumber, $"Refund of ${amount} for Booking #{bookingId} processed.");
        }

        private string GeneratePaymentFailureEmailTemplate(string customerName, int bookingId)
        {
            return $@"
<!DOCTYPE html>
<html>
<head>
    <style>
        body {{ font-family: Arial, sans-serif; background-color: #f4f7fa; margin: 0; padding: 20px; }}
        .container {{ max-width: 600px; margin: 0 auto; background: white; border-radius: 12px; overflow: hidden; box-shadow: 0 4px 12px rgba(0,0,0,0.1); }}
        .header {{ background: linear-gradient(135deg, #FF5C5C 0%, #E53F3F 100%); padding: 40px; text-align: center; color: white; }}
        .error-icon-wrapper {{ text-align: center; margin-bottom: 20px; }}
        .error-icon {{ width: 80px; height: 80px; background: #ff4757; border-radius: 50%; display: inline-block; line-height: 80px; text-align: center; font-size: 40px; color: white; }}
        .content {{ padding: 40px 30px; }}
        .greeting {{ font-size: 20px; color: #1e2329; margin-bottom: 15px; font-weight: 600; }}
        .message {{ font-size: 16px; color: #5a6b7c; line-height: 1.6; margin-bottom: 30px; }}
        .booking-info {{ background: #fff3cd; border-left: 4px solid #ffc107; padding: 20px; border-radius: 8px; margin-bottom: 30px; color: #856404; }}
        .next-steps {{ background: #f0f4ff; padding: 20px; border-radius: 8px; margin-bottom: 30px; }}
        .next-steps h3 {{ color: #7C5CFF; margin-bottom: 15px; }}
        .next-steps ul {{ padding-left: 20px; color: #5a6b7c; }}
        .next-steps li {{ margin-bottom: 10px; }}
        .support-box {{ background: #f8f9fa; border: 2px solid #7C5CFF; border-radius: 8px; padding: 20px; text-align: center; }}
        .support-box a {{ color: #7C5CFF; text-decoration: none; font-weight: bold; }}
        .footer {{ background: #1e2329; padding: 30px; text-align: center; color: #98a0ab; }}
    </style>
</head>
<body>
    <div class=""container"">
        <div class=""header"">
            <div class=""error-icon-wrapper"">
                <div class=""error-icon"">✕</div>
            </div>
            <h1>Payment Failed</h1>
            <p>We couldn't process your payment</p>
        </div>
        <div class=""content"">
            <div class=""greeting"">Hi {customerName},</div>
            <p class=""message"">
                We're sorry, but we were unable to process your payment for booking #{bookingId:D6}. 
                Your booking has not been confirmed at this time.
            </p>
            <div class=""booking-info"">
                <strong>⚠️ Booking Status:</strong> Payment Failed - Action Required
            </div>
            <div class=""next-steps"">
                <h3>What to do next:</h3>
                <ul>
                    <li>Check that your card details are correct</li>
                    <li>Ensure you have sufficient funds</li>
                    <li>Verify your card is authorized for online transactions</li>
                    <li>Try again with a different payment method</li>
                </ul>
            </div>
            <div class=""support-box"">
                <p><strong>Need Help?</strong></p>
                <p>If you continue to experience issues, please contact our support team.</p>
                <a href=""mailto:support@eventify.com"">support@eventify.com</a>
            </div>
        </div>
        <div class=""footer"">
            <p>© 2025 Eventify. All rights reserved.</p>
        </div>
    </div>
</body>
</html>";
        }

        private string GenerateRefundEmailTemplate(string customerName, int bookingId, decimal amount)
        {
            return $@"
<!DOCTYPE html>
<html>
<head>
    <style>
        body {{ font-family: Arial, sans-serif; background-color: #f4f7fa; margin: 0; padding: 20px; }}
        .container {{ max-width: 600px; margin: 0 auto; background: white; border-radius: 12px; overflow: hidden; box-shadow: 0 4px 12px rgba(0,0,0,0.1); }}
        .header {{ background: linear-gradient(135deg, #00d97a 0%, #00b562 100%); padding: 40px; text-align: center; color: white; }}
        .refund-icon-wrapper {{ text-align: center; margin-bottom: 20px; }}
        .refund-icon {{ width: 80px; height: 80px; background: #00d97a; border-radius: 50%; display: inline-block; line-height: 80px; text-align: center; font-size: 40px; color: white; }}
        .content {{ padding: 40px 30px; }}
        .greeting {{ font-size: 20px; color: #1e2329; margin-bottom: 15px; font-weight: 600; }}
        .message {{ font-size: 16px; color: #5a6b7c; line-height: 1.6; margin-bottom: 30px; }}
        .refund-details {{ background: #e8f5e9; border-left: 4px solid #00d97a; padding: 20px; border-radius: 8px; margin-bottom: 30px; }}
        .detail-row {{ display: flex; justify-content: space-between; padding: 10px 0; border-bottom: 1px solid #c8e6c9; }}
        .detail-row:last-child {{ border-bottom: none; }}
        .amount-highlight {{ font-size: 32px; color: #00d97a; font-weight: bold; text-align: center; margin: 20px 0; }}
        .info-box {{ background: #f0f4ff; padding: 20px; border-radius: 8px; margin-bottom: 30px; color: #5a6b7c; }}
        .info-box strong {{ color: #7C5CFF; }}
        .footer {{ background: #1e2329; padding: 30px; text-align: center; color: #98a0ab; }}
    </style>
</head>
<body>
    <div class=""container"">
        <div class=""header"">
            <div class=""refund-icon-wrapper"">
                <div class=""refund-icon"">💰</div>
            </div>
            <h1>Refund Processed</h1>
            <p>Your refund has been successfully processed</p>
        </div>
        <div class=""content"">
            <div class=""greeting"">Hi {customerName},</div>
            <p class=""message"">
                Your refund request for booking #{bookingId:D6} has been successfully processed. 
                The amount will be credited back to your original payment method.
            </p>
            <div class=""amount-highlight"">
                ${amount:F2}
            </div>
            <div class=""refund-details"">
                <h3>Refund Summary</h3>
                <div class=""detail-row"">
                    <span><strong>Booking ID:</strong></span>
                    <span>#{bookingId:D6}</span>
                </div>
                <div class=""detail-row"">
                    <span><strong>Refund Amount:</strong></span>
                    <span>${amount:F2}</span>
                </div>
                <div class=""detail-row"">
                    <span><strong>Status:</strong></span>
                    <span>Processed</span>
                </div>
                <div class=""detail-row"">
                    <span><strong>Date:</strong></span>
                    <span>{DateTime.UtcNow:MMMM dd, yyyy}</span>
                </div>
            </div>
            <div class=""info-box"">
                <strong>⏱️ Processing Time:</strong><br>
                Refunds typically appear in your account within 5-10 business days, depending on your bank or card provider.
            </div>
            <div class=""info-box"">
                <strong>📧 Questions?</strong><br>
                If you have any questions about this refund, please don't hesitate to contact our support team at support@eventify.com
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
