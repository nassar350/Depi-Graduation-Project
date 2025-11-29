using System.ComponentModel.DataAnnotations;

namespace Eventify.Service.DTOs.Categories
{
    public class UpdateCategoryDto
    {
        
        [StringLength(50, MinimumLength = 2)]
        public string? Title { get; set; }
        [Range(1, 10000, ErrorMessage = "Seats must be between 1 and 10000")]
        public int? Seats { get; set; }
        [Range(0, 10000, ErrorMessage = "Booked must be between 0 and 10000")]
        public int? Booked { get; set; }
    }

}
