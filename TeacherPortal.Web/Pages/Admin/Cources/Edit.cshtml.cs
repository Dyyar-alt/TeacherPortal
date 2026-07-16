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
public class EditModel : PageModel
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<EditModel> _logger;

    public EditModel(ApplicationDbContext context, ILogger<EditModel> logger)
    {
        _context = context;
        _logger = logger;
    }

    [BindProperty]
    public CourseCreateEditViewModel Course { get; set; } = new();

    public List<SelectListItem> Filials { get; set; } = new();

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var course = await _context.Courses
            .Include(c => c.Filial)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (course == null)
        {
            return NotFound($"Курс с ID {id} не найден");
        }

        Course = new CourseCreateEditViewModel
        {
            Id = course.Id,
            Name = course.Name,
            Code = course.Code,
            TotalLessons = course.TotalLessons,
            FilialId = course.FilialId
        };

        await LoadFilialsAsync();
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            await LoadFilialsAsync();
            return Page();
        }

        var course = await _context.Courses.FindAsync(Course.Id);
        if (course == null)
        {
            return NotFound($"Курс с ID {Course.Id} не найден");
        }

        var oldName = course.Name;
        course.Name = Course.Name;
        course.Code = Course.Code;
        course.TotalLessons = Course.TotalLessons;
        course.FilialId = Course.FilialId;

        await _context.SaveChangesAsync();

        _logger.LogInformation($"Обновлен курс: {oldName} -> {course.Name} (ID: {course.Id})");
        TempData["SuccessMessage"] = $"Курс \"{course.Name}\" успешно обновлен!";

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