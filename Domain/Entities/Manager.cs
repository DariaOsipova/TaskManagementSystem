using Domain.Base;
using System;
using System.Collections.Generic;

namespace Domain.Entities
{
    public class Manager : Entity<Guid>
    {
        public string Name { get; private set; }
        public string Email { get; private set; }
        public string Phone { get; private set; }
        public string Role { get; private set; }

        public ICollection<Property> Properties { get; protected set; } = new List<Property>();

        public Manager(Guid id, string name, string email, string phone, string role) : base(id) //конструктор класса
        {
            if (string.IsNullOrWhiteSpace(name)) //или содержит только пробелы
                throw new ArgumentException("Имя не может быть пустым.");
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Email не может быть пустым.");

            Name = name;
            Email = email;
            Phone = phone;
            Role = role;
        }

        protected Manager() : base() { }

        public void UpdateContactInfo(string email, string phone)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Email не может быть пустым.");

            Email = email;
            Phone = phone;
        }

        public void AddProperty(Property property)
        {
            Properties.Add(property); // // Добавляем объект в коллекцию Properties менеджера
        }

        public void UpdateName(string newName)
        {
            if (string.IsNullOrWhiteSpace(newName))
                throw new ArgumentException("Имя не может быть пустым.");

            Name = newName;
        }
    }
}