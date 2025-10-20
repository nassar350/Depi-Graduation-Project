namespace Eventify.Core.Entities;

public class Event
{
    public int  Id  { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string Address { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int  OrganizerID { get; set; }  // Reference to PK in User
    public byte[] Photo { get; set; }
    public ICollection<UserAttendEvent> EventsAttendedByUsers { get; set; }
        = new HashSet<UserAttendEvent>();
    public User Organizer { get; set; } 
    public ICollection<Category> Categories { get; set; }
        = new HashSet<Category>();  
    
    public ICollection<Ticket> Tickets { get; set; }
        = new HashSet<Ticket>(); 
}