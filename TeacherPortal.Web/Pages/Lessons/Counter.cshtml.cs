using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TeacherPortal.Web.Data;
using TeacherPortal.Web.Models.Entities;
using TeacherPortal.Web.Models.ViewModels;
using TeacherPortal.Web.Services;

namespace TeacherPortal.Web.Pages.Lessons;

[Authorize]
public class CounterModel : PageModel
{
    private readonly ApplicationDbContext _context;
    private readonly ILessonCounterService _counterService;

    public CounterModel(ApplicationDbContext context, ILessonCounterService counterService)
    {
        _context = context;
        _counterService = counterService;
    }

    public List<Group> Groups { get; set; } = new();
    public int? SelectedGroupId { get; set; }
    public LessonCounterViewModel? Progress { get; set; }

    public async Task OnGetAsync(int? groupId)
    {
        Groups = await _context.Groups
            .Include(g => g.Course)
            .OrderBy(g => g.Course.Name)
            .ThenBy(g => g.Name)
            .ToListAsync();

        SelectedGroupId = groupId;

        if (groupId.HasValue)
        {
            Progress = await _counterService.GetGroupProgressAsync(groupId.Value);
        }
    }

    public async Task<IActionResult> OnPostIssueNextAsync(int groupId)
    {
        var success = await _counterService.IssueNextLessonAsync(groupId);

        if (success)
        {
            TempData["SuccessMessage"] = "Пара успешно выдана!";
            return RedirectToPage(new { groupId, success = true });
        }

        TempData["ErrorMessage"] = "Не удалось выдать пару. Проверьте, что курс не завершен.";
        return RedirectToPage(new { groupId });
    }
}