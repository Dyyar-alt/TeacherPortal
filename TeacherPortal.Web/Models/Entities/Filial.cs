using System.ComponentModel.DataAnnotations;
using TeacherPortal.Web.Models.Validation;

namespace TeacherPortal.Web.Models.Entities;

public class Filial
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty; // "Условный Ф 1", "Условный Ф 2" и т.д.

    public string? Address { get; set; }

     [RussianPhone]
    [Display(Name = "Телефон")]
    public string? Phone { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Связь с курсами
    public ICollection<Course> Courses { get; set; } = new List<Course>();
}