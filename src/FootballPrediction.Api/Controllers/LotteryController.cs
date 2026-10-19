using FootballPrediction.Api.Services;
using FootballPrediction.Shared.Models;
using Microsoft.AspNetCore.Mvc;

namespace FootballPrediction.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LotteryController : ControllerBase
{
    private readonly ILotteryService _lotteryService;

    public LotteryController(ILotteryService lotteryService)
    {
        _lotteryService = lotteryService;
    }

    [HttpGet("2d/live")]
    public async Task<ActionResult<TwoDLiveDto>> GetTwoDLive()
    {
        var live = await _lotteryService.GetTwoDLiveAsync();
        return Ok(live);
    }

    [HttpGet("2d/history")]
    public async Task<ActionResult<List<TwoDHistoryRecordDto>>> GetTwoDHistory()
    {
        var history = await _lotteryService.GetTwoDHistoryAsync();
        return Ok(history);
    }

    [HttpPost("2d/predict")]
    public async Task<ActionResult<TwoDAiPredictionDto>> PredictTwoD([FromQuery] string session = "Full Day", [FromQuery] string? targetDate = null, [FromQuery] bool forceRefresh = false)
    {
        var prediction = await _lotteryService.PredictTwoDAsync(session, targetDate, forceRefresh);
        return Ok(prediction);
    }

    [HttpGet("3d/history")]
    public async Task<ActionResult<List<ThreeDHistoryRecordDto>>> GetThreeDHistory()
    {
        var history = await _lotteryService.GetThreeDHistoryAsync();
        return Ok(history);
    }

    [HttpPost("3d/predict")]
    public async Task<ActionResult<ThreeDAiPredictionDto>> PredictThreeD([FromQuery] string? targetDate = null, [FromQuery] bool forceRefresh = false)
    {
        var prediction = await _lotteryService.PredictThreeDAsync(targetDate, forceRefresh);
        return Ok(prediction);
    }
}
