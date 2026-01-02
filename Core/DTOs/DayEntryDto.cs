namespace PaceFriendsBackend.Core.DTOs;

public record DayEntryDto(
    DateTime Date,
    long TotalSteps,
    long TotalCalories,
    long TimeSpentSeconds,
    long Score,
    List<RoutePointDto> RoutePoints
);