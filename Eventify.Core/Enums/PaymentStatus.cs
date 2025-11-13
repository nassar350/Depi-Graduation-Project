using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eventify.Core.Enums
{
    public enum PaymentStatus
    {
        Paid = 0,
        Pending = 1,
        Cancelled = 2,
        Rejected = 3,
        Refunded = 4
    }
}
