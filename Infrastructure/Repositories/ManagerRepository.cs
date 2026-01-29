using System;
using System.Collections.Generic;
using System.Linq; //Для работы с LINQ (Language Integrated Query) - запросы к данным
using System.Threading.Tasks;
using Domain.Entities; //cущности
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using Infrastructure.Data;

namespace Infrastructure.Repositories
{
    public class ManagerRepository : IManagerRepository //Реализует интерфейс
    {
        private readonly AppDbContext _db; //поле

        public ManagerRepository(AppDbContext db) //конструктор класса с внедрением зависимостей
        {
            _db = db;
        }

        public async Task<Manager?> GetByIdAsync(Guid id)
        {
            return await _db.Managers //Начинаем запрос к таблице Managers в базе данных
                .Include(m => m.Properties) //Загружаем связанные объекты недвижимости (жадная загрузка)
                .FirstOrDefaultAsync(m => m.Id == id); //Ищем первую запись где Id равен переданному id (или null если не найден)
        }

        public async Task<Manager?> GetByEmailAsync(string email)
        {
            try //Пытаемся выполнить операцию, если ошибка - возвращаем null"
            {
                return await _db.Managers
                    .Include(m => m.Properties)
                    .FirstOrDefaultAsync(m => m.Email == email); //Сравнение email (регистрозависимое)
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task AddAsync(Manager manager)
        {
            try //Начало блока обработки исключений
            {
                await _db.Managers.AddAsync(manager);
                await _db.SaveChangesAsync();
            }
            catch (DbUpdateException ex) //исключение при ошибках обновления бд
            {
                // Логируем ошибку, но не выбрасываем исключение
                Console.WriteLine($"Ошибка при добавлении менеджера: {ex.Message}");
                throw; //Повторное выбрасывание пойманного исключения (сохраняет стек вызовов)
            }
        }

        public async Task UpdateAsync(Manager manager)
        {
            _db.Managers.Update(manager);
            await _db.SaveChangesAsync();
        }

        public async Task DeleteAsync(Manager manager)
        {
            _db.Managers.Remove(manager);
            await _db.SaveChangesAsync();
        }

        public async Task<IEnumerable<Manager>> GetAllAsync()
        {
            return await _db.Managers
                .Include(m => m.Properties)
                .OrderBy(m => m.Name)
                .ToListAsync();
        }
    }
}