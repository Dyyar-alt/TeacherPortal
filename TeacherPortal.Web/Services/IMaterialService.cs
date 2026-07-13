using TeacherPortal.Web.Models.ViewModels;

namespace TeacherPortal.Web.Services;

public interface IMaterialService
{
    // Получить список групп для выпадающего списка
    Task<List<GroupViewModel>> GetGroupsAsync();

    // Получить все пары группы с материалами
    Task<GroupLessonsViewModel> GetGroupLessonsWithMaterialsAsync(int groupId);
}