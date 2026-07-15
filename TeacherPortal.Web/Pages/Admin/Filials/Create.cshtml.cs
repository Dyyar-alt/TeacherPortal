using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TeacherPortal.Web.Data;
using TeacherPortal.Web.Models.Entities;
using TeacherPortal.Web.Models.ViewModels.Admin;

namespace TeacherPortal.Web.Pages.Admin.Filials;

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
    public FilialCreateEditViewModel Filial { get; set; } = new();

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        var filial = new Filial
        {
            Name = Filial.Name,
            Address = Filial.Address,
            Phone = Filial.Phone,
            CreatedAt = DateTime.UtcNow
        };

        await _context.Filials.AddAsync(filial);
        await _context.SaveChangesAsync();

        _logger.LogInformation($"Создан филиал: {filial.Name} (ID: {filial.Id})");
        TempData["SuccessMessage"] = $"Филиал \"{filial.Name}\" успешно создан!";

        return RedirectToPage("Index");
    }
}