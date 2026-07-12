using System.ComponentModel.DataAnnotations;

namespace TeacherPortal.Web.Models.Entities;

public class Course
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(20)]
    public string Code { get; set; } = string.Empty;

    public int TotalLessons { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<Group> Groups { get; set; } = new List<Group>();
    public ICollection<Lesson> Lessons { get; set; } = new List<Lesson>();
}