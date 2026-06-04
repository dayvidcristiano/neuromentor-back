namespace NeuroMentor.Api.DTOs.Auth;

public record RegisterRequest(string Name, string Email, string Password, string Role);
public record LoginRequest(string Email, string Password);
public record ChangePasswordRequest(string CurrentPassword, string NewPassword);
public record UpdateProfileRequest(string Name, string? PhotoUrl, string? Matricula, string? Subject);

public record AuthResponse(
    string Token,
    string Id,
    string Name,
    string Email,
    string Role,
    string? PhotoUrl,
    string? Matricula,
    string? Subject,
    bool IsAiEnabled,
    bool IsAdmin
);
