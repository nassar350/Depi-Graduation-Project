using Eventify.Service.DTOs.Payments;
using Eventify.Service.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Eventify.APIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;

        public PaymentController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll() =>
            Ok(await _paymentService.GetAllAsync());

        [HttpGet("{bookingId}")]
        public async Task<IActionResult> GetById(int bookingId)
        {
            var payment = await _paymentService.GetByIdAsync(bookingId);
            if (payment == null) return NotFound();
            return Ok(payment);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreatePaymentDto dto)
        {
            var created = await _paymentService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { bookingId = created.BookingId }, created);
        }

        [HttpPut("{bookingId}")]
        public async Task<IActionResult> Update(int bookingId, [FromBody] UpdatePaymentDto dto)
        {
            var updated = await _paymentService.UpdateAsync(bookingId, dto);
            if (updated == null) return NotFound();
            return Ok(updated);
        }

        [HttpDelete("{bookingId}")]
        public async Task<IActionResult> Delete(int bookingId)
        {
            var deleted = await _paymentService.DeleteAsync(bookingId);
            return deleted ? NoContent() : NotFound();
        }

        [HttpPost("{bookingId}/refund")]
        public async Task<IActionResult> Refund(int bookingId)
        {
            var success = await _paymentService.RefundAsync(bookingId);

            if (!success)
                return BadRequest("Refund failed or payment not found.");

            return Ok("Payment refunded successfully.");
        }
    }
}
