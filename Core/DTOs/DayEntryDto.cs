using PaceFriendsBackend.Core.Utils;

namespace PaceFriendsBackend.Core.DTOs;

public record DayEntryDto(
    DateTime Date,
    long TotalSteps,
    long TotalCalories,
    long TimeSpentSeconds,
    double AreaInSquareMeters,
    long Score,
    Activity Activity,
    List<RoutePointDto> RoutePoints
);