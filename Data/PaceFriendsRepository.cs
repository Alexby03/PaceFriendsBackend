using Microsoft.EntityFrameworkCore;
using PaceFriendsBackend.Core.DTOs;
using PaceFriendsBackend.Core.Models;

namespace PaceFriendsBackend.Data;

public class PaceFriendsRepository
{
    private readonly PaceFriendsDbContext _dbContext;

    public PaceFriendsRepository(PaceFriendsDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    // =================================================================
    // Register User
    // =================================================================
    public async Task<ServiceResult<PlayerDto>> RegisterUserAsync(PlayerDto dto)
    {
        // Check if email already exists
        if (await _dbContext.Players.AnyAsync(p => p.Email == dto.Email))
        {
            return ServiceResult<PlayerDto>.Fail("Email already exists", 409);
        }

        var newPlayer = new Player
        {
            PlayerID = Guid.NewGuid(),
            FullName = dto.FullName,
            Email = dto.Email,
            Password = dto.Password, 
            Age = dto.Age,
            WeightKg = dto.WeightKg,
            HeightCm = dto.HeightCm,
            Gender = dto.Gender,
            LastUpdated = DateTime.UtcNow,
            TotalScore = 0,
            WeekScore = 0,
            CurrentStreak = 0,
            WeeklySteps = 0,
            CompletedDaily = false,
            TotalTimePlayed = 0,
        };

        _dbContext.Players.Add(newPlayer);
        await _dbContext.SaveChangesAsync();

        return ServiceResult<PlayerDto>.Ok(MapToPlayerDto(newPlayer));
    }

    // =================================================================
    // Update User
    // =================================================================
    public async Task<ServiceResult<PlayerDto>> UpdateUserAsync(PlayerDto dto)
    {
        if (dto.PlayerId == null) 
            return ServiceResult<PlayerDto>.Fail("Player ID is required for update", 400);

        var player = await _dbContext.Players.FindAsync(dto.PlayerId.Value);
        if (player == null) 
            return ServiceResult<PlayerDto>.Fail("Player not found", 404);

        // Update fields
        player.FullName = dto.FullName;
        player.Age = dto.Age;
        player.Gender = dto.Gender;
        player.HeightCm = dto.HeightCm;
        player.WeightKg = dto.WeightKg;
        
        if (!string.IsNullOrEmpty(dto.Password))
        {
            player.Password = dto.Password; 
        }

        player.LastUpdated = DateTime.UtcNow;

        await _dbContext.SaveChangesAsync();

        return ServiceResult<PlayerDto>.Ok(MapToPlayerDto(player));
    }

    // =================================================================
    // Login
    // =================================================================
    public async Task<ServiceResult<PlayerDto>> LoginAsync(PlayerLoginDto loginDto)
    {
        var player = await _dbContext.Players
            .FirstOrDefaultAsync(p => p.Email == loginDto.Email);

        if (player == null)
            return ServiceResult<PlayerDto>.Fail("Invalid credentials", 401);
        
        if (player.Password != loginDto.Password)
            return ServiceResult<PlayerDto>.Fail("Invalid credentials", 401);

        return ServiceResult<PlayerDto>.Ok(MapToPlayerDto(player));
    }

    // =================================================================
    // Add / Update Day
    // =================================================================
    public async Task<ServiceResult<bool>> ProcessActivitySessionAsync(Guid playerId, DayEntryDto sessionData)
    {
        
        using var transaction = await _dbContext.Database.BeginTransactionAsync();

        try
        {
            var player = await _dbContext.Players.FindAsync(playerId);
            if (player == null) return ServiceResult<bool>.Fail("Player not found", 404);
            
            // Decide whether to POST or PUT
            var todayDate = sessionData.Date.Date;
            var existingDay = await _dbContext.Days
                .FirstOrDefaultAsync(d => d.PlayerID == playerId && d.Date == todayDate);

            if (existingDay == null)
            {
                // First session of the day
                var newDay = new Day
                {
                    DayID = Guid.NewGuid(),
                    PlayerID = playerId,
                    Date = todayDate,
                    TotalSteps = sessionData.TotalSteps,
                    TotalCalories = sessionData.TotalCalories,
                    TimeSpent = sessionData.TimeSpentSeconds,
                    Score = sessionData.Score,
                    RoutePoints = sessionData.RoutePoints.Select(rp => new RoutePoint
                    {
                        Latitude = rp.Latitude,
                        Longitude = rp.Longitude,
                        SequenceOrder = rp.SequenceOrder
                    }).ToList()
                };
                
                _dbContext.Days.Add(newDay);
                
                // update streak
                if (!player.CompletedDaily)
                {
                    player.CompletedDaily = true;
                    player.CurrentStreak += 1;
                }
            }
            else
            {
                existingDay.TotalSteps += sessionData.TotalSteps;
                existingDay.TotalCalories += sessionData.TotalCalories;
                existingDay.TimeSpent += sessionData.TimeSpentSeconds;
                existingDay.Score += sessionData.Score;

                var maxOrder = await _dbContext.RoutePoints
                    .Where(rp => rp.DayID == existingDay.DayID)
                    .MaxAsync(rp => (int?)rp.SequenceOrder) ?? -1; 
                
                var startOrder = maxOrder + 1;
                var newPoints = sessionData.RoutePoints.Select((rp, index) => new RoutePoint
                {
                    DayID = existingDay.DayID,
                    Latitude = rp.Latitude,
                    Longitude = rp.Longitude,
                    SequenceOrder = startOrder + index // update sequence of subsequent indexes
                });

                _dbContext.RoutePoints.AddRange(newPoints);
            }
            
            player.WeekScore += sessionData.Score;
            player.TotalScore += sessionData.Score;
            player.TotalTimePlayed += sessionData.TimeSpentSeconds;
            player.WeeklySteps += sessionData.TotalSteps;
            player.LastUpdated = DateTime.UtcNow;

            await _dbContext.SaveChangesAsync();
            await transaction.CommitAsync();

            return ServiceResult<bool>.Ok(true);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            return ServiceResult<bool>.Fail($"Database error: {ex.Message}", 500);
        }
    }

    // =================================================================
    // Get all days in a month
    // =================================================================
    public async Task<ServiceResult<List<DaySummaryDto>>> GetCalendarDaysAsync(Guid playerId, int year, int month)
    {
        var startDate = new DateTime(year, month, 1);
        var endDate = startDate.AddMonths(1).AddDays(-1);

        var days = await _dbContext.Days
            .Where(d => d.PlayerID == playerId && d.Date >= startDate && d.Date <= endDate)
            .OrderBy(d => d.Date)
            .Select(d => new DaySummaryDto(
                d.DayID,
                d.Date,
                d.TotalSteps,
                d.Score,
                true
            ))
            .ToListAsync();

        return ServiceResult<List<DaySummaryDto>>.Ok(days);
    }

    // =================================================================
    // Get a Day eagerly
    // =================================================================
    public async Task<ServiceResult<DayDetailDto>> GetDayDetailAsync(Guid dayId)
    {
        var day = await _dbContext.Days
            .Include(d => d.RoutePoints.OrderBy(rp => rp.SequenceOrder))
            .FirstOrDefaultAsync(d => d.DayID == dayId);

        if (day == null)
            return ServiceResult<DayDetailDto>.Fail("Day entry not found", 404);

        var dto = new DayDetailDto(
            day.DayID,
            day.Date,
            day.TotalSteps,
            day.TotalCalories,
            day.RoutePoints.Select(rp => new RoutePointDto(
                rp.Latitude,
                rp.Longitude,
                rp.SequenceOrder
            )).ToList()
        );

        return ServiceResult<DayDetailDto>.Ok(dto);
    }

    // =================================================================
    // Get Leaderboard
    // =================================================================
    public async Task<ServiceResult<List<LeaderboardEntryDto>>> GetLeaderboardAsync()
    {
        var dtos = await _dbContext.Players
            .Where(p => p.WeekScore > 0)
            .OrderByDescending(p => p.WeekScore)
            .Take(50)
            .Select(p => new LeaderboardEntryDto(
                p.PlayerID,
                p.FullName,
                p.WeekScore
            ))
            .ToListAsync();

        return ServiceResult<List<LeaderboardEntryDto>>.Ok(dtos);
    }

    // =================================================================
    // Mapper
    // =================================================================
    private static PlayerDto MapToPlayerDto(Player p)
    {
        return new PlayerDto(
            p.PlayerID,
            p.FullName,
            p.Email,
            null,
            p.Age,
            p.HeightCm,
            p.WeightKg,
            p.Gender,
            p.TotalScore,
            p.CurrentStreak,
            p.WeekScore
        );
    }
}