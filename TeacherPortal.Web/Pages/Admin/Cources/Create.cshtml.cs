using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TeacherPortal.Web.Data;
using TeacherPortal.Web.Models.Entities;
using TeacherPortal.Web.Models.ViewModels.Admin;

namespace TeacherPortal.Web.Pages.Admin.Courses;

[Authorize(Roles = "Admin")]
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
    public CourseCreateEditViewModel Course { get; set; } = new();

    public List<SelectListItem> Filials { get; set; } = new();

    public async Task OnGetAsync()
    {
        await LoadFilialsAsync();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            await LoadFilialsAsync();
            return Page();
        }

        // Проверяем, существует ли филиал
        var filial = await _context.Filials.FindAsync(Course.FilialId);
        if (filial == null)
        {
            ModelState.AddModelError("Course.FilialId", "Выбранный филиал не существует.");
            await LoadFilialsAsync();
            return Page();
        }

        var course = new Course
        {
            Name = Course.Name,
            Code = Course.Code,
            TotalLessons = Course.TotalLessons,
            FilialId = Course.FilialId,
            CreatedAt = DateTime.UtcNow
        };

        await _context.Courses.AddAsync(course);
        await _context.SaveChangesAsync();

        _logger.LogInformation($"Создан курс: {course.Name} (ID: {course.Id}) в филиале {filial.Name}");
        TempData["SuccessMessage"] = $"Курс \"{course.Name}\" успешно создан!";

        return RedirectToPage("Index");
    }

    private async Task LoadFilialsAsync()
    {
        var filials = await _context.Filials
            .OrderBy(f => f.Name)
            .ThenBy(f => f.Address)
            .ToListAsync();

        Filials = filials
            .Select(f => new SelectListItem
            {
                Value = f.Id.ToString(),
                Text = $"{f.Name} ({f.Address})"
            })
            .ToList();
    }
}