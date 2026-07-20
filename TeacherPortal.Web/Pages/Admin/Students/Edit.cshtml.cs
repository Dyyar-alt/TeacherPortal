using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TeacherPortal.Web.Data;
using TeacherPortal.Web.Models.Entities;
using TeacherPortal.Web.Models.ViewModels.Admin;

namespace TeacherPortal.Web.Pages.Admin.Students;

[Authorize(Roles = "Admin")]
public class EditModel : PageModel
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<AppUser> _userManager;
    private readonly ILogger<EditModel> _logger;

    public EditModel(
        ApplicationDbContext context,
        UserManager<AppUser> userManager,
        ILogger<EditModel> logger)
    {
        _context = context;
        _userManager = userManager;
        _logger = logger;
    }

    [BindProperty]
    public StudentCreateEditViewModel Student { get; set; } = new();

    public List<SelectListItem> Groups { get; set; } = new();

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var student = await _context.Students
            .Include(s => s.Group)
            .Include(s => s.IdentityUser)
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

        // 1. Находим студента в базе
        var student = await _context.Students
            .Include(s => s.IdentityUser)
            .FirstOrDefaultAsync(s => s.Id == Student.Id);

        if (student == null)
        {
            return NotFound($"Студент с ID {Student.Id} не найден");
        }

        // 2. Обновляем основные данные
        var oldName = student.FullName;
        student.FullName = Student.FullName;
        student.Email = Student.Email;
        student.GroupId = Student.GroupId;

        // 3. Если указан новый пароль — обновляем его
        if (!string.IsNullOrWhiteSpace(Student.Password))
        {
            var user = await _userManager.FindByIdAsync(student.IdentityUserId);
            if (user != null)
            {
                // Удаляем старый пароль и устанавливаем новый
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var result = await _userManager.ResetPasswordAsync(user, token, Student.Password);
                if (result.Succeeded)
                {
                    _logger.LogInformation($"Пароль для студента {student.FullName} (ID: {student.Id}) изменен");
                    TempData["SuccessMessage"] = $"Студент \"{student.FullName}\" обновлен! Новый пароль: {Student.Password}";
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                    await LoadGroupsAsync();
                    return Page();
                }
            }
        }
        else
        {
            // Если пароль не меняли — просто сообщаем об успехе
            TempData["SuccessMessage"] = $"Студент \"{student.FullName}\" обновлен!";
        }

        await _context.SaveChangesAsync();
        return RedirectToPage("Index");
    }

    private async Task LoadGroupsAsync()
    {
        // 1. Сначала загружаем данные из базы с нужными связями
        var groups = await _context.Groups
            .Include(g => g.Course)
            .ThenInclude(c => c.Filial)
            .ToListAsync();

        // 2. Затем формируем список для выпадающего списка в памяти
        Groups = groups
            .Select(g => new SelectListItem
            {
                Value = g.Id.ToString(),
                Text = $"{g.Name} ({g.Course.Name} - {g.Course.Filial.Name})"
            })
            .OrderBy(g => g.Text)
            .ToList();
    }
}