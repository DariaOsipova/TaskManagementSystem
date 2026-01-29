using System;

namespace Domain.Exceptions
{
    public class InvalidStatusTransitionException : Exception //ѕользовательский класс исключени€ дл€ недопустимых переходов статусов
    {
       //  онструктор, принимающий сообщение об ошибке и передающий его в базовый класс Exception
        public InvalidStatusTransitionException(string message) : base(message) { } 
    }
}
