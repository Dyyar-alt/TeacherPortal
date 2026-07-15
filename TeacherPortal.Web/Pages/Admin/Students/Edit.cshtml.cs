using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TeacherPortal.Web.Data;
using TeacherPortal.Web.Models.Entities;
using TeacherPortal.Web.Models.ViewModels.Admin;

namespace TeacherPortal.Web.Pages.Admin.Students;

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
    public StudentCreateEditViewModel Student { get; set; } = new();

    public List<SelectListItem> Groups { get; set; } = new();

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var student = await _context.Students
            .Include(s => s.Group)
            .FirstOrDefaultAsync(s => s.Id == id);

        if (student == null)
        {
            _logger.LogWarning($"Студент с ID {id} не найден");
            TempData["ErrorMessage"] = $"Студент с ID {id} не найден.";
            return RedirectToPage("Index");
        }

        Student = new StudentCreateEditViewModel
        {
            Id = student.Id,
            FullName = student.FullName,
            Email = student.Email,
            GroupId = student.GroupId
        };

        await LoadGroupsAsync();
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            await LoadGroupsAsync();
            return Page();
        }

        // Проверяем, существует ли группа
        var group = await _context.Groups.FindAsync(Student.GroupId);
        if (group == null)
        {
            ModelState.AddModelError("Student.GroupId", "Выбранная группа не существует.");
            await LoadGroupsAsync();
            return Page();
        }

        var student = await _context.Students.FindAsync(Student.Id);
        if (student == null)
        {
            _logger.LogWarning($"Студент с ID {Student.Id} не найден при попытке обновления");
            TempData["ErrorMessage"] = $"Студент с ID {Student.Id} не найден.";
            return RedirectToPage("Index");
        }

        var oldName = student.FullName;
        var oldGroupId = student.GroupId;

        student.FullName = Student.FullName;
        student.Email = Student.Email;
        student.GroupId = Student.GroupId;

        await _context.SaveChangesAsync();

        if (oldName != student.FullName)
        {
            _logger.LogInformation($"Студент переименован: '{oldName}' -> '{student.FullName}' (ID: {student.Id})");
        }
        if (oldGroupId != student.GroupId)
        {
            var oldGroup = await _context.Groups.FindAsync(oldGroupId);
            var newGroup = await _context.Groups.FindAsync(student.GroupId);
            _logger.LogInformation($"Студент '{student.FullName}' (ID: {student.Id}) переведен из группы '{oldGroup?.Name ?? "не указана"}' в '{newGroup?.Name ?? "не указана"}'");
        }

        TempData["SuccessMessage"] = $"Студент \"{student.FullName}\" успешно обновлен!";
        return RedirectToPage("Index");
    }

    private async Task LoadGroupsAsync()
    {
        Groups = await _context.Groups
            .Include(g => g.Course)
            .ThenInclude(c => c.Filial)
            .Select(g => new SelectListItem
            {
                Value = g.Id.ToString(),
                Text = $"{g.Name} ({g.Course.Name} - {g.Course.Filial.Name})"
            })
            .OrderBy(g => g.Text)
            .ToListAsync();
    }
}