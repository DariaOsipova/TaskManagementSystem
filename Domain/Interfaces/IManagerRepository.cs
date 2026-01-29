using System;
using System.Collections.Generic; //работает с разными типами данных проверка типов во врем€ компил€ции
using System.Threading.Tasks; //асинхронное дл€ task
using Domain.Entities; //сущности

namespace Domain.Interfaces
{
    public interface IManagerRepository
    {
        Task<Manager?> GetByIdAsync(Guid id); //јсинхронный метод получени€ менеджера по ID (может вернуть null если не найден
        Task<Manager?> GetByEmailAsync(string email);
        Task AddAsync(Manager manager);
        Task UpdateAsync(Manager manager);
        Task DeleteAsync(Manager manager);
        Task<IEnumerable<Manager>> GetAllAsync();
    }
}