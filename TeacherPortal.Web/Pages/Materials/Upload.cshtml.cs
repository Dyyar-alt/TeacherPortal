using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TeacherPortal.Web.Data;
using TeacherPortal.Web.Models.Entities;
using TeacherPortal.Web.Models.ViewModels;

namespace TeacherPortal.Web.Pages.Materials;

[Authorize]
public class UploadModel : PageModel
{
    private readonly ApplicationDbContext _context;
    private readonly IWebHostEnvironment _environment;
    private readonly ILogger<UploadModel> _logger;

    public UploadModel(
        ApplicationDbContext context,
        IWebHostEnvironment environment,
        ILogger<UploadModel> logger)
    {
        _context = context;
        _environment = environment;
        _logger = logger;
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
            var fileName = $"{Guid.NewGuid():N}_{Path.GetFileName(Upload.File.FileName)}";
            var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads", "materials");

            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            var filePath = Path.Combine(uploadsFolder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await Upload.File.CopyToAsync(stream);
            }

            var material = new Material
            {
                Title = Upload.Title,
                Description = Upload.Description,
                FilePath = $"/uploads/materials/{fileName}",
                LessonId = Upload.LessonId,
                UploadedAt = DateTime.UtcNow,
                UploadedBy = User.Identity?.Name ?? "Преподаватель"
            };

            await _context.Materials.AddAsync(material);
            await _context.SaveChangesAsync();

            SuccessMessage = $"Материал \"{Upload.Title}\" успешно загружен!";
            _logger.LogInformation($"Загружен материал {Upload.Title} для пары {Upload.LessonId}");

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