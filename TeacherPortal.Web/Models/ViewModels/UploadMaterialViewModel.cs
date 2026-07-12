using Microsoft.AspNetCore.Http;

namespace TeacherPortal.Web.Models.ViewModels;

public class UploadMaterialViewModel
{
    public int LessonId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public IFormFile File { get; set; } = null!;
    public bool IsPublic { get; set; } = true;
}