using System;
using System.Collections.Generic;
using System.Linq;//Для LINQ запросов (Language Integrated Query)
using System.Threading.Tasks; //Для асинхронного программирования (Task, async/await)
using Domain.Entities; //Для доменных сущностей (классы Manager, Property)
using Domain.Interfaces;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Infrastructure.Data; //для контекста базы данных (AppDbContext)

namespace Infrastructure.Repositories
{
    public class PropertyRepository : IPropertyRepository //реализует интерфейс
    {
        private readonly AppDbContext _db;

        public PropertyRepository(AppDbContext db) //Конструктор класса PropertyRepository с внедрением зависимости
        {
            _db = db; //Присваивание переданного контекста приватному полю
        }

        public async Task<Property?> GetByIdAsync(Guid id)
        {
            return await _db.Properties.FirstOrDefaultAsync(p => p.Id == id); //по умолчанию
        }

        public async Task AddAsync(Property property)
        {
            await _db.Properties.AddAsync(property);
            await _db.SaveChangesAsync();
        }

        public async Task UpdateAsync(Property property)
        {
            _db.Properties.Update(property);
            await _db.SaveChangesAsync();
        }

        public async Task DeleteAsync(Property property)
        {
            _db.Properties.Remove(property);
            await _db.SaveChangesAsync();
        }

        public async Task<IEnumerable<Property>> GetAllByManagerAsync(Guid managerId)
        {
            return await _db.Properties
                .Where(p => p.ManagerId == managerId)
                .OrderByDescending(p => p.ListingDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Property>> FilterAsync(
            Guid? managerId = null,
            PropertyStatus? status = null,
            PropertyType? type = null,
            DateTime? listingDate = null,
            decimal? minPrice = null,
            decimal? maxPrice = null)
        {
            var query = _db.Properties.AsQueryable();//запрос

            if (managerId.HasValue) //Если параметр managerId указан (не null), добавляем условие фильтрации по менеджеру
                query = query.Where(p => p.ManagerId == managerId.Value);
            if (status.HasValue)
                query = query.Where(p => p.Status == status.Value);
            if (type.HasValue)
                query = query.Where(p => p.Type == type.Value);
            if (listingDate.HasValue)
                query = query.Where(p => p.ListingDate.Date == listingDate.Value.Date);
            if (minPrice.HasValue)
                query = query.Where(p => p.Price >= minPrice.Value);
            if (maxPrice.HasValue)
                query = query.Where(p => p.Price <= maxPrice.Value);

            return await query //Асинхронно выполняем построенный запрос
                .OrderByDescending(p => p.ListingDate) //Сортируем результат по убыванию даты размещения (новые объекты первыми)
                .ToListAsync(); //Асинхронно преобразуем результат в List<T> и возвращаем
        }
    }
}