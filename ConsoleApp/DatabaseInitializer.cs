
using System;
using Microsoft.EntityFrameworkCore;
using Infrastructure.Data;

namespace ConsoleApp;

public static class DatabaseInitializer
{
    public static void CreateDatabase()
    {
        Console.WriteLine("=== ИНИЦИАЛИЗАЦИЯ БАЗЫ ДАННЫХ ===");

        string[] ports = { "5432", "5433" };
        string[] passwords = { "postgres", "", "password" };

        foreach (var port in ports)
        {
            Console.WriteLine($"\nПорт: {port}");

            foreach (var password in passwords)
            {
                try
                {
                    Console.Write($"  Пароль '{password}': ");

                    var masterConn = $"Host=localhost;Port={port};Database=postgres;Username=postgres;Password={password};";

                    using (var db = CreateDbContext(masterConn))
                    {
                        if (db.Database.CanConnect())
                        {
                            Console.WriteLine("OK");

                            // Создаем базу
                            db.Database.ExecuteSqlRaw("CREATE DATABASE IF NOT EXISTS \"PropertyManagerDb\"");

                            // Миграции
                            var dbConn = $"Host=localhost;Port={port};Database=PropertyManagerDb;Username=postgres;Password={password};";
                            using (var realEstateDb = CreateDbContext(dbConn))
                            {
                                realEstateDb.Database.Migrate();
                            }

                            Console.WriteLine($"\n✅ База создана! Порт: {port}, Пароль: {password}");
                            return;
                        }
                    }
                }
                catch
                {
                    Console.WriteLine("FAIL");
                }
            }
        }

        Console.WriteLine("\n❌ Не удалось создать базу!");
    }

    private static AppDbContext CreateDbContext(string connectionString)
    {
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
        optionsBuilder.UseNpgsql(connectionString);
        return new AppDbContext(optionsBuilder.Options);
    }
}