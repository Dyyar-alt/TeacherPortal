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
    public GroupCreateEditViewModel Group { get; set; } = new();

    public List<SelectListItem> Courses { get; set; } = new();

    public async Task<IActionResult> OnGetAsync(int id)
    {
        // Загружаем группу с курсом
        var group = await _context.Groups
            .Include(g => g.Course)
            .FirstOrDefaultAsync(g => g.Id == id);

        if (group == null)
        {
            _logger.LogWarning($"Группа с ID {id} не найдена");
            TempData["ErrorMessage"] = $"Группа с ID {id} не найдена.";
            return RedirectToPage("Index");
        }

        // Заполняем ViewModel
        Group = new GroupCreateEditViewModel
        {
            Id = group.Id,
            Name = group.Name,
            CourseId = group.CourseId
        };

        // Загружаем список курсов для выпадающего списка
        await LoadCoursesAsync();

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            await LoadCoursesAsync();
            return Page();
        }

        // Проверяем, существует ли выбранный курс
        var course = await _context.Courses.FindAsync(Group.CourseId);
        if (course == null)
        {
            ModelState.AddModelError("Group.CourseId", "Выбранный курс не существует.");
            await LoadCoursesAsync();
            return Page();
        }

        // Загружаем группу для обновления
        var group = await _context.Groups.FindAsync(Group.Id);
        if (group == null)
        {
            _logger.LogWarning($"Группа с ID {Group.Id} не найдена при попытке обновления");
            TempData["ErrorMessage"] = $"Группа с ID {Group.Id} не найдена.";
            return RedirectToPage("Index");
        }

        // Обновляем данные
        var oldName = group.Name;
        var oldCourseId = group.CourseId;

        group.Name = Group.Name;
        group.CourseId = Group.CourseId;

        await _context.SaveChangesAsync();

        // Логируем изменения
        if (oldName != group.Name)
        {
            _logger.LogInformation($"Группа переименована: '{oldName}' -> '{group.Name}' (ID: {group.Id})");
        }
        if (oldCourseId != group.CourseId)
        {
            var oldCourse = await _context.Courses.FindAsync(oldCourseId);
            var newCourse = await _context.Courses.FindAsync(group.CourseId);
            _logger.LogInformation($"Группа '{group.Name}' (ID: {group.Id}) переведена с курса '{oldCourse?.Name ?? "не указан"}' на '{newCourse?.Name ?? "не указан"}'");
        }

        TempData["SuccessMessage"] = $"Группа \"{group.Name}\" успешно обновлена!";
        return RedirectToPage("Index");
    }

    private async Task LoadCoursesAsync()
    {
        Courses = await _context.Courses
            .Include(c => c.Filial)
            .Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = $"{c.Name} ({c.Filial.Name})"
            })
            .OrderBy(c => c.Text)
            .ToListAsync();
    }
}