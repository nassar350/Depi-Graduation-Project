using Eventify.Service.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc; 
using Eventify.Service.DTOs.Bookings;






namespace Eventify.APIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingController : ControllerBase
    {
        private readonly IBookingService _service;

        public BookingController(IBookingService service)
        {
            _service = service;
        }
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var bookings = await _service.GetAllAsync();
            if (bookings.Count() == 0) return NotFound($"There is No Booking");
            return Ok(bookings);
        }
        
        
        
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var booking = await _service.GetByIdAsync(id);
            if (booking == null) return NotFound($"Booking with id {id} not found");
            return Ok(booking);
        }
        
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateBookingDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var created = await _service.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }
        
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateBookingDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var success = await _service.UpdateAsync(id, dto);
            if (!success) return NotFound($"Booking with id {id} not found");

            return NoContent();
        }
        
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _service.DeleteAsync(id);
            if (!success) return NotFound($"Booking with id {id} not found");

            return NoContent();
        }
        
        
    }
}
