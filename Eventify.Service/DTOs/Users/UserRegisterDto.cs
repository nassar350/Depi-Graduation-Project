using System.ComponentModel.DataAnnotations;

namespace Eventify.APIs.DTOs.Users
{
    public class UserRegisterDto
    {
        [Required]
        public string Name { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        public string Phone { get; set; }

    }

}
