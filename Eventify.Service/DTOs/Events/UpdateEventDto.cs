using Microsoft.AspNetCore.Http;

namespace Eventify.Service.DTOs.Events
{
    public class UpdateEventDto
    {
        public string? Name { get; set; }

        public string? Description { get; set; }

        public string? Address { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public IFormFile? Photo { get; set; }

        public List<int>? CategoryIds { get; set; }
    }
}
