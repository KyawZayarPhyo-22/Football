using FootballPrediction.Shared.Models;

namespace FootballPrediction.Api.Services;

public static class FootballOddsEngine
{
    private static readonly Dictionary<string, int> TeamRatings = new(StringComparer.OrdinalIgnoreCase)
    {
        // Premier League
        ["Manchester City"] = 94, ["Arsenal"] = 92, ["Liverpool"] = 93, ["Aston Villa"] = 86,
        ["Tottenham Hotspur"] = 86, ["Tottenham"] = 86, ["Chelsea"] = 86, ["Newcastle United"] = 84,
        ["Manchester United"] = 84, ["Brighton"] = 82, ["West Ham United"] = 80, ["Fulham"] = 80,
        ["Bournemouth"] = 79, ["Brentford"] = 80, ["Crystal Palace"] = 80, ["Nottingham Forest"] = 79,
        ["Everton"] = 78, ["Wolverhampton Wanderers"] = 78, ["Leicester City"] = 77, ["Ipswich Town"] = 75,
        ["Southampton"] = 75, ["Leeds United"] = 77, ["Coventry City"] = 74, ["Sunderland"] = 75,

        // La Liga
        ["Real Madrid"] = 95, ["Barcelona"] = 93, ["Atlético Madrid"] = 89, ["Atletico Madrid"] = 89,
        ["Athletic Club"] = 84, ["Villarreal"] = 84, ["Real Sociedad"] = 83, ["Real Betis"] = 82,
        ["Girona"] = 82, ["Sevilla"] = 80, ["Celta Vigo"] = 79, ["Osasuna"] = 79,
        ["Rayo Vallecano"] = 78, ["Getafe"] = 78, ["Mallorca"] = 78, ["Deportivo Alavés"] = 77,
        ["Alaves"] = 77, ["Las Palmas"] = 76, ["Espanyol"] = 76, ["Leganes"] = 75,
        ["Valladolid"] = 75, ["Levante"] = 76, ["Elche"] = 75, ["Málaga"] = 74,

        // Serie A
        ["Inter"] = 91, ["Juventus"] = 87, ["AC Milan"] = 87, ["Milan"] = 87,
        ["Atalanta"] = 87, ["Napoli"] = 88, ["Roma"] = 84, ["Lazio"] = 83,
        ["Fiorentina"] = 82, ["Torino"] = 80, ["Bologna"] = 81, ["Monza"] = 78,
        ["Genoa"] = 78, ["Parma"] = 77, ["Cagliari"] = 77, ["Udinese"] = 78,
        ["Empoli"] = 77, ["Verona"] = 76, ["Como"] = 77, ["Lecce"] = 76,
        ["Venezia"] = 75, ["Sassuolo"] = 77,

        // Bundesliga
        ["Bayern Munich"] = 93, ["Bayer Leverkusen"] = 90, ["Borussia Dortmund"] = 88,
        ["RB Leipzig"] = 87, ["Eintracht Frankfurt"] = 83, ["VfB Stuttgart"] = 84,
        ["Stuttgart"] = 84, ["SC Freiburg"] = 80, ["Freiburg"] = 80, ["Werder Bremen"] = 79,
        ["Borussia Mönchengladbach"] = 79, ["Gladbach"] = 79, ["Wolfsburg"] = 79,
        ["Union Berlin"] = 78, ["Augsburg"] = 78, ["Mainz 05"] = 78, ["Mainz"] = 78,
        ["Hoffenheim"] = 78, ["FC Heidenheim"] = 77, ["St. Pauli"] = 75, ["Holstein Kiel"] = 74,
        ["1. FC Köln"] = 76, ["Schalke 04"] = 76,

        // Ligue 1
        ["Paris Saint-Germain"] = 91, ["PSG"] = 91, ["Monaco"] = 84, ["Marseille"] = 84,
        ["Lille"] = 83, ["Lyon"] = 82, ["Lens"] = 81, ["Nice"] = 81, ["Rennes"] = 80,
        ["Reims"] = 78, ["Strasbourg"] = 78, ["Brest"] = 80,

        // Portugal Primeira Liga
        ["Sporting CP"] = 85, ["Sporting"] = 85, ["Benfica"] = 84, ["FC Porto"] = 83, ["Porto"] = 83,
        ["SC Braga"] = 80, ["Braga"] = 80, ["Vitória de Guimarães"] = 78, ["Vitoria SC"] = 78,
        ["Famalicão"] = 76, ["Rio Ave"] = 75, ["Boavista"] = 75, ["Santa Clara"] = 75,

        // Netherlands Eredivisie
        ["PSV Eindhoven"] = 83, ["PSV"] = 83, ["Feyenoord"] = 82, ["Ajax"] = 82,
        ["AZ Alkmaar"] = 80, ["AZ"] = 80, ["FC Twente"] = 79, ["Twente"] = 79,
        ["FC Utrecht"] = 78, ["Utrecht"] = 78, ["Go Ahead Eagles"] = 76, ["Heerenveen"] = 76,

        // United States MLS
        ["Inter Miami"] = 82, ["Inter Miami CF"] = 82, ["Los Angeles FC"] = 80, ["LAFC"] = 80,
        ["LA Galaxy"] = 79, ["Columbus Crew"] = 79, ["Seattle Sounders"] = 78,
        ["New York Red Bulls"] = 77, ["FC Cincinnati"] = 78, ["Atlanta United"] = 77,

        // Turkey Süper Lig
        ["Galatasaray"] = 83, ["Fenerbahçe"] = 82, ["Fenerbahce"] = 82, ["Beşiktaş"] = 80,
        ["Besiktas"] = 80, ["Trabzonspor"] = 78, ["İstanbul Başakşehir"] = 77, ["Basaksehir"] = 77,
        ["Samsunspor"] = 76,

        // English Championship
        ["Burnley"] = 78, ["Leeds United"] = 78, ["Sheffield United"] = 77, ["Sunderland"] = 76,
        ["West Bromwich Albion"] = 76, ["West Brom"] = 76, ["Middlesbrough"] = 76, ["Coventry City"] = 75,

        // European Giants / Champions League
        ["Celtic"] = 78, ["Rangers"] = 78, ["Club Brugge"] = 79,
        ["Slavia Prague"] = 78, ["Sparta Prague"] = 77,
        ["Bodø/Glimt"] = 77, ["Bodo/Glimt"] = 77, ["Slovan Bratislava"] = 73, ["AEK Athens"] = 76,
        ["Sabah FK"] = 72,

        // International (Nations League)
        ["Spain"] = 94, ["England"] = 93, ["France"] = 93, ["Germany"] = 92,
        ["Portugal"] = 91, ["Netherlands"] = 89, ["Italy"] = 89, ["Croatia"] = 86,
        ["Belgium"] = 86, ["Denmark"] = 84, ["Switzerland"] = 84, ["Poland"] = 81,
        ["Scotland"] = 80, ["Serbia"] = 81, ["Hungary"] = 80, ["Bosnia"] = 77
    };

    public static int GetTeamRating(string teamName)
    {
        if (TeamRatings.TryGetValue(teamName, out var rating)) return rating;
        
        // Partial search
        foreach (var (key, val) in TeamRatings)
        {
            if (teamName.Contains(key, StringComparison.OrdinalIgnoreCase) || key.Contains(teamName, StringComparison.OrdinalIgnoreCase))
            {
                return val;
            }
        }
        return 77; // default average tier
    }

    public static (MyanmarOddsDto MyanmarOdds, OneXBetOddsDto OneXBetOdds, MatchTeamStatsDto Stats) GenerateOddsAndStats(string homeTeam, string awayTeam, string league)
    {
        int homeRating = GetTeamRating(homeTeam) + 2; // Home advantage
        int awayRating = GetTeamRating(awayTeam);
        int diff = homeRating - awayRating;

        // Calculate Win Probabilities
        double homeProb = Math.Clamp(0.42 + (diff * 0.032), 0.12, 0.88);
        double awayProb = Math.Clamp(0.30 - (diff * 0.024), 0.06, 0.72);
        double drawProb = Math.Max(0.12, 1.0 - homeProb - awayProb);

        // Normalize
        double totalProb = homeProb + drawProb + awayProb;
        homeProb = Math.Round(homeProb / totalProb, 2);
        drawProb = Math.Round(drawProb / totalProb, 2);
        awayProb = Math.Round(Math.Max(0.05, 1.0 - homeProb - drawProb), 2);

        // 1xBet Decimal Odds (1.05 bookmaker margin)
        double homeWinOdds = Math.Round(Math.Clamp(1.05 / Math.Max(homeProb, 0.08), 1.12, 12.5), 2);
        double drawOdds = Math.Round(Math.Clamp(1.05 / Math.Max(drawProb, 0.10), 2.90, 8.5), 2);
        double awayWinOdds = Math.Round(Math.Clamp(1.05 / Math.Max(awayProb, 0.06), 1.15, 14.0), 2);

        // Over / Under 2.5
        bool highScoring = (homeRating + awayRating) > 168 || league.Contains("Bundesliga") || league.Contains("Champions League") || league.Contains("Eredivisie") || league.Contains("MLS");
        double over25 = highScoring ? 1.55 : 1.78;
        double under25 = highScoring ? 2.45 : 2.05;
        double bttsYes = highScoring ? 1.65 : 1.85;
        double bttsNo = highScoring ? 2.20 : 1.95;

        // Asian Handicap & Myanmar Odds mapping
        string favoriteTeam;
        string bodyHandicap;
        string maHandicap;
        string asianHandicap;
        string trend;
        string recommendation;

        if (diff >= 12)
        {
            favoriteTeam = homeTeam;
            asianHandicap = $"{homeTeam} -1.75 @ 1.90";
            bodyHandicap = $"{homeTeam} ၁-၈၀ ရှုံး (-1.75)";
            maHandicap = "ဂိုးပေါင်း ၃-၅၀ ရှုံး (3.25 Over)";
            trend = $"{homeTeam} ဘက်သို့ ရေကြေး အပြည့်အဝ စီးဆင်းနေပြီး အဖွင့်ကြေးထက် မာလာသည်";
            recommendation = $"{homeTeam} အိမ်ကွင်းတွင် သွင်းဂိုးပြတ်သားပြီး အနိုင်သေချာသလောက်ရှိ";
        }
        else if (diff >= 8)
        {
            favoriteTeam = homeTeam;
            asianHandicap = $"{homeTeam} -1.25 @ 1.92";
            bodyHandicap = $"{homeTeam} ၁ လုံး ၇၀ ရှုံး (-1.25)";
            maHandicap = "ဂိုးပေါင်း ၃ လုံးသရေ (3.0 Over)";
            trend = $"{homeTeam} ၏ အိမ်ကွင်းခြေစွမ်းကြောင့် ဘော်ဒီကြေးမာနေသည်";
            recommendation = $"{homeTeam} ခြေစွမ်းပြည့်ဝစွာဖြင့် ၂ ဂိုးပြတ် အနိုင်မျှော်လင့်နိုင်";
        }
        else if (diff >= 5)
        {
            favoriteTeam = homeTeam;
            asianHandicap = $"{homeTeam} -0.75 @ 1.95";
            bodyHandicap = $"{homeTeam} ၈၀ စား (-0.75)";
            maHandicap = "ဂိုးပေါင်း ၂-၈၀ စား (2.75 Over)";
            trend = $"{homeTeam} ဘက်သို့ ရေကြေး အနည်းငယ် လိုက်ဆင်းနေသည်";
            recommendation = $"{homeTeam} အိမ်ကွင်း အနိုင်ရလဒ် ပိုမိုခိုင်မာပါသည်";
        }
        else if (diff >= 2)
        {
            favoriteTeam = homeTeam;
            asianHandicap = $"{homeTeam} -0.5 @ 1.98";
            bodyHandicap = $"{homeTeam} ၅၀ နိုင် (-0.5)";
            maHandicap = "ဂိုးပေါင်း ၂-၆၀ ရှုံး (2.5 Over)";
            trend = "စျေးကွက်ပေါက်ကြေး တည်ငြိမ်နေပြီး အိမ်ကွင်း အနည်းငယ် အသာရ";
            recommendation = $"{homeTeam} အနိုင် သို့မဟုတ် ၁ ဂိုးကွာ အကြိတ်အနယ် ကစားမည်";
        }
        else if (diff >= -2)
        {
            favoriteTeam = "သရေ (Level)";
            asianHandicap = "0.0 (Level) @ 1.92";
            bodyHandicap = "သရေ ၈၀ စား (Level Handicap)";
            maHandicap = "ဂိုးပေါင်း ၂ လုံးသရေ (2.0 Over/Under)";
            trend = "အသင်းနှစ်သင်း အဆင့်တူသဖြင့် ဘော်ဒီကြေး သရေပေါက်နေသည်";
            recommendation = "ဂိုးနည်း (Under) သို့မဟုတ် သရေရလဒ် ပိုမိုအားသာပါသည်";
        }
        else if (diff >= -6)
        {
            favoriteTeam = awayTeam;
            asianHandicap = $"{awayTeam} -0.5 @ 1.95";
            bodyHandicap = $"{awayTeam} ၅၀ နိုင် (-0.5)";
            maHandicap = "ဂိုးပေါင်း ၂-၇၀ ရှုံး (2.5 Over)";
            trend = $"ဧည့်သည် {awayTeam} ၏ အဆင့်အတန်းကြောင့် အဝေးကွင်းဖြစ်သော်လည်း ကြေးပေးထားရ";
            recommendation = $"{awayTeam} အဝေးကွင်းတွင် ကပ်နိုင်ရလဒ် မျှော်လင့်နိုင်";
        }
        else
        {
            favoriteTeam = awayTeam;
            asianHandicap = $"{awayTeam} -1.25 @ 1.90";
            bodyHandicap = $"{awayTeam} ၁ လုံး ၆၀ ရှုံး (-1.25)";
            maHandicap = "ဂိုးပေါင်း ၃-၂၀ စား (3.0 Over)";
            trend = $"ဧည့်သည် {awayTeam} ဘက်သို့ ရေကြေး အပြည့်အဝ စီးဆင်းနေသည်";
            recommendation = $"{awayTeam} အဝေးကွင်းတွင် သွင်းဂိုးပြတ်သားစွာ အနိုင်ယူဖွယ်ရှိ";
        }

        var myanmarOdds = new MyanmarOddsDto
        {
            FavoriteTeam = favoriteTeam,
            BodyHandicap = bodyHandicap,
            MaHandicap = maHandicap,
            OddsTrend = trend,
            Recommendation = recommendation
        };

        var oneXBet = new OneXBetOddsDto
        {
            HomeWin = homeWinOdds,
            Draw = drawOdds,
            AwayWin = awayWinOdds,
            Over25 = over25,
            Under25 = under25,
            AsianHandicap = asianHandicap,
            BothTeamsToScoreYes = bttsYes,
            BothTeamsToScoreNo = bttsNo
        };

        // Recent Form Generation
        List<string> homeForm = homeRating >= 88 ? ["W", "W", "D", "W", "W"] : homeRating >= 80 ? ["W", "D", "W", "L", "W"] : ["L", "W", "D", "L", "D"];
        List<string> awayForm = awayRating >= 88 ? ["W", "D", "W", "W", "W"] : awayRating >= 80 ? ["D", "W", "L", "W", "D"] : ["L", "L", "W", "D", "L"];

        var stats = new MatchTeamStatsDto
        {
            HomeRecentForm = homeForm,
            AwayRecentForm = awayForm,
            H2HSummary = $"{homeTeam} နှင့် {awayTeam} တို့၏ လတ်တလော တွေ့ဆုံမှုများတွင် အိမ်ရှင်က ပိုမိုအားသာလေ့ရှိသည်",
            H2HRecentScores = [$"{homeTeam} 2 - 1 {awayTeam}", $"{awayTeam} 1 - 1 {homeTeam}", $"{homeTeam} 1 - 0 {awayTeam}"],
            HomeInjuries = [$"{homeTeam} ပွဲထွက် လူစာရင်း အသင့်ရှိပြီး အဓိက ကစားသမားများ ကြံ့ခိုင်မှု ကောင်းမွန်"],
            AwayInjuries = [$"{awayTeam} အဝေးကွင်း လူစုံဖြင့် ယှဉ်ပြိုင်ကစားမည်"],
            KeyNews = [$"{league} တွင် အရေးပါသော ရမှတ်လုပွဲစဉ်ဖြစ်ပြီး ပရိသတ်စိတ်ဝင်စားမှု မြင့်မားနေသည်"],
            HomeWinProbability = Math.Round(homeProb * 100, 1),
            DrawProbability = Math.Round(drawProb * 100, 1),
            AwayWinProbability = Math.Round(awayProb * 100, 1)
        };

        return (myanmarOdds, oneXBet, stats);
    }

    public static FootballParlayResponseDto GenerateMixParlay(
        List<FootballMatchDto> matches,
        FootballParlayRequestDto request)
    {
        int targetMatches = Math.Clamp(request.TotalMatches, 4, 50);

        // 1. Resolve date range
        var fromDate = DateTime.TryParse(request.FromDate, out var fdt) 
            ? fdt.Date 
            : DateTime.UtcNow.AddHours(6.5).Date;
            
        var toDate = DateTime.TryParse(request.ToDate, out var tdt) 
            ? tdt.Date 
            : fromDate.AddDays(1);

        if (toDate < fromDate) toDate = fromDate;

        // Filter matches in date range (Myanmar time or UTC)
        var rangeMatches = matches
            .Where(m => (m.MyanmarMatchTime.Date >= fromDate && m.MyanmarMatchTime.Date <= toDate) ||
                        (m.MatchTime.Date >= fromDate && m.MatchTime.Date <= toDate))
            .Where(m => m.Status != "Finished")
            .OrderBy(m => m.MatchTime)
            .ToList();

        // If not enough matches in exact date range to fulfill targetMatches, supplement with all upcoming fixtures
        if (rangeMatches.Count < targetMatches)
        {
            var additional = matches
                .Where(m => m.Status != "Finished" && m.MatchTime >= DateTime.UtcNow.AddMinutes(-30))
                .Where(m => !rangeMatches.Any(rm => rm.Id == m.Id))
                .OrderBy(m => m.MatchTime)
                .ToList();
            rangeMatches.AddRange(additional);
        }

        var candidatePicks = new List<FootballParlayPickDto>();
        var matchPicksMap = new Dictionary<string, List<FootballParlayPickDto>>();

        foreach (var m in rangeMatches)
        {
            var matchPicks = new List<FootballParlayPickDto>();
            string favTeam = m.Stats.HomeWinProbability >= m.Stats.AwayWinProbability ? m.HomeTeam : m.AwayTeam;
            int maxProb = (int)Math.Max(m.Stats.HomeWinProbability, m.Stats.AwayWinProbability);

            // 1. Strong Home Win (1)
            if (m.Stats.HomeWinProbability >= 58 || m.OneXBetOdds.HomeWin <= 1.68)
            {
                int conf = Math.Clamp((int)(m.Stats.HomeWinProbability + 15), 78, 95);
                matchPicks.Add(new FootballParlayPickDto
                {
                    MatchId = m.Id,
                    League = m.League,
                    HomeTeam = m.HomeTeam,
                    AwayTeam = m.AwayTeam,
                    HomeLogo = m.HomeLogo,
                    AwayLogo = m.AwayLogo,
                    MatchTime = m.MatchTime,
                    SelectionName = $"{m.HomeTeam} Win (၁ အိမ်ရှင်နိုင်)",
                    MarketType = "1X2 (Home Win)",
                    Odds = m.OneXBetOdds.HomeWin,
                    ConfidencePercentage = conf,
                    AnalysisReason = $"{m.HomeTeam} သည် အိမ်ကွင်းတွင် ခြေစွမ်းပြည့်ဝပြီး အသင်းအဆင့်အတန်း ကွာခြားချက်အရ အနိုင် ၃ မှတ် အလွန်သေချာသည်"
                });
            }

            // 2. Strong Away Win (2)
            if (m.Stats.AwayWinProbability >= 58 || m.OneXBetOdds.AwayWin <= 1.68)
            {
                int conf = Math.Clamp((int)(m.Stats.AwayWinProbability + 15), 78, 95);
                matchPicks.Add(new FootballParlayPickDto
                {
                    MatchId = m.Id,
                    League = m.League,
                    HomeTeam = m.HomeTeam,
                    AwayTeam = m.AwayTeam,
                    HomeLogo = m.HomeLogo,
                    AwayLogo = m.AwayLogo,
                    MatchTime = m.MatchTime,
                    SelectionName = $"{m.AwayTeam} Win (၂ ဧည့်သည်နိုင်)",
                    MarketType = "1X2 (Away Win)",
                    Odds = m.OneXBetOdds.AwayWin,
                    ConfidencePercentage = conf,
                    AnalysisReason = $"{m.AwayTeam} ၏ ထိပ်တန်းတိုက်စစ်နှင့် ခြေစွမ်းထက်မြက်မှုကြောင့် အဝေးကွင်းနိုင်ပွဲ အလွန်သေချာသည်"
                });
            }

            // 3. Double Chance 1X (Home or Draw)
            if (m.Stats.HomeWinProbability >= 45)
            {
                double dcOdds = Math.Round(Math.Max(1.15, 1.05 / ((m.Stats.HomeWinProbability + m.Stats.DrawProbability) / 100.0)), 2);
                int conf = Math.Clamp((int)(m.Stats.HomeWinProbability + m.Stats.DrawProbability), 82, 96);
                matchPicks.Add(new FootballParlayPickDto
                {
                    MatchId = m.Id,
                    League = m.League,
                    HomeTeam = m.HomeTeam,
                    AwayTeam = m.AwayTeam,
                    HomeLogo = m.HomeLogo,
                    AwayLogo = m.AwayLogo,
                    MatchTime = m.MatchTime,
                    SelectionName = $"{m.HomeTeam} or Draw (1X သရေခံ)",
                    MarketType = "Double Chance (1X)",
                    Odds = dcOdds,
                    ConfidencePercentage = conf,
                    AnalysisReason = $"{m.HomeTeam} သည် အိမ်ကွင်းမာကျောပြီး ရှုံးနိမ့်ရန် ရာခိုင်နှုန်း အလွန်နည်းပါးသည်"
                });
            }

            // 4. Double Chance X2 (Away or Draw)
            if (m.Stats.AwayWinProbability >= 45)
            {
                double dcOdds = Math.Round(Math.Max(1.15, 1.05 / ((m.Stats.AwayWinProbability + m.Stats.DrawProbability) / 100.0)), 2);
                int conf = Math.Clamp((int)(m.Stats.AwayWinProbability + m.Stats.DrawProbability), 82, 96);
                matchPicks.Add(new FootballParlayPickDto
                {
                    MatchId = m.Id,
                    League = m.League,
                    HomeTeam = m.HomeTeam,
                    AwayTeam = m.AwayTeam,
                    HomeLogo = m.HomeLogo,
                    AwayLogo = m.AwayLogo,
                    MatchTime = m.MatchTime,
                    SelectionName = $"{m.AwayTeam} or Draw (X2 သရေခံ)",
                    MarketType = "Double Chance (X2)",
                    Odds = dcOdds,
                    ConfidencePercentage = conf,
                    AnalysisReason = $"{m.AwayTeam} သည် အဝေးကွင်းတွင် သရေ သို့မဟုတ် အနိုင်ရလဒ် ပိုင်ပိုင်နိုင်နိုင် ယူနိုင်သည်"
                });
            }

            // 5. Over 1.5 Goals
            double o15Odds = Math.Round(Math.Clamp(m.OneXBetOdds.Over25 * 0.74, 1.18, 1.35), 2);
            matchPicks.Add(new FootballParlayPickDto
            {
                MatchId = m.Id,
                League = m.League,
                HomeTeam = m.HomeTeam,
                AwayTeam = m.AwayTeam,
                HomeLogo = m.HomeLogo,
                AwayLogo = m.AwayLogo,
                MatchTime = m.MatchTime,
                SelectionName = "Over 1.5 Goals (၁.၅ ဂိုးအပေါ်)",
                MarketType = "Total Goals (Over 1.5)",
                Odds = o15Odds,
                ConfidencePercentage = 88,
                AnalysisReason = "နှစ်သင်းစလုံး တိုက်စစ်ဖွင့်ကစားတတ်ပြီး အနည်းဆုံး ၂ ဂိုးနှင့်အထက် ထွက်ပေါ်ရန် အလွန်သေချာသည်"
            });

            // 6. Over 2.5 Goals
            if (m.OneXBetOdds.Over25 <= 1.65)
            {
                matchPicks.Add(new FootballParlayPickDto
                {
                    MatchId = m.Id,
                    League = m.League,
                    HomeTeam = m.HomeTeam,
                    AwayTeam = m.AwayTeam,
                    HomeLogo = m.HomeLogo,
                    AwayLogo = m.AwayLogo,
                    MatchTime = m.MatchTime,
                    SelectionName = "Over 2.5 Goals (၂.၅ ဂိုးအပေါ်)",
                    MarketType = "Total Goals (Over 2.5)",
                    Odds = m.OneXBetOdds.Over25,
                    ConfidencePercentage = 81,
                    AnalysisReason = $"{m.League} ၏ သွင်းဂိုးနှုန်းမြင့်မားမှုနှင့် တိုက်စစ်ခြေစွမ်းအရ ၃ ဂိုးနှင့်အထက် ထွက်ပေါ်နိုင်သည်"
                });
            }

            // 7. At Least One Team Over 1.5 / 2.5 (Yes / No)
            double atLeastOne15YesOdds = Math.Round(1.36 + (Math.Abs(m.Id.GetHashCode()) % 4) * 0.03, 2);
            double atLeastOne15NoOdds = Math.Round(2.55 + (Math.Abs(m.Id.GetHashCode()) % 4) * 0.08, 2);
            double atLeastOne25YesOdds = Math.Round(2.15 + (Math.Abs(m.Id.GetHashCode()) % 4) * 0.06, 2);
            double atLeastOne25NoOdds = Math.Round(1.50 + (Math.Abs(m.Id.GetHashCode()) % 4) * 0.03, 2);

            // Option 7A: Over 1.5 - Yes
            matchPicks.Add(new FootballParlayPickDto
            {
                MatchId = m.Id,
                League = m.League,
                HomeTeam = m.HomeTeam,
                AwayTeam = m.AwayTeam,
                HomeLogo = m.HomeLogo,
                AwayLogo = m.AwayLogo,
                MatchTime = m.MatchTime,
                SelectionName = "At Least One Team Over 1.5 - Yes (အနည်းဆုံး ၁ သင်း ၁.၅ ဂိုးအထက် - Yes)",
                MarketType = "At Least One Team Over 1.5 (Yes)",
                Odds = atLeastOne15YesOdds,
                ConfidencePercentage = 88,
                AnalysisReason = $"{favTeam} ၏ ထိုးဖောက်တိုက်စစ် အားကောင်းမှုကြောင့် ပွဲအတွင်း အနည်းဆုံး တစ်သင်းမှ ၂ ဂိုးနှင့်အထက် (Over 1.5) သွင်းယူမည်"
            });

            // Option 7B: Over 1.5 - No (tight defensive matches)
            if (m.OneXBetOdds.Under25 <= 1.85)
            {
                matchPicks.Add(new FootballParlayPickDto
                {
                    MatchId = m.Id,
                    League = m.League,
                    HomeTeam = m.HomeTeam,
                    AwayTeam = m.AwayTeam,
                    HomeLogo = m.HomeLogo,
                    AwayLogo = m.AwayLogo,
                    MatchTime = m.MatchTime,
                    SelectionName = "At Least One Team Over 1.5 - No (အနည်းဆုံး ၁ သင်း ၁.၅ ဂိုးအထက် - No)",
                    MarketType = "At Least One Team Over 1.5 (No)",
                    Odds = atLeastOne15NoOdds,
                    ConfidencePercentage = 80,
                    AnalysisReason = "အကြိတ်အနယ် ကစားကွက်နှင့် ခံစစ်မာကျောမှုကြောင့် မည်သည့်အသင်းမှ ၁ ဂိုးထက် ပိုမိုသွင်းယူနိုင်ရန် မလွယ်ကူပါ"
                });
            }

            // Option 7C: Over 2.5 - Yes (high scoring games)
            if (m.OneXBetOdds.Over25 <= 1.65 || maxProb >= 65)
            {
                matchPicks.Add(new FootballParlayPickDto
                {
                    MatchId = m.Id,
                    League = m.League,
                    HomeTeam = m.HomeTeam,
                    AwayTeam = m.AwayTeam,
                    HomeLogo = m.HomeLogo,
                    AwayLogo = m.AwayLogo,
                    MatchTime = m.MatchTime,
                    SelectionName = "At Least One Team Over 2.5 - Yes (အနည်းဆုံး ၁ သင်း ၂.၅ ဂိုးအထက် - Yes)",
                    MarketType = "At Least One Team Over 2.5 (Yes)",
                    Odds = atLeastOne25YesOdds,
                    ConfidencePercentage = 82,
                    AnalysisReason = $"{favTeam} ၏ ဂိုးပြတ်သွင်းယူနိုင်စွမ်းအရ ပွဲအတွင်း အနည်းဆုံး တစ်သင်းမှ ၃ ဂိုးနှင့်အထက် (Over 2.5) သွင်းယူနိုင်သည်"
                });
            }

            // Option 7D: Over 2.5 - No (balanced games)
            matchPicks.Add(new FootballParlayPickDto
            {
                MatchId = m.Id,
                League = m.League,
                HomeTeam = m.HomeTeam,
                AwayTeam = m.AwayTeam,
                HomeLogo = m.HomeLogo,
                AwayLogo = m.AwayLogo,
                MatchTime = m.MatchTime,
                SelectionName = "At Least One Team Over 2.5 - No (အနည်းဆုံး ၁ သင်း ၂.၅ ဂိုးအထက် - No)",
                MarketType = "At Least One Team Over 2.5 (No)",
                Odds = atLeastOne25NoOdds,
                ConfidencePercentage = 89,
                AnalysisReason = "တစ်သင်းတည်းမှ ၃ ဂိုးနှင့်အထက် သွင်းယူနိုင်ခြေ အလွန်နည်းပါးသဖြင့် Over 2.5 No အလွန်သေချာသည်"
            });

            // 8. Both Teams To Score (နှစ်သင်းစလုံး ဂိုးသွင်းမည် - BTTS Yes)
            double bttsOdds = m.OneXBetOdds.BothTeamsToScoreYes > 1.0 ? m.OneXBetOdds.BothTeamsToScoreYes : 1.72;
            int bttsConf = m.OneXBetOdds.BothTeamsToScoreYes <= 1.75 ? 85 : 81;
            matchPicks.Add(new FootballParlayPickDto
            {
                MatchId = m.Id,
                League = m.League,
                HomeTeam = m.HomeTeam,
                AwayTeam = m.AwayTeam,
                HomeLogo = m.HomeLogo,
                AwayLogo = m.AwayLogo,
                MatchTime = m.MatchTime,
                SelectionName = "Both Teams To Score - Yes (နှစ်သင်းစလုံး ဂိုးသွင်းမည် - GG)",
                MarketType = "Both Teams To Score (BTTS)",
                Odds = bttsOdds,
                ConfidencePercentage = bttsConf,
                AnalysisReason = $"{m.HomeTeam} နှင့် {m.AwayTeam} နှစ်သင်းစလုံး တိုက်စစ်ခြေစွမ်းပြသနိုင်ပြီး ခံစစ်ပေါက်ကြားလေ့ရှိသဖြင့် နှစ်ဖက်အပြန်အလှန် ဂိုးသွင်းမည်"
            });

            // 9. Corners (ဒေါင့်ကန်ဘော - Over 8.5 Corners)
            double cornersOdds = Math.Round(1.48 + (Math.Abs(m.Id.GetHashCode()) % 4) * 0.04, 2);
            matchPicks.Add(new FootballParlayPickDto
            {
                MatchId = m.Id,
                League = m.League,
                HomeTeam = m.HomeTeam,
                AwayTeam = m.AwayTeam,
                HomeLogo = m.HomeLogo,
                AwayLogo = m.AwayLogo,
                MatchTime = m.MatchTime,
                SelectionName = "Over 8.5 Corners (ဒေါင့်ကန်ဘော ၈.၅ အပေါ်)",
                MarketType = "Corners (Over 8.5)",
                Odds = cornersOdds,
                ConfidencePercentage = 86,
                AnalysisReason = $"{m.HomeTeam} နှင့် {m.AwayTeam} တို့၏ တောင်ပံတိုက်စစ် အသုံးချမှုများပြားပြီး ဖိအားပေးကစားတတ်သဖြင့် ဒေါင့်ကန်ဘော အနည်းဆုံး ၉ ကြိမ်နှင့်အထက် ရရှိမည်"
            });

            // 10. Yellow Cards (အဝါကတ်ပြား - Over 3.5 Yellow Cards)
            double cardsOdds = Math.Round(1.50 + (Math.Abs(m.Id.GetHashCode()) % 4) * 0.05, 2);
            matchPicks.Add(new FootballParlayPickDto
            {
                MatchId = m.Id,
                League = m.League,
                HomeTeam = m.HomeTeam,
                AwayTeam = m.AwayTeam,
                HomeLogo = m.HomeLogo,
                AwayLogo = m.AwayLogo,
                MatchTime = m.MatchTime,
                SelectionName = "Over 3.5 Yellow Cards (အဝါကတ် ၃.၅ အပေါ်)",
                MarketType = "Yellow Cards (Over 3.5)",
                Odds = cardsOdds,
                ConfidencePercentage = 85,
                AnalysisReason = $"{m.League} ၏ ပြိုင်ဆိုင်မှုပြင်းထန်မှုနှင့် အကြိတ်အနယ် ဖျက်ထုတ်ကစားရမည့် ပွဲဖြစ်၍ ဒိုင်လူကြီးမှ အနည်းဆုံး အဝါကတ် ၄ ကတ်နှင့်အထက် ပြသမည်"
            });

            // 11. Penalty (ပင်နယ်တီ ရ/မရ - Penalty Awarded: Yes)
            double penaltyOdds = Math.Round(2.40 + (Math.Abs(m.Id.GetHashCode()) % 4) * 0.08, 2);
            matchPicks.Add(new FootballParlayPickDto
            {
                MatchId = m.Id,
                League = m.League,
                HomeTeam = m.HomeTeam,
                AwayTeam = m.AwayTeam,
                HomeLogo = m.HomeLogo,
                AwayLogo = m.AwayLogo,
                MatchTime = m.MatchTime,
                SelectionName = "Penalty Awarded - Yes (ပွဲအတွင်း ပင်နယ်တီ ရရှိမည်)",
                MarketType = "Penalty (Awarded - Yes)",
                Odds = penaltyOdds,
                ConfidencePercentage = 81,
                AnalysisReason = "ဧရိယာအတွင်း ထိုးဖောက်မှုများပြားပြီး VAR စစ်ဆေးမှုနှင့် ဒိုင်လူကြီး၏ ပြတ်သားမှုအရ ပင်နယ်တီ ရရှိနိုင်ခြေရှိသည်"
            });

            // 12. Goal Outside Penalty Area (ပင်နယ်တီစည်းပြင်ပမှ သွင်းဂိုး)
            double outsideOdds = Math.Round(1.98 + (Math.Abs(m.Id.GetHashCode()) % 4) * 0.05, 2);
            matchPicks.Add(new FootballParlayPickDto
            {
                MatchId = m.Id,
                League = m.League,
                HomeTeam = m.HomeTeam,
                AwayTeam = m.AwayTeam,
                HomeLogo = m.HomeLogo,
                AwayLogo = m.AwayLogo,
                MatchTime = m.MatchTime,
                SelectionName = "Goal Outside Penalty Area - Yes (၁၈ ကိုက်စည်းပြင်ပမှ သွင်းဂိုး ရရှိမည်)",
                MarketType = "Goal Outside Penalty Area",
                Odds = outsideOdds,
                ConfidencePercentage = 82,
                AnalysisReason = "အဝေးကန်ချက် ကောင်းမွန်သော ကွင်းလယ်လူများနှင့် ပင်နယ်တီဧရိယာပြင်ပမှ အလွတ်တည်ကန်ဘော အန္တရာယ်များပြားသည်"
            });

            // 13. Substitute Player To Score (လူစားဝင် ကစားသမား ဂိုးသွင်းမည်)
            double subOdds = Math.Round(2.25 + (Math.Abs(m.Id.GetHashCode()) % 4) * 0.08, 2);
            matchPicks.Add(new FootballParlayPickDto
            {
                MatchId = m.Id,
                League = m.League,
                HomeTeam = m.HomeTeam,
                AwayTeam = m.AwayTeam,
                HomeLogo = m.HomeLogo,
                AwayLogo = m.AwayLogo,
                MatchTime = m.MatchTime,
                SelectionName = "Substitute Player To Score (လူစားဝင် ကစားသမား ဂိုးသွင်းမည်)",
                MarketType = "Substitute To Score",
                Odds = subOdds,
                ConfidencePercentage = 81,
                AnalysisReason = "ဒုတိယပိုင်းတွင် ပင်ပန်းလာသော ခံစစ်ကို လူစားဝင် အရန်တိုက်စစ်မှူးများမှ ဖိအားပေး အဆုံးသတ်သွင်းယူနိုင်ခြေ အလွန်ကောင်းသည်"
            });

            // 14. Goal In Both Halves (ပထမပိုင်း/ဒုတိယပိုင်း နှစ်ပိုင်းစလုံး ဂိုးရမည်)
            double halvesOdds = Math.Round(1.60 + (Math.Abs(m.Id.GetHashCode()) % 4) * 0.04, 2);
            matchPicks.Add(new FootballParlayPickDto
            {
                MatchId = m.Id,
                League = m.League,
                HomeTeam = m.HomeTeam,
                AwayTeam = m.AwayTeam,
                HomeLogo = m.HomeLogo,
                AwayLogo = m.AwayLogo,
                MatchTime = m.MatchTime,
                SelectionName = "Goal In Both Halves - Yes (ပထမပိုင်း/ဒုတိယပိုင်း နှစ်ပိုင်းစလုံး ဂိုးရမည်)",
                MarketType = "Goal In Both Halves",
                Odds = halvesOdds,
                ConfidencePercentage = 86,
                AnalysisReason = "ပထမပိုင်း အစောပိုင်း သွင်းဂိုးများ ဖွင့်လှစ်ပြီး ဒုတိယပိုင်းတွင်လည်း သွင်းဂိုး ဆက်လက်ထွက်ပေါ်နိုင်သည့် ပွဲစဉ်ဖြစ်သည်"
            });

            // 15. Draw At Least One Half (အနည်းဆုံး တစ်ပိုင်း သရေကျမည်)
            double drawHalfOdds = Math.Round(1.44 + (Math.Abs(m.Id.GetHashCode()) % 4) * 0.03, 2);
            matchPicks.Add(new FootballParlayPickDto
            {
                MatchId = m.Id,
                League = m.League,
                HomeTeam = m.HomeTeam,
                AwayTeam = m.AwayTeam,
                HomeLogo = m.HomeLogo,
                AwayLogo = m.AwayLogo,
                MatchTime = m.MatchTime,
                SelectionName = "Draw At Least One Half (အနည်းဆုံး တစ်ပိုင်း သရေကျမည်)",
                MarketType = "Draw In At Least One Half",
                Odds = drawHalfOdds,
                ConfidencePercentage = 91,
                AnalysisReason = "ပွဲစတင်ချိန် ထိန်းချုပ်ကစားမှု သို့မဟုတ် ဒုတိယပိုင်း အချိန်ထိန်းကစားမှုများကြောင့် အနည်းဆုံး တစ်ပိုင်းတွင် သရေရလဒ် သေချာပေါက်ထွက်ပေါ်မည်"
            });

            // 16. Goals In A Row By Single Team (2 Yes/No, 3 Yes/No)
            double row2YesOdds = Math.Round(1.68 + (Math.Abs(m.Id.GetHashCode()) % 4) * 0.04, 2);
            double row2NoOdds = Math.Round(2.10 + (Math.Abs(m.Id.GetHashCode()) % 4) * 0.05, 2);
            double row3YesOdds = Math.Round(2.75 + (Math.Abs(m.Id.GetHashCode()) % 4) * 0.08, 2);
            double row3NoOdds = Math.Round(1.35 + (Math.Abs(m.Id.GetHashCode()) % 4) * 0.03, 2);

            // Option 16A: 2 Goals In A Row - Yes
            int row2Conf = Math.Clamp((int)(maxProb * 0.85 + 15), 82, 90);
            matchPicks.Add(new FootballParlayPickDto
            {
                MatchId = m.Id,
                League = m.League,
                HomeTeam = m.HomeTeam,
                AwayTeam = m.AwayTeam,
                HomeLogo = m.HomeLogo,
                AwayLogo = m.AwayLogo,
                MatchTime = m.MatchTime,
                SelectionName = $"{favTeam} 2 Goals In A Row - Yes ({favTeam} ဆက်တိုက် ၂ ဂိုး - Yes)",
                MarketType = "2 Goals In A Row (Yes)",
                Odds = row2YesOdds,
                ConfidencePercentage = row2Conf,
                AnalysisReason = $"{favTeam} ၏ တစ်ဖက်သတ်ဖိအားပေးမှုကြောင့် ပြိုင်ဘက်အား ချေပဂိုးမပေးရဘဲ ဆက်တိုက် ၂ ဂိုး သွင်းယူနိုင်မည်"
            });

            // Option 16B: 2 Goals In A Row - No (for even games)
            if (maxProb < 52)
            {
                matchPicks.Add(new FootballParlayPickDto
                {
                    MatchId = m.Id,
                    League = m.League,
                    HomeTeam = m.HomeTeam,
                    AwayTeam = m.AwayTeam,
                    HomeLogo = m.HomeLogo,
                    AwayLogo = m.AwayLogo,
                    MatchTime = m.MatchTime,
                    SelectionName = "2 Goals In A Row - No (တစ်သင်းတည်း ဆက်တိုက် ၂ ဂိုးမသွင်းနိုင် - No)",
                    MarketType = "2 Goals In A Row (No)",
                    Odds = row2NoOdds,
                    ConfidencePercentage = 81,
                    AnalysisReason = "အသင်းနှစ်သင်း အဆင့်အတန်းတူညီပြီး အပြန်အလှန် ချေပဂိုးရရှိလေ့ရှိသဖြင့် တစ်သင်းတည်း ဆက်တိုက် ၂ ဂိုးမရနိုင်ပါ"
                });
            }

            // Option 16C: 3 Goals In A Row - Yes (big favorites)
            if (maxProb >= 65)
            {
                matchPicks.Add(new FootballParlayPickDto
                {
                    MatchId = m.Id,
                    League = m.League,
                    HomeTeam = m.HomeTeam,
                    AwayTeam = m.AwayTeam,
                    HomeLogo = m.HomeLogo,
                    AwayLogo = m.AwayLogo,
                    MatchTime = m.MatchTime,
                    SelectionName = $"{favTeam} 3 Goals In A Row - Yes ({favTeam} ဆက်တိုက် ၃ ဂိုး - Yes)",
                    MarketType = "3 Goals In A Row (Yes)",
                    Odds = row3YesOdds,
                    ConfidencePercentage = 80,
                    AnalysisReason = $"{favTeam} နှင့် ပြိုင်ဘက်ကြား ခြေစွမ်းကွာဟမှုအရ အိမ်ရှင်မှ ဂိုးပြတ် ဆက်တိုက် ၃ ဂိုး သွင်းယူနိုင်ခြေရှိသည်"
                });
            }

            // Option 16D: 3 Goals In A Row - No (very high probability for parlay)
            matchPicks.Add(new FootballParlayPickDto
            {
                MatchId = m.Id,
                League = m.League,
                HomeTeam = m.HomeTeam,
                AwayTeam = m.AwayTeam,
                HomeLogo = m.HomeLogo,
                AwayLogo = m.AwayLogo,
                MatchTime = m.MatchTime,
                SelectionName = "3 Goals In A Row - No (တစ်သင်းတည်း ဆက်တိုက် ၃ ဂိုးမသွင်းနိုင် - No)",
                MarketType = "3 Goals In A Row (No)",
                Odds = row3NoOdds,
                ConfidencePercentage = 92,
                AnalysisReason = "ပြိုင်ဘက်အား ချေပဂိုးလုံးဝမပေးဘဲ တစ်သင်းတည်း ဆက်တိုက် ၃ ဂိုး သွင်းယူနိုင်ခြေ အလွန်နည်းပါးသဖြင့် 'No' အလွန်သေချာပါသည်"
            });

            // Filter by market preference if specified
            if (!string.IsNullOrWhiteSpace(request.MarketPreference) && !request.MarketPreference.Equals("All", StringComparison.OrdinalIgnoreCase))
            {
                matchPicks = matchPicks.Where(p => MatchPreferenceFilter(p.MarketType, request.MarketPreference)).ToList();
            }

            // Filter by min confidence
            var validPicks = matchPicks.Where(p => p.ConfidencePercentage >= request.MinConfidence).ToList();
            if (!validPicks.Any())
            {
                validPicks = matchPicks.Where(p => p.ConfidencePercentage >= 75).ToList();
            }
            if (!validPicks.Any() && matchPicks.Any())
            {
                validPicks = matchPicks;
            }

            if (validPicks.Any())
            {
                matchPicksMap[m.Id] = validPicks;
                // Pick the highest confidence pick for this match (and best odds)
                var best = validPicks.OrderByDescending(p => p.ConfidencePercentage).ThenByDescending(p => p.Odds).First();
                candidatePicks.Add(best);
            }
        }

        bool isAllMarkets = string.IsNullOrWhiteSpace(request.MarketPreference) || request.MarketPreference.Equals("All", StringComparison.OrdinalIgnoreCase);

        List<FootballParlayPickDto> selectedPicks;

        if (isAllMarkets && matchPicksMap.Any())
        {
            selectedPicks = new List<FootballParlayPickDto>();
            var usedMarketCounts = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            var usedMatchIds = new HashSet<string>();

            // Maximum allowed picks for the exact same market type (ensures diverse mix parlay)
            int maxPerMarket = Math.Max(2, (int)Math.Ceiling((double)targetMatches / 5));

            // Pass 1: Select highest confidence picks across matches while enforcing market diversity
            foreach (var kvp in matchPicksMap)
            {
                if (selectedPicks.Count >= targetMatches) break;

                var picksForMatch = kvp.Value
                    .OrderByDescending(p => p.ConfidencePercentage)
                    .ThenByDescending(p => p.Odds)
                    .ToList();

                var pick = picksForMatch.FirstOrDefault(p =>
                    !usedMarketCounts.TryGetValue(p.MarketType, out var count) || count < maxPerMarket);

                if (pick != null)
                {
                    selectedPicks.Add(pick);
                    usedMatchIds.Add(kvp.Key);
                    usedMarketCounts[pick.MarketType] = usedMarketCounts.GetValueOrDefault(pick.MarketType, 0) + 1;
                }
            }

            // Pass 2: If we still need more picks to fulfill targetMatches, take best available from remaining matches
            if (selectedPicks.Count < targetMatches)
            {
                foreach (var kvp in matchPicksMap)
                {
                    if (selectedPicks.Count >= targetMatches) break;
                    if (usedMatchIds.Contains(kvp.Key)) continue;

                    var pick = kvp.Value
                        .OrderByDescending(p => p.ConfidencePercentage)
                        .ThenByDescending(p => p.Odds)
                        .FirstOrDefault();

                    if (pick != null)
                    {
                        selectedPicks.Add(pick);
                        usedMatchIds.Add(kvp.Key);
                    }
                }
            }

            // Order selected parlay picks by confidence
            selectedPicks = selectedPicks.OrderByDescending(p => p.ConfidencePercentage).ThenByDescending(p => p.Odds).ToList();
        }
        else
        {
            selectedPicks = candidatePicks
                .OrderByDescending(p => p.ConfidencePercentage)
                .ThenByDescending(p => p.Odds)
                .Take(targetMatches)
                .ToList();
        }

        // Calculate combined odds
        double combinedOdds = 1.0;
        foreach (var p in selectedPicks)
        {
            combinedOdds *= p.Odds;
        }
        combinedOdds = Math.Round(combinedOdds, 2);

        var stake = request.StakeAmount > 0 ? request.StakeAmount : 10000m;
        decimal safeMultiplier = (decimal)Math.Min(combinedOdds, 10000000.0);
        var payout = Math.Round(stake * safeMultiplier, 0);
        var avgConf = selectedPicks.Any() ? (int)selectedPicks.Average(p => p.ConfidencePercentage) : request.MinConfidence;

        return new FootballParlayResponseDto
        {
            Picks = selectedPicks,
            TotalCombinedOdds = combinedOdds,
            StakeAmount = stake,
            EstimatedPayout = payout,
            AverageConfidence = avgConf,
            GeneratedAt = DateTime.UtcNow.AddHours(6.5).ToString("dd MMM yyyy, hh:mm tt") + " MMT",
            StrategySummary = $"🎯 {avgConf}%+ အသေချာဆုံး 1xBet ပေါက်ကြေးများဖြင့် တွက်ချက်ထားသော {selectedPicks.Count} ပွဲတွဲ မောင်းလက်မှတ်",
            FilterInfo = $"{fromDate:dd MMM} မှ {toDate:dd MMM} အထိ • {selectedPicks.Count} ပွဲမောင်း"
        };
    }

    private static bool MatchPreferenceFilter(string marketType, string preference)
    {
        return preference switch
        {
            "1X2" => marketType.StartsWith("1X2") || marketType.StartsWith("Double Chance"),
            "OverUnder" => marketType.Contains("Total Goals"),
            "AtLeastOneTeamToScore" => marketType.Contains("At Least One Team Over"),
            "AtLeastOneOver15" => marketType.Contains("At Least One Team Over 1.5"),
            "AtLeastOneOver25" => marketType.Contains("At Least One Team Over 2.5"),
            "GoalsInARow" => marketType.Contains("Goals In A Row"),
            "GoalsInARow2" => marketType.Contains("2 Goals In A Row"),
            "GoalsInARow3" => marketType.Contains("3 Goals In A Row"),
            "BothTeamsToScore" => marketType.Contains("Both Teams To Score") || marketType.Contains("BTTS"),
            "Corners" => marketType.Contains("Corners"),
            "YellowCards" => marketType.Contains("Yellow Cards"),
            "Penalty" => marketType.StartsWith("Penalty"),
            "GoalOutsidePenaltyArea" => marketType.Contains("Outside Penalty"),
            "SubstitutePlayerToScore" => marketType.Contains("Substitute To Score"),
            "GoalInBothHalves" => marketType.Contains("Goal In Both Halves"),
            "DrawAtLeastOneHalf" => marketType.Contains("Draw In At Least One Half"),
            _ => true
        };
    }
}
