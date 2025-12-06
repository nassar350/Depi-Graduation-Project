using Eventify.Core.Enums;

namespace Eventify.Service.DTOs.Admin
{
    public class AdminBookingDto
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string UserEmail { get; set; }
        public int UserId { get; set; }
        public string EventName { get; set; }
        public int EventId { get; set; }
        public int TicketsNum { get; set; }
        public decimal TotalPrice { get; set; }
        public BookingStatus Status { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CategoryName { get; set; }
        public string EventPhotoUrl { get; set; }
        public string EventAddress { get; set; }
        public DateTime EventStartDate { get; set; }
    }
}
