using Microsoft.AspNetCore.Mvc; //cоздания Web API контроллеров в ASP.NET Core
using Application.Interfaces;
using Application.DTOs; //Data Transfer Objects - объекты для передачи данных между слоями


namespace Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ManagersController : ControllerBase
    {
        private readonly IManagerService _managerService; //поле

        public ManagersController(IManagerService managerService) //конструктор класса
        {
            _managerService = managerService; //Присваивание переданной зависимости приватному полю
        }

        [HttpGet] //Атрибут для HTTP GET запроса к этому методу
        public async Task<ActionResult<IEnumerable<ManagerDto>>> GetAll() //асинтронный метод возвращает коллекцию Dto в формате
            //http ответа
        {
            var managers = await _managerService.GetAllAsync();
            return Ok(managers);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ManagerDto>> GetById(Guid id) //Асинхронный метод, возвращающий коллекцию ManagerDto
                                                                     //в формате HTTP-ответа
        {
            var manager = await _managerService.GetByIdAsync(id); //Вызываем асинхронный метод сервиса для получения всех менеджеров
            if (manager == null)
                return NotFound();
            return Ok(manager); //озвращаем HTTP 200 OK статус с данными менеджеров
        }

        [HttpPost]
        public async Task<ActionResult<ManagerDto>> Create([FromBody] ManagerDto dto)
        {
            var manager = await _managerService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = manager.Id }, manager);
        }
    }
}