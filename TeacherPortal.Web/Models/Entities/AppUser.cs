using Microsoft.AspNetCore.Identity;

namespace TeacherPortal.Web.Models.Entities;

public class AppUser : IdentityUser
{
    public string? FullName { get; set; }
    public bool IsTeacher { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}