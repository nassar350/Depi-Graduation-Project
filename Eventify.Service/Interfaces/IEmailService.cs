using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eventify.Service.Interfaces
{
    public interface IEmailService
    {
        Task SendEmailAsync(string toEmail, string subject, string content);
        Task SendEmailWithAttachmentAsync(string toEmail, string subject, string htmlContent, byte[] attachmentData, string attachmentFileName, string attachmentContentType);
        Task SendPaymentSuccessEmailWithTicketsAsync(int bookingId, string customerEmail, string customerName);
    }
}
