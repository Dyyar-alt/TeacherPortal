using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TeacherPortal.Web.Data;
using TeacherPortal.Web.Models.ViewModels.Admin;

namespace TeacherPortal.Web.Pages.Admin.Students;

[Authorize(Policy = "AdminOnly")]
public class IndexModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public IndexModel(ApplicationDbContext context)
    {
        _context = context;
    }

    public List<StudentAdminViewModel> Students { get; set; } = new();
    public List<GroupAdminViewModel> Groups { get; set; } = new();
    public int? SelectedGroupId { get; set; }
    public string? SearchTerm { get; set; }

    public async Task OnGetAsync(int? groupId, string? search)
    {
        SelectedGroupId = groupId;
        SearchTerm = search;

        // Загружаем группы для фильтра
        Groups = await _context.Groups
            .Include(g => g.Course)
            .ThenInclude(c => c.Filial)
            .Select(g => new GroupAdminViewModel
            {
                Id = g.Id,
                Name = g.Name,
                FilialName = g.Course.Filial.Name
            })
            .OrderBy(g => g.FilialName)
            .ThenBy(g => g.Name)
            .ToListAsync();

        // Загружаем студентов с фильтрацией
        var query = _context.Students
            .Include(s => s.Group)
            .ThenInclude(g => g.Course)
            .ThenInclude(c => c.Filial)
            .AsQueryable();

        if (groupId.HasValue)
        {
            query = query.Where(s => s.GroupId == groupId.Value);
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(s => s.FullName.Contains(search) || (s.Email != null && s.Email.Contains(search)));
        }

        Students = await query
            .Select(s => new StudentAdminViewModel
            {
                Id = s.Id,
                FullName = s.FullName,
                Email = s.Email,
                GroupId = s.GroupId,
                GroupName = s.Group.Name,
                FilialName = s.Group.Course.Filial.Name,
                FilialAddress = s.Group.Course.Filial.Address ?? ""
            })
            .OrderBy(s => s.FilialName)
            .ThenBy(s => s.GroupName)
            .ThenBy(s => s.FullName)
            .ToListAsync();
    }
}