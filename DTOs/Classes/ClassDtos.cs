namespace NeuroMentor.Api.DTOs.Classes;

public record CreateClassRequest(string Name);
public record JoinClassRequest(string Code);
public record AddLessonToClassRequest(Guid LessonId, string Title);

public record ClassDto(
    Guid Id,
    string Name,
    string Code,
    string TeacherName,
    DateTime CreatedAt,
    List<ClassLessonDto> Lessons,
    List<ClassStudentDto> Students
);

public record ClassLessonDto(Guid Id, string Title);
public record ClassStudentDto(Guid Id, string Name, string Email, DateTime JoinedAt);
