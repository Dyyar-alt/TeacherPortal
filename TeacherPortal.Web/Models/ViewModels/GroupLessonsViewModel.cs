namespace TeacherPortal.Web.Models.ViewModels;

public class GroupLessonsViewModel
{
    public int GroupId { get; set; }
    public string GroupName { get; set; } = string.Empty;
    public string CourseName { get; set; } = string.Empty;
    public List<LessonWithMaterialsViewModel> Lessons { get; set; } = new();
}

public class LessonWithMaterialsViewModel
{
    public int Id { get; set; }
    public int LessonNumber { get; set; }
    public DateTime? DateIssued { get; set; }
    public string? Topic { get; set; }
    public List<MaterialViewModel> Materials { get; set; } = new();
}

public class MaterialViewModel
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string FilePath { get; set; } = string.Empty;
    public DateTime UploadedAt { get; set; }
    public string FileName => System.IO.Path.GetFileName(FilePath);
}