using Eventify.Service.DTOs.Bookings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eventify.Service.Interfaces
{
    public interface ICheckOutService
    {
        Task<CheckoutResponseDto> CreateCheckoutAsync(CheckOutRequestDto dto);
    }
}
