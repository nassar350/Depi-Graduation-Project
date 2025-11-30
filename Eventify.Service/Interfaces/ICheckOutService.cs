using Eventify.Service.DTOs.Bookings;
using Eventify.Service.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eventify.Service.Interfaces
{
    public interface ICheckOutService
    {
        Task<ServiceResult<CheckoutResponseDto>> CreateCheckoutAsync(CheckOutRequestDto dto);
    }
}
