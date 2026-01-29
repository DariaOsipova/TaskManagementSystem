using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Application.DTOs;

namespace Application.Interfaces
{
    public interface IManagerService
    {
        Task<ManagerDto?> GetByIdAsync(Guid id); //получить менеджера по id
        Task<ManagerDto?> GetByEmailAsync(string email); // email менеджера
        Task<IEnumerable<ManagerDto>> GetAllAsync();
        Task<ManagerDto> CreateAsync(ManagerDto managerDto);
        Task UpdateAsync(ManagerDto managerDto);
        Task DeleteAsync(Guid id);
    }
}