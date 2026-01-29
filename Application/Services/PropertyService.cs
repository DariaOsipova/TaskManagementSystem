using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.DTOs; //объекты для передачи данных
using Application.Interfaces;//содержит основную бизнес-логику приложения
using Domain.Entities;//сущности
using Domain.Interfaces;
using Domain.Enums; //перечисления
using Domain.Exceptions;

namespace Application.Services
{
    public class PropertyService : IPropertyService //реализует интерфейс IPropertyService
    {
        private readonly IPropertyRepository _propertyRepository; //метод содержащий ссылку на объект
        private readonly IManagerRepository _managerRepository;

        public PropertyService(IPropertyRepository propertyRepository, IManagerRepository managerRepository) //поле
        {
            _propertyRepository = propertyRepository; //значение параметра  присваивается полю  класса
            _managerRepository = managerRepository;
        }

        public async Task<PropertyDto?> GetByIdAsync(Guid id)
        {
            var property = await _propertyRepository.GetByIdAsync(id);
return property == null ? null : MapToDto(property);//Если объект не найден (null),возвращаем null,иначе преобразуем в DTO
        }

        public async Task<IEnumerable<PropertyDto>> GetAllByManagerAsync(Guid managerId)
        {
            var properties = await _propertyRepository.GetAllByManagerAsync(managerId);
            return properties.Select(MapToDto);//это метод преобразования (маппинга) данных из одного формата в другой
        }

        public async Task<IEnumerable<PropertyDto>> FilterAsync(
            Guid? managerId = null,
            PropertyStatus? status = null,
            PropertyType? type = null,
            DateTime? listingDate = null,
            decimal? minPrice = null,
            decimal? maxPrice = null)
        {
            var properties = await _propertyRepository.FilterAsync(managerId, status, type, listingDate, minPrice, maxPrice);
            return properties.Select(MapToDto);
        }

        public async Task<PropertyDto> CreateAsync(PropertyDto propertyDto)
        {
            var manager = await _managerRepository.GetByIdAsync(propertyDto.ManagerId);
            if (manager == null)
                throw new ManagerNotFoundException("Менеджер не найден.");

            var property = new Property(
                Guid.NewGuid(),
                propertyDto.Title,
                propertyDto.Description, //описание
                propertyDto.Address,
                propertyDto.Price,
                propertyDto.ListingDate,
                propertyDto.Type,
                propertyDto.Status,
                propertyDto.ManagerId
            );

            await _propertyRepository.AddAsync(property);// Ждем сохранения объекта в базе данных через репозиторий

            return MapToDto(property); // Преобразуем сохраненную сущность в DTO и возвращаем клиенту
        }

        public async Task UpdateAsync(PropertyDto propertyDto)
        {
            var property = await _propertyRepository.GetByIdAsync(propertyDto.Id);
            if (property == null)
                throw new PropertyNotFoundException("Объект недвижимости не найден."); 

       
            property.UpdateTitle(propertyDto.Title);
            property.UpdateDescription(propertyDto.Description);
            property.UpdateAddress(propertyDto.Address);
            property.UpdatePrice(propertyDto.Price);
            property.UpdateListingDate(propertyDto.ListingDate);
            property.UpdateType(propertyDto.Type);
            property.UpdateManager(propertyDto.ManagerId);

            // Статус обновляем отдельно
            if (property.Status != propertyDto.Status)
            {
                property.ChangeStatus(propertyDto.Status);
            }

            await _propertyRepository.UpdateAsync(property);
        }

        public async Task DeleteAsync(Guid id)
        {
            var property = await _propertyRepository.GetByIdAsync(id);
            if (property == null)
                throw new PropertyNotFoundException("Объект недвижимости не найден."); 

            await _propertyRepository.DeleteAsync(property);
        }

        public async Task ChangeStatusAsync(Guid propertyId, PropertyStatus newStatus)
        {
            var property = await _propertyRepository.GetByIdAsync(propertyId);
            if (property == null)
                throw new PropertyNotFoundException("Объект недвижимости не найден."); 

            property.ChangeStatus(newStatus);
            await _propertyRepository.UpdateAsync(property);
        }

        public async Task UpdatePriceAsync(Guid propertyId, decimal newPrice)
        {
            var property = await _propertyRepository.GetByIdAsync(propertyId);
            if (property == null)
                throw new PropertyNotFoundException("Объект недвижимости не найден."); 

            property.UpdatePrice(newPrice);
            await _propertyRepository.UpdateAsync(property);
        }

        public async Task UpdateManagerAsync(Guid propertyId, Guid newManagerId)
        {
            var property = await _propertyRepository.GetByIdAsync(propertyId);
            if (property == null)
                throw new PropertyNotFoundException("Объект недвижимости не найден.");

            var manager = await _managerRepository.GetByIdAsync(newManagerId);
            if (manager == null)
                throw new ManagerNotFoundException("Новый менеджер не найден.");

            property.UpdateManager(newManagerId);
            await _propertyRepository.UpdateAsync(property);
        }

private PropertyDto MapToDto(Property property)//MapToDto—метод для преобразования (маппинга) одного типа объекта в другой
//Data Transfer Object-Объект передачи данных
        {
            return new PropertyDto
            {
                Id = property.Id,
                Title = property.Title,
                Description = property.Description, //присвоение значения из одного объекта в другой
                Address = property.Address,
                Price = property.Price,
                ListingDate = property.ListingDate,
                Type = property.Type,
                Status = property.Status,
                ManagerId = property.ManagerId
            };
        }
    }
}