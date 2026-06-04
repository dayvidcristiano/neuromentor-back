using Microsoft.EntityFrameworkCore;
using NeuroMentor.Api.Models;

namespace NeuroMentor.Api.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Lesson> Lessons { get; set; }
        public DbSet<LessonModule> LessonModules { get; set; }
        public DbSet<ClassRoom> Classes { get; set; }
        public DbSet<ClassStudent> ClassStudents { get; set; }
        public DbSet<ClassLesson> ClassLessons { get; set; }
        public DbSet<ExerciseAttempt> ExerciseAttempts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ClassLesson>()
                .HasKey(cl => new { cl.ClassRoomId, cl.LessonId });

            modelBuilder.Entity<ClassStudent>()
                .HasKey(cs => new { cs.ClassRoomId, cs.UserId });
        }
    }
}