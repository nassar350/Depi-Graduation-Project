using Eventify.Core.Enums;
using System.ComponentModel.DataAnnotations;

namespace Eventify.Service.DTOs.Bookings
{
    public class CreateBookingDto
    {
        [Required(ErrorMessage = "You Must Enter Your First Name")]
        [MaxLength(60, ErrorMessage = "First Name Can not Exceed 60 Character")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "You Must Enter Your Last Name")]
        [MaxLength(60, ErrorMessage = "Last Name Can not Exceed 60 Character")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "You Must Enter Your Email Address")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        [StringLength(100, MinimumLength = 5, ErrorMessage = "Email must be between 5 and 100 characters")]
        public string EmailAddress { get; set; }

        public string? PhoneNumber { get; set; }

        [Required(ErrorMessage = "Number of tickets is required")]
        [Range(1, 100, ErrorMessage = "You can book between 1 and 100 tickets per request")]
        public int TicketsNum { get; set; }

        [Required(ErrorMessage = "Total Price is required")]
        public decimal TotalPrice { get; set; }

        [Required(ErrorMessage = "Category is Required")]
        public string CategoryName { get; set; }

        public string? PromoCode { get; set; }

        [Required(ErrorMessage = "Event ID is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Event ID must be a positive number")]
        public int EventId { get; set; }
       
        //[Required(ErrorMessage = "User ID is required")]
        [Range(1, int.MaxValue, ErrorMessage = "User ID must be a positive number")]
        public int? UserId { get; set; }

        public TicketStatus TicketStatus { get; set; } = TicketStatus.Pending;

    }

}
