using System.ComponentModel.DataAnnotations;

namespace TeacherPortal.Web.Models.ViewModels;

public class RegisterViewModel
{
    [Required]
    [Display(Name = "Полное имя")]
    public string FullName { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [Display(Name = "Email")]
    public string Email { get; set; } = string.Empty;

    [Required]
    [StringLength(100, ErrorMessage = "Пароль должен содержать не менее {2} символов.", MinimumLength = 4)]
    [DataType(DataType.Password)]
    [Display(Name = "Пароль")]
    public string Password { get; set; } = string.Empty;

    [DataType(DataType.Password)]
    [Display(Name = "Подтверждение пароля")]
    [Compare("Password", ErrorMessage = "Пароли не совпадают.")]
    public string ConfirmPassword { get; set; } = string.Empty;
}