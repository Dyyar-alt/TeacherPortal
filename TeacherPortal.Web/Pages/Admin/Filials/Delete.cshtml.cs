using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TeacherPortal.Web.Data;
using TeacherPortal.Web.Models.Entities;
using TeacherPortal.Web.Models.ViewModels.Admin;

namespace TeacherPortal.Web.Pages.Admin.Filials;

[Authorize(Policy = "AdminOnly")]
public class DeleteModel : PageModel
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<DeleteModel> _logger;

    public DeleteModel(ApplicationDbContext context, ILogger<DeleteModel> logger)
    {
        _context = context;
        _logger = logger;
    }

    [BindProperty]
    public FilialCreateEditViewModel Filial { get; set; } = new();

    public bool HasRelatedCourses { get; set; }
    public int CoursesCount { get; set; }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var filial = await _context.Filials
            .Include(f => f.Courses)
            .FirstOrDefaultAsync(f => f.Id == id);

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

        CoursesCount = filial.Courses.Count;
        HasRelatedCourses = CoursesCount > 0;

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var filial = await _context.Filials
            .Include(f => f.Courses)
            .FirstOrDefaultAsync(f => f.Id == Filial.Id);

        if (filial == null)
        {
            return NotFound($"Филиал с ID {Filial.Id} не найден");
        }

        var name = filial.Name;
        var coursesCount = filial.Courses.Count;

        _context.Filials.Remove(filial);
        await _context.SaveChangesAsync();

        _logger.LogWarning($"Удален филиал {name} (ID: {filial.Id}) вместе с {coursesCount} курсами");
        TempData["SuccessMessage"] = $"Филиал \"{name}\" успешно удален! Удалено курсов: {coursesCount}";

        return RedirectToPage("Index");
    }
}