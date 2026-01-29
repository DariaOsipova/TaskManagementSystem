using System;
using System.Collections.Generic;
using System.Threading.Tasks; //Для работы с асинхронным программированием (Task, async-показывает что метод содержит асинхронный код
                              //, await-приостановки выполнения метода до завершения асинхронной операции)
using Domain.Entities;//сущности(классы Manager, Property)
using Domain.Enums; 

namespace Domain.Interfaces
{
    public interface IPropertyRepository
    {
        Task<Property?> GetByIdAsync(Guid id); //или null
        Task AddAsync(Property property);
        Task UpdateAsync(Property property);
        Task DeleteAsync(Property property);
        Task<IEnumerable<Property>> GetAllByManagerAsync(Guid managerId);
        Task<IEnumerable<Property>> FilterAsync( //Параметр необязательный, если не указан - фильтр не применяется
            Guid? managerId = null,
            PropertyStatus? status = null,
            PropertyType? type = null,
            DateTime? listingDate = null,
            decimal? minPrice = null,
            decimal? maxPrice = null);
    }
}