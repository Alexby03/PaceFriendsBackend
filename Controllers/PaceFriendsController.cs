using Microsoft.AspNetCore.Mvc;
using PaceFriendsBackend.Core.DTOs;
using PaceFriendsBackend.Data;

namespace PaceFriendsBackend.Controllers;


[ApiController]
[Route("api")]
public class PaceFriendsController : ControllerBase
{
    private readonly PaceFriendsRepository _repository;

    public PaceFriendsController(PaceFriendsRepository repository)
    {
        _repository = repository;
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
    [HttpGet("days/{dayId}")]
    public async Task<IActionResult> GetDayDetail(Guid dayId)
    {
        var result = await _repository.GetDayDetailAsync(dayId);
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
}
