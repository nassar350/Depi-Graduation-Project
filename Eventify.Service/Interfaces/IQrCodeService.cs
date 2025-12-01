using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eventify.Service.Interfaces
{
    public interface IQrCodeService
    {
        byte[] GenerateQrCode(string text);
    }
}
