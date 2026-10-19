namespace FootballPrediction.Shared.Models;

public class FootballMatchDto
{
    public string Id { get; set; } = string.Empty;
    public string League { get; set; } = string.Empty;
    public string HomeTeam { get; set; } = string.Empty;
    public string AwayTeam { get; set; } = string.Empty;
    public string HomeLogo { get; set; } = string.Empty;
    public string AwayLogo { get; set; } = string.Empty;
    public DateTime MatchTime { get; set; }
    public string Status { get; set; } = "Upcoming"; // "Upcoming", "Live", "Finished"
    public string? CurrentScore { get; set; }
    public string? Venue { get; set; }
    public string? FotMobId { get; set; }
    public string? FotMobUrl { get; set; }
    public string DataSource { get; set; } = "FotMob Official Live Feed";

    // Odds
    public MyanmarOddsDto MyanmarOdds { get; set; } = new();
    public OneXBetOddsDto OneXBetOdds { get; set; } = new();

    // Stats & Team Info
    public MatchTeamStatsDto Stats { get; set; } = new();

    // AI Prediction
    public FootballAiPredictionDto? AiPrediction { get; set; }

    public string? Round { get; set; }
    public string MatchDateString => MatchTime.ToString("yyyy-MM-dd");

    [System.Text.Json.Serialization.JsonIgnore]
    public DateTime MyanmarMatchTime => MatchTime.Kind == DateTimeKind.Utc ? MatchTime.AddHours(6.5) : MatchTime.ToUniversalTime().AddHours(6.5);
}

public class FootballCalendarDayDto
{
    public string Date { get; set; } = string.Empty; // "yyyy-MM-dd"
    public string DisplayDayOfWeek { get; set; } = string.Empty; // "MON", "TUE"
    public string DisplayDate { get; set; } = string.Empty; // "19 Oct"
    public int MatchCount { get; set; }
    public List<string> Leagues { get; set; } = [];
    public bool IsToday { get; set; }
    public bool IsTomorrow { get; set; }
}

public class MyanmarOddsDto
{
    public string FavoriteTeam { get; set; } = string.Empty;
    public string BodyHandicap { get; set; } = string.Empty; // e.g. "မန်စီးတီး ၁ လုံး ၇၀ ရှုံး", "အာဆင်နယ် ၅၀ နိုင်"
    public string MaHandicap { get; set; } = string.Empty; // e.g. "ဂိုးပေါင်း ၂-၆၀ ရှုံး", "၃+၃၀", "၂ လုံးသရေ"
    public string OddsTrend { get; set; } = string.Empty; // e.g. "ကြေးမာနေသည်", "ကြေးကျဆင်းနေသည်"
    public string Recommendation { get; set; } = string.Empty;
}

public class OneXBetOddsDto
{
    public double HomeWin { get; set; } // 1
    public double Draw { get; set; }    // X
    public double AwayWin { get; set; } // 2
    public double Over25 { get; set; }
    public double Under25 { get; set; }
    public string AsianHandicap { get; set; } = string.Empty; // e.g. "-0.75 @ 1.92"
    public double BothTeamsToScoreYes { get; set; }
    public double BothTeamsToScoreNo { get; set; }
}

public class MatchTeamStatsDto
{
    public List<string> HomeRecentForm { get; set; } = []; // ["W", "W", "D", "W", "L"]
    public List<string> AwayRecentForm { get; set; } = []; // ["L", "W", "D", "W", "W"]
    public string H2HSummary { get; set; } = string.Empty;
    public List<string> H2HRecentScores { get; set; } = [];
    public List<string> HomeInjuries { get; set; } = [];
    public List<string> AwayInjuries { get; set; } = [];
    public List<string> KeyNews { get; set; } = [];
    public double HomeWinProbability { get; set; }
    public double DrawProbability { get; set; }
    public double AwayWinProbability { get; set; }
}

public class FootballAiPredictionDto
{
    public string MatchId { get; set; } = string.Empty;
    public int PredictedHomeScore { get; set; }
    public int PredictedAwayScore { get; set; }
    public int ConfidencePercentage { get; set; }
    public string RecommendedBodyChoice { get; set; } = string.Empty;
    public string RecommendedMaChoice { get; set; } = string.Empty;
    public string RecommendedOneXBet { get; set; } = string.Empty;
    public string TacticalReasoningMy { get; set; } = string.Empty; // Myanmar Language Detailed Analysis
    public string TacticalReasoningEn { get; set; } = string.Empty;
    public List<WebSourceCitationDto> SearchSources { get; set; } = [];
    public string EngineUsed { get; set; } = "Google Gemini 3.8 Flash (Web Grounded)";
    public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;
}

public class FootballParlayRequestDto
{
    public string FromDate { get; set; } = string.Empty; // "yyyy-MM-dd"
    public string ToDate { get; set; } = string.Empty;   // "yyyy-MM-dd"
    public int TotalMatches { get; set; } = 4;
    public int MinConfidence { get; set; } = 80; // Default 80% sure
    public string MarketPreference { get; set; } = "All"; // "All", "1X2", "OverUnder"
    public decimal StakeAmount { get; set; } = 10000;
}

public class FootballParlayResponseDto
{
    public List<FootballParlayPickDto> Picks { get; set; } = [];
    public double TotalCombinedOdds { get; set; }
    public decimal StakeAmount { get; set; }
    public decimal EstimatedPayout { get; set; }
    public int AverageConfidence { get; set; }
    public string GeneratedAt { get; set; } = string.Empty;
    public string StrategySummary { get; set; } = string.Empty;
    public string FilterInfo { get; set; } = string.Empty;
}

public class FootballParlayPickDto
{
    public string MatchId { get; set; } = string.Empty;
    public string League { get; set; } = string.Empty;
    public string HomeTeam { get; set; } = string.Empty;
    public string AwayTeam { get; set; } = string.Empty;
    public string HomeLogo { get; set; } = string.Empty;
    public string AwayLogo { get; set; } = string.Empty;
    public DateTime MatchTime { get; set; }
    public string SelectionName { get; set; } = string.Empty; // e.g. "Tottenham Hotspur Win", "Over 2.5 Goals"
    public string MarketType { get; set; } = string.Empty;    // "1X2 (Win/Draw/Loss)", "Over/Under 2.5", "Double Chance"
    public double Odds { get; set; }                          // 1xBet Decimal Odds
    public int ConfidencePercentage { get; set; }             // e.g. 85%
    public string AnalysisReason { get; set; } = string.Empty;
}

