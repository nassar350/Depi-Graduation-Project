using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eventify.Service.DTOs.Tickets
{
    public class TicketDownloadResponseDto
    {
        public byte[] PdfBytes { get; set; }
        public string FileName { get; set; }
        public int TicketCount { get; set; }
    }
}
