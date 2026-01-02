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
    long TotalScore,
    int CurrentStreak,
    long WeekScore
);
