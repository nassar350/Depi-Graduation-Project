using System.ComponentModel.DataAnnotations;

namespace Eventify.Service.DTOs.Categories
{
    public class CreateCategoryDto
    {
        [Required]
        public int EventId { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public int Seats { get; set; }
        public CreateCategoryDto(int eventid , string title , int seats)
        {
            EventId = eventid;
            Title = title;
            Seats = seats;
        }
    }

}
