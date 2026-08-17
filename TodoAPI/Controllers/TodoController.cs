using Microsoft.AspNetCore.Mvc;
using TodoAPI.Services;

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
    }
}
