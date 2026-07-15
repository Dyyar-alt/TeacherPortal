using System.ComponentModel.DataAnnotations;

namespace TeacherPortal.Web.Models.ViewModels.Admin;

public class GroupAdminViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Название группы обязательно")]
    [Display(Name = "Название")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Курс обязателен")]
    [Display(Name = "Курс")]
    public int CourseId { get; set; }

    [Display(Name = "Курс")]
    public string CourseName { get; set; } = string.Empty;

    [Display(Name = "Филиал")]
    public string FilialName { get; set; } = string.Empty;

    [Display(Name = "Студентов")]
    public int StudentsCount { get; set; }
}

public class GroupCreateEditViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Название группы обязательно")]
    [Display(Name = "Название")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Курс обязателен")]
    [Display(Name = "Курс")]
    public int CourseId { get; set; }
}