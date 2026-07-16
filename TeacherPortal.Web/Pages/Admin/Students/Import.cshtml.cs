using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using TeacherPortal.Web.Data;
using TeacherPortal.Web.Models.Entities;
using TeacherPortal.Web.Models.ViewModels.Admin;

namespace TeacherPortal.Web.Pages.Admin.Students;

[Authorize(Roles = "Admin")]
public class ImportModel : PageModel
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<ImportModel> _logger;

    public ImportModel(ApplicationDbContext context, ILogger<ImportModel> logger)
    {
        _context = context;
        _logger = logger;
    }

    [BindProperty]
    public StudentImportViewModel Import { get; set; } = new();

    public List<SelectListItem> Groups { get; set; } = new();
    public string? ErrorMessage { get; set; }
    public string? SuccessMessage { get; set; }
    public int ImportedCount { get; set; }
    public int SkippedCount { get; set; }

    public async Task OnGetAsync()
    {
        await LoadGroupsAsync();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            await LoadGroupsAsync();
            return Page();
        }

        if (Import.File == null || Import.File.Length == 0)
        {
            ErrorMessage = "Пожалуйста, выберите файл.";
            await LoadGroupsAsync();
            return Page();
        }

        var extension = Path.GetExtension(Import.File.FileName).ToLowerInvariant();
        if (extension != ".xlsx" && extension != ".xls")
        {
            ErrorMessage = "Поддерживаются только файлы Excel (.xlsx, .xls).";
            await LoadGroupsAsync();
            return Page();
        }

        var group = await _context.Groups.FindAsync(Import.GroupId);
        if (group == null)
        {
            ErrorMessage = "Выбранная группа не найдена.";
            await LoadGroupsAsync();
            return Page();
        }

        try
        {
            using var stream = new MemoryStream();
            await Import.File.CopyToAsync(stream);
            stream.Position = 0;

            using var package = new ExcelPackage(stream);
            var worksheet = package.Workbook.Worksheets[0];
            var rowCount = worksheet.Dimension?.Rows ?? 0;

            if (rowCount < 2)
            {
                ErrorMessage = "Файл не содержит данных для импорта (нет строк с данными).";
                await LoadGroupsAsync();
                return Page();
            }

            var headerRow = 1;
            var nameCol = -1;
            var emailCol = -1;

            for (int col = 1; col <= worksheet.Dimension.Columns; col++)
            {
                var header = worksheet.Cells[headerRow, col].Text.Trim().ToLowerInvariant();
                if (header == "фио" || header.Contains("ф.и.о.") || header == "fullname" || header == "name")
                {
                    nameCol = col;
                }
                if (header == "email" || header == "почта")
                {
                    emailCol = col;
                }
            }

            if (nameCol == -1)
            {
                ErrorMessage = "В файле не найден столбец 'ФИО'. Проверьте заголовки.";
                await LoadGroupsAsync();
                return Page();
            }

            var students = new List<Student>();
            var skipped = 0;

            for (int row = headerRow + 1; row <= rowCount; row++)
            {
                var fullName = worksheet.Cells[row, nameCol].Text.Trim();
                if (string.IsNullOrWhiteSpace(fullName))
                {
                    skipped++;
                    continue;
                }

                var email = emailCol != -1 ? worksheet.Cells[row, emailCol].Text.Trim() : null;
                if (!string.IsNullOrEmpty(email) && !IsValidEmail(email))
                {
                    skipped++;
                    continue;
                }

                students.Add(new Student
                {
                    FullName = fullName,
                    Email = email,
                    GroupId = Import.GroupId,
                    EnrolledAt = DateTime.UtcNow
                });
            }

            if (students.Any())
            {
                await _context.Students.AddRangeAsync(students);
                await _context.SaveChangesAsync();

                ImportedCount = students.Count;
                SkippedCount = skipped;
                SuccessMessage = $"Импортировано {ImportedCount} студентов.";
                if (SkippedCount > 0)
                {
                    SuccessMessage += $" Пропущено: {SkippedCount} (пустые или некорректные строки).";
                }

                _logger.LogInformation($"Импортировано {ImportedCount} студентов в группу {group.Name}");
            }
            else
            {
                ErrorMessage = "Не найдено данных для импорта. Проверьте формат файла.";
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Ошибка при импорте: {ex.Message}";
            _logger.LogError(ex, "Ошибка импорта студентов");
        }

        await LoadGroupsAsync();
        return Page();
    }

    private async Task LoadGroupsAsync()
    {
        // Загружаем группы с курсами и филиалами
        var groups = await _context.Groups
            .Include(g => g.Course)
            .ThenInclude(c => c.Filial)
            .ToListAsync();

        // Формируем список для выпадающего списка в памяти
        Groups = groups
            .Select(g => new SelectListItem
            {
                Value = g.Id.ToString(),
                Text = $"{g.Name} ({g.Course.Name} - {g.Course.Filial.Name}, {g.Course.Filial.Address})"
            })
            .OrderBy(g => g.Text)
            .ToList();
    }

    private bool IsValidEmail(string email)
    {
        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }
}