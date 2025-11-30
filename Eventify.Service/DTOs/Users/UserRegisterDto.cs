using Eventify.Core.Enums;
using System.ComponentModel.DataAnnotations;

namespace Eventify.Service.DTOs.Users
{
    public class UserRegisterDto
    {
        [Required(ErrorMessage = "Name is required")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Name must be between 2 and 50 characters")]
        [RegularExpression(
            @"^(?!.*\s{2})[\p{L}\s]+(?<!\s)$",
            ErrorMessage = "Name can only contain letters and single spaces, and cannot start or end with a space")]
        public string Name { get; set; }=string.Empty;

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        [StringLength(100, MinimumLength = 5, ErrorMessage = "Email must be between 5 and 100 characters")]
        public string Email { get; set; }=string.Empty;

        [Required(ErrorMessage = "Password is required")]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters")]
        [MaxLength(50, ErrorMessage = "Password cannot exceed 50 characters")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{6,}$",
    ErrorMessage = "Password must contain at least one lowercase letter, one uppercase letter, and one digit")]
        public string Password { get; set; }=string.Empty;
     
        [Required(ErrorMessage = "Phone number is required")]
        [RegularExpression(@"^(010|011|012|015)\d{8}$",
    ErrorMessage = "Phone number must be a valid Egyptian mobile number (11 digits starting with 010, 011, 012, or 015)")]
        public string Phone { get; set; }=string.Empty;

    }

}
