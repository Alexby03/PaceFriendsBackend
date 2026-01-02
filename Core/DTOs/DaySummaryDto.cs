namespace PaceFriendsBackend.Core.DTOs;

public record DaySummaryDto(
    Guid DayId,
    DateTime Date,
    long Steps,
    long Score,
    bool Completed
);
