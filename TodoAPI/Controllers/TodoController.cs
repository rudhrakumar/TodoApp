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

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var item = _todoService.GetItemById(id);
            if (item == null)
            {
                return NotFound();
            }
            return Ok(item);
        }

        [HttpPost]
        public IActionResult Add([FromBody] TodoItem item)
        {
            _todoService.Add(item);
            return CreatedAtAction(nameof(GetAll), new { id = item.Id }, item);
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] TodoItem item)
        {
            item.Id = id;
            _todoService.Update(item);
            return Ok();
        }

    }
}
