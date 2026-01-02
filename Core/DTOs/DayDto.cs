namespace PaceFriendsBackend.Core.DTOs;

public record DayDto(
    Guid? DayId,
    DateTime Date,
    long TotalSteps,
    long TotalCalories,
    long TimeSpentSeconds,
    List<RoutePointDto> RoutePoints
);
