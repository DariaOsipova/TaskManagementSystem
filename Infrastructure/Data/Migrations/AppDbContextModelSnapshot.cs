
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

#nullable disable
namespace Infrastructure.Data.Migrations
{
    [DbContext(typeof(AppDbContext))]
    partial class AppDbContextModelSnapshot : ModelSnapshot
//partial-Один класс→несколько файлов "снимок" состояния модели базы данных Entity Framework сохраняет здесь текущую структуру
//БД Используется для сравнения с новыми миграциями
    {
 //protected-Доступ только внутри своего класса и производных классов, Нельзя вызвать извне override — переопределение
 //виртуальный/абстрактный метод базового класса
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618//Отключает предупреждения компилятора о устаревших API 612,618-коды предупреждений для EF Core
            modelBuilder
.HasAnnotation("ProductVersion", "9.0.0") // Устанавливает метаданные (аннотации) для модели "ProductVersion" - версия EF Core
  .HasAnnotation("Relational:MaxIdentifierLength", 63); //максимальная длина имен объектов в БД

NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);//Extensions — расширения
            //Используй автономера(Identity) по умолчанию для столбцов(Columns)

            modelBuilder.Entity("Domain.Entities.Manager", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)");

                    b.Property<string>("Phone")
                        .HasMaxLength(20)
                        .HasColumnType("character varying(20)");

                    b.Property<string>("Role")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)");

                    b.HasKey("Id");

                    b.HasIndex("Email")
                        .IsUnique();

                    b.ToTable("Managers");
                });

            modelBuilder.Entity("Domain.Entities.Property", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("Address")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("character varying(200)");

                    b.Property<string>("Description")
                        .HasColumnType("text");

                    b.Property<DateTime>("ListingDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<Guid>("ManagerId")
                        .HasColumnType("uuid");

                    b.Property<decimal>("Price")
                        .HasColumnType("numeric(18,2)");

                    b.Property<int>("Status")
                        .HasColumnType("integer");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)");

                    b.Property<int>("Type")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("ListingDate");

                    b.HasIndex("ManagerId");

                    b.HasIndex("Status");

                    b.HasIndex("Type");

                    b.ToTable("Properties");
                });

            modelBuilder.Entity("Domain.Entities.Property", b =>
                {
                    b.HasOne("Domain.Entities.Manager", null)
                        .WithMany("Properties") //один менеджер может иметь МНОГО объектов недвижимости
                        .HasForeignKey("ManagerId")
                        .OnDelete(DeleteBehavior.Cascade) //если удалить менеджера, удалятся ВСЕ его объекты недвижимости
   .IsRequired(); //"внешний ключ обязателен" (не может быть null) Каждый объект недвижимости ДОЛЖЕН иметь менеджера
                });
//навигационное свойство для доступа к объектам недвижимости Позволяет из менеджера получить список его объектов: manager.Properties
            modelBuilder.Entity("Domain.Entities.Manager", b =>
                {
                    b.Navigation("Properties");
                });
#pragma warning restore 612, 618//Восстанавливаем предупреждения компилятора 612,618-коды предупреждений EF Core restore = 
            //"верни обратно, включи снова"
        }
    }
}
