using System;

namespace Domain.Exceptions
{
    //Пользовательский класс исключения для случая, когда объект недвижимости не найден
    public class PropertyNotFoundException : Exception
    {
        //Конструктор исключения, принимающий сообщение об ошибке
        public PropertyNotFoundException(string message) : base(message) { } 
    }
}