using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eventify.Service.DTOs.Zoom
{
    public class ZoomMeetingResponse
    {
        public long Id { get; set; }
        public string JoinUrl { get; set; }
       public string StartUrl { get; set; }
       public string Password { get; set; }
       public string? MeetingId { get; internal set; }
    }
}
