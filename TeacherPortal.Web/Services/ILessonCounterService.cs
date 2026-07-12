using TeacherPortal.Web.Models.ViewModels;

namespace TeacherPortal.Web.Services;

public interface ILessonCounterService
{
    Task<LessonCounterViewModel> GetGroupProgressAsync(int groupId);
    Task<bool> IssueNextLessonAsync(int groupId);
    Task<int> GetTotalLessonsIssuedAsync(int courseId);
}