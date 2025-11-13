using Eventify.Core.Enums;
using System.ComponentModel.DataAnnotations;

namespace Eventify.Service.DTOs.Users
{
    public class UserUpdateDto
    {
        public string? Name { get; set; }

        [EmailAddress]
        public string? Email { get; set; }

        public string? Phone { get; set; }

        public Role? Role { get; set; }

    }

}
