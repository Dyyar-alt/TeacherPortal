using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TeacherPortal.Web.Data;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace TeacherPortal.Web.Services;

public class TelegramBotService : BackgroundService
{
    private readonly ITelegramBotClient _botClient;
    private readonly ILogger<TelegramBotService> _logger;
    private readonly IServiceProvider _serviceProvider;

    public TelegramBotService(
        IOptions<TelegramBotSettings> botSettings,
        ILogger<TelegramBotService> logger,
        IServiceProvider serviceProvider)
    {
        _botClient = new TelegramBotClient(botSettings.Value.Token);
        _logger = logger;
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Telegram-бот запущен");

        try
        {
            var me = await _botClient.GetMeAsync(stoppingToken);
            _logger.LogInformation($"Бот {me.Username} успешно запущен");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Не удалось подключиться к Telegram API. Проверьте токен.");
            return;
        }

        int offset = 0;
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var updates = await _botClient.GetUpdatesAsync(
                    offset: offset,
                    limit: 100,
                    timeout: 30,
                    cancellationToken: stoppingToken);

                foreach (var update in updates)
                {
                    await OnUpdateReceived(update, stoppingToken);
                    offset = update.Id + 1;
                }

                await Task.Delay(1000, stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка в Telegram-боте");
                await Task.Delay(5000, stoppingToken);
            }
        }

        _logger.LogInformation("Telegram-бот остановлен");
    }

    private async Task OnUpdateReceived(Update update, CancellationToken cancellationToken)
    {
        if (update.Type != UpdateType.Message || update.Message?.Text == null)
            return;

        var message = update.Message;
        var chatId = message.Chat.Id;
        var text = message.Text.Trim();

        // Обработка команды /start
        if (text.StartsWith("/start"))
        {
            // Ожидаем формат: /start email@example.com
            var parts = text.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length < 2)
            {
                await _botClient.SendTextMessageAsync(
                    chatId,
                    "👋 Добро пожаловать!\n\n" +
                    "Чтобы подписаться на уведомления, укажите ваш email:\n" +
                    "/start email@example.com\n\n" +
                    "Или используйте /status для проверки.",
                    cancellationToken: cancellationToken);
                return;
            }

            var email = parts[1];
            if (!IsValidEmail(email))
            {
                await _botClient.SendTextMessageAsync(
                    chatId,
                    "❌ Неверный формат email. Попробуйте снова: /start your@email.com",
                    cancellationToken: cancellationToken);
                return;
            }

            // Используем отдельный scope для доступа к базе данных
            using (var scope = _serviceProvider.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                // Находим студента по email
                var student = await dbContext.Students
                    .FirstOrDefaultAsync(s => s.Email == email, cancellationToken);

                if (student == null)
                {
                    await _botClient.SendTextMessageAsync(
                        chatId,
                        "❌ Студент с таким email не найден. Проверьте правильность email.",
                        cancellationToken: cancellationToken);
                    return;
                }

                // Сохраняем ChatId
                student.ChatId = chatId;
                await dbContext.SaveChangesAsync(cancellationToken);

                await _botClient.SendTextMessageAsync(
                    chatId,
                    $"✅ Вы успешно подписались на уведомления, {student.FullName}!\n\n" +
                    "Теперь вы будете получать сообщения о новых материалах.",
                    cancellationToken: cancellationToken);

                _logger.LogInformation($"Студент {student.FullName} (ID: {student.Id}) подписался на уведомления (ChatId: {chatId})");
            }
        }
        else if (text == "/status")
        {
            await _botClient.SendTextMessageAsync(
                chatId,
                "✅ Бот работает!",
                cancellationToken: cancellationToken);
        }
        else
        {
            await _botClient.SendTextMessageAsync(
                chatId,
                "Я не понимаю эту команду. Используйте:\n" +
                "/start email@example.com - подписаться на уведомления\n" +
                "/status - проверить статус бота",
                cancellationToken: cancellationToken);
        }
    }

    // Метод для отправки уведомления студенту
    public async Task SendNotificationAsync(long chatId, string message)
    {
        try
        {
            await _botClient.SendTextMessageAsync(chatId, message);
            _logger.LogInformation($"Уведомление отправлено в чат {chatId}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Не удалось отправить уведомление в чат {chatId}");
        }
    }

    // Простая проверка email
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

public class TelegramBotSettings
{
    public string Token { get; set; } = string.Empty;
}