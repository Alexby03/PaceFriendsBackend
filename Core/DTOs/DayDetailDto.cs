namespace PaceFriendsBackend.Core.DTOs;

public record DayDetailDto(
    Guid DayId,
    DateTime Date,
    long Steps,
    long Calories,
    long Score,
    List<RoutePointDto> RoutePoints
);
