using Eventify.Core.Enums;

namespace Eventify.Service.DTOs.Events
{
    public class EventDto
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public string Address { get; set; } = string.Empty;

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public EventCategory EventCategory { get; set; }

        public int OrganizerID { get; set; }

        public string OrganizerName { get; set; } = string.Empty;

        public string? PhotoUrl { get; set; }

        public string PhotoBase64 { get; set; } = string.Empty;

        public int AvailableTickets { get; set; }
        public int BookedTickets { get; set; }

        public bool IsUpcoming { get; set; }

        public string Status { get; set; } = string.Empty;
    }
}
