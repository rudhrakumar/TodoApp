using Microsoft.AspNetCore.Mvc;
using TodoAPI.Services;
using TodoAPI.Models;

namespace TodoAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TodoController : ControllerBase
    {
        private readonly TodoService _todoService;
        public TodoController() {
            _todoService = new TodoService();
        }

        [HttpGet("list")]
        public IActionResult GetAll()
        {
            var items = _todoService.GetAll();
            return Ok(items);
        }
        
        [HttpPost("add")]
        public IActionResult Add([FromBody] TodoItem item)
        {
            _todoService.Add(item);
            return CreatedAtAction(nameof(GetAll), new { id = item.Id }, item);
        }
    }
}
