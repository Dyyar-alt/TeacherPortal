using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TeacherPortal.Web.Data;
using TeacherPortal.Web.Models.Entities;
using TeacherPortal.Web.Models.ViewModels;

namespace TeacherPortal.Web.Pages.Account;

public class RegisterModel : PageModel
{
    private readonly UserManager<AppUser> _userManager;
    private readonly SignInManager<AppUser> _signInManager;
    private readonly ApplicationDbContext _context; // <-- Добавляем контекст
    private readonly ILogger<RegisterModel> _logger;

    public RegisterModel(
        UserManager<AppUser> userManager,
        SignInManager<AppUser> signInManager,
        ApplicationDbContext context, // <-- Добавляем в конструктор
        ILogger<RegisterModel> logger)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _context = context;
        _logger = logger;
    }

    [BindProperty]
    public RegisterViewModel Input { get; set; } = new();

    public string? ErrorMessage { get; set; }
    public string? SuccessMessage { get; set; }

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        // 1. Проверяем, есть ли email в списке приглашений
        var invite = await _context.TeacherInvites
            .FirstOrDefaultAsync(i => i.Email == Input.Email && !i.IsUsed);

        if (invite == null)
        {
            ErrorMessage = "Этот email не найден в списке приглашенных преподавателей. Обратитесь к администратору.";
            return Page();
        }

        // 2. Проверяем, не зарегистрирован ли уже пользователь с таким email
        var existingUser = await _userManager.FindByEmailAsync(Input.Email);
        if (existingUser != null)
        {
            ErrorMessage = "Пользователь с таким email уже зарегистрирован.";
            return Page();
        }

        // 3. Создаем пользователя
        var user = new AppUser
        {
            UserName = Input.Email,
            Email = Input.Email,
            FullName = Input.FullName,
            IsTeacher = true,
            CreatedAt = DateTime.UtcNow
        };

        var result = await _userManager.CreateAsync(user, Input.Password);

        if (result.Succeeded)
        {
            // 4. Помечаем приглашение как использованное
            invite.IsUsed = true;
            await _context.SaveChangesAsync();

            _logger.LogInformation($"Создан новый преподаватель: {Input.Email}");

            // Назначаем роль Teacher
            await _userManager.AddToRoleAsync(user, "Teacher");

            // Автоматически входим после регистрации
            await _signInManager.SignInAsync(user, isPersistent: false);

            SuccessMessage = "Регистрация успешна!";
            return RedirectToPage("/Filials/Select");
        }

        ErrorMessage = string.Join(", ", result.Errors.Select(e => e.Description));
        return Page();
    }
}