using TeacherPortal.Web.Models.Entities;

namespace TeacherPortal.Web.Data;

public static class DbInitializer
{
    public static async Task InitializeAsync(ApplicationDbContext context)
    {
        // Проверяем, есть ли уже филиалы
        if (context.Filials.Any())
        {
            return;
        }

        // ========================================
        // 1. СОЗДАЕМ 6 ФИЛИАЛОВ
        // ========================================
        var filials = new[]
        {
            new Filial { Name = "Условный Ф 1", Address = "Адрес филиала 1", Phone = "+7 (111) 111-11-11", CreatedAt = DateTime.UtcNow },
            new Filial { Name = "Условный Ф 2", Address = "Адрес филиала 2", Phone = "+7 (222) 222-22-22", CreatedAt = DateTime.UtcNow },
            new Filial { Name = "Условный Ф 3", Address = "Адрес филиала 3", Phone = "+7 (333) 333-33-33", CreatedAt = DateTime.UtcNow },
            new Filial { Name = "Условный Ф 4", Address = "Адрес филиала 4", Phone = "+7 (444) 444-44-44", CreatedAt = DateTime.UtcNow },
            new Filial { Name = "Условный Ф 5", Address = "Адрес филиала 5", Phone = "+7 (555) 555-55-55", CreatedAt = DateTime.UtcNow },
            new Filial { Name = "Условный Ф 6", Address = "Адрес филиала 6", Phone = "+7 (666) 666-66-66", CreatedAt = DateTime.UtcNow }
        };

        await context.Filials.AddRangeAsync(filials);
        await context.SaveChangesAsync();

        // ========================================
        // 2. СОЗДАЕМ КУРСЫ (ПРИВЯЗЫВАЕМ К ФИЛИАЛАМ)
        // ========================================
        var filial1 = filials[0]; // Условный Ф 1
        var filial2 = filials[1]; // Условный Ф 2
        var filial3 = filials[2]; // Условный Ф 3

        var courses = new[]
        {
            new Course
            {
                Name = "ASP.NET Core Разработка",
                Code = "IT-401",
                TotalLessons = 32,
                FilialId = filial1.Id,
                CreatedAt = DateTime.UtcNow
            },
            new Course
            {
                Name = "Базы данных SQL",
                Code = "IT-302",
                TotalLessons = 28,
                FilialId = filial1.Id,
                CreatedAt = DateTime.UtcNow
            },
            new Course
            {
                Name = "Frontend Разработка (React)",
                Code = "IT-403",
                TotalLessons = 30,
                FilialId = filial2.Id,
                CreatedAt = DateTime.UtcNow
            },
            new Course
            {
                Name = "Python для анализа данных",
                Code = "IT-501",
                TotalLessons = 24,
                FilialId = filial2.Id,
                CreatedAt = DateTime.UtcNow
            },
            new Course
            {
                Name = "DevOps практика",
                Code = "IT-601",
                TotalLessons = 20,
                FilialId = filial3.Id,
                CreatedAt = DateTime.UtcNow
            }
        };

        await context.Courses.AddRangeAsync(courses);
        await context.SaveChangesAsync();

        // ========================================
        // 3. СОЗДАЕМ ГРУППЫ
        // ========================================
        var aspNetCourse = courses[0]; // ASP.NET Core
        var sqlCourse = courses[1];    // Базы данных SQL
        var reactCourse = courses[2];  // Frontend

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
            },
            new Group
            {
                Name = "Группа 42-БД-1",
                CourseId = sqlCourse.Id,
                CurrentLessonNumber = 0,
                CreatedAt = DateTime.UtcNow
            },
            new Group
            {
                Name = "Группа 43-ФР-1",
                CourseId = reactCourse.Id,
                CurrentLessonNumber = 0,
                CreatedAt = DateTime.UtcNow
            }
        };

        await context.Groups.AddRangeAsync(groups);
        await context.SaveChangesAsync();

        // ========================================
        // 4. ДОБАВЛЯЕМ СТУДЕНТОВ (для первых двух групп)
        // ========================================
        var group1 = groups[0]; // 41-ИС-1
        var group2 = groups[1]; // 41-ИС-2

        var students = new[]
        {
            new Student { FullName = "Иванов Иван Иванович", Email = "ivanov@college.edu", GroupId = group1.Id },
            new Student { FullName = "Петрова Мария Сергеевна", Email = "petrova@college.edu", GroupId = group1.Id },
            new Student { FullName = "Сидоров Алексей Дмитриевич", Email = "sidorov@college.edu", GroupId = group1.Id },
            new Student { FullName = "Козлова Елена Андреевна", Email = "kozlova@college.edu", GroupId = group1.Id },
            new Student { FullName = "Морозов Денис Алексеевич", Email = "morozov@college.edu", GroupId = group2.Id },
            new Student { FullName = "Волкова Анна Сергеевна", Email = "volkova@college.edu", GroupId = group2.Id }
        };

        await context.Students.AddRangeAsync(students);
        await context.SaveChangesAsync();

        Console.WriteLine($"✅ База данных инициализирована! Филиалов: {filials.Length}, Курсов: {courses.Length}, Групп: {groups.Length}, Студентов: {students.Length}");
    }
}