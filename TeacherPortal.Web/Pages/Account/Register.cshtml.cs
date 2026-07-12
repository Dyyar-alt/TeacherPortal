using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TeacherPortal.Web.Models.Entities;
using TeacherPortal.Web.Models.ViewModels;

namespace TeacherPortal.Web.Pages.Account;

public class RegisterModel : PageModel
{
    private readonly UserManager<AppUser> _userManager;
    private readonly SignInManager<AppUser> _signInManager;
    private readonly ILogger<RegisterModel> _logger;

    public RegisterModel(
        UserManager<AppUser> userManager,
        SignInManager<AppUser> signInManager,
        ILogger<RegisterModel> logger)
    {
        _userManager = userManager;
        _signInManager = signInManager;
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
            _logger.LogInformation($"Создан новый пользователь: {Input.Email}");

            // Автоматически входим после регистрации
            await _signInManager.SignInAsync(user, isPersistent: false);

            SuccessMessage = "Регистрация успешна!";
            return RedirectToPage("/Lessons/Counter");
        }

        ErrorMessage = string.Join(", ", result.Errors.Select(e => e.Description));
        return Page();
    }
}