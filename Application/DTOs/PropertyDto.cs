using System;
using Domain.Enums;

namespace Application.DTOs
{
    public class PropertyDto  
    {
        public Guid Id { get; set; }
        public string Title { get; set; } 
        public string Description { get; set; } // Подробное описание
        public string Address { get; set; } 
        public decimal Price { get; set; } 
        public DateTime ListingDate { get; set; } // Дата размещения
        public PropertyType Type { get; set; } 
        public PropertyStatus Status { get; set; } 
        public Guid ManagerId { get; set; } 
    }
}