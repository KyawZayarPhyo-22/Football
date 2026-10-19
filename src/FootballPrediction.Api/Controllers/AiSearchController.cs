using FootballPrediction.Api.Services;
using FootballPrediction.Shared.Models;
using Microsoft.AspNetCore.Mvc;

namespace FootballPrediction.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AiSearchController : ControllerBase
{
    private readonly IAiPredictionService _aiPredictionService;

    public AiSearchController(IAiPredictionService aiPredictionService)
    {
        _aiPredictionService = aiPredictionService;
    }

    [HttpPost("analyze")]
    public async Task<ActionResult<CustomAiSearchResponse>> Analyze([FromBody] CustomAiSearchRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Query))
        {
            return BadRequest(new { message = "Query cannot be empty." });
        }

        var response = await _aiPredictionService.AnalyzeCustomQueryAsync(request);
        return Ok(response);
    }
}
