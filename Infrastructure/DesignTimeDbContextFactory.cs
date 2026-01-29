
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Infrastructure.Data;

namespace Infrastructure
{
    //// Реализует интерфейс IDesignTimeDbContextFactory для EF Core инструментов
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        //// Метод создания экземпляра AppDbContext для миграций EF Core
        public AppDbContext CreateDbContext(string[] args)
        {
            // // Создаем построитель опций для контекста базы данных
            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();

       
            var connectionString = "Host=localhost;Port=5433;Database=RealEstateDB;Username=postgres;Password=postgres;";
            optionsBuilder.UseNpgsql(connectionString);

            return new AppDbContext(optionsBuilder.Options);
        }
    }
}