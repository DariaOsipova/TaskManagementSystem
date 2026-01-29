using Microsoft.EntityFrameworkCore; //для работы с бд и .net
using Domain.Entities;
namespace Infrastructure.Data
{
    public class AppDbContext : DbContext // наследуется от DbContext, представляет сессию работы с БД(Create, Read, Update, Delete)
    {
        public DbSet<Manager> Managers { get; set; } //Можно читать/получать менеджеров из БД(get), set-внутренняя настройка
        public DbSet<Property> Properties { get; set; }
        //Конструктор принимает настройки DbContext и передает их в базовый класс
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

      //  Переопределенный метод для конфигурации модели базы данных через Fluent(свободный) API
        protected override void OnModelCreating(ModelBuilder modelBuilder) 
        {
            modelBuilder.Entity<Manager>(entity => //Начинаем конфигурацию сущности Manager
            {
                entity.HasKey(m => m.Id); //Определяем первичный ключ сущности (поле Id)
entity.Property(m => m.Name).IsRequired().HasMaxLength(100); //"Конфигурируем свойство:обязательно для заполнения,максимальная длина
                entity.Property(m => m.Email).IsRequired().HasMaxLength(100);
                entity.Property(m => m.Phone).HasMaxLength(20);
                entity.Property(m => m.Role).IsRequired().HasMaxLength(50);
                entity.HasIndex(m => m.Email).IsUnique(); //Создаем уникальный индекс на поле Email 

                entity.HasMany(m => m.Properties) //Сущность Manager имеет МНОГО объектов Property
                      .WithOne() // // Каждый объект Property имеет ОДНОГО менеджера (обратная сторона связи)
                      .HasForeignKey(p => p.ManagerId) //// Определяет поле ManagerId как ВНЕШНИЙ КЛЮЧ (FOREIGN KEY)
                      .OnDelete(DeleteBehavior.Cascade); //При удалении менеджера автоматически удалить все его объекты
            });

            modelBuilder.Entity<Property>(entity => 
            {
                entity.HasKey(p => p.Id);
                entity.Property(p => p.Title).IsRequired().HasMaxLength(100);
                entity.Property(p => p.Description).HasColumnType("text");
                entity.Property(p => p.Address).IsRequired().HasMaxLength(200);

                // Для PostgreSQL используем numeric
                entity.Property(p => p.Price)
                    .IsRequired()
                    .HasColumnType("numeric(18,2)");

                entity.Property(p => p.ListingDate).IsRequired();

                // Конвертация enum в int для PostgreSQL
                entity.Property(p => p.Type)
                    .HasConversion<int>()
                    .IsRequired();

                entity.Property(p => p.Status)
                    .HasConversion<int>()
                    .IsRequired();

                
                // Индексы для производительности
                entity.HasIndex(p => p.ManagerId);
                entity.HasIndex(p => p.Status);
                entity.HasIndex(p => p.Type);
                entity.HasIndex(p => p.ListingDate);
            });

            base.OnModelCreating(modelBuilder); //вызываем метод родительского класса для базовой конфигурации
        }
    }
}