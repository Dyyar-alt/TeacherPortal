using Microsoft.EntityFrameworkCore;
using TeacherPortal.Web.Data;
using TeacherPortal.Web.Models.Entities;
using TeacherPortal.Web.Models.ViewModels;

namespace TeacherPortal.Web.Services;

public class LessonCounterService : ILessonCounterService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<LessonCounterService> _logger;

    public LessonCounterService(ApplicationDbContext context, ILogger<LessonCounterService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<LessonCounterViewModel> GetGroupProgressAsync(int groupId)
    {
        var group = await _context.Groups
            .Include(g => g.Course)
            .Include(g => g.Lessons)
            .FirstOrDefaultAsync(g => g.Id == groupId);

        if (group == null)
        {
            return null!;
        }

        var totalLessons = group.Course.TotalLessons;
        var issuedLessons = group.CurrentLessonNumber;
        var remainingLessons = totalLessons - issuedLessons;

        return new LessonCounterViewModel
        {
            GroupId = group.Id,
            GroupName = group.Name,
            CourseName = group.Course.Name,
            TotalLessons = totalLessons,
            IssuedLessons = issuedLessons,
            RemainingLessons = remainingLessons,
            ProgressPercentage = totalLessons > 0
                ? Math.Round((double)issuedLessons / totalLessons * 100, 2)
                : 0,
            IsComplete = issuedLessons >= totalLessons
        };
    }

    public async Task<bool> IssueNextLessonAsync(int groupId)
    {
        var group = await _context.Groups
            .Include(g => g.Course)
            .FirstOrDefaultAsync(g => g.Id == groupId);

        if (group == null || group.CurrentLessonNumber >= group.Course.TotalLessons)
        {
            return false;
        }

        var nextLessonNumber = group.CurrentLessonNumber + 1;

        var lesson = new Lesson
        {
            GroupId = groupId,
            LessonNumber = nextLessonNumber,
            DateIssued = DateTime.UtcNow,
            IsCompleted = false,
            Topic = $"Занятие {nextLessonNumber}"
        };

        group.CurrentLessonNumber = nextLessonNumber;

        await _context.Lessons.AddAsync(lesson);
        await _context.SaveChangesAsync();

        _logger.LogInformation($"Выдана пара №{nextLessonNumber} для группы {group.Name}");
        return true;
    }

    public async Task<int> GetTotalLessonsIssuedAsync(int courseId)
    {
        return await _context.Groups
            .Where(g => g.CourseId == courseId)
            .SumAsync(g => g.CurrentLessonNumber);
    }
}