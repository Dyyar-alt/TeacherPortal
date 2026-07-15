using System.ComponentModel.DataAnnotations;

namespace TeacherPortal.Web.Models.Entities;

public class TeacherInvite
{
    [Key]
    public int Id { get; set; }

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public bool IsUsed { get; set; } = false; // Становится true, когда преподаватель зарегистрировался
}