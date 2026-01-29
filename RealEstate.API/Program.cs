using Application.Interfaces;
using Application.Services;
using Domain.Interfaces;
using Infrastructure.Data;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Добавляем сервисы
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// База данных InMemory
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseInMemoryDatabase("RealEstateDB"));

// Регистрация зависимостей
builder.Services.AddScoped<IManagerRepository, ManagerRepository>();
builder.Services.AddScoped<IPropertyRepository, PropertyRepository>();
builder.Services.AddScoped<IManagerService, ManagerService>();
builder.Services.AddScoped<IPropertyService, PropertyService>();

var app = builder.Build();

// Конвейер middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();

// Маршрутизация контроллеров
app.MapControllers();

// Простой тестовый маршрут (опционально)
app.MapGet("/", () => "RealEstate API работает! Перейдите к /swagger для документации.");
app.MapGet("/api/test", () => new
{
    message = "API работает!",
    status = "OK",
    timestamp = DateTime.UtcNow
});

app.Run();