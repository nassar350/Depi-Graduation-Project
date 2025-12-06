using Eventify.Core.Enums;

namespace Eventify.Core.Entities;

public class Event
{
    public int  Id  { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string Address { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public EventCategory EventCategory { get; set; }
    public int  OrganizerID { get; set; }  // Reference to PK in User
    public string? PhotoUrl { get; set; }
    public bool IsOnline { get; set; }
    public string? ZoomJoinUrl { get; set; }
    public string? ZoomPassword { get; set; }
    public string? ZoomMeetingId { get; set; }
    public ICollection<UserAttendEvent> EventsAttendedByUsers { get; set; }
        = new HashSet<UserAttendEvent>();
    public User Organizer { get; set; } 
    public ICollection<Category> Categories { get; set; }
        = new HashSet<Category>();  
    
    public ICollection<Ticket> Tickets { get; set; }
        = new HashSet<Ticket>(); 
}