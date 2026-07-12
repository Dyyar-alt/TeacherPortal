using System.ComponentModel.DataAnnotations;

namespace TeacherPortal.Web.Models.Entities;

public class Lesson
{
    [Key]
    public int Id { get; set; }

    public int GroupId { get; set; }
    public Group Group { get; set; } = null!;

    public int LessonNumber { get; set; }

    public DateTime? DateIssued { get; set; }

    public bool IsCompleted { get; set; } = false;

    public string? Topic { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<Material> Materials { get; set; } = new List<Material>();
}