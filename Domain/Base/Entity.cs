using System;

namespace Domain.Base
{
    //реализован интерфейс IEquatable который определяет метод для сравнения объектов класса на равенство
    public abstract class Entity<TId> where TId : struct, IEquatable<TId>
    {
        public TId Id { get; protected set; } //убличное свойство  типа TId с защищенным сеттером (только наследники могут менять)

        protected Entity(TId id) //конструктор
        {
            Id = id;
        }

        protected Entity()
        {
            Id = default!; //по умолчанию
        }
    }
}
