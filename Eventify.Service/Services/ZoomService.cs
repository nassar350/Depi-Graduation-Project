using Eventify.Service.DTOs.Zoom;
using Eventify.Service.Helpers;
using Eventify.Service.Interfaces;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;

public class ZoomService : IZoomService
{
    private readonly ZoomSettings _settings;
    private readonly HttpClient _http;

    public ZoomService(IOptions<ZoomSettings> settings)
    {
        _settings = settings.Value;
        _http = new HttpClient();
    }

    private async Task<string> GetAccessTokenAsync()
    {
        var tokenUrl = "https://zoom.us/oauth/token";

        var requestContent = new FormUrlEncodedContent(new[]
        {
            new KeyValuePair<string, string>("grant_type", "account_credentials"),
            new KeyValuePair<string, string>("account_id", _settings.AccountId!)
        });

        // Create a new request message with Basic Auth
        var request = new HttpRequestMessage(HttpMethod.Post, tokenUrl)
        {
            Content = requestContent
        };

        // Add Basic Auth to THIS request only
        var auth = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{_settings.ClientId}:{_settings.ClientSecret}"));
        request.Headers.Authorization = new AuthenticationHeaderValue("Basic", auth);

        var response = await _http.SendAsync(request);

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            throw new Exception($"Zoom Token Error: {response.StatusCode} - {error}");
        }

        var json = await response.Content.ReadAsStringAsync();
        dynamic data = JsonConvert.DeserializeObject(json)!;
        return data.access_token.ToString();
    }

    public async Task<ZoomMeetingResponse?> CreateMeeting(string topic, DateTime startTime, int duration)
    {
        try
        {
            var token = await GetAccessTokenAsync();

            var payload = new
            {
                topic,
                type = 2,
                start_time = startTime.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ"),
                duration,
                timezone = "UTC",
                settings = new
                {
                    join_before_host = true,
                    waiting_room = false
                }
            };

            var content = new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json");

            // Create a new request with Bearer token
            var request = new HttpRequestMessage(HttpMethod.Post, "https://api.zoom.us/v2/users/me/meetings")
            {
                Content = content
            };
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _http.SendAsync(request);

            var body = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"Zoom Error: {response.StatusCode}\n{body}");
                return null;
            }
            return JsonConvert.DeserializeObject<ZoomMeetingResponse>(body);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Zoom Exception: {ex.Message}");
            return null;
        }
    }
}