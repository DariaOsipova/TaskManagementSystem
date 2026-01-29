using Microsoft.AspNetCore.Mvc;
using Application.Interfaces;
using Application.DTOs;
using Domain.Enums; //подключаем перечисления


namespace Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PropertiesController : ControllerBase
    {
        private readonly IPropertyService _propertyService;

        public PropertiesController(IPropertyService propertyService) //Конструктор с внедрением зависимости IPropertyService
        {
            _propertyService = propertyService; //Присваивание переданной зависимости приватному полю
        }

        [HttpGet] //Атрибут для HTTP GET запроса
        public async Task<ActionResult<IEnumerable<PropertyDto>>> GetAll(
            [FromQuery] Guid? managerId = null,
            [FromQuery] PropertyStatus? status = null,
            [FromQuery] PropertyType? type = null)
        {
            var properties = await _propertyService.FilterAsync(managerId, status, type);
            return Ok(properties);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<PropertyDto>> GetById(Guid id) //Асинхронный метод получения объекта по ID
        {
            var property = await _propertyService.GetByIdAsync(id); //// Вызываем сервис для поиска объекта
            if (property == null)
                return NotFound();
            return Ok(property);
        }

        [HttpPost]
        public async Task<ActionResult<PropertyDto>> Create([FromBody] PropertyDto dto)
        {
            var property = await _propertyService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = property.Id }, property);
        }
    }
}