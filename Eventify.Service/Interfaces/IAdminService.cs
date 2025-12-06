using Eventify.Service.DTOs.Admin;
using Eventify.Service.Helpers;

namespace Eventify.Service.Interfaces
{
    public interface IAdminService
    {
        // User Management
        Task<ServiceResult<IEnumerable<AdminUserDto>>> GetAllUsersAsync(string searchTerm = null);
        Task<ServiceResult<bool>> UpdateUserAsync(int id, AdminUserDto dto);
        Task<ServiceResult<bool>> DeleteUserAsync(int id);

        // Event Management
        Task<ServiceResult<IEnumerable<AdminEventDto>>> GetAllEventsAsync(string searchTerm = null);
        Task<ServiceResult<bool>> DeleteEventAsync(int id);

        // Booking Management
        Task<ServiceResult<IEnumerable<AdminBookingDto>>> GetAllBookingsAsync(string searchTerm = null);
        Task<ServiceResult<bool>> CancelBookingAsync(int id);

        // Payment Management
        Task<ServiceResult<IEnumerable<AdminPaymentDto>>> GetAllPaymentsAsync(string searchTerm = null);

        // Event Category Management (Enum-based categories: Music, Sports, etc.)
        Task<ServiceResult<IEnumerable<AdminEventCategoryDto>>> GetAllEventCategoriesAsync();

        // Statistics
        Task<ServiceResult<AdminStatisticsDto>> GetDashboardStatisticsAsync();
    }
}
