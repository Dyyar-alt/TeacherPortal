using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace TeacherPortal.Web.Models.Entities;

public class Student
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string FullName { get; set; } = string.Empty;

    [MaxLength(50)]
    public string? Email { get; set; }

    public int GroupId { get; set; }
    public Group Group { get; set; } = null!;

    public DateTime EnrolledAt { get; set; } = DateTime.UtcNow;

    // Связь с пользователем Identity (AppUser)
    public string? IdentityUserId { get; set; }
    public AppUser? IdentityUser { get; set; }
}