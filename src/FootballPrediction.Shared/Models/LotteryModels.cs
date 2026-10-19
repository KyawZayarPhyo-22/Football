namespace FootballPrediction.Shared.Models;

public class TwoDLiveDto
{
    public DateTime Date { get; set; } = DateTime.UtcNow;
    public string FormattedDate { get; set; } = string.Empty;
    public double SetIndex { get; set; }
    public double SetValue { get; set; }
    public string Current2D { get; set; } = "--";
    public string? MorningOpen { get; set; } // 11:00 AM
    public string? MorningResult { get; set; } // 12:01 PM
    public string? AfternoonOpen { get; set; } // 03:00 PM
    public string? EveningResult { get; set; } // 04:30 PM
    public string Status { get; set; } = "Live"; // "Live", "Closed", "Pending"
    public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
    public string MarketTrend { get; set; } = "Stable";
    public string DataSource { get; set; } = "The Stock Exchange of Thailand (SET) & ThaiStock2D";
}

public class TwoDHistoryRecordDto
{
    public string Date { get; set; } = string.Empty;
    public string DayOfWeek { get; set; } = string.Empty;
    public string? MorningResult { get; set; }
    public string? MorningSet { get; set; }
    public string? MorningValue { get; set; }
    public string? EveningResult { get; set; }
    public string? EveningSet { get; set; }
    public string? EveningValue { get; set; }
}

public class TwoDAiPredictionDto
{
    public string TargetDate { get; set; } = string.Empty;
    public string Session { get; set; } = "Full Day"; // "12:01 PM (Morning)" or "04:30 PM (Evening)" or "Full Day"
    public List<string> KeyNumbers { get; set; } = []; // ကီး - e.g. ["3 - 8", "1 - 6"]
    public List<string> BreakNumbers { get; set; } = []; // ဘရိတ် - e.g. ["4 ဘရိတ်", "7 ဘရိတ်"]
    public List<string> TopDirectNumbers { get; set; } = []; // ထိပ်တန်း အကွက်များ - e.g. ["31", "38", "83", "16", "64"]
    public List<string> PowerNatKhatPairs { get; set; } = []; // ပါဝါ / နက္ခတ် တွဲများ - e.g. ["38 (Power)", "18 (Natkhat)"]
    public List<string> AparNumbers { get; set; } = []; // အပူးဂဏန်းများ - e.g. ["33", "88", "11"]
    public int ConfidencePercentage { get; set; } = 85;
    public string AiAnalysisExplanation { get; set; } = string.Empty; // သုံးသပ်ချက် မြန်မာလို
    public List<WebSourceCitationDto> SearchedWebSources { get; set; } = [];
    public string EngineUsed { get; set; } = "Google Gemini 3.8 Flash (Web Grounded)";
    public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;
}

public class ThreeDHistoryRecordDto
{
    public string DrawDate { get; set; } = string.Empty;
    public string Result { get; set; } = string.Empty;
    public int DigitSum { get; set; }
    public string PrizeDetails { get; set; } = string.Empty;
    public string? FirstPrize { get; set; }
    public string DataSource { get; set; } = "Thai Government Lottery Official (Sanook GLO)";
}

public class ThreeDAiPredictionDto
{
    public string TargetDrawDate { get; set; } = string.Empty;
    public List<string> HotDigits { get; set; } = []; // e.g. ["7", "2", "8"]
    public List<string> DirectPicks { get; set; } = []; // e.g. ["728", "827", "391", "405"]
    public List<string> RPicks { get; set; } = []; // e.g. ["728 R", "391 R"]
    public List<int> DigitSums { get; set; } = []; // e.g. [17, 13, 9]
    public int ConfidencePercentage { get; set; } = 82;
    public string AiAnalysisExplanation { get; set; } = string.Empty;
    public List<WebSourceCitationDto> SearchedWebSources { get; set; } = [];
    public string EngineUsed { get; set; } = "Google Gemini 3.8 Flash (Web Grounded)";
    public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;
}
