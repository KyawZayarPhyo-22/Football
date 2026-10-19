using FootballPrediction.Api.Services;
using FootballPrediction.Shared.Models;
using Microsoft.AspNetCore.Mvc;

namespace FootballPrediction.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FootballController : ControllerBase
{
    private readonly IFootballService _footballService;

    public FootballController(IFootballService footballService)
    {
        _footballService = footballService;
    }

    [HttpGet("matches")]
    public async Task<ActionResult<List<FootballMatchDto>>> GetMatches(
        [FromQuery] string? date = null, 
        [FromQuery] string? league = null, 
        [FromQuery] bool refresh = false)
    {
        var matches = await _footballService.GetMatchesAsync(date, league, refresh);
        return Ok(matches);
    }

    [HttpGet("calendar-days")]
    public async Task<ActionResult<List<FootballCalendarDayDto>>> GetCalendarDays([FromQuery] int daysAhead = 21)
    {
        var days = await _footballService.GetCalendarDaysAsync(daysAhead);
        return Ok(days);
    }

    [HttpGet("matches/{id}")]
    public async Task<ActionResult<FootballMatchDto>> GetMatch(string id)
    {
        var match = await _footballService.GetMatchByIdAsync(id);
        if (match == null) return NotFound(new { message = $"Match with ID {id} not found." });
        return Ok(match);
    }

    [HttpPost("predict/{id}")]
    public async Task<ActionResult<FootballAiPredictionDto>> PredictMatch(string id)
    {
        var prediction = await _footballService.RunAiMatchPredictionAsync(id);
        if (prediction == null) return NotFound(new { message = $"Match with ID {id} not found." });
        return Ok(prediction);
    }

    [HttpPost("parlay")]
    public async Task<ActionResult<FootballParlayResponseDto>> GenerateParlay([FromBody] FootballParlayRequestDto request)
    {
        var result = await _footballService.GenerateMixParlayAsync(request);
        return Ok(result);
    }
}
