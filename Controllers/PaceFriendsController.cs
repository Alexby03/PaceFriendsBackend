using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using PaceFriendsBackend.Core.DTOs;
using PaceFriendsBackend.Data;
using System.Net.Http;

namespace PaceFriendsBackend.Controllers;

[ApiController]
[Route("api")]
public class PaceFriendsController : ControllerBase
{
    private readonly PaceFriendsRepository _repository;
    private readonly ILogger<PaceFriendsController> _logger;
    private readonly IConfiguration _configuration;

    public PaceFriendsController(
        PaceFriendsRepository repository, 
        ILogger<PaceFriendsController> logger, 
        IConfiguration configuration)
    {
        _repository = repository;
        _logger = logger;
        _configuration = configuration;
    }

    // =================================================================
    // DEBUG ENDPOINT (Check Outbound IP)
    // =================================================================
    [HttpGet("debug-ip")]
    public async Task<IActionResult> GetDebugInfo()
    {
        _logger.LogInformation("Starting IP debug check...");
        string outboundIp = "Unknown";

        try 
        {
            using var client = new HttpClient();
            outboundIp = await client.GetStringAsync("https://api.ipify.org");
            _logger.LogInformation($"DETECTED OUTBOUND IP: {outboundIp}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Failed to detect IP: {ex.Message}");
            return StatusCode(500, new { error = "Failed to detect IP", details = ex.Message });
        }

        // Check connection string (masked)
        var connString = _configuration.GetConnectionString("DefaultConnection");
        var maskedConnString = connString?.Replace("Pwd=Admin123_", "Pwd=*****"); 
        
        _logger.LogInformation($"CONNECTION STRING USED: {maskedConnString}");

        return Ok(new { 
            ip = outboundIp, 
            connectionString = maskedConnString 
        });
    }

    // =================================================================
    // Register, Login, Update Profile
    // =================================================================

    [HttpPost("auth/register")]
    public async Task<IActionResult> Register([FromBody] PlayerDto dto)
    {
        var result = await _repository.RegisterUserAsync(dto);
        if (!result.Success) return StatusCode(result.ErrorCode, result.ErrorMessage);
        
        return Ok(result.Data);
    }

    [HttpPost("auth/login")]
    public async Task<IActionResult> Login([FromBody] PlayerLoginDto dto)
    {
        var result = await _repository.LoginAsync(dto);
        if (!result.Success) return StatusCode(result.ErrorCode, result.ErrorMessage);

        return Ok(result.Data);
    }

    [HttpPut("profile")]
    public async Task<IActionResult> UpdateProfile([FromBody] PlayerDto dto)
    {
        var result = await _repository.UpdateUserAsync(dto);
        if (!result.Success) return StatusCode(result.ErrorCode, result.ErrorMessage);

        return Ok(result.Data);
    }

    // =================================================================
    // Create / Update Day
    // =================================================================

    // URL: POST /api/activity/sync?playerId={guid}
    [HttpPost("activity/sync")]
    public async Task<IActionResult> SyncActivity(Guid playerId, [FromBody] DayEntryDto sessionData)
    {
        var result = await _repository.ProcessActivitySessionAsync(playerId, sessionData);
        if (!result.Success) return StatusCode(result.ErrorCode, result.ErrorMessage);

        return Ok(new { message = "Activity synced successfully" });
    }

    // =================================================================
    // Get Calendar, Get Day
    // =================================================================

    // URL: GET /api/calendar?playerId={guid}&year=2026&month=1
    [HttpGet("calendar")]
    public async Task<IActionResult> GetCalendar(Guid playerId, int year, int month)
    {
        var result = await _repository.GetCalendarDaysAsync(playerId, year, month);
        if (!result.Success) return StatusCode(result.ErrorCode, result.ErrorMessage);

        return Ok(result.Data);
    }

    // URL: GET /api/days/{dayId}
    [HttpGet("day")]
    public async Task<IActionResult> GetDayDetail([FromQuery] DateTime date, [FromQuery] Guid playerId)
    {
        var result = await _repository.GetDayDetailAsync(date, playerId);
        if (!result.Success) return StatusCode(result.ErrorCode, result.ErrorMessage);
        return Ok(result.Data);
    }

    // =================================================================
    // Get Leaderboard
    // =================================================================

    [HttpGet("leaderboard")]
    public async Task<IActionResult> GetLeaderboard()
    {
        var result = await _repository.GetLeaderboardAsync();
        if (!result.Success) return StatusCode(result.ErrorCode, result.ErrorMessage);

        return Ok(result.Data);
    }

    [HttpGet("winner")]
    public async Task<IActionResult> GetWinner()
    {
        var result = await _repository.GetWeeklyWinnerAsync();
        if (!result.Success) return StatusCode(result.ErrorCode, result.ErrorMessage);
        if (result.Data == null) return StatusCode(result.ErrorCode, "Winner is null.");
        return Ok(result.Data);
    }

    [HttpGet("player/{playerId:guid}")]
    public async Task<IActionResult> GetPlayer(Guid playerId)
    {
        var result = await _repository.GetPlayerById(playerId);
        if (!result.Success) return StatusCode(result.ErrorCode, result.ErrorMessage);
        return Ok(result.Data);
    }
}
