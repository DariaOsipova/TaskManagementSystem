using Domain.Entities;//Сущности
using Domain.Enums; //перечисления
using Domain.Exceptions;
using System;
using Xunit;

namespace ConsoleApp.Tests
{
    public class SimpleDomainTests //Simple - простые
    {
        [Fact]
        public void Property_CanBeCreated_WithValidData()
        {
          
            var property = new Property(
                Guid.NewGuid(),
                "Тестовая квартира",
                "Описание",
                "Адрес",
                10000000m,
                DateTime.Now,
                PropertyType.Apartment,
                PropertyStatus.New,
                Guid.NewGuid());

          
            Assert.Equal("Тестовая квартира", property.Title);  //Assert.Equal — это метод для проверки равенства в тестах
            Assert.Equal(10000000m, property.Price);
        }

        [Fact]
        public void Property_CannotHaveZeroPrice()
        {
      
            Assert.Throws<ArgumentException>(() => //Assert-утвержать
                new Property(
                    Guid.NewGuid(),
                    "Квартира",
                    "Описание",
                    "Адрес",
                    0m,
                    DateTime.Now,
                    PropertyType.Apartment,
                    PropertyStatus.New,
                    Guid.NewGuid()));
        }

        [Fact]
        public void Manager_CanBeCreated_WithValidData() //с валидными (корректными) данными
        {
            
            var manager = new Manager(
                Guid.NewGuid(),
                "Иван Петров",
                "ivan@agency.ru",
                "+79991234567",
                "Менеджер");

         //проверяем что данные сохранились правильно
            Assert.Equal("Иван Петров", manager.Name);
            Assert.Equal("ivan@agency.ru", manager.Email);
        }

        [Fact]
        public void Manager_CannotHaveEmptyName()
        {
           
            Assert.Throws<ArgumentException>(() =>// Assert-утверждаем
                new Manager(
                    Guid.NewGuid(),
                    "",
                    "test@test.ru",
                    "123",
                    "Роль"));
        }

        [Fact]
        public void Property_CanChangeStatus_FromNewToActive()
        {
          
            var property = new Property(
                Guid.NewGuid(),
                "Квартира",
                "Описание",
                "Адрес",
                10000000m,
                DateTime.Now,
                PropertyType.Apartment,
                PropertyStatus.New,
                Guid.NewGuid());

            
            property.ChangeStatus(PropertyStatus.Active);

         
  Assert.Equal(PropertyStatus.Active, property.Status); // //Assert.Equal — это метод для проверки равенства в тестах
        }

        [Fact]
        public void Property_CannotChangeStatus_FromNewToSold() //Нельзя продать объект, который ещё не был активен
        {
            
            var property = new Property(
                Guid.NewGuid(),
                "Квартира",
                "Описание",
                "Адрес",
                10000000m,
                DateTime.Now,
                PropertyType.Apartment,
                PropertyStatus.New,
                Guid.NewGuid());

          
            Assert.Throws<InvalidStatusTransitionException>(() =>
                property.ChangeStatus(PropertyStatus.Sold));
        }

        [Fact]
        public void Property_CanUpdatePrice()
        {
         
            var property = new Property(
                Guid.NewGuid(),
                "Квартира",
                "Описание",
                "Адрес",
                10000000m,
                DateTime.Now,
                PropertyType.Apartment,
                PropertyStatus.New,
                Guid.NewGuid());

           
            property.UpdatePrice(12000000m);

            Assert.Equal(12000000m, property.Price);
        }

        [Fact]
        public void Property_CannotUpdatePrice_ToNegative()
        {
          
            var property = new Property(
                Guid.NewGuid(),
                "Квартира",
                "Описание",
                "Адрес",
                10000000m,
                DateTime.Now,
                PropertyType.Apartment,
                PropertyStatus.New,
                Guid.NewGuid());

          
            Assert.Throws<ArgumentException>(() =>
                property.UpdatePrice(-1000m));
        }

        [Fact]
        public void Manager_CanUpdateContactInfo()
        {
         
            var manager = new Manager(
                Guid.NewGuid(),
                "Иван",
                "old@test.ru",
                "111",
                "Менеджер");

            
            manager.UpdateContactInfo("new@test.ru", "222");

           
            Assert.Equal("new@test.ru", manager.Email);
            Assert.Equal("222", manager.Phone);
        }

        [Fact]
        public void Manager_CannotUpdateContactInfo_WithEmptyEmail()
        {
            
            var manager = new Manager(
                Guid.NewGuid(),
                "Иван",
                "old@test.ru",
                "111",
                "Менеджер");

            
            Assert.Throws<ArgumentException>(() =>
                manager.UpdateContactInfo("", "222")); //ничего не обновится
        }
    }
}