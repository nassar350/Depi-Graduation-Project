using Eventify.Core.Enums;
using Eventify.Repository.Interfaces;
using Eventify.Service.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Threading.Tasks;

namespace Eventify.APIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StripeWebhookController : ControllerBase
    {
        private readonly IPaymentService _paymentService;
        private readonly INotificationService _notificationService;
        private readonly IConfiguration _configuration;
        private readonly ILogger<StripeWebhookController> _logger;

        public StripeWebhookController(
            IPaymentService paymentService,
            INotificationService notificationService,
            IConfiguration configuration,
            ILogger<StripeWebhookController> logger)
        {
            _paymentService = paymentService;
            _notificationService = notificationService;
            _configuration = configuration;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> HandleWebhook()
        {
            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
            var endpointSecret = _configuration["Stripe:WebhookSecret"];

            Event stripeEvent;

            try
            {
                var signatureHeader = Request.Headers["Stripe-Signature"];
                stripeEvent = EventUtility.ConstructEvent(json, signatureHeader, endpointSecret);
            }
            catch (StripeException ex)
            {
                _logger.LogError(ex, "Webhook signature verification failed");
                return BadRequest($"Webhook signature verification failed: {ex.Message}");
            }

            try
            {
                switch (stripeEvent.Type)
                {
                    case "payment_intent.succeeded":
                        var succeededIntent = stripeEvent.Data.Object as PaymentIntent;
                        await _paymentService.UpdatePaymentStatusAsync(succeededIntent.Id, PaymentStatus.Paid);
                        await _notificationService.SendPaymentSuccessAsyncByIntentId(succeededIntent.Id);
                        break;

                    case "payment_intent.payment_failed":
                        var failedIntent = stripeEvent.Data.Object as PaymentIntent;
                        await _paymentService.UpdatePaymentStatusAsync(failedIntent.Id, PaymentStatus.Rejected);
                        await _notificationService.SendPaymentFailureAsyncByIntentId(failedIntent.Id);
                        break;

                    case "payment_intent.canceled":
                        var canceledIntent = stripeEvent.Data.Object as PaymentIntent;
                        await _paymentService.UpdatePaymentStatusAsync(canceledIntent.Id, PaymentStatus.Cancelled);
                        break;

                    case "charge.refunded":
                        var charge = stripeEvent.Data.Object as Charge;
                        await _paymentService.UpdateRefundStatusAsync(charge.PaymentIntentId, PaymentStatus.Refunded);
                        await _notificationService.SendRefundNotificationAsyncByIntentId(charge.PaymentIntentId);
                        break;

                    default:
                        _logger.LogInformation($"Unhandled Stripe event type: {stripeEvent.Type}");
                        break;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing Stripe webhook");
                return StatusCode(500);
            }

            return Ok();
        }



        //        private readonly IPaymentRepository _paymentRepository;
        //        private readonly IConfiguration _configuration;

        //        public StripeWebhookController(IPaymentRepository paymentRepository, IConfiguration configuration)
        //        {
        //            _paymentRepository = paymentRepository;
        //            _configuration = configuration;
        //        }

        //        [HttpPost]
        //        public async Task<IActionResult> HandleWebhook()
        //        {
        //            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
        //            var endpointSecret = _configuration["Stripe:WebhookSecret"];

        //            Event stripeEvent;

        //            try
        //            {
        //                var signatureHeader = Request.Headers["Stripe-Signature"];
        //                stripeEvent = EventUtility.ConstructEvent(json, signatureHeader, endpointSecret);
        //            }
        //            catch (StripeException e)
        //            {
        //                return BadRequest($"⚠️ Webhook signature verification failed: {e.Message}");
        //            }

        //            // Handle specific Stripe events
        //            switch (stripeEvent.Type)
        //            {
        //                case "payment_intent.succeeded":
        //                    {
        //                        var intent = stripeEvent.Data.Object as PaymentIntent;
        //                        await UpdatePaymentStatus(intent, PaymentStatus.Paid);
        //                        break;
        //                    }

        //                case "payment_intent.payment_failed":
        //                    {
        //                        var intent = stripeEvent.Data.Object as PaymentIntent;
        //                        await UpdatePaymentStatus(intent, PaymentStatus.Rejected);
        //                        break;
        //                    }

        //                case "payment_intent.canceled":
        //                    {
        //                        var intent = stripeEvent.Data.Object as PaymentIntent;
        //                        await UpdatePaymentStatus(intent, PaymentStatus.Cancelled);
        //                        break;
        //                    }

        //                case "charge.refunded":
        //                {
        //                    var charge = stripeEvent.Data.Object as Charge;
        //                    await UpdateRefundStatus(charge.PaymentIntentId, PaymentStatus.Refunded);
        //                    break;
        //                }

        //                // Optional: Stripe sometimes sends this when refund is partial
        //                case "payment_intent.amount_refunded":
        //                {
        //                    var intent = stripeEvent.Data.Object as PaymentIntent;
        //                    await UpdatePaymentStatus(intent, PaymentStatus.Refunded);
        //                    break;
        //                }

        //                default:
        //                    // You can log unhandled event types for debugging
        //                    Console.WriteLine($"Unhandled event type: {stripeEvent.Type}");
        //                    break;
        //            }

        //            return Ok();
        //        }

        //        private async Task UpdatePaymentStatus(PaymentIntent intent, PaymentStatus status)
        //        {
        //            // Match by StripePaymentIntentId
        //            var allPayments = await _paymentRepository.GetAllAsync();
        //            var payment = allPayments.FirstOrDefault(p => p.StripePaymentIntentId == intent.Id);

        //            if (payment != null)
        //            {
        //                payment.Status = status;
        //                await _paymentRepository.UpdateAsync(payment);
        //                await _paymentRepository.SaveChangesAsync();

        //                Console.WriteLine($"✅ Payment with Intent {intent.Id} updated to {status}");
        //            }
        //            else
        //            {
        //                Console.WriteLine($"⚠️ No local payment found for Stripe PaymentIntent {intent.Id}");
        //            }
        //        }

        //        private async Task UpdateRefundStatus(string paymentIntentId, PaymentStatus status)
        //        {
        //            if (string.IsNullOrEmpty(paymentIntentId)) return;

        //            var payments = await _paymentRepository.GetAllAsync();
        //            var payment = payments.FirstOrDefault(p => p.StripePaymentIntentId == paymentIntentId);

        //            if (payment != null)
        //            {
        //                payment.Status = status;
        //                await _paymentRepository.UpdateAsync(payment);
        //                await _paymentRepository.SaveChangesAsync();

        //                Console.WriteLine($"💸 Payment refunded: {paymentIntentId}");
        //            }
        //            else
        //            {
        //                Console.WriteLine($"⚠️ No local payment found for refund (Intent: {paymentIntentId})");
        //            }
        //        }
    }
}
