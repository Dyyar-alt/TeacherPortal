using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TeacherPortal.Web.Services;
using TeacherPortal.Web.Models.ViewModels;

namespace TeacherPortal.Web.Pages.Materials;

public class ByGroupModel : PageModel
{
    private readonly IMaterialService _materialService;

    public ByGroupModel(IMaterialService materialService)
    {
        _materialService = materialService;
    }

    public List<GroupViewModel> Groups { get; set; } = new();
    public int? SelectedGroupId { get; set; }
    public string GroupName { get; set; } = string.Empty;
    public string CourseName { get; set; } = string.Empty;
    public List<LessonWithMaterialsViewModel> Lessons { get; set; } = new();

    public async Task OnGetAsync(int? groupId)
    {
        // Загружаем список групп для выпадающего списка
        Groups = await _materialService.GetGroupsAsync();

        SelectedGroupId = groupId;

        if (groupId.HasValue)
        {
            var data = await _materialService.GetGroupLessonsWithMaterialsAsync(groupId.Value);
            if (data != null)
            {
                GroupName = data.GroupName;
                CourseName = data.CourseName;
                Lessons = data.Lessons;
            }
        }
    }
}