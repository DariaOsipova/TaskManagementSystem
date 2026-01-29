using Microsoft.EntityFrameworkCore;
using Infrastructure.Data;
using Microsoft.OpenApi.Models;
using Application.Interfaces;
using Application.Services;
using Domain.Interfaces;
using Infrastructure.Repositories;

// Создаём "строитель" для настройки веб-приложения
var builder = WebApplication.CreateBuilder(args);

// Полностью отключаем логирование от Microsoft
builder.Logging.ClearProviders();

// Добавляем сервисы в контейнер
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Настраиваем Swagger
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Property Manager API",
        Version = "v1",
        Description = "API для управления недвижимостью"
    });
});

// Настраиваем бд
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql("Host=localhost;Port=5432;Database=PropertyManagerDb;Username=postgres;Password=password;"));

// Регистрируем сервисы и репозитории
//AddScoped - Один экземпляр на область (scope)
builder.Services.AddScoped<IManagerService, ManagerService>();
builder.Services.AddScoped<IPropertyService, PropertyService>();
builder.Services.AddScoped<IManagerRepository, ManagerRepository>();
builder.Services.AddScoped<IPropertyRepository, PropertyRepository>();

// Собираем приложение
var app = builder.Build();

// В режиме разработки подключаем Swagger
if (app.Environment.IsDevelopment()) // Если запущено в режиме разработки
{
    app.UseSwagger(); 
    app.UseSwaggerUI(c => //// Включаем веб-интерфейс Swagger UI
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Property Manager API v1"); // Указываем путь к JSON документации
        c.RoutePrefix = "swagger";// Swagger UI будет доступен по адресу: /swagger
    });
}

app.UseHttpsRedirection();// Автоматически перенаправляет HTTP запросы на HTTPS
app.UseAuthorization();// Включает механизм авторизации (проверка прав доступа)
app.MapControllers();// Регистрирует все маршруты из контроллеров


Console.WriteLine("\n====================================================");
Console.WriteLine("         PROPERTY MANAGER API ЗАПУЩЕН!             ");
Console.WriteLine("====================================================");
Console.WriteLine();
Console.WriteLine("ДОСТУПНЫЕ АДРЕСА:");
Console.WriteLine("  HTTPS: https://localhost:7083");
Console.WriteLine();
Console.WriteLine("SWAGGER ДОКУМЕНТАЦИЯ:");
Console.WriteLine("  https://localhost:7083/swagger");
Console.WriteLine("  http://localhost:5080/swagger");
Console.WriteLine();
Console.WriteLine("РЕЖИМ: Разработка (Development)");
Console.WriteLine("ПУТЬ: C:\\kursova\\WebAPI");
Console.WriteLine();
Console.WriteLine("ДЛЯ ОСТАНОВКИ: Ctrl+C");
Console.WriteLine("====================================================\n");


app.Run();// ЗАПУСКАЕМ веб-сервер и начинаем принимать HTTP запросы