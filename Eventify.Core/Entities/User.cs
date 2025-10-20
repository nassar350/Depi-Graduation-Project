using Eventify.Core.Enums;

namespace Eventify.Core.Entities;


public class User
{
    public int  Id  { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public string Phone { get; set; }
    public Role Role { get; set; }
    public bool IsActive { get; set; }
    public ICollection<UserAttendEvent> UserAttendEvents { get; set; }
        = new HashSet<UserAttendEvent>();
    public ICollection<Event> Events { get; set; }
        = new HashSet<Event>();
    public ICollection<Booking> Bookings { get; set; }
        = new HashSet<Booking>();
}
