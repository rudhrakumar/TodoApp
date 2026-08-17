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

        public TodoController(TodoService todoService)
        {
            _todoService = todoService;
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
            return Ok();
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] TodoItem item)
        {
            item.Id = id;
            _todoService.Update(item);
            return Ok();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            _todoService.Delete(id);
            return Ok();
        }
    }
}
