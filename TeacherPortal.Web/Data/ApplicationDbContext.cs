using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TeacherPortal.Web.Models.Entities;

namespace TeacherPortal.Web.Data;

public class ApplicationDbContext : IdentityDbContext<AppUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Filial> Filials { get; set; }
    public DbSet<Course> Courses { get; set; }
    public DbSet<Group> Groups { get; set; }
    public DbSet<Lesson> Lessons { get; set; }
    public DbSet<Material> Materials { get; set; }
    public DbSet<Student> Students { get; set; }
    public DbSet<TeacherInvite> TeacherInvites { get; set; }

    public static async Task InitializeAsync(ApplicationDbContext context)
    {
        if (context.Filials.Any())
        {
            return;
        }

        // 1. Создаем филиалы
        var filials = new[]
        {
        new Filial { Name = "Условный Ф 1", Address = "Адрес филиала 1", CreatedAt = DateTime.UtcNow },
        new Filial { Name = "Условный Ф 2", Address = "Адрес филиала 2", CreatedAt = DateTime.UtcNow },
        new Filial { Name = "Условный Ф 3", Address = "Адрес филиала 3", CreatedAt = DateTime.UtcNow },
        new Filial { Name = "Условный Ф 4", Address = "Адрес филиала 4", CreatedAt = DateTime.UtcNow },
        new Filial { Name = "Условный Ф 5", Address = "Адрес филиала 5", CreatedAt = DateTime.UtcNow },
        new Filial { Name = "Условный Ф 6", Address = "Адрес филиала 6", CreatedAt = DateTime.UtcNow }
    };

        await context.Filials.AddRangeAsync(filials);
        await context.SaveChangesAsync();

        // 2. Теперь создаем курсы, привязывая их к филиалам
        var filial1 = filials[0];
        var filial2 = filials[1];

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
        }
    };

   
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
     modelBuilder.Entity<Course>()
    .HasOne(c => c.Filial)
    .WithMany(f => f.Courses)
    .HasForeignKey(c => c.FilialId)
    .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Group>()
            .HasOne(g => g.Course)
            .WithMany(c => c.Groups)
            .HasForeignKey(g => g.CourseId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Lesson>()
            .HasOne(l => l.Group)
            .WithMany(g => g.Lessons)
            .HasForeignKey(l => l.GroupId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Material>()
            .HasOne(m => m.Lesson)
            .WithMany(l => l.Materials)
            .HasForeignKey(m => m.LessonId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Student>()
            .HasOne(s => s.Group)
            .WithMany(g => g.Students)
            .HasForeignKey(s => s.GroupId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Lesson>()
            .HasIndex(l => new { l.GroupId, l.LessonNumber })
            .IsUnique();
    }
}