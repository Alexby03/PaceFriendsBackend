namespace PaceFriendsBackend.Core.DTOs;

public record DayDetailDto(
    Guid DayId,
    DateTime Date,
    long Steps,
    long Calories,
    List<RoutePointDto> RoutePoints
);
