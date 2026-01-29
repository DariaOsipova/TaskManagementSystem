using System;

namespace Application.DTOs
{
    public class ManagerDto
    {
        public Guid Id { get; set; } //свойства с доступом на чтение, запись
        public string Name { get; set; } 
        public string Email { get; set; } 
        public string Phone { get; set; } 
        public string Role { get; set; } //обычный менеджер, старший менеджер и т.д.
    }
}