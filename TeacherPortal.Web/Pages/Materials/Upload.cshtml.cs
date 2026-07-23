using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TeacherPortal.Web.Data;
using TeacherPortal.Web.Models.Entities;
using TeacherPortal.Web.Models.ViewModels;
using TeacherPortal.Web.Services;

namespace TeacherPortal.Web.Pages.Materials;

[Authorize]
public class UploadModel : PageModel
{
    private readonly ApplicationDbContext _context;
    private readonly IWebHostEnvironment _environment;
    private readonly ILogger<UploadModel> _logger;
    private readonly TelegramBotService _telegramBot;

    public UploadModel(
        ApplicationDbContext context,
        IWebHostEnvironment environment,
        ILogger<UploadModel> logger,
        TelegramBotService telegramBot) // 
    {
        _context = context;
        _environment = environment;
        _logger = logger;
        _telegramBot = telegramBot;
    }
   

    [BindProperty]
    public UploadMaterialViewModel Upload { get; set; } = new();

    public string LessonInfo { get; set; } = string.Empty;
    public string GroupInfo { get; set; } = string.Empty;
    public string CourseInfo { get; set; } = string.Empty;
    public string? ErrorMessage { get; set; }
    public string? SuccessMessage { get; set; }

    public async Task<IActionResult> OnGetAsync(int? lessonId)
    {
        if (!lessonId.HasValue)
        {
            return RedirectToPage("/Lessons/Counter");
        }

        var lesson = await _context.Lessons
            .Include(l => l.Group)
            .ThenInclude(g => g.Course)
            .FirstOrDefaultAsync(l => l.Id == lessonId);

        if (lesson == null)
        {
            return NotFound($"Пара с ID {lessonId} не найдена");
        }

        Upload.LessonId = lesson.Id;
        LessonInfo = $"Занятие №{lesson.LessonNumber}";
        GroupInfo = lesson.Group.Name;
        CourseInfo = lesson.Group.Course.Name;

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        if (Upload.File.Length > 50 * 1024 * 1024)
        {
            ErrorMessage = "Файл слишком большой. Максимальный размер: 50 МБ.";
            return Page();
        }

        var allowedExtensions = new[] { ".pdf", ".docx", ".pptx", ".zip", ".rar" };
        var extension = Path.GetExtension(Upload.File.FileName).ToLowerInvariant();
        if (!allowedExtensions.Contains(extension))
        {
            ErrorMessage = "Неподдерживаемый формат файла. Разрешены: PDF, DOCX, PPTX, ZIP, RAR.";
            return Page();
        }

        try
        {
            // 1. Получаем оригинальное имя файла
            var originalFileName = Path.GetFileName(Upload.File.FileName);
            var fileName = originalFileName;
            var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads", "materials");

            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            // 2. Проверяем, существует ли файл с таким именем
            var filePath = Path.Combine(uploadsFolder, fileName);
            int counter = 1;

            // 3. Если файл существует, добавляем суффикс (1), (2), etc.
            while (System.IO.File.Exists(filePath))
            {
                var nameWithoutExt = Path.GetFileNameWithoutExtension(originalFileName);
                var ext = Path.GetExtension(originalFileName);
                fileName = $"{nameWithoutExt} ({counter}){ext}";
                filePath = Path.Combine(uploadsFolder, fileName);
                counter++;
            }

            // 4. Сохраняем файл
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await Upload.File.CopyToAsync(stream);
            }

            // 5. Сохраняем информацию в БД (сохраняем человеческое имя)
            var material = new Material
            {
                Title = Upload.Title,
                Description = Upload.Description,
                FilePath = $"/uploads/materials/{fileName}", // <-- Теперь без GUID
                LessonId = Upload.LessonId,
                UploadedAt = DateTime.UtcNow,
                UploadedBy = User.Identity?.Name ?? "Преподаватель"
            };

            await _context.Materials.AddAsync(material);
            await _context.SaveChangesAsync();

            SuccessMessage = $"Материал \"{Upload.Title}\" успешно загружен!";
            _logger.LogInformation($"Загружен материал {Upload.Title} для пары {Upload.LessonId}");

            if (SuccessMessage != null)
            {
                // Получаем информацию о паре и группе
                var lesson = await _context.Lessons
                    .Include(l => l.Group)
                    .FirstOrDefaultAsync(l => l.Id == Upload.LessonId);

                if (lesson != null)
                {
                    var group = lesson.Group;
                    if (group != null)
                    {
                        try
                        {
                            // Здесь нужно получить chatId студентов группы
                            // Пока отправляем в чат с ID 123456789 (замените на реальный)
                            var chatId = 123456789L;

                            var message = $"📚 Новый материал для группы {group.Name}!\n\n" +
                                          $"📄 {Upload.Title}\n" +
                                          $"📝 {Upload.Description}\n\n" +
                                          $"🔗 Ссылка: https://ваш-сайт.ru/materials/by-group?groupId={group.Id}";

                            await _telegramBot.SendNotificationAsync(chatId, message);
                            _logger.LogInformation($"Уведомление отправлено в Telegram для группы {group.Name}");
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Не удалось отправить уведомление в Telegram");
                        }
                    }
                }
            }

            // Очищаем форму для новой загрузки
            Upload = new UploadMaterialViewModel { LessonId = Upload.LessonId };
            await OnGetAsync(Upload.LessonId);

            return Page();
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Ошибка при загрузке файла: {ex.Message}";
            _logger.LogError(ex, "Ошибка загрузки материала");
            return Page();
        }
    }
}