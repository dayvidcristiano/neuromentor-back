namespace NeuroMentor.Api.Models;

public class Lesson
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Title { get; set; } = "";
    public string SourceFileName { get; set; } = "";
    public string RawText { get; set; } = "";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Guid TeacherId { get; set; }
    public User Teacher { get; set; } = null!;

    public ICollection<LessonModule> Modules { get; set; } = [];
    public ICollection<ClassLesson> ClassLessons { get; set; } = [];
}

public enum ModuleStatus { Pending, Approved, Rejected }

public class LessonModule
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Title { get; set; } = "";
    public string Summary { get; set; } = "";
    public List<string> Concepts { get; set; } = [];
    public double MatchScore { get; set; }
    public ModuleStatus Status { get; set; } = ModuleStatus.Pending;
    public int Order { get; set; }
    public string TextChunk { get; set; } = "";

    public Guid LessonId { get; set; }
    public Lesson Lesson { get; set; } = null!;
}
