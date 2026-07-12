namespace TeacherPortal.Web.Models.ViewModels;

public class LessonCounterViewModel
{
    public int GroupId { get; set; }
    public string GroupName { get; set; } = string.Empty;
    public string CourseName { get; set; } = string.Empty;
    public int TotalLessons { get; set; }
    public int IssuedLessons { get; set; }
    public int RemainingLessons { get; set; }
    public double ProgressPercentage { get; set; }
    public bool IsComplete { get; set; }
}