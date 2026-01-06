namespace PaceFriendsBackend.Core.DTOs;

public record WeeklyWinnerDto(
    Guid PlayerId,
    string FullName,
    long Score
);