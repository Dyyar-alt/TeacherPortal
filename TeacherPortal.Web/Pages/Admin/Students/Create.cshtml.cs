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
    public StudentCreateEditViewModel Student { get; set; } = new();

    public List<SelectListItem> Groups { get; set; } = new();

    public async Task OnGetAsync()
    {
        await LoadGroupsAsync();
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

        var student = new Student
        {
            FullName = Student.FullName,
            Email = Student.Email,
            GroupId = Student.GroupId,
            EnrolledAt = DateTime.UtcNow
        };

        await _context.Students.AddAsync(student);
        await _context.SaveChangesAsync();

        _logger.LogInformation($"Добавлен студент: {student.FullName} (ID: {student.Id}) в группу {group.Name}");
        TempData["SuccessMessage"] = $"Студент \"{student.FullName}\" успешно добавлен!";

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