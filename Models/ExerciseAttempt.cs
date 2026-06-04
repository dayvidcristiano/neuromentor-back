namespace NeuroMentor.Api.Models;

public enum ReviewStatus
{
    AutoApproved = 0,
    PendingReview = 1,
    Accepted = 2,
    Rejected = 3,
}

public class ExerciseAttempt
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Question { get; set; } = "";
    public string Answer { get; set; } = "";
    public bool IsCorrect { get; set; }
    public string Feedback { get; set; } = "";
    public string? TeacherExplanation { get; set; }
    public ReviewStatus ReviewStatus { get; set; } = ReviewStatus.AutoApproved;
    public int XpGained { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Guid UserId { get; set; }
    public User User { get; set; } = null!;

    public Guid LessonId { get; set; }
    public string ModuleId { get; set; } = "";
}
