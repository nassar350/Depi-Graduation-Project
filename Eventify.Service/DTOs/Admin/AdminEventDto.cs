namespace Eventify.Service.DTOs.Admin
{
    public class AdminEventDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string OrganizerName { get; set; }
        public int OrganizerId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Address { get; set; }
        public int Capacity { get; set; }
        public int BookedTickets { get; set; }
        public decimal Revenue { get; set; }
        public string Status { get; set; }
        public string PhotoUrl { get; set; }
        public decimal Price { get; set; }
    }
}
