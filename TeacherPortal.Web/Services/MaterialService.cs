using Microsoft.EntityFrameworkCore;
using TeacherPortal.Web.Data;
using TeacherPortal.Web.Models.ViewModels;

namespace TeacherPortal.Web.Services;

public class MaterialService : IMaterialService
{
    private readonly ApplicationDbContext _context;

    public MaterialService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<GroupViewModel>> GetGroupsAsync()
    {
        return await _context.Groups
            .Include(g => g.Course)
            .Select(g => new GroupViewModel
            {
                Id = g.Id,
                Name = g.Name,
                CourseName = g.Course.Name
            })
            .OrderBy(g => g.CourseName)
            .ThenBy(g => g.Name)
            .ToListAsync();
    }

    public async Task<GroupLessonsViewModel> GetGroupLessonsWithMaterialsAsync(int groupId)
    {
        var group = await _context.Groups
            .Include(g => g.Course)
            .Include(g => g.Lessons)
                .ThenInclude(l => l.Materials)
            .FirstOrDefaultAsync(g => g.Id == groupId);

        if (group == null)
        {
            return null!;
        }

        return new GroupLessonsViewModel
        {
            GroupId = group.Id,
            GroupName = group.Name,
            CourseName = group.Course.Name,
            Lessons = group.Lessons
                .OrderBy(l => l.LessonNumber)
                .Select(l => new LessonWithMaterialsViewModel
                {
                    Id = l.Id,
                    LessonNumber = l.LessonNumber,
                    DateIssued = l.DateIssued,
                    Topic = l.Topic ?? $"Занятие {l.LessonNumber}",
                    Materials = l.Materials
                        .Select(m => new MaterialViewModel
                        {
                            Id = m.Id,
                            Title = m.Title,
                            Description = m.Description,
                            FilePath = m.FilePath,
                            UploadedAt = m.UploadedAt
                        })
                        .ToList()
                })
                .ToList()
        };
    }
}