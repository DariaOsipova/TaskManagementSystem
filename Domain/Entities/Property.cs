using Domain.Base;
using Domain.Enums;
using Domain.Exceptions;
using System;

namespace Domain.Entities
{
    public class Property : Entity<Guid>
    {
        public string Title { get; private set; }
        public string Description { get; private set; }
        public string Address { get; private set; }
        public decimal Price { get; private set; }
        public DateTime ListingDate { get; private set; }
        public PropertyType Type { get; private set; }
        public PropertyStatus Status { get; private set; }
        public Guid ManagerId { get; private set; }

        public Property(
            Guid id,
            string title,
            string description,
            string address,
            decimal price,
            DateTime listingDate,
            PropertyType type,
            PropertyStatus status,
            Guid managerId)
            : base(id)
        {
            Validate(title, description, address, price, listingDate, managerId);

            Title = title;
            Description = description;
            Address = address;
            Price = price;
            ListingDate = listingDate;
            Type = type;
            Status = status;
            ManagerId = managerId;
        }

        protected Property() : base() { }

        private void Validate(
            string title,
            string description,
            string address,
            decimal price,
            DateTime listingDate,
            Guid managerId)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("Название не может быть пустым.", nameof(title));

            if (string.IsNullOrWhiteSpace(address))
                throw new ArgumentException("Адрес не может быть пустым.", nameof(address));

            if (price <= 0)
                throw new ArgumentException("Цена должна быть положительной.", nameof(price));

            if (listingDate > DateTime.UtcNow.AddDays(1))
                throw new ArgumentException("Дата размещения не может быть в будущем больше чем на 1 день.", nameof(listingDate));

            if (managerId == Guid.Empty)
                throw new ArgumentException("ID менеджера обязателен.", nameof(managerId));
        }

        // Методы для обновления свойств
        public void UpdateTitle(string newTitle)
        {
            if (string.IsNullOrWhiteSpace(newTitle))
                throw new ArgumentException("Название не может быть пустым.", nameof(newTitle));
            Title = newTitle;
        }

        public void UpdateDescription(string newDescription) //описание
        {
            Description = newDescription;
        }

        public void UpdateAddress(string newAddress)
        {
            if (string.IsNullOrWhiteSpace(newAddress))
                throw new ArgumentException("Адрес не может быть пустым.", nameof(newAddress));
            Address = newAddress;
        }

        public void UpdatePrice(decimal newPrice)
        {
            if (newPrice <= 0)
                throw new ArgumentException("Цена должна быть положительной.", nameof(newPrice));
            Price = newPrice;
        }

        public void UpdateListingDate(DateTime newDate)
        {
            if (newDate > DateTime.UtcNow.AddDays(1))
                throw new ArgumentException("Дата размещения не может быть в будущем больше чем на 1 день.", nameof(newDate));
            ListingDate = newDate;
        }

        public void UpdateType(PropertyType newType)
        {
            Type = newType;
        }

        public void UpdateManager(Guid newManagerId)
        {
            if (newManagerId == Guid.Empty)
                throw new ArgumentException("ID менеджера обязателен.", nameof(newManagerId));
            ManagerId = newManagerId;
        }

        public void ChangeStatus(PropertyStatus newStatus)
        {
            if (!CanTransitionTo(newStatus))
                throw new InvalidStatusTransitionException(
                    $"Нельзя перейти из статуса '{Status}' в '{newStatus}' для объекта недвижимости");

            Status = newStatus;
        }

        private bool CanTransitionTo(PropertyStatus newStatus)
        {
            // Логика переходов для недвижимости
            return (Status, newStatus) switch
            {
                // Новый объект можно активировать или приостановить
                (PropertyStatus.New, PropertyStatus.Active) => true,
                (PropertyStatus.New, PropertyStatus.Suspended) => true,
                (PropertyStatus.New, PropertyStatus.Archived) => true,

                // Активный объект можно продать, сдать или приостановить
                (PropertyStatus.Active, PropertyStatus.Sold) => true,
                (PropertyStatus.Active, PropertyStatus.Rented) => true,
                (PropertyStatus.Active, PropertyStatus.Suspended) => true,
                (PropertyStatus.Active, PropertyStatus.Archived) => true,

                // Приостановленный можно снова активировать или отправить в архив
                (PropertyStatus.Suspended, PropertyStatus.Active) => true,
                (PropertyStatus.Suspended, PropertyStatus.Archived) => true,

                // Проданный или сданный можно отправить в архив
                (PropertyStatus.Sold, PropertyStatus.Archived) => true,
                (PropertyStatus.Rented, PropertyStatus.Archived) => true,

                // Из архива нельзя никуда перейти
                _ => false
            };
        }

        // Дополнительные бизнес-методы
        public bool IsActive()
        {
            return Status == PropertyStatus.Active;
        }

        public bool IsSold()
        {
            return Status == PropertyStatus.Sold;
        }

        public bool IsRented()
        {
            return Status == PropertyStatus.Rented;
        }

        public bool IsArchived()
        {
            return Status == PropertyStatus.Archived;
        }

        public bool IsNew()
        {
            return Status == PropertyStatus.New;
        }

        public bool IsSuspended()
        {
            return Status == PropertyStatus.Suspended;
        }

        public bool CanBeSold()
        {
            return Status == PropertyStatus.Active;
        }

        public bool CanBeRented()
        {
            return Status == PropertyStatus.Active;
        }

        public bool CanBeArchived()
        {
            return Status == PropertyStatus.New ||
                   Status == PropertyStatus.Active ||
                   Status == PropertyStatus.Suspended ||
                   Status == PropertyStatus.Sold ||
                   Status == PropertyStatus.Rented;
        }

        public bool CanBeActivated()
        {
            return Status == PropertyStatus.New || Status == PropertyStatus.Suspended;
        }

        public bool CanBeSuspended()
        {
            return Status == PropertyStatus.New || Status == PropertyStatus.Active;
        }

        // Метод для получения информации об объекте
        public string GetStatusDescription()
        {
            return Status switch
            {
                PropertyStatus.New => "Новый объект",
                PropertyStatus.Active => "Активный",
                PropertyStatus.Suspended => "Приостановлен",
                PropertyStatus.Sold => "Продан",
                PropertyStatus.Rented => "Сдан в аренду",
                PropertyStatus.Archived => "В архиве",
                _ => "Неизвестный статус"
            };
        }

        // Метод для получения типа объекта текстом
        public string GetTypeDescription()
        {
            return Type switch
            {
                PropertyType.Apartment => "Квартира",
                PropertyType.House => "Дом",
                PropertyType.Commercial => "Коммерческая недвижимость",
                PropertyType.Land => "Земельный участок",
                PropertyType.Other => "Другое",
                _ => "Неизвестный тип"
            };
        }

        // Переопределение методов для отладки
        public override string ToString()
        {
            return $"Property: {Title} ({GetTypeDescription()}) - {Price:C} - {GetStatusDescription()}";
        }
    }
}
