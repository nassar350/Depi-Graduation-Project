using Eventify.Repository.Interfaces;
using Eventify.Repository.Repositories;
using Eventify.Service.Interfaces;
using Eventify.Service.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Eventify.Service.DependencyInjection
{
    public static class ServiceRegistration
    {
        public static IServiceCollection AddServiceLayer(this IServiceCollection services)
        {
            // Unit of Work
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            // Users
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IUserService, UserService>();

            // Auth
            services.AddScoped<IAuthService, AuthService>();

            // Events
            services.AddScoped<IEventRepository, EventRepository>();
            services.AddScoped<IEventService, EventService>();

            // Categories
            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<ICategoryService, CategoryService>();

            // Payments
            services.AddScoped<IPaymentRepository, PaymentRepository>();
            services.AddScoped<IPaymentService, PaymentService>();

            // Notifications
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<ISmsService, SmsService>();
            services.AddScoped<INotificationService, NotificationService>();

            // Bookings
            services.AddScoped<IBookingRepository, BookingRepository>();
            services.AddScoped<IBookingService, BookingService>();

            // Checkout
            services.AddScoped<ICheckOutService, CheckOutService>();

            // Tickets
            services.AddScoped<ITicketRepository, TicketRepository>();
            services.AddScoped<ITicketService, TicketService>();
            services.AddScoped<IQrCodeService, QrCodeService>();
            services.AddScoped<ITicketPdfService, TicketPdfService>();
            services.AddScoped<ITicketVerificationService, TicketVerificationService>();
            services.AddScoped<ITicketDownloadService, TicketDownloadService>();

            return services;
        }
    }
}
