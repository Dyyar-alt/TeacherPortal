using TeacherPortal.Web.Models.Entities;

namespace TeacherPortal.Web.Data;

public static class DbInitializer
{
    public static async Task InitializeAsync(ApplicationDbContext context)
    {
        if (context.Courses.Any())
        {
            return;
        }

        var courses = new[]
        {
            new Course
            {
                Name = "ASP.NET Core Разработка",
                Code = "IT-401",
                TotalLessons = 32,
                CreatedAt = DateTime.UtcNow
            },
            new Course
            {
                Name = "Базы данных SQL",
                Code = "IT-302",
                TotalLessons = 28,
                CreatedAt = DateTime.UtcNow
            },
            new Course
            {
                Name = "Frontend Разработка (React)",
                Code = "IT-403",
                TotalLessons = 30,
                CreatedAt = DateTime.UtcNow
            }
        };

        await context.Courses.AddRangeAsync(courses);
        await context.SaveChangesAsync();

        var aspNetCourse = courses[0];
        var groups = new[]
        {
            new Group
            {
                Name = "Группа 41-ИС-1",
                CourseId = aspNetCourse.Id,
                CurrentLessonNumber = 0,
                CreatedAt = DateTime.UtcNow
            },
            new Group
            {
                Name = "Группа 41-ИС-2",
                CourseId = aspNetCourse.Id,
                CurrentLessonNumber = 0,
                CreatedAt = DateTime.UtcNow
            }
        };

        await context.Groups.AddRangeAsync(groups);
        await context.SaveChangesAsync();

        var firstGroup = groups[0];
        var students = new[]
        {
            new Student { FullName = "Иванов Иван Иванович", Email = "ivanov@college.edu", GroupId = firstGroup.Id },
            new Student { FullName = "Петрова Мария Сергеевна", Email = "petrova@college.edu", GroupId = firstGroup.Id },
            new Student { FullName = "Сидоров Алексей Дмитриевич", Email = "sidorov@college.edu", GroupId = firstGroup.Id },
            new Student { FullName = "Козлова Елена Андреевна", Email = "kozlova@college.edu", GroupId = firstGroup.Id }
        };

        await context.Students.AddRangeAsync(students);
        await context.SaveChangesAsync();

        Console.WriteLine("✅ База данных инициализирована тестовыми данными!");
    }
}