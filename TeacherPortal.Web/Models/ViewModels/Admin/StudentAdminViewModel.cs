using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace TeacherPortal.Web.Models.ViewModels.Admin;

public class StudentAdminViewModel
{
    public int Id { get; set; }

    [Display(Name = "ФИО")]
    public string FullName { get; set; } = string.Empty;

    [Display(Name = "Email")]
    public string? Email { get; set; }

    [Display(Name = "Группа")]
    public string GroupName { get; set; } = string.Empty;

    [Display(Name = "Филиал")]
    public string FilialName { get; set; } = string.Empty;

    public int GroupId { get; set; }
}

public class StudentCreateEditViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "ФИО обязательно")]
    [Display(Name = "ФИО")]
    public string FullName { get; set; } = string.Empty;

    [EmailAddress]
    [Display(Name = "Email")]
    public string? Email { get; set; }

    [Required(ErrorMessage = "Группа обязательна")]
    [Display(Name = "Группа")]
    public int GroupId { get; set; }
}

public class StudentImportViewModel
{
    [Required(ErrorMessage = "Выберите файл")]
    [Display(Name = "Excel-файл")]
    public IFormFile File { get; set; } = null!;

    [Required(ErrorMessage = "Выберите группу")]
    [Display(Name = "Группа")]
    public int GroupId { get; set; }
}