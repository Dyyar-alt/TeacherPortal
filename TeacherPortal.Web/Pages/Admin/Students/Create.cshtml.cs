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
public class CreateModel : PageModel
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<AppUser> _userManager; // <-- Добавляем
    private readonly ILogger<CreateModel> _logger;

    public CreateModel(
        ApplicationDbContext context,
        UserManager<AppUser> userManager, // <-- Добавляем в конструктор
        ILogger<CreateModel> logger)
    {
        _context = context;
        _userManager = userManager;
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

        // Проверяем, не занят ли email
        if (!string.IsNullOrWhiteSpace(Student.Email))
        {
            var existingUser = await _userManager.FindByEmailAsync(Student.Email);
            if (existingUser != null)
            {
                ModelState.AddModelError("Student.Email", "Этот email уже используется другим пользователем.");
                await LoadGroupsAsync();
                return Page();
            }
        }

        // Генерируем пароль, если не задан
        var password = string.IsNullOrWhiteSpace(Student.Password)
            ? GenerateRandomPassword()
            : Student.Password;

        // Создаем пользователя Identity
        var identityUser = new AppUser
        {
            UserName = Student.Email,
            Email = Student.Email,
            FullName = Student.FullName,
            IsTeacher = false // Студент!
        };

        var result = await _userManager.CreateAsync(identityUser, password);
        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }
            await LoadGroupsAsync();
            return Page();
        }

        // Назначаем роль "Student"
        await _userManager.AddToRoleAsync(identityUser, "Student");

        // Создаем студента
        var student = new Student
        {
            FullName = Student.FullName,
            Email = Student.Email,
            GroupId = Student.GroupId,
            IdentityUserId = identityUser.Id,
            EnrolledAt = DateTime.UtcNow
        };

        await _context.Students.AddAsync(student);
        await _context.SaveChangesAsync();

        _logger.LogInformation($"Добавлен студент: {student.FullName} (ID: {student.Id}) в группу {group.Name}");
        TempData["SuccessMessage"] = $"Студент \"{student.FullName}\" добавлен! Пароль: {password}";

        return RedirectToPage("Index");
    }

    private async Task LoadGroupsAsync()
    {
        var groups = await _context.Groups
            .Include(g => g.Course)
            .ThenInclude(c => c.Filial)
            .ToListAsync();

        Groups = groups
            .Select(g => new SelectListItem
            {
                Value = g.Id.ToString(),
                Text = $"{g.Name} ({g.Course.Name} - {g.Course.Filial.Name})"
            })
            .OrderBy(g => g.Text)
            .ToList();
    }

    private string GenerateRandomPassword()
    {
        const string chars = "ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz23456789";
        return new string(Enumerable.Repeat(chars, 8)
            .Select(s => s[Random.Shared.Next(s.Length)])
            .ToArray());
    }
}