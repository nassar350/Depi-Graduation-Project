namespace Eventify.Service.DTOs.Admin
{
    public class AdminStatisticsDto
    {
        public int TotalUsers { get; set; }
        public int TotalEvents { get; set; }
        public int TotalBookings { get; set; }
        public decimal TotalRevenue { get; set; }
        public int PendingPayments { get; set; }
        public int ActiveEvents { get; set; }
        public int TotalCategories { get; set; }
    }
}
