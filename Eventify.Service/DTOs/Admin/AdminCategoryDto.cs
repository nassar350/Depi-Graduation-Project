namespace Eventify.Service.DTOs.Admin
{
    public class AdminCategoryDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int Capacity { get; set; }
        public int Booked { get; set; }
        public int EventId { get; set; }
        public string EventName { get; set; }
    }
}
