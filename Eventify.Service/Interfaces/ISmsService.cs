using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eventify.Service.Interfaces
{
    public interface ISmsService
    {
        Task SendSmsAsync(string toNumber, string message);
    }
}
