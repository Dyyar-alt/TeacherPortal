using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TeacherPortal.Web.Data;
using TeacherPortal.Web.Models.ViewModels.Admin;

namespace TeacherPortal.Web.Pages.Admin.Courses;

[Authorize(Roles = "Admin")]
public class IndexModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public IndexModel(ApplicationDbContext context)
    {
        _context = context;
    }

    public List<CourseAdminViewModel> Courses { get; set; } = new();
    public List<FilialAdminViewModel> Filials { get; set; } = new();
    public int? SelectedFilialId { get; set; }
    public string? SearchTerm { get; set; }

    public async Task OnGetAsync(int? filialId, string? search)
    {
        SelectedFilialId = filialId;
        SearchTerm = search;

        // Загружаем филиалы для фильтра
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

        // Загружаем курсы с фильтрацией
        var query = _context.Courses
            .Include(c => c.Filial)
            .Include(c => c.Groups)
            .AsQueryable();

        if (filialId.HasValue)
        {
            query = query.Where(c => c.FilialId == filialId.Value);
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(c => c.Name.Contains(search) || c.Code.Contains(search));
        }

        Courses = await query
            .Select(c => new CourseAdminViewModel
            {
                Id = c.Id,
                Name = c.Name,
                Code = c.Code,
                TotalLessons = c.TotalLessons,
                FilialId = c.FilialId,
                FilialName = c.Filial.Name,
                FilialAddress = c.Filial.Address ?? "",
                GroupsCount = c.Groups.Count
            })
            .OrderBy(c => c.FilialName)
            .ThenBy(c => c.Name)
            .ToListAsync();
    }
}