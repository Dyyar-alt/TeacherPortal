using System.ComponentModel.DataAnnotations;

namespace TeacherPortal.Web.Models.ViewModels.Admin;

public class FilialAdminViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Название филиала обязательно")]
    [Display(Name = "Название")]
    public string Name { get; set; } = string.Empty;

    [Display(Name = "Адрес")]
    public string? Address { get; set; } // <-- Это поле используется

    [Display(Name = "Телефон")]
    public string? Phone { get; set; }

    public int CoursesCount { get; set; }
}

public class FilialCreateEditViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Название филиала обязательно")]
    [Display(Name = "Название")]
    public string Name { get; set; } = string.Empty;

    [Display(Name = "Адрес")]
    public string? Address { get; set; }

    [Display(Name = "Телефон")]
    public string? Phone { get; set; }
}