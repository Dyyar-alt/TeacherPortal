using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace TeacherPortal.Web.Models.ViewModels.Admin;

// Модель для отображения списка студентов
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
    [Display(Name = "Адрес филиала")]  // <-- ДОБАВЛЯЕМ
    public string FilialAddress { get; set; } = string.Empty;
    public int GroupId { get; set; }
    
}

// Модель для создания и редактирования (Id не требуется при создании)
public class StudentCreateEditViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "ФИО обязательно")]
    [Display(Name = "ФИО")]
    public string FullName { get; set; } = string.Empty;

    [EmailAddress(ErrorMessage = "Введите корректный email")]
    [Display(Name = "Email")]
    public string? Email { get; set; }

    [Required(ErrorMessage = "Группа обязательна")]
    [Display(Name = "Группа")]
    public int GroupId { get; set; }



    // Поле для пароля (будет отображаться и изменяться)
    [Display(Name = "Пароль")]
    [DataType(DataType.Password)]
    public string? Password { get; set; }

    // Флаг, чтобы показывать, что пароль был сброшен
    public bool IsPasswordReset { get; set; }
}
// Модель для импорта
public class StudentImportViewModel
{
    [Required(ErrorMessage = "Выберите файл")]
    [Display(Name = "Excel-файл")]
    public IFormFile File { get; set; } = null!;

    [Required(ErrorMessage = "Выберите группу")]
    [Display(Name = "Группа")]
    public int GroupId { get; set; }
}