using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Eventify.Service.DTOs.Events
{
    public class CreateEventDto
    {
        [Required(ErrorMessage = "Event name is required")]
        [StringLength(100, MinimumLength = 3 ,ErrorMessage = "Name must be between 3 and 100 characters")]
        public string Name { get; set; }= string.Empty;

        [Required(ErrorMessage = "Description is required")]
        [StringLength(1000, MinimumLength = 10 ,ErrorMessage = "Description must be between 10 and 1000 characters")]
        public string Description { get; set; }= string.Empty;

        [Required(ErrorMessage = "Address is required")]
        [StringLength(200, MinimumLength = 5 ,ErrorMessage = "Address must be between 5 and 200 characters")]
        public string Address { get; set; }= string.Empty;

        [Required(ErrorMessage = "Start date is required")]
        public DateTime StartDate { get; set; }

        [Required(ErrorMessage = "End date is required")]
        public DateTime EndDate { get; set; }
        //remove orgainizer id as it will be taken from the logged in user
       
        public IFormFile? Photo { get; set; }

        public List<int> CategoryIds { get; set; } = new();
    }

}
