using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.DTOs;
using Application.Interfaces;
using Domain.Entities;
using Domain.Interfaces;
using Domain.Exceptions;

namespace Application.Services
{
    public class ManagerService : IManagerService
    {
        private readonly IManagerRepository _managerRepository;

        public ManagerService(IManagerRepository managerRepository)
        {
            _managerRepository = managerRepository;
        }

        public async Task<ManagerDto?> GetByIdAsync(Guid id)
        {
            var manager = await _managerRepository.GetByIdAsync(id);
            if (manager == null)
                return null;

            return MapToDto(manager);
        }

        public async Task<ManagerDto?> GetByEmailAsync(string email)
        {
            var manager = await _managerRepository.GetByEmailAsync(email);
            if (manager == null)
                return null;

            return MapToDto(manager);
        }

        public async Task<IEnumerable<ManagerDto>> GetAllAsync()
        {
            var managers = await _managerRepository.GetAllAsync();
            return managers.Select(MapToDto);
        }

        public async Task<ManagerDto> CreateAsync(ManagerDto managerDto)
        {
            // Проверяем, существует ли менеджер с таким email
            var existingManager = await _managerRepository.GetByEmailAsync(managerDto.Email);
            if (existingManager != null)
            {
                return MapToDto(existingManager);
            }

            var manager = new Manager(
                Guid.NewGuid(),
                managerDto.Name,
                managerDto.Email,
                managerDto.Phone,
                managerDto.Role);

            await _managerRepository.AddAsync(manager);

            return MapToDto(manager);
        }

        public async Task UpdateAsync(ManagerDto managerDto)
        {
            var manager = await _managerRepository.GetByIdAsync(managerDto.Id);
            if (manager == null)
                throw new ManagerNotFoundException("Менеджер не найден.");

            manager.GetType().GetProperty("Name")?.SetValue(manager, managerDto.Name);
            manager.GetType().GetProperty("Email")?.SetValue(manager, managerDto.Email);
            manager.GetType().GetProperty("Phone")?.SetValue(manager, managerDto.Phone);
            manager.GetType().GetProperty("Role")?.SetValue(manager, managerDto.Role);

            await _managerRepository.UpdateAsync(manager);
        }

        public async Task DeleteAsync(Guid id)
        {
            var manager = await _managerRepository.GetByIdAsync(id);
            if (manager == null)
                throw new ManagerNotFoundException("Менеджер не найден.");

            await _managerRepository.DeleteAsync(manager);
        }

        private ManagerDto MapToDto(Manager manager) =>
            new ManagerDto
            {
                Id = manager.Id,
                Name = manager.Name,
                Email = manager.Email,
                Phone = manager.Phone,
                Role = manager.Role
            };
    }
}