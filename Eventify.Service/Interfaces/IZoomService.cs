using Eventify.Service.DTOs.Zoom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eventify.Service.Interfaces
{
    public interface IZoomService
    {
    Task<ZoomMeetingResponse> CreateMeeting(string topic, DateTime startTime, int duration);
    
}
}
