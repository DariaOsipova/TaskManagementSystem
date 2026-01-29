using Application.DTOs;
using Application.Interfaces;
using Application.Services;
using Domain.Enums;
using Domain.Interfaces;
using Infrastructure.Data;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;//DI(Dependency Injection)-внедрение зависимостей,Регистрация сервисов
 //в контейнере зависимостей,Управление жизненным циклом объектов
using Microsoft.Extensions.Hosting;//Управление жизненным циклом приложения,Конфигурация хоста (HostBuilder),Запуск и
//остановка приложения
using System;
using System.Linq;//sql в коде
using System.Threading.Tasks;//Работа с асинхронными операциями,Класс Task-представляет асинхронную операцию,Ключевые слова
//async/await

namespace ConsoleApp
{
    class Program
    {
        static async Task Main(string[] args) 
        {
            Console.WriteLine("================================================");
            Console.WriteLine("   СИСТЕМА УПРАВЛЕНИЯ НЕДВИЖИМОСТЬЮ");
            Console.WriteLine("   PostgreSQL + Entity Framework Code First");
            Console.WriteLine("================================================\n");

            try 
            {
                
                var connectionString = GetPostgresConnectionString();

                Console.WriteLine($"📡 Используем PostgreSQL: {connectionString}");
                Console.WriteLine();

                var host = CreateHost(connectionString, args); 

                
                await InitializeDatabase(host, connectionString);

                await TestBusinessLogic(host);

             
                ShowSuccessMessage();
            }
            catch (Exception ex)
            {
                ShowErrorMessage(ex);
            }

            Console.WriteLine("\n🚪 Нажмите любую клавишу для выхода...");
            Console.ReadKey();
        }

        static string GetPostgresConnectionString()
        {
            return "Host=localhost;Port=5432;Database=PropertyManagerDb;Username=postgres;Password=password;";
        }

        static IHost CreateHost(string connectionString, string[] args)  //добавляем параметр args
        {
            Console.WriteLine("🔧 Настройка сервисов...");

            return Host.CreateDefaultBuilder(args)  // используем args
                .ConfigureServices(services =>
                {
                    // База данных PostgreSQL
                    services.AddDbContext<AppDbContext>(options =>
                        options.UseNpgsql(connectionString));

                    // Репозитории
services.AddScoped<IManagerRepository, ManagerRepository>();//когда запросят IManagerRepository,вернуть экземпляр
 //ManagerRepository с областью видимости Scoped(время жизни объекта в контейнере зависимостей)
                    services.AddScoped<IPropertyRepository, PropertyRepository>();

                    // Сервисы приложения
                    services.AddScoped<IManagerService, ManagerService>();
                    services.AddScoped<IPropertyService, PropertyService>();
                })
                .Build();
        }

        //ИНИЦИАЛИЗАЦИЯ БАЗЫ
        static async Task InitializeDatabase(IHost host, string connectionString)
        {
            Console.WriteLine("🗄️  Инициализация базы данных...");

            using var scope = host.Services.CreateScope();//// Создаем новую область (scope) для работы с сервисами
//ServiceProvider - поставщик сервисов (DI контейнер) внутри этой областиGetRequiredService<T>() - метод, который:
//Ищет сервис типа AppDbContext в контейнере,Если найден - возвращает его экземпляр,Если не найден - выбрасывает исключение 
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            try
            {
                // Проверка подключения
                if (!await dbContext.Database.CanConnectAsync())
                {
                    Console.WriteLine("   ⚠️  Предупреждение: Не удалось подключиться к базе");
                    Console.WriteLine("   📋 Создайте базу вручную в pgAdmin4:");
                    Console.WriteLine("       1. Правой кнопкой на Databases → Create → Database");
                    Console.WriteLine("       2. Name: PropertyManagerDb");
                    Console.WriteLine("       3. Owner: postgres"); //владелец
                    Console.WriteLine("       4. Нажмите Save и перезапустите программу");
                    return;
                }

                // Применение миграций (автоматическое создание таблиц)
                await dbContext.Database.MigrateAsync();
                Console.WriteLine("   ✅ Таблицы созданы через миграции EF Core");

                // Проверка существующих данных
                var managersCount = await dbContext.Managers.CountAsync();
                var propertiesCount = await dbContext.Properties.CountAsync();

                Console.WriteLine($"   📊 В базе: {managersCount} менеджеров, {propertiesCount} объектов");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"   ⚠️  Ошибка инициализации: {ex.Message}");
                throw;
            }
        }

        //ТЕСТИРОВАНИЕ БИЗНЕС-ЛОГИКИ
        static async Task TestBusinessLogic(IHost host)
        {
            Console.WriteLine("\n🧪 Тестирование бизнес-логики...\n");

            using var scope = host.Services.CreateScope(); // Создаем новую область (scope) для работы с сервисами
                                                           //Required-требуемый Получаем сервис для работы с менеджерами
            var managerService = scope.ServiceProvider.GetRequiredService<IManagerService>();
            var propertyService = scope.ServiceProvider.GetRequiredService<IPropertyService>();

            try
            {
                // 1. СОЗДАНИЕ МЕНЕДЖЕРОВ
                Console.WriteLine("1. 📋 Создание менеджеров:");
                var manager1 = await CreateManager(managerService, "Иван Петров", "ivan.petrov@agency.ru", "+7-999-123-45-67", "Старший менеджер");
                var manager2 = await CreateManager(managerService, "Мария Сидорова", "maria.sidorova@agency.ru", "+7-999-987-65-43", "Менеджер");

                // 2. СОЗДАНИЕ ОБЪЕКТОВ НЕДВИЖИМОСТИ
                Console.WriteLine("\n2. 🏠 Создание объектов недвижимости:");
                var property1 = await CreateProperty(propertyService, manager1.Id,
                    "3-комн. квартира в центре",
                    "Просторная квартира с евроремонтом, вид на парк",
                    "ул. Ленина, 15, кв. 42",
                    8500000.50m, PropertyType.Apartment, PropertyStatus.New);

                var property2 = await CreateProperty(propertyService, manager2.Id,
                    "Загородный дом с участком",
                    "Дом 150 кв.м., участок 10 соток, баня, гараж",
                    "пос. Дачный, ул. Лесная, 25",
                    12500000, PropertyType.House, PropertyStatus.Active);

                var property3 = await CreateProperty(propertyService, manager1.Id,
                    "Офисное помещение в бизнес-центре",
                    "Бизнес-центр класса А, 80 кв.м., ресепшен, конференц-зал",
                    "пр. Мира, 100, оф. 305",
                    20000000, PropertyType.Commercial, PropertyStatus.Active);

                // 3. ОПЕРАЦИИ С ОБЪЕКТАМИ
                Console.WriteLine("\n3. ⚙️  Операции с объектами:");
                //Perform-выполнение
                await PerformOperations(propertyService, property1.Id, property2.Id, manager1.Id, manager2.Id);

                // 4. ПРОВЕРКА ДАННЫХ
                Console.WriteLine("\n4. 📊 Проверка данных в системе:");
                await CheckSystemData(managerService, propertyService, manager1.Id, manager2.Id);

                Console.WriteLine("\n✅ Все тесты выполнены успешно!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n❌ Ошибка при тестировании: {ex.Message}");
                throw;
            }
        }

        static async Task<ManagerDto> CreateManager(IManagerService service, string name, string email, string phone, string role)
        {
            var dto = new ManagerDto
            {
                Name = name,
                Email = email,
                Phone = phone,
                Role = role
            };

            var manager = await service.CreateAsync(dto);
            Console.WriteLine($"   ✅ {manager.Name} ({manager.Role})");
            return manager;
        }

        static async Task<PropertyDto> CreateProperty(IPropertyService service, Guid managerId,
            string title, string description, string address, decimal price,
            PropertyType type, PropertyStatus status)
        {
            var dto = new PropertyDto
            {
                Title = title,
                Description = description,//описание
                Address = address,
                Price = price,
                ListingDate = DateTime.UtcNow,
                Type = type,
                Status = status,
                ManagerId = managerId
            };

            var property = await service.CreateAsync(dto);
            Console.WriteLine($"   ✅ {property.Title} - {property.Price:C}");
            return property;
        }
        //IPropertyService service - это интерфейс сервиса для работы с объектами недвижимости
        static async Task PerformOperations(IPropertyService service, Guid property1Id, Guid property2Id, Guid manager1Id, Guid manager2Id)
        {
            //PerformOperations-выполнение операций
            // Изменение статуса
            await service.ChangeStatusAsync(property1Id, PropertyStatus.Active);
            Console.WriteLine("   ✅ Изменение статуса (New → Active)");

            // Обновление цены
            await service.UpdatePriceAsync(property1Id, 8200000);
            Console.WriteLine("   ✅ Обновление цены (8.5M → 8.2M)");

            // Смена менеджера
            await service.UpdateManagerAsync(property2Id, manager1Id);
            Console.WriteLine("   ✅ Смена ответственного менеджера");

            // Продажа объекта
            await service.ChangeStatusAsync(property1Id, PropertyStatus.Sold);
            Console.WriteLine("   ✅ Продажа объекта (Active → Sold)");
        }

        static async Task CheckSystemData(IManagerService managerService, IPropertyService propertyService,
            Guid manager1Id, Guid manager2Id)
        {
            var allManagers = await managerService.GetAllAsync();
            var allProperties = await propertyService.FilterAsync();
            var activeProperties = await propertyService.FilterAsync(status: PropertyStatus.Active);
            var soldProperties = await propertyService.FilterAsync(status: PropertyStatus.Sold);
            var manager1Properties = await propertyService.GetAllByManagerAsync(manager1Id); //все объеты недвижимости
            var manager2Properties = await propertyService.GetAllByManagerAsync(manager2Id);

            Console.WriteLine($"   👥 Всего менеджеров: {allManagers.Count()}");
            Console.WriteLine($"   🏘️  Всего объектов: {allProperties.Count()}");
            Console.WriteLine($"   📈 Активных объектов: {activeProperties.Count()}");
            Console.WriteLine($"   📉 Проданных объектов: {soldProperties.Count()}");
            Console.WriteLine($"   👤 Объектов у менеджера 1: {manager1Properties.Count()}");
            Console.WriteLine($"   👤 Объектов у менеджера 2: {manager2Properties.Count()}");
        }

        static void ShowSuccessMessage() //Success-успешное
        {
            Console.WriteLine("\n" + new string('=', 50));
            Console.WriteLine("🎉 ПРОГРАММА УСПЕШНО ВЫПОЛНЕНА!");
            Console.WriteLine(new string('=', 50));

            Console.WriteLine("\n📋 ЧТО БЫЛО СДЕЛАНО:");
            Console.WriteLine("1. ✅ Создана база данных PostgreSQL");
            Console.WriteLine("2. ✅ Созданы таблицы через EF Core Code First");
            Console.WriteLine("3. ✅ Протестирована бизнес-логика приложения");
            Console.WriteLine("4. ✅ Проверена работа всех слоёв архитектуры");

            Console.WriteLine("\n🔍 ПРОВЕРЬТЕ В PGADMIN4:");
            Console.WriteLine("   • База данных: PropertyManagerDb");
            Console.WriteLine("   • Таблицы: Managers, Properties");
            Console.WriteLine("   • Данные: 2 менеджера, 3 объекта недвижимости");

            Console.WriteLine("\n🏗️  АРХИТЕКТУРА ПРОЕКТА:");
            Console.WriteLine("   • Presentation - Контроллеры Web API");
            Console.WriteLine("   • Application - Бизнес-логика и DTO");
            Console.WriteLine("   • Domain - Сущности и интерфейсы");
            Console.WriteLine("   • Infrastructure - Репозитории и EF Core");
            Console.WriteLine("   • ConsoleApp - Консольное приложение");
        }

        static void ShowErrorMessage(Exception ex)
        {
            Console.WriteLine("\n" + new string('=', 50));
            Console.WriteLine("❌ ПРОИЗОШЛА ОШИБКА");
            Console.WriteLine(new string('=', 50));

            Console.WriteLine($"\n💥 Ошибка: {ex.Message}");

            if (ex.InnerException != null)
            {
                Console.WriteLine($"\n📄 Детали: {ex.InnerException.Message}");
            }

            Console.WriteLine("\n🔧 ВОЗМОЖНЫЕ РЕШЕНИЯ:");
            Console.WriteLine("1. Убедитесь что PostgreSQL запущен");
            Console.WriteLine("2. Проверьте пароль в строке подключения");
            Console.WriteLine("3. Создайте базу вручную в pgAdmin4:");
            Console.WriteLine("   - Databases → Create → Database");
            Console.WriteLine("   - Name: PropertyManagerDb");
            Console.WriteLine("   - Owner: postgres");//владелец
            Console.WriteLine("4. Или используйте SQLite для демонстрации");
        }
    }
}