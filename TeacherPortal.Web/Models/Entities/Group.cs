using System.ComponentModel.DataAnnotations;

namespace TeacherPortal.Web.Models.Entities;

public class Group
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(50)]
    public string Name { get; set; } = string.Empty;

    public int CourseId { get; set; }
    public Course Course { get; set; } = null!;

    public int CurrentLessonNumber { get; set; } = 0;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<Student> Students { get; set; } = new List<Student>();
    public ICollection<Lesson> Lessons { get; set; } = new List<Lesson>();
}