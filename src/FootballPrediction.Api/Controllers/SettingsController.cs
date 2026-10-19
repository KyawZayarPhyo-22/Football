using FootballPrediction.Api.Services;
using FootballPrediction.Shared.Models;
using Microsoft.AspNetCore.Mvc;

namespace FootballPrediction.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SettingsController : ControllerBase
{
    private readonly ISettingsService _settingsService;

    public SettingsController(ISettingsService settingsService)
    {
        _settingsService = settingsService;
    }

    [HttpGet]
    public ActionResult<SystemSettingsDto> Get()
    {
        return Ok(_settingsService.GetSettings());
    }

    [HttpPost]
    public ActionResult<SystemSettingsDto> Update([FromBody] SystemSettingsDto settings)
    {
        _settingsService.UpdateSettings(settings);
        return Ok(_settingsService.GetSettings());
    }

    [HttpGet("test-gemini")]
    public async Task<ActionResult<GeminiTestResultDto>> TestGemini()
    {
        var result = await _settingsService.TestGeminiLiveAsync();
        return Ok(result);
    }
}
