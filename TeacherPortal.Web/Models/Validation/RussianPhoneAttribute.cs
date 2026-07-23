using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace TeacherPortal.Web.Models.Validation;

public class RussianPhoneAttribute : ValidationAttribute
{
    public RussianPhoneAttribute()
    {
        ErrorMessage = "Введите корректный номер телефона в формате +7 (XXX) XXX-XX-XX";
    }

    public override bool IsValid(object? value)
    {
        if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
            return true; // Поле необязательное

        var phone = value.ToString()!;
        // Регулярное выражение для формата +7 (XXX) XXX-XX-XX
        var regex = new Regex(@"^\+7 \(\d{3}\) \d{3}-\d{2}-\d{2}$");
        return regex.IsMatch(phone);
    }
}