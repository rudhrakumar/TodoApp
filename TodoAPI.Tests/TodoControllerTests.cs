using Xunit;
using TodoAPI.Controllers;
using Microsoft.AspNetCore.Mvc;
using TodoAPI.Models;
using System.Collections.Generic;
using System.Linq;

namespace TodoAPI.Tests
{
    public class TodoControllerTests
    {
        [Fact]
        public void GetAll_ReturnsOk_WithEmptyListOfTodoItems()
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

        [Fact]
        public void GetAll_ReturnsOk_WithListOfTodoItems()
        {
            var controller = new TodoController();
            var newItem1 = new TodoItem { Title = "Test Todo Item 1" };
            var newItem2 = new TodoItem { Title = "Test Todo Item 2" };
            controller.Add(newItem1);
            controller.Add(newItem2);
            var result = controller.GetAll();
            Assert.IsType<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            Assert.NotNull(okResult);
            Assert.IsAssignableFrom<IEnumerable<TodoItem>>(okResult.Value);
            var items = okResult.Value as IEnumerable<TodoItem>;
            Assert.Equal(2, items.Count());
        }

        [Fact]
        public void Add_ReturnsCreatedAtAction_WithAddedTodoItem()
        {
            var controller = new TodoController();
            var newItem = new TodoItem { Title = "Test Todo Item" };
            var result = controller.Add(newItem);
            Assert.IsType<CreatedAtActionResult>(result);
            var createdResult = result as CreatedAtActionResult;
            Assert.NotNull(createdResult);
            Assert.Equal(nameof(TodoController.GetAll), createdResult.ActionName);
            Assert.IsType<TodoItem>(createdResult.Value);
            var addedItem = createdResult.Value as TodoItem;
            Assert.Equal(newItem.Title, addedItem.Title);
            Assert.Equal(1, addedItem.Id); // Since it's the first item added, its Id should be 1
        }
    }
}
