namespace Eventify.Service.DTOs.Categories
{
    public class CategoryDto
    {
        public int Id { get; set; }

        public int EventId { get; set; }

        public string Title { get; set; }= string.Empty;

        public int Seats { get; set; }

        public int Booked { get; set; }
    }

}
