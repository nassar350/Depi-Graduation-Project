namespace Eventify.Core.Entities;
public class Category
{
    public int  Id { get; set; }
    public int EventId { get; set; }
    public string Title { get; set; }
    public int Seats { get; set; }
    public int  Booked { get; set; } 
    public Event Event { get; set; }
    public ICollection<Ticket> Tickets { get; set; }
        = new HashSet<Ticket>();
    
}