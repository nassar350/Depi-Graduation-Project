using Eventify.Core.Enums;
using Eventify.Service.DTOs.Payments;
using Stripe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eventify.Service.Interfaces
{
    public interface IPaymentService
    {
        Task<IEnumerable<PaymentDto>> GetAllAsync();
        Task<PaymentDetailsDto?> GetByIdAsync(int bookingId);
        Task<PaymentDto> CreateAsync(CreatePaymentDto dto);
        Task<PaymentDto?> UpdateAsync(int bookingId, UpdatePaymentDto dto);
        Task<bool> DeleteAsync(int bookingId);
        Task<bool> RefundAsync(int bookingId);
        Task UpdatePaymentStatusAsync(string paymentIntentId, PaymentStatus status);
        Task UpdateRefundStatusAsync(string paymentIntentId, PaymentStatus status);
        Task<PaymentDetailsDto?> GetByIntentIdAsync(string paymentIntentId);
    }
}
