using System.ComponentModel.DataAnnotations;

namespace Eventify.Service.DTOs.Categories
{
    public class CreateCategoryDto
    {    //seeeeeeee
        [Required(ErrorMessage = "Event ID is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Event ID must be a positive number")]
        public int EventId { get; set; }

        [Required(ErrorMessage = "Category title is required")]
        [StringLength(50, MinimumLength = 2)]
        public string Title { get; set; }= string.Empty;
        [Required(ErrorMessage = "Total seats are required")]
        [Range(1, 10000, ErrorMessage = "Seats must be between 1 and 10000")]
        public int Seats { get; set; }
        public CreateCategoryDto(int eventid , string title , int seats)
        {
            EventId = eventid;
            Title = title;
            Seats = seats;
        }
    }

}
