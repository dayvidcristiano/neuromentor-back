namespace NeuroMentor.Api.DTOs.Exercises;

public record GenerateExercisesRequest(Guid? LessonId, string ModuleTitle, string? Context, int Count = 3);
public record CorrectExerciseRequest(string Question, string Answer, string? Context);
public record RecordAttemptRequest(Guid? LessonId, string ModuleId, string Question, string Answer, bool IsCorrect, string Feedback, string? TeacherExplanation = null, bool PendingReview = false);
public record ReviewAttemptRequest(string Status, string? Note = null);
public record GenerateReviewRequest(Guid? LessonId, string? Context, List<string> WrongAnswers);
public record ChatRequest(List<ChatMessage> Messages, string? Context, Guid? ModuleId = null);
public record ChatMessage(string Role, object Content);
