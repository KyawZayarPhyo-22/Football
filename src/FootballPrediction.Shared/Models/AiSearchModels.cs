namespace FootballPrediction.Shared.Models;

public class WebSourceCitationDto
{
    public string Title { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public string Snippet { get; set; } = string.Empty;
    public string Domain { get; set; } = string.Empty;
    public DateTime FetchedAt { get; set; } = DateTime.UtcNow;
}

public class CustomAiSearchRequest
{
    public string Query { get; set; } = string.Empty;
    public string Category { get; set; } = "Football"; // "Football", "2D", "3D", "General"
}

public class CustomAiSearchResponse
{
    public string Query { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string FormattedMarkdownReport { get; set; } = string.Empty;
    public List<WebSourceCitationDto> SearchedSources { get; set; } = [];
    public int CalculatedConfidence { get; set; } = 85;
    public string EngineUsed { get; set; } = "Google Gemini 3.8 Flash (Web Grounded)";
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}

public class SystemSettingsDto
{
    public string GeminiApiKey { get; set; } = string.Empty;
    public string AiModel { get; set; } = "gemini-3.8-flash";
    public bool UseSimulatedSearchIfOffline { get; set; } = true;
    public bool IsGeminiConfigured { get; set; }
    public string ActiveEngineStatus { get; set; } = "Active";
}

public class GeminiTestResultDto
{
    public bool IsSuccess { get; set; }
    public string Message { get; set; } = string.Empty;
    public string TestedModel { get; set; } = string.Empty;
    public string ResponseSnippet { get; set; } = string.Empty;
    public DateTime TestedAt { get; set; } = DateTime.UtcNow;
}
