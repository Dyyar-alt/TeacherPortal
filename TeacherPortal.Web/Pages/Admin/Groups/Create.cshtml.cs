using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TeacherPortal.Web.Data;
using TeacherPortal.Web.Models.Entities;
using TeacherPortal.Web.Models.ViewModels.Admin;

namespace TeacherPortal.Web.Pages.Admin.Groups;

[Authorize(Policy = "AdminOnly")]
public class CreateModel : PageModel
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<CreateModel> _logger;

    public CreateModel(ApplicationDbContext context, ILogger<CreateModel> logger)
    {
        _context = context;
        _logger = logger;
    }

    [BindProperty]
    public GroupCreateEditViewModel Group { get; set; } = new();

    public List<SelectListItem> Courses { get; set; } = new();

    public async Task OnGetAsync()
    {
        await LoadCoursesAsync();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            await LoadCoursesAsync();
            return Page();
        }

        var group = new Group
        {
            Name = Group.Name,
            CourseId = Group.CourseId,
            CurrentLessonNumber = 0,
            CreatedAt = DateTime.UtcNow
        };

        await _context.Groups.AddAsync(group);
        await _context.SaveChangesAsync();

        _logger.LogInformation($"Создана группа: {group.Name} (ID: {group.Id})");
        TempData["SuccessMessage"] = $"Группа \"{group.Name}\" успешно создана!";

        return RedirectToPage("Index");
    }

    private async Task LoadCoursesAsync()
    {
        var courses = await _context.Courses
            .Include(c => c.Filial)
            .ToListAsync();

        Courses = courses
            .Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = $"{c.Name} ({c.Filial.Name}, {c.Filial.Address})"
            })
            .OrderBy(c => c.Text)
            .ToList();
    }
}