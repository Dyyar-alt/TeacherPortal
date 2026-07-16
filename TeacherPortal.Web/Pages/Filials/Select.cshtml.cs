using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TeacherPortal.Web.Data;
using TeacherPortal.Web.Models.Entities;

namespace TeacherPortal.Web.Pages.Filials;

[Authorize]
public class SelectModel : PageModel
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<SelectModel> _logger;

    public SelectModel(ApplicationDbContext context, ILogger<SelectModel> logger)
    {
        _context = context;
        _logger = logger;
    }

    public List<Filial> Filials { get; set; } = new();
    public string? ErrorMessage { get; set; }

    public async Task OnGetAsync()
    {
        Filials = await _context.Filials
            .OrderBy(f => f.Name)
            .ThenBy(f => f.Address) // Сортировка по адресу
            .ToListAsync();
    }

    public async Task<IActionResult> OnPostAsync(int filialId)
    {
        var filial = await _context.Filials.FindAsync(filialId);
        if (filial == null)
        {
            ErrorMessage = "Филиал не найден. Попробуйте еще раз.";
            Filials = await _context.Filials.OrderBy(f => f.Name).ThenBy(f => f.Address).ToListAsync();
            return Page();
        }

        HttpContext.Session.SetInt32("SelectedFilialId", filialId);

        _logger.LogInformation($"Пользователь {User.Identity?.Name} выбрал филиал: {filial.Name} (ID: {filialId})");

        return RedirectToPage("/Lessons/Counter");
    }
}