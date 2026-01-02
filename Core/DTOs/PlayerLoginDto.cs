namespace PaceFriendsBackend.Core.DTOs;

public record PlayerLoginDto(
    string Email,
    string Password
);