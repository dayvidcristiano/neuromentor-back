namespace NeuroMentor.Api.Models;

public class ClassRoom
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = "";
    public string Code { get; set; } = "";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Guid TeacherId { get; set; }
    public User Teacher { get; set; } = null!;

    public ICollection<ClassLesson> ClassLessons { get; set; } = [];
    public ICollection<ClassStudent> Students { get; set; } = [];
}

public class ClassLesson
{
    public Guid ClassRoomId { get; set; }
    public ClassRoom ClassRoom { get; set; } = null!;

    public Guid LessonId { get; set; }
    public Lesson Lesson { get; set; } = null!;
}

public class ClassStudent
{
    public Guid ClassRoomId { get; set; }
    public ClassRoom ClassRoom { get; set; } = null!;

    public Guid UserId { get; set; }
    public User User { get; set; } = null!;

    public DateTime JoinedAt { get; set; } = DateTime.UtcNow;
}
