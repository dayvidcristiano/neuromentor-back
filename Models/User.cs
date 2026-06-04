namespace NeuroMentor.Api.Models;

public enum UserRole { Student, Teacher }

public class User
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = "";
    public string Email { get; set; } = "";
    public string PasswordHash { get; set; } = "";
    public UserRole Role { get; set; }
    public string? PhotoUrl { get; set; }
    public string? Matricula { get; set; }
    public string? Subject { get; set; }
    public bool IsAiEnabled { get; set; } = false;
    public bool IsAdmin { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<Lesson> Lessons { get; set; } = [];
    public ICollection<ClassRoom> OwnedClasses { get; set; } = [];
    public ICollection<ClassStudent> ClassEnrollments { get; set; } = [];
    public ICollection<ExerciseAttempt> Attempts { get; set; } = [];
}
