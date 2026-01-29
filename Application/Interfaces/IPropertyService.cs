using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Application.DTOs;
using Domain.Enums;

namespace Application.Interfaces
{
    public interface IPropertyService
    {
        //Получение объекта недвижимости по ID, возвращает объект или null если не найден
        Task<PropertyDto?> GetByIdAsync(Guid id); 
        Task<IEnumerable<PropertyDto>> GetAllByManagerAsync(Guid managerId);
        Task<IEnumerable<PropertyDto>> FilterAsync(
            Guid? managerId = null,
            PropertyStatus? status = null,
            PropertyType? type = null,
            DateTime? listingDate = null,
            decimal? minPrice = null,
            decimal? maxPrice = null);
        Task<PropertyDto> CreateAsync(PropertyDto propertyDto);
        Task UpdateAsync(PropertyDto propertyDto);
        Task DeleteAsync(Guid id);
        Task ChangeStatusAsync(Guid propertyId, PropertyStatus newStatus);
        Task UpdatePriceAsync(Guid propertyId, decimal newPrice);
        Task UpdateManagerAsync(Guid propertyId, Guid newManagerId);
    }
}