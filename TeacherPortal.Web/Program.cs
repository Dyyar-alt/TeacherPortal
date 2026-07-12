using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TeacherPortal.Web.Data;
using TeacherPortal.Web.Models.Entities;
using TeacherPortal.Web.Services;

var builder = WebApplication.CreateBuilder(args);

// Добавляем сервисы Razor Pages
builder.Services.AddRazorPages();

// Настраиваем SQLite
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(connectionString)
    .EnableSensitiveDataLogging(builder.Environment.IsDevelopment()));

// Добавляем Identity
builder.Services.AddIdentity<AppUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

// Настройка паролей (можно ослабить для разработки)
builder.Services.Configure<IdentityOptions>(options =>
{
    options.Password.RequireDigit = false;
    options.Password.RequiredLength = 4;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = false;
});

// Настройка кук
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.LogoutPath = "/Account/Logout";
    options.AccessDeniedPath = "/Account/AccessDenied";
});

// Регистрируем сервисы
builder.Services.AddScoped<ILessonCounterService, LessonCounterService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

// Применяем миграции и инициализируем базу данных
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    dbContext.Database.Migrate();
    await DbInitializer.InitializeAsync(dbContext);
}

app.Run();