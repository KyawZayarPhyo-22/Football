using FootballPrediction.Shared.Models;

namespace FootballPrediction.Api.Services;

public interface ISettingsService
{
    SystemSettingsDto GetSettings();
    void UpdateSettings(SystemSettingsDto settings);
    string GetActualApiKey();
    Task<GeminiTestResultDto> TestGeminiLiveAsync();
}

public class SettingsService : ISettingsService
{
    private readonly SystemSettingsDto _settings = new()
    {
        GeminiApiKey = string.Empty,
        AiModel = "gemini-flash-lite-latest",
        UseSimulatedSearchIfOffline = true,
        IsGeminiConfigured = false,
        ActiveEngineStatus = "Active"
    };

    private readonly IConfiguration _configuration;
    private readonly HttpClient _httpClient;

    public SettingsService(IConfiguration configuration, HttpClient httpClient)
    {
        _configuration = configuration;
        _httpClient = httpClient;
        ReloadFromConfig();
    }

    private void ReloadFromConfig()
    {
        var key = _configuration["GEMINI_API_KEY"] ?? _configuration["Gemini:ApiKey"];
        if (!string.IsNullOrWhiteSpace(key))
        {
            _settings.GeminiApiKey = key.Trim();
            _settings.IsGeminiConfigured = true;
        }
    }

    public SystemSettingsDto GetSettings()
    {
        ReloadFromConfig();
        var key = GetActualApiKey();
        var hasKey = !string.IsNullOrWhiteSpace(key);

        return new SystemSettingsDto
        {
            GeminiApiKey = MaskKey(key),
            AiModel = _settings.AiModel,
            UseSimulatedSearchIfOffline = _settings.UseSimulatedSearchIfOffline,
            IsGeminiConfigured = hasKey,
            ActiveEngineStatus = hasKey 
                ? "🟢 Google Gemini AI (Active & Grounded with Web Intel)" 
                : "🟡 Autonomous Smart Agent (Active & Grounded with Web Intel)"
        };
    }

    public void UpdateSettings(SystemSettingsDto settings)
    {
        if (!string.IsNullOrWhiteSpace(settings.GeminiApiKey) && !settings.GeminiApiKey.Contains("****"))
        {
            _settings.GeminiApiKey = settings.GeminiApiKey.Trim();
            _settings.IsGeminiConfigured = true;
        }
        else if (string.IsNullOrWhiteSpace(settings.GeminiApiKey))
        {
            _settings.GeminiApiKey = string.Empty;
            _settings.IsGeminiConfigured = false;
        }

        _settings.AiModel = string.IsNullOrWhiteSpace(settings.AiModel) ? "gemini-flash-lite-latest" : settings.AiModel;
        _settings.UseSimulatedSearchIfOffline = settings.UseSimulatedSearchIfOffline;
    }

    public string GetActualApiKey()
    {
        if (!string.IsNullOrWhiteSpace(_settings.GeminiApiKey))
            return _settings.GeminiApiKey;
        return _configuration["GEMINI_API_KEY"] ?? _configuration["Gemini:ApiKey"] ?? string.Empty;
    }

    public async Task<GeminiTestResultDto> TestGeminiLiveAsync()
    {
        var key = GetActualApiKey();
        if (string.IsNullOrWhiteSpace(key))
        {
            return new GeminiTestResultDto
            {
                IsSuccess = false,
                Message = "Gemini API Key ထည့်သွင်းထားခြင်း မရှိသေးပါ။ (Smart Autonomous Agent ဖြင့် ဆက်လက် အလုပ်လုပ်နေပါသည်)",
                TestedModel = _settings.AiModel,
                TestedAt = DateTime.UtcNow
            };
        }

        var candidateModels = new[] { _settings.AiModel, "gemini-flash-lite-latest", "gemini-3.6-flash", "gemini-3.5-flash-lite", "gemini-3.1-flash-lite", "gemini-3.8-flash", "gemini-3.5-flash", "gemini-flash-latest" }
            .Where(m => !string.IsNullOrWhiteSpace(m))
            .Distinct()
            .ToList();

        string lastError = string.Empty;

        foreach (var model in candidateModels)
        {
            var url = $"https://generativelanguage.googleapis.com/v1beta/models/{model}:generateContent?key={key}";
            var payload = new
            {
                contents = new[]
                {
                    new { parts = new[] { new { text = "Respond with 'OK' if you are active." } } }
                }
            };

            try
            {
                var res = await _httpClient.PostAsJsonAsync(url, payload);
                if (res.IsSuccessStatusCode)
                {
                    var json = await res.Content.ReadAsStringAsync();
                    return new GeminiTestResultDto
                    {
                        IsSuccess = true,
                        Message = $"✓ Google Gemini API ({model}) နှင့် အောင်မြင်စွာ ချိတ်ဆက်မိပါသည်!",
                        TestedModel = model,
                        ResponseSnippet = "Connection Verified: OK",
                        TestedAt = DateTime.UtcNow
                    };
                }
                else
                {
                    var errJson = await res.Content.ReadAsStringAsync();
                    var status = (int)res.StatusCode;
                    lastError = $"HTTP {status} ({model}): {errJson}";
                    if (status != 503 && status != 429)
                    {
                        return new GeminiTestResultDto
                        {
                            IsSuccess = false,
                            Message = $"Google Gemini API Error: HTTP {status} ({res.ReasonPhrase})",
                            TestedModel = model,
                            ResponseSnippet = errJson,
                            TestedAt = DateTime.UtcNow
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                lastError = ex.Message;
            }
        }

        return new GeminiTestResultDto
        {
            IsSuccess = false,
            Message = "Google Gemini Servers are currently busy (503/429). Autonomous Agent is ready.",
            TestedModel = _settings.AiModel,
            ResponseSnippet = lastError,
            TestedAt = DateTime.UtcNow
        };
    }

    private static string MaskKey(string key)
    {
        if (string.IsNullOrWhiteSpace(key)) return string.Empty;
        if (key.Length <= 8) return "********";
        return key[..4] + "********" + key[^4..];
    }
}
