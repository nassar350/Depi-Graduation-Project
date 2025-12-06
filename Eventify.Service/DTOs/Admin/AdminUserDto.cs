using Eventify.Core.Enums;

namespace Eventify.Service.DTOs.Admin
{
    public class AdminUserDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public Role Role { get; set; }
        public DateTime CreatedDate { get; set; }
        public int EventsCreated { get; set; }
        public int BookingsCount { get; set; }
    }
}
