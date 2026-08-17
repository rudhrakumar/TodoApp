using Xunit;
using TodoAPI.Controllers;
using Microsoft.AspNetCore.Mvc;
using TodoAPI.Models;
using System.Collections.Generic;

namespace TodoAPI.Tests
{
    public class TodoControllerTests
    {
        [Fact]
        public void GetAll_ReturnsOk_WithEnumerableOfTodoItem()
        {
            var controller = new TodoController();
            var result = controller.GetAll();

            Assert.IsType<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            Assert.NotNull(okResult);

            Assert.IsAssignableFrom<IEnumerable<TodoItem>>(okResult.Value);
            var items = okResult.Value as IEnumerable<TodoItem>;
            Assert.Empty(items);
        }
    }
}
