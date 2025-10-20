namespace Eventify.Core.Entities;

public class UserAttendEvent
{
    public int  UserId { get; set; }
    public int  Event_Id { get; set; }
    public User User { get; set; }
    public Event Event { get; set; }
}