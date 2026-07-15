using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TeacherPortal.Web.Data;
using TeacherPortal.Web.Models.Entities;
using TeacherPortal.Web.Models.ViewModels.Admin;

namespace TeacherPortal.Web.Pages.Admin.Filials;

[Authorize(Policy = "AdminOnly")]
public class EditModel : PageModel
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<EditModel> _logger;

    public EditModel(ApplicationDbContext context, ILogger<EditModel> logger)
    {
        _context = context;
        _logger = logger;
    }

    [BindProperty]
    public FilialCreateEditViewModel Filial { get; set; } = new();

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var filial = await _context.Filials.FindAsync(id);
        if (filial == null)
        {
            return NotFound($"Филиал с ID {id} не найден");
        }

        Filial = new FilialCreateEditViewModel
        {
            Id = filial.Id,
            Name = filial.Name,
            Address = filial.Address,
            Phone = filial.Phone
        };

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        var filial = await _context.Filials.FindAsync(Filial.Id);
        if (filial == null)
        {
            return NotFound($"Филиал с ID {Filial.Id} не найден");
        }

        filial.Name = Filial.Name;
        filial.Address = Filial.Address;
        filial.Phone = Filial.Phone;

        await _context.SaveChangesAsync();

        _logger.LogInformation($"Филиал {filial.Name} (ID: {filial.Id}) обновлен");
        TempData["SuccessMessage"] = $"Филиал \"{filial.Name}\" успешно обновлен!";

        return RedirectToPage("Index");
    }
}