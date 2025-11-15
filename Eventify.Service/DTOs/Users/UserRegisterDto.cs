using Eventify.Core.Enums;
using System.ComponentModel.DataAnnotations;

namespace Eventify.Service.DTOs.Users
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
        [Phone , MinLength(11) , MaxLength(11)]
        public string Phone { get; set; }

    }

}
