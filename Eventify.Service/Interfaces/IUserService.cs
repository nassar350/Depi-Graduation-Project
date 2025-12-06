using Eventify.Core.Entities;
using Eventify.Service.DTOs.Users;
using Eventify.Service.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eventify.Service.Interfaces
{
    public interface IUserService
    {
        Task<ServiceResult<IEnumerable<UserDto>>> GetAllAsync();
        Task<ServiceResult<UserDto>> GetByIdAsync(int id);
        Task<ServiceResult<bool>> DeleteAsync(int id);
        Task<ServiceResult<UserUpdateDto>> UpdateAsync(int id, UserUpdateDto user);
        
        
        int GetTicketsBookedCount(int UserId);
        ServiceResult<decimal> GetTotalRevenueById(int id);
    }
}
