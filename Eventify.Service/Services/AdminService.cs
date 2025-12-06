using AutoMapper;
using Eventify.Core.Entities;
using Eventify.Core.Enums;
using Eventify.Repository.Interfaces;
using Eventify.Service.DTOs.Admin;
using Eventify.Service.Helpers;
using Eventify.Service.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Eventify.Service.Services
{
    public class AdminService : IAdminService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public AdminService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        #region User Management

        public async Task<ServiceResult<IEnumerable<AdminUserDto>>> GetAllUsersAsync(string searchTerm = null)
        {
            try
            {
                var users = await _unitOfWork._userRepository.GetAllAsync();

                if (!string.IsNullOrEmpty(searchTerm))
                {
                    searchTerm = searchTerm.ToLower();
                    users = users.Where(u =>
                        u.Name.ToLower().Contains(searchTerm) ||
                        u.Email.ToLower().Contains(searchTerm) ||
                        u.PhoneNumber.Contains(searchTerm)
                    ).ToList();
                }

                var userDtos = new List<AdminUserDto>();
                var allEvents = await _unitOfWork._eventRepository.GetAllAsync();

                foreach (var user in users)
                {
                    var eventsCreated = allEvents.Where(e => e.OrganizerID == user.Id).ToList();
                    var bookings = await _unitOfWork._bookingRepository.GetDetailedByUserId(user.Id);

                    var userDto = _mapper.Map<AdminUserDto>(user);
                    userDto.EventsCreated = eventsCreated?.Count ?? 0;
                    userDto.BookingsCount = bookings?.Count() ?? 0;

                    userDtos.Add(userDto);
                }

                return ServiceResult<IEnumerable<AdminUserDto>>.Ok(userDtos);
            }
            catch (Exception ex)
            {
                return ServiceResult<IEnumerable<AdminUserDto>>.Fail("Error", $"Error retrieving users: {ex.Message}");
            }
        }

        public async Task<ServiceResult<bool>> UpdateUserAsync(int id, AdminUserDto dto)
        {
            try
            {
                var user = await _unitOfWork._userRepository.GetByIdAsync(id);
                if (user == null)
                {
                    return ServiceResult<bool>.Fail("NotFound", "User not found");
                }

                user.Name = dto.Name;
                user.Email = dto.Email;
                user.PhoneNumber = dto.PhoneNumber;
                user.Role = dto.Role;

                await _unitOfWork._userRepository.UpdateAsync(id, user);
                await _unitOfWork.SaveChangesAsync();

                return ServiceResult<bool>.Ok(true);
            }
            catch (Exception ex)
            {
                return ServiceResult<bool>.Fail("Error", $"Error updating user: {ex.Message}");
            }
        }

        public async Task<ServiceResult<bool>> DeleteUserAsync(int id)
        {
            try
            {
                var user = await _unitOfWork._userRepository.GetByIdAsync(id);
                if (user == null)
                {
                    return ServiceResult<bool>.Fail("NotFound", "User not found");
                }

                await _unitOfWork._userRepository.DeleteAsync(id);
                await _unitOfWork.SaveChangesAsync();

                return ServiceResult<bool>.Ok(true);
            }
            catch (Exception ex)
            {
                return ServiceResult<bool>.Fail("Error", $"Error deleting user: {ex.Message}");
            }
        }

        #endregion

        #region Event Management

        public async Task<ServiceResult<IEnumerable<AdminEventDto>>> GetAllEventsAsync(string searchTerm = null)
        {
            try
            {
                var events = await _unitOfWork._eventRepository.GetAllAsync();

                if (!string.IsNullOrEmpty(searchTerm))
                {
                    searchTerm = searchTerm.ToLower();
                    events = events.Where(e =>
                        e.Name.ToLower().Contains(searchTerm) ||
                        e.Address.ToLower().Contains(searchTerm)
                    ).ToList();
                }

                var eventDtos = new List<AdminEventDto>();
                var allBookings = new List<Booking>();
                var users = await _unitOfWork._userRepository.GetAllAsync();
                
                // Get all bookings from all users
                foreach (var user in users)
                {
                    var userBookings = await _unitOfWork._bookingRepository.GetDetailedByUserId(user.Id);
                    if (userBookings != null && userBookings.Any())
                    {
                        allBookings.AddRange(userBookings);
                    }
                }

                var allPayments = await _unitOfWork._paymentRepository.GetAllAsync();

                foreach (var evt in events)
                {
                    var organizer = await _unitOfWork._userRepository.GetByIdAsync(evt.OrganizerID);
                    
                    // Get bookings for this specific event
                    var eventBookings = allBookings.Where(b => b.EventId == evt.Id).ToList();
                    var bookedTickets = eventBookings.Sum(b => b.TicketsNum);
                    
                    // Calculate revenue from paid payments for this event
                    var bookingIds = eventBookings.Select(b => b.Id).ToList();
                    var revenue = allPayments
                        .Where(p => bookingIds.Contains(p.BookingId) && p.Status == PaymentStatus.Paid)
                        .Sum(p => p.TotalPrice);

                    var eventDto = _mapper.Map<AdminEventDto>(evt);
                    eventDto.OrganizerName = organizer?.Name ?? "Unknown";
                    eventDto.BookedTickets = bookedTickets;
                    eventDto.Revenue = revenue;
                    eventDto.Status = evt.StartDate < DateTime.UtcNow ? "Past" : "Upcoming";

                    eventDtos.Add(eventDto);
                }

                return ServiceResult<IEnumerable<AdminEventDto>>.Ok(eventDtos);
            }
            catch (Exception ex)
            {
                return ServiceResult<IEnumerable<AdminEventDto>>.Fail("Error", $"Error retrieving events: {ex.Message}");
            }
        }

        public async Task<ServiceResult<bool>> DeleteEventAsync(int id)
        {
            try
            {
                var evt = await _unitOfWork._eventRepository.GetByIdAsync(id);
                if (evt == null)
                {
                    return ServiceResult<bool>.Fail("NotFound", "Event not found");
                }

                _unitOfWork._eventRepository.Delete(evt);
                await _unitOfWork.SaveChangesAsync();

                return ServiceResult<bool>.Ok(true);
            }
            catch (Exception ex)
            {
                return ServiceResult<bool>.Fail("Error", $"Error deleting event: {ex.Message}");
            }
        }

        #endregion

        #region Booking Management

        public async Task<ServiceResult<IEnumerable<AdminBookingDto>>> GetAllBookingsAsync(string searchTerm = null)
        {
            try
            {
                var allBookings = new List<Booking>();
                var users = await _unitOfWork._userRepository.GetAllAsync();

                foreach (var user in users)
                {
                    var userBookings = await _unitOfWork._bookingRepository.GetDetailedByUserId(user.Id);
                    if (userBookings != null && userBookings.Any())
                    {
                        allBookings.AddRange(userBookings);
                    }
                }

                if (!string.IsNullOrEmpty(searchTerm))
                {
                    searchTerm = searchTerm.ToLower();
                    allBookings = allBookings.Where(b =>
                        (b.User?.Name?.ToLower().Contains(searchTerm) ?? false) ||
                        (b.User?.Email?.ToLower().Contains(searchTerm) ?? false) ||
                        (b.Event?.Name?.ToLower().Contains(searchTerm) ?? false)
                    ).ToList();
                }

                var bookingDtos = new List<AdminBookingDto>();

                foreach (var booking in allBookings)
                {
                    var bookingDto = _mapper.Map<AdminBookingDto>(booking);
                    bookingDto.UserName = booking.User?.Name ?? "Unknown";
                    bookingDto.UserEmail = booking.User?.Email ?? "Unknown";
                    bookingDto.EventName = booking.Event?.Name ?? "Unknown";
                    bookingDto.TotalPrice = booking.Payment?.TotalPrice ?? 0;
                    bookingDto.EventPhotoUrl = booking.Event?.PhotoUrl ?? "";
                    bookingDto.EventAddress = booking.Event?.Address ?? "N/A";
                    bookingDto.EventStartDate = booking.Event?.StartDate ?? DateTime.MinValue;

                    bookingDtos.Add(bookingDto);
                }

                return ServiceResult<IEnumerable<AdminBookingDto>>.Ok(bookingDtos);
            }
            catch (Exception ex)
            {
                return ServiceResult<IEnumerable<AdminBookingDto>>.Fail("Error", $"Error retrieving bookings: {ex.Message}");
            }
        }

        public async Task<ServiceResult<bool>> CancelBookingAsync(int id)
        {
            try
            {
                var booking = await _unitOfWork._bookingRepository.GetByIdAsync(id);
                if (booking == null)
                {
                    return ServiceResult<bool>.Fail("NotFound", "Booking not found");
                }

                booking.Status = BookingStatus.Cancelled;
                await _unitOfWork._bookingRepository.UpdateAsync(id, booking);
                await _unitOfWork.SaveChangesAsync();

                return ServiceResult<bool>.Ok(true);
            }
            catch (Exception ex)
            {
                return ServiceResult<bool>.Fail("Error", $"Error cancelling booking: {ex.Message}");
            }
        }

        #endregion

        #region Payment Management

        public async Task<ServiceResult<IEnumerable<AdminPaymentDto>>> GetAllPaymentsAsync(string searchTerm = null)
        {
            try
            {
                var payments = await _unitOfWork._paymentRepository.GetAllAsync();

                if (payments == null)
                {
                    return ServiceResult<IEnumerable<AdminPaymentDto>>.Ok(new List<AdminPaymentDto>());
                }

                var paymentDtos = new List<AdminPaymentDto>();

                foreach (var payment in payments)
                {
                    try
                    {
                        var booking = await _unitOfWork._bookingRepository.GetByIdAsync(payment.BookingId);
                        var user = booking != null ? await _unitOfWork._userRepository.GetByIdAsync(booking.UserId) : null;
                        var evt = booking != null ? await _unitOfWork._eventRepository.GetByIdAsync(booking.EventId) : null;

                        if (!string.IsNullOrEmpty(searchTerm))
                        {
                            searchTerm = searchTerm.ToLower();
                            if (!(user?.Name?.ToLower().Contains(searchTerm) ?? false) &&
                                !(evt?.Name?.ToLower().Contains(searchTerm) ?? false))
                            {
                                continue;
                            }
                        }

                        var paymentDto = _mapper.Map<AdminPaymentDto>(payment);
                        paymentDto.UserName = user?.Name ?? "Unknown";
                        paymentDto.EventName = evt?.Name ?? "Unknown";

                        paymentDtos.Add(paymentDto);
                    }
                    catch (Exception paymentEx)
                    {
                        // Log individual payment error but continue processing others
                        Console.WriteLine($"Error processing payment {payment.BookingId}: {paymentEx.Message}");
                        continue;
                    }
                }

                return ServiceResult<IEnumerable<AdminPaymentDto>>.Ok(paymentDtos);
            }
            catch (Exception ex)
            {
                return ServiceResult<IEnumerable<AdminPaymentDto>>.Fail("PaymentError", $"Error retrieving payments: {ex.Message}. Stack: {ex.StackTrace}");
            }
        }

        #endregion

        #region Event Category Management

        public async Task<ServiceResult<IEnumerable<AdminEventCategoryDto>>> GetAllEventCategoriesAsync()
        {
            try
            {
                var events = await _unitOfWork._eventRepository.GetAllAsync();
                
                // Get all enum values
                var eventCategories = new List<AdminEventCategoryDto>();
                
                foreach (EventCategory category in Enum.GetValues(typeof(EventCategory)))
                {
                    var eventCount = events.Count(e => e.EventCategory == category);
                    
                    eventCategories.Add(new AdminEventCategoryDto
                    {
                        Value = (int)category,
                        Name = category.ToString(),
                        EventCount = eventCount
                    });
                }

                return ServiceResult<IEnumerable<AdminEventCategoryDto>>.Ok(eventCategories);
            }
            catch (Exception ex)
            {
                return ServiceResult<IEnumerable<AdminEventCategoryDto>>.Fail("Error", $"Error retrieving event categories: {ex.Message}");
            }
        }

        #endregion

        #region Statistics

        public async Task<ServiceResult<AdminStatisticsDto>> GetDashboardStatisticsAsync()
        {
            try
            {
                var users = await _unitOfWork._userRepository.GetAllAsync();
                var events = await _unitOfWork._eventRepository.GetAllAsync();
                var payments = await _unitOfWork._paymentRepository.GetAllAsync();

                // Get all bookings
                var allBookings = new List<Booking>();
                foreach (var user in users)
                {
                    var userBookings = await _unitOfWork._bookingRepository.GetDetailedByUserId(user.Id);
                    if (userBookings != null && userBookings.Any())
                    {
                        allBookings.AddRange(userBookings);
                    }
                }

                // Count total EventCategory enum values (Music, Sports, Arts, Food, Technology, Business, Education, Entertainment, Health, Community, Other = 11 total)
                var totalCategories = Enum.GetValues(typeof(Core.Enums.EventCategory)).Length;

                var statistics = new AdminStatisticsDto
                {
                    TotalUsers = users.Count(),
                    TotalEvents = events.Count(),
                    TotalBookings = allBookings.Count,
                    TotalRevenue = payments.Where(p => p.Status == PaymentStatus.Paid).Sum(p => p.TotalPrice),
                    PendingPayments = payments.Count(p => p.Status == PaymentStatus.Pending),
                    ActiveEvents = events.Count(e => e.StartDate >= DateTime.UtcNow),
                    TotalCategories = totalCategories
                };

                return ServiceResult<AdminStatisticsDto>.Ok(statistics);
            }
            catch (Exception ex)
            {
                return ServiceResult<AdminStatisticsDto>.Fail("Error", $"Error retrieving statistics: {ex.Message}");
            }
        }

        #endregion
    }
}
