using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Infrastructure.Data;

namespace Infrastructure
{
    // Реализует интерфейс IDesignTimeDbContextFactory для EF Core инструментов
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        // Метод создания экземпляра AppDbContext для миграций EF Core
        public AppDbContext CreateDbContext(string[] args)
        {
            // Создаем построитель опций для контекста базы данных
            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();

            // ИСПРАВЛЕННАЯ СТРОКА ПОДКЛЮЧЕНИЯ:
            // - Порт: 5432 (правильный, был 5433)
            // - База: PropertyManagerDb (правильная, была RealEstateDB)
            // - Пароль: 123456 (правильный, был password)
            var connectionString = "Host=localhost;Port=5432;Database=PropertyManagerDb;Username=postgres;Password=123456;";
            
            optionsBuilder.UseNpgsql(connectionString);

            return new AppDbContext(optionsBuilder.Options);
        }
    }
}
