using System.ComponentModel.DataAnnotations;

namespace TeacherPortal.Web.Models.Entities;

public class Material
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; }

    public string FilePath { get; set; } = string.Empty;

    public int LessonId { get; set; }
    public Lesson Lesson { get; set; } = null!;

    public DateTime UploadedAt { get; set; } = DateTime.UtcNow;

    public string? UploadedBy { get; set; }
}