using System;

namespace Domain.Exceptions
{
    public class ManagerNotFoundException : Exception
    {
        public ManagerNotFoundException(string message) : base(message) { } //констурктор
    }
}