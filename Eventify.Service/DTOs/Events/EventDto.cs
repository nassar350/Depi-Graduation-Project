namespace Eventify.APIs.DTOs.Events
{
    public class EventDto
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string Address { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public int OrganizerID { get; set; }

        public byte[] Photo { get; set; }
    }

}
