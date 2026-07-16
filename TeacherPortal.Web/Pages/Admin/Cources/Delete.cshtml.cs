using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TeacherPortal.Web.Data;
using TeacherPortal.Web.Models.Entities;
using TeacherPortal.Web.Models.ViewModels.Admin;

namespace TeacherPortal.Web.Pages.Admin.Courses;

[Authorize(Roles = "Admin")]
public class DeleteModel : PageModel
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<DeleteModel> _logger;

    public DeleteModel(ApplicationDbContext context, ILogger<DeleteModel> logger)
    {
        _context = context;
        _logger = logger;
    }

    [BindProperty]
    public CourseCreateEditViewModel Course { get; set; } = new();

    public string FilialName { get; set; } = string.Empty;
    public string FilialAddress { get; set; } = string.Empty;
    public bool HasRelatedGroups { get; set; }
    public int GroupsCount { get; set; }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var course = await _context.Courses
            .Include(c => c.Filial)
            .Include(c => c.Groups)
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

        FilialName = course.Filial?.Name ?? "Не указан";
        FilialAddress = course.Filial?.Address ?? "";
        GroupsCount = course.Groups.Count;
        HasRelatedGroups = GroupsCount > 0;

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var course = await _context.Courses
            .Include(c => c.Groups)
            .FirstOrDefaultAsync(c => c.Id == Course.Id);

        if (course == null)
        {
            return NotFound($"Курс с ID {Course.Id} не найден");
        }

        var name = course.Name;
        var groupsCount = course.Groups.Count;

        _context.Courses.Remove(course);
        await _context.SaveChangesAsync();

        _logger.LogWarning($"Удален курс {name} (ID: {course.Id}) вместе с {groupsCount} группами");
        TempData["SuccessMessage"] = $"Курс \"{name}\" успешно удален! Удалено групп: {groupsCount}";

        return RedirectToPage("Index");
    }
}