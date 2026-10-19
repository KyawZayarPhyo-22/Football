using FootballPrediction.Shared.Models;
using System.Net.Http.Json;

namespace FootballPrediction.Client.Services;

public class PredictionApiClient
{
    private readonly HttpClient _httpClient;
    private string _apiBaseUrl = "http://localhost:5006";

    public string ApiBaseUrl
    {
        get => _apiBaseUrl;
        set => _apiBaseUrl = value.TrimEnd('/');
    }

    public PredictionApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
        if (httpClient.BaseAddress != null)
        {
            var baseStr = httpClient.BaseAddress.ToString().TrimEnd('/');
            // If running deployed together (or on custom domain, not on local client dev port 5181), use same origin
            if (!baseStr.Contains(":5181"))
            {
                _apiBaseUrl = baseStr;
            }
        }
    }

    private string Url(string path) => $"{_apiBaseUrl}{path}";

    public async Task<List<FootballMatchDto>> GetMatchesAsync(string? date = null, string? league = null, bool refresh = false)
    {
        try
        {
            var queryParams = new List<string>();
            if (!string.IsNullOrWhiteSpace(date)) queryParams.Add($"date={Uri.EscapeDataString(date)}");
            if (!string.IsNullOrWhiteSpace(league) && !league.Equals("All", StringComparison.OrdinalIgnoreCase)) queryParams.Add($"league={Uri.EscapeDataString(league)}");
            if (refresh) queryParams.Add("refresh=true");

            var queryString = queryParams.Count > 0 ? "?" + string.Join("&", queryParams) : "";
            var res = await _httpClient.GetFromJsonAsync<List<FootballMatchDto>>(Url($"/api/football/matches{queryString}"));
            return res ?? [];
        }
        catch
        {
            return [];
        }
    }

    public async Task<List<FootballCalendarDayDto>> GetCalendarDaysAsync(int daysAhead = 21)
    {
        try
        {
            var res = await _httpClient.GetFromJsonAsync<List<FootballCalendarDayDto>>(Url($"/api/football/calendar-days?daysAhead={daysAhead}"));
            return res ?? [];
        }
        catch
        {
            return [];
        }
    }

    public async Task<FootballMatchDto?> GetMatchByIdAsync(string id)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<FootballMatchDto>(Url($"/api/football/matches/{id}"));
        }
        catch
        {
            return null;
        }
    }

    public async Task<FootballAiPredictionDto?> RunAiMatchPredictionAsync(string matchId)
    {
        try
        {
            var res = await _httpClient.PostAsync(Url($"/api/football/predict/{matchId}"), null);
            if (res.IsSuccessStatusCode)
            {
                return await res.Content.ReadFromJsonAsync<FootballAiPredictionDto>();
            }
        }
        catch { }
        return null;
    }

    public async Task<FootballParlayResponseDto?> GenerateMixParlayAsync(FootballParlayRequestDto request)
    {
        try
        {
            var res = await _httpClient.PostAsJsonAsync(Url("/api/football/parlay"), request);
            if (res.IsSuccessStatusCode)
            {
                return await res.Content.ReadFromJsonAsync<FootballParlayResponseDto>();
            }
        }
        catch { }
        return null;
    }

    public async Task<TwoDLiveDto?> GetTwoDLiveAsync()
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<TwoDLiveDto>(Url("/api/lottery/2d/live"));
        }
        catch
        {
            return null;
        }
    }

    public async Task<List<TwoDHistoryRecordDto>> GetTwoDHistoryAsync()
    {
        try
        {
            var res = await _httpClient.GetFromJsonAsync<List<TwoDHistoryRecordDto>>(Url("/api/lottery/2d/history"));
            return res ?? [];
        }
        catch
        {
            return [];
        }
    }

    public async Task<TwoDAiPredictionDto?> PredictTwoDAsync(string session = "Full Day", string? targetDate = null, bool forceRefresh = false)
    {
        try
        {
            var url = Url($"/api/lottery/2d/predict?session={Uri.EscapeDataString(session)}" 
                + (!string.IsNullOrEmpty(targetDate) ? $"&targetDate={Uri.EscapeDataString(targetDate)}" : "")
                + (forceRefresh ? "&forceRefresh=true" : ""));
            var res = await _httpClient.PostAsync(url, null);
            if (res.IsSuccessStatusCode)
            {
                return await res.Content.ReadFromJsonAsync<TwoDAiPredictionDto>();
            }
        }
        catch { }
        return null;
    }

    public async Task<List<ThreeDHistoryRecordDto>> GetThreeDHistoryAsync()
    {
        try
        {
            var res = await _httpClient.GetFromJsonAsync<List<ThreeDHistoryRecordDto>>(Url("/api/lottery/3d/history"));
            return res ?? [];
        }
        catch
        {
            return [];
        }
    }

    public async Task<ThreeDAiPredictionDto?> PredictThreeDAsync(string? targetDate = null, bool forceRefresh = false)
    {
        try
        {
            var query = string.IsNullOrWhiteSpace(targetDate) ? "" : $"?targetDate={Uri.EscapeDataString(targetDate)}";
            if (forceRefresh)
            {
                query += string.IsNullOrEmpty(query) ? "?forceRefresh=true" : "&forceRefresh=true";
            }
            var res = await _httpClient.PostAsync(Url($"/api/lottery/3d/predict{query}"), null);
            if (res.IsSuccessStatusCode)
            {
                return await res.Content.ReadFromJsonAsync<ThreeDAiPredictionDto>();
            }
        }
        catch { }
        return null;
    }

    public async Task<CustomAiSearchResponse?> AnalyzeCustomQueryAsync(CustomAiSearchRequest request)
    {
        try
        {
            var res = await _httpClient.PostAsJsonAsync(Url("/api/aisearch/analyze"), request);
            if (res.IsSuccessStatusCode)
            {
                return await res.Content.ReadFromJsonAsync<CustomAiSearchResponse>();
            }
        }
        catch { }
        return null;
    }

    public async Task<SystemSettingsDto?> GetSettingsAsync()
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<SystemSettingsDto>(Url("/api/settings"));
        }
        catch
        {
            return null;
        }
    }

    public async Task<SystemSettingsDto?> UpdateSettingsAsync(SystemSettingsDto settings)
    {
        try
        {
            var res = await _httpClient.PostAsJsonAsync(Url("/api/settings"), settings);
            if (res.IsSuccessStatusCode)
            {
                return await res.Content.ReadFromJsonAsync<SystemSettingsDto>();
            }
        }
        catch { }
        return null;
    }

    public async Task<GeminiTestResultDto?> TestGeminiLiveAsync()
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<GeminiTestResultDto>(Url("/api/settings/test-gemini"));
        }
        catch (Exception ex)
        {
            return new GeminiTestResultDto
            {
                IsSuccess = false,
                Message = $"API Server နှင့် ချိတ်ဆက်မရပါ: {ex.Message}"
            };
        }
    }
}
