using Eventify.Core.Enums;
using System.ComponentModel.DataAnnotations;

namespace Eventify.Service.DTOs.Users
{
    public class UserUpdateDto
    {
        public string Name { get; set; }

        [EmailAddress]
        public string Email { get; set; }

        public string PhoneNumber { get; set; }

    }

}
