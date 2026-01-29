using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RealEstate.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ManagersController : ControllerBase
    {
        private static readonly List<ManagerDto> _managers = new()
        {
            new ManagerDto 
            { 
                Id = Guid.NewGuid(), 
                Name = "Иван Петров", 
                Email = "ivan@agency.ru", 
                Phone = "+7-999-123-45-67", 
                Role = "Старший менеджер" 
            },
            new ManagerDto 
            { 
                Id = Guid.NewGuid(), 
                Name = "Мария Сидорова", 
                Email = "maria@agency.ru", 
                Phone = "+7-999-987-65-43", 
                Role = "Менеджер" 
            }
        };

        [HttpGet]
        public IActionResult GetAll()
        {
            return Ok(_managers);
        }
        
        [HttpGet("{id}")]
        public IActionResult GetById(Guid id)
        {
            var manager = _managers.FirstOrDefault(m => m.Id == id);
            if (manager == null) 
                return NotFound(new { error = "Менеджер не найден" });
            
            return Ok(manager);
        }
        
        [HttpPost]
        public IActionResult Create([FromBody] ManagerDto managerDto)
        {
            if (string.IsNullOrEmpty(managerDto.Name))
                return BadRequest(new { error = "Имя обязательно" });
            
            managerDto.Id = Guid.NewGuid();
            _managers.Add(managerDto);
            
            return CreatedAtAction(nameof(GetById), new { id = managerDto.Id }, managerDto);
        }
        
        [HttpPut("{id}")]
        public IActionResult Update(Guid id, [FromBody] ManagerDto managerDto)
        {
            var manager = _managers.FirstOrDefault(m => m.Id == id);
            if (manager == null) 
                return NotFound(new { error = "Менеджер не найден" });
            
            manager.Name = managerDto.Name;
            manager.Email = managerDto.Email;
            manager.Phone = managerDto.Phone;
            manager.Role = managerDto.Role;
            
            return NoContent();
        }
        
        [HttpDelete("{id}")]
        public IActionResult Delete(Guid id)
        {
            var manager = _managers.FirstOrDefault(m => m.Id == id);
            if (manager == null) 
                return NotFound(new { error = "Менеджер не найден" });
            
            _managers.Remove(manager);
            return NoContent();
        }
    }
    
    public class ManagerDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Role { get; set; }
    }
}