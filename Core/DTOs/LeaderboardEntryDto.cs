namespace PaceFriendsBackend.Core.DTOs;

public record LeaderboardEntryDto(
    Guid PlayerId,
    string FullName,
    long Score
);