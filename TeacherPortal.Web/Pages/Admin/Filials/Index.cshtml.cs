using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TeacherPortal.Web.Data;
using TeacherPortal.Web.Models.ViewModels.Admin;

namespace TeacherPortal.Web.Pages.Admin.Filials;

[Authorize(Policy = "AdminOnly")]
public class IndexModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public IndexModel(ApplicationDbContext context)
    {
        _context = context;
    }

    public List<FilialAdminViewModel> Filials { get; set; } = new();

    public async Task OnGetAsync()
    {
        Filials = await _context.Filials
            .Select(f => new FilialAdminViewModel
            {
                Id = f.Id,
                Name = f.Name,
                Address = f.Address,
                Phone = f.Phone,
                CoursesCount = f.Courses.Count
            })
            .OrderBy(f => f.Name)
            .ToListAsync();
    }
}