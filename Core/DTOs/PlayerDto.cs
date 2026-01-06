namespace PaceFriendsBackend.Core.DTOs;

public record PlayerDto(
    Guid? PlayerId,
    string FullName,
    string Email,
    string? Password,
    int Age, 
    double HeightCm,
    double WeightKg,
    string Gender,
    int CurrentStreak,
    bool CompletedDaily,
    long WeekScore,
    long TotalTimePlayed,
    long WeeklySteps,
    DateTime LastUpdated,
    long TotalScore
);
