using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TeacherPortal.Web.Data;
using TeacherPortal.Web.Models.ViewModels.Admin;

namespace TeacherPortal.Web.Pages.Admin.Groups;

[Authorize(Roles = "Admin")]
public class IndexModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public IndexModel(ApplicationDbContext context)
    {
        _context = context;
    }

    public List<GroupAdminViewModel> Groups { get; set; } = new();
    public List<FilialAdminViewModel> Filials { get; set; } = new();
    public List<CourseAdminViewModel> Courses { get; set; } = new();
    public int? SelectedFilialId { get; set; }
    public int? SelectedCourseId { get; set; }

    public async Task OnGetAsync(int? filialId, int? courseId)
    {
        SelectedFilialId = filialId;
        SelectedCourseId = courseId;

        Filials = await _context.Filials
            .Select(f => new FilialAdminViewModel
            {
                Id = f.Id,
                Name = f.Name,
                Address = f.Address,
                Phone = f.Phone
            })
            .OrderBy(f => f.Name)
            .ThenBy(f => f.Address)
            .ToListAsync();

        Courses = await _context.Courses
            .Include(c => c.Filial)
            .Select(c => new CourseAdminViewModel
            {
                Id = c.Id,
                Name = c.Name,
                FilialName = c.Filial.Name,
                FilialAddress = c.Filial.Address ?? ""
            })
            .OrderBy(c => c.FilialName)
            .ThenBy(c => c.Name)
            .ToListAsync();

        var query = _context.Groups
            .Include(g => g.Course)
            .ThenInclude(c => c.Filial)
            .Include(g => g.Students)
            .AsQueryable();

        if (filialId.HasValue)
        {
            query = query.Where(g => g.Course.FilialId == filialId.Value);
        }

        if (courseId.HasValue)
        {
            query = query.Where(g => g.CourseId == courseId.Value);
        }

        Groups = await query
            .Select(g => new GroupAdminViewModel
            {
                Id = g.Id,
                Name = g.Name,
                CourseId = g.CourseId,
                CourseName = g.Course.Name,
                FilialName = g.Course.Filial.Name,
                FilialAddress = g.Course.Filial.Address ?? "",
                StudentsCount = g.Students.Count
            })
            .OrderBy(g => g.FilialName)
            .ThenBy(g => g.CourseName)
            .ThenBy(g => g.Name)
            .ToListAsync();
    }
}

public class CourseAdminViewModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string FilialName { get; set; } = string.Empty;
    public string FilialAddress { get; set; } = string.Empty;
}