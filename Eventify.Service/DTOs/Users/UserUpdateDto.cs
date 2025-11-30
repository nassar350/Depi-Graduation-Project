using Eventify.Core.Enums;
using System.ComponentModel.DataAnnotations;

namespace Eventify.Service.DTOs.Users
{
    public class UserUpdateDto
    {
        
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Name must be between 2 and 50 characters")]
        [RegularExpression(
           @"^(?!.*\s{2})[\p{L}\s]+(?<!\s)$",
           ErrorMessage = "Name can only contain letters and single spaces, and cannot start or end with a space")]
        public string Name { get; set; } = string.Empty;

        
        [EmailAddress(ErrorMessage = "Invalid email format")]
        [StringLength(100, MinimumLength = 5, ErrorMessage = "Email must be between 5 and 100 characters")]
        public string Email { get; set; } = string.Empty;

       
        [RegularExpression(@"^(010|011|012|015)\d{8}$",
    ErrorMessage = "Phone number must be a valid Egyptian mobile number (11 digits starting with 010, 011, 012, or 015)")]
        public string PhoneNumber { get; set; } = string.Empty;

    }

}
