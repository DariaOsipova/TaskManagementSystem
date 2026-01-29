using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RealEstate.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PropertiesController : ControllerBase
    {
        private static readonly List<PropertyDto> _properties = new()
        {
            new PropertyDto 
            { 
                Id = Guid.NewGuid(), 
                Title = "3-комн. квартира в центре", 
                Address = "ул. Ленина, 15", 
                Price = 8500000, 
                Status = "Active", 
                Type = "Apartment",
                ManagerId = Guid.NewGuid()
            },
            new PropertyDto 
            { 
                Id = Guid.NewGuid(), 
                Title = "Загородный дом с участком", 
                Address = "пос. Дачный, 25", 
                Price = 12500000, 
                Status = "New", 
                Type = "House",
                ManagerId = Guid.NewGuid()
            }
        };

        [HttpGet]
        public IActionResult GetAll()
        {
            return Ok(_properties);
        }
        
        [HttpGet("{id}")]
        public IActionResult GetById(Guid id)
        {
            var property = _properties.FirstOrDefault(p => p.Id == id);
            if (property == null) 
                return NotFound(new { error = "Объект не найден" });
            
            return Ok(property);
        }
        
        [HttpPost]
        public IActionResult Create([FromBody] PropertyDto propertyDto)
        {
            if (propertyDto.Price <= 0)
                return BadRequest(new { error = "Цена должна быть положительной" });
            
            propertyDto.Id = Guid.NewGuid();
            _properties.Add(propertyDto);
            
            return CreatedAtAction(nameof(GetById), new { id = propertyDto.Id }, propertyDto);
        }
        
        [HttpPut("{id}")]
        public IActionResult Update(Guid id, [FromBody] PropertyDto propertyDto)
        {
            var property = _properties.FirstOrDefault(p => p.Id == id);
            if (property == null) 
                return NotFound(new { error = "Объект не найден" });
            
            property.Title = propertyDto.Title;
            property.Address = propertyDto.Address;
            property.Price = propertyDto.Price;
            property.Status = propertyDto.Status;
            property.Type = propertyDto.Type;
            property.ManagerId = propertyDto.ManagerId;
            
            return NoContent();
        }
        
        [HttpDelete("{id}")]
        public IActionResult Delete(Guid id)
        {
            var property = _properties.FirstOrDefault(p => p.Id == id);
            if (property == null) 
                return NotFound(new { error = "Объект не найден" });
            
            _properties.Remove(property);
            return NoContent();
        }
        
        [HttpGet("filter")]
        public IActionResult Filter([FromQuery] string? status, [FromQuery] string? type, [FromQuery] decimal? minPrice)
        {
            var filtered = _properties.AsEnumerable();
            
            if (!string.IsNullOrEmpty(status))
                filtered = filtered.Where(p => p.Status == status);
            
            if (!string.IsNullOrEmpty(type))
                filtered = filtered.Where(p => p.Type == type);
            
            if (minPrice.HasValue)
                filtered = filtered.Where(p => p.Price >= minPrice.Value);
            
            return Ok(filtered);
        }
    }
    
    public class PropertyDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Address { get; set; }
        public decimal Price { get; set; }
        public string Status { get; set; }
        public string Type { get; set; }
        public Guid ManagerId { get; set; }
    }
}