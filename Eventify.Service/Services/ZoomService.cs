using Eventify.Service.DTOs.Zoom;
using Eventify.Service.Helpers;
using Eventify.Service.Interfaces;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Net.Http.Headers;

public class ZoomService : IZoomService
{
    private readonly ZoomSettings _settings;
    private readonly HttpClient _http;

    public ZoomService(IOptions<ZoomSettings> settings)
    {
        _settings = settings.Value;
        _http = new HttpClient();
    }

    // 1) get access token
    private async Task<string> GetAccessTokenAsync()
    {
        var tokenUrl = "https://zoom.us/oauth/token";

        _http.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Basic",
            Convert.ToBase64String(
                System.Text.Encoding.UTF8.GetBytes(
                    $"{_settings.ClientId}:{_settings.ClientSecret}"
                )
            ));

        var response = await _http.PostAsync(
            $"{tokenUrl}?grant_type=account_credentials&account_id={_settings.AccountId}",
            null
        );

        var json = await response.Content.ReadAsStringAsync();
        dynamic data = JsonConvert.DeserializeObject(json);

        return data.access_token;
    }

    // 2) create meeting
    public async Task<ZoomMeetingResponse> CreateMeeting(string topic, DateTime startTime, int duration)
    {
        var token = await GetAccessTokenAsync();
        _http.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

        var payload = new
        {
            topic = topic,
            type = 2,
            start_time = startTime.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ"),
            duration = duration,
            timezone = "UTC"
        };

        var content = new StringContent(
            JsonConvert.SerializeObject(payload),
            System.Text.Encoding.UTF8,
            "application/json"
        );

        var response = await _http.PostAsync(
            $"https://api.zoom.us/v2/users/{_settings.AccountId}/meetings",
            content
        );

        var json = await response.Content.ReadAsStringAsync();
        return JsonConvert.DeserializeObject<ZoomMeetingResponse>(json);
    }
}
