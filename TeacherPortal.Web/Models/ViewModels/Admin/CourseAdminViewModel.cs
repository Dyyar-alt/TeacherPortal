using System.ComponentModel.DataAnnotations;

namespace TeacherPortal.Web.Models.ViewModels.Admin;

public class CourseAdminViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Название курса обязательно")]
    [Display(Name = "Название")]
    public string Name { get; set; } = string.Empty;

    [Display(Name = "Код курса")]
    public string Code { get; set; } = string.Empty;

    [Required(ErrorMessage = "Общее количество пар обязательно")]
    [Display(Name = "Общее количество пар")]
    [Range(1, 500, ErrorMessage = "Количество пар должно быть от 1 до 500")]
    public int TotalLessons { get; set; }

    [Required(ErrorMessage = "Филиал обязателен")]
    [Display(Name = "Филиал")]
    public int FilialId { get; set; }

    [Display(Name = "Филиал")]
    public string FilialName { get; set; } = string.Empty;

    [Display(Name = "Адрес филиала")]
    public string FilialAddress { get; set; } = string.Empty;

    [Display(Name = "Групп")]
    public int GroupsCount { get; set; }
}

public class CourseCreateEditViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Название курса обязательно")]
    [Display(Name = "Название")]
    public string Name { get; set; } = string.Empty;

    [Display(Name = "Код курса")]
    public string Code { get; set; } = string.Empty;

    [Required(ErrorMessage = "Общее количество пар обязательно")]
    [Display(Name = "Общее количество пар")]
    [Range(1, 500, ErrorMessage = "Количество пар должно быть от 1 до 500")]
    public int TotalLessons { get; set; }

    [Required(ErrorMessage = "Филиал обязателен")]
    [Display(Name = "Филиал")]
    public int FilialId { get; set; }
}