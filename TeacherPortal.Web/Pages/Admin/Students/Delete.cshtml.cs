using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TeacherPortal.Web.Data;
using TeacherPortal.Web.Models.ViewModels.Admin;

namespace TeacherPortal.Web.Pages.Admin.Students;

[Authorize(Policy = "AdminOnly")]
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
    public StudentCreateEditViewModel Student { get; set; } = new();

    public string GroupName { get; set; } = string.Empty;

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

        GroupName = student.Group?.Name ?? "Не указана";

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var student = await _context.Students
            .Include(s => s.Group)
            .FirstOrDefaultAsync(s => s.Id == Student.Id);

        if (student == null)
        {
            _logger.LogWarning($"Студент с ID {Student.Id} не найден при попытке удаления");
            TempData["ErrorMessage"] = $"Студент с ID {Student.Id} не найден.";
            return RedirectToPage("Index");
        }

        var name = student.FullName;
        var groupName = student.Group?.Name ?? "не указана";

        _context.Students.Remove(student);
        await _context.SaveChangesAsync();

        _logger.LogWarning($"Удален студент {name} (ID: {student.Id}) из группы {groupName}");
        TempData["SuccessMessage"] = $"Студент \"{name}\" успешно удален!";

        return RedirectToPage("Index");
    }
}