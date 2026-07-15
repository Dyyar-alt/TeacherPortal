using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TeacherPortal.Web.Data;
using TeacherPortal.Web.Models.Entities;

namespace TeacherPortal.Web.Pages.Admin.Invites;

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
    public string Email { get; set; } = string.Empty;

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        // Проверяем, есть ли уже такой email в списке
        var existing = await _context.TeacherInvites
            .FirstOrDefaultAsync(i => i.Email == Email);

        if (existing != null)
        {
            ModelState.AddModelError("Email", "Этот email уже добавлен.");
            return Page();
        }

        // Проверяем, не зарегистрирован ли уже пользователь с таким email
        // (тут нужен UserManager, его можно получить через dependency injection)
        // Пока пропускаем эту проверку, но позже можно добавить

        var invite = new TeacherInvite
        {
            Email = Email,
            CreatedAt = DateTime.UtcNow,
            IsUsed = false
        };

        await _context.TeacherInvites.AddAsync(invite);
        await _context.SaveChangesAsync();

        _logger.LogInformation($"Добавлен email для приглашения: {Email}");
        TempData["SuccessMessage"] = $"Email {Email} добавлен в список приглашенных преподавателей!";

        return RedirectToPage("Index");
    }
}