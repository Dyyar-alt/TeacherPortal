using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TeacherPortal.Web.Models.Entities;

namespace TeacherPortal.Web.Pages.Account;

[AllowAnonymous]
public class LogoutModel : PageModel
{
    private readonly SignInManager<AppUser> _signInManager;
    private readonly ILogger<LogoutModel> _logger;

    public LogoutModel(SignInManager<AppUser> signInManager, ILogger<LogoutModel> logger)
    {
        _signInManager = signInManager;
        _logger = logger;
    }

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var userName = User.Identity?.Name;
        await _signInManager.SignOutAsync();

        _logger.LogInformation($"Пользователь {userName} вышел из системы");

        // Очищаем сессию
        HttpContext.Session.Clear();

        return RedirectToPage("/Materials/ByGroup");
    }
}