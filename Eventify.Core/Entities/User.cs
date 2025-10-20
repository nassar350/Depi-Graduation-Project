using Microsoft.AspNetCore.Identity;

using Eventify.Core.Enums;

namespace Eventify.Core.Entities;


public class User : IdentityUser<int>
{
    public string Name { get; set; }
    public Role Role { get; set; }

    public ICollection<UserAttendEvent> UserAttendEvents { get; set; } = new HashSet<UserAttendEvent>();
    public ICollection<Event> Events { get; set; } = new HashSet<Event>();
    public ICollection<Booking> Bookings { get; set; } = new HashSet<Booking>();
}
