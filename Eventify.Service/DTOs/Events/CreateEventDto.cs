using System.ComponentModel.DataAnnotations;

namespace Eventify.APIs.DTOs.Events
{
    public class CreateEventDto
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public string Address { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        [Required]
        public int OrganizerID { get; set; }

        public byte[] Photo { get; set; }

        public List<int> CategoryIds { get; set; } = new();
    }

}
