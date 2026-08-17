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
            // Arrange
            var controller = new TodoController();

            //Act
            var result = controller.GetAll();

            // Assert
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
            // Arrange
            var controller = new TodoController();
            var newItem1 = new TodoItem { Title = "Test Todo Item 1" };
            var newItem2 = new TodoItem { Title = "Test Todo Item 2" };

            // Act
            controller.Add(newItem1);
            controller.Add(newItem2);

            // Assert
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
            // Arrange
            var controller = new TodoController();
            var newItem = new TodoItem { Title = "Test Todo Item" };

            // Act
            var result = controller.Add(newItem);

            //Assert
            Assert.IsType<CreatedAtActionResult>(result);
            var createdResult = result as CreatedAtActionResult;
            Assert.NotNull(createdResult);
            Assert.Equal(nameof(TodoController.GetAll), createdResult.ActionName);
            Assert.IsType<TodoItem>(createdResult.Value);
            var addedItem = createdResult.Value as TodoItem;
            Assert.Equal(newItem.Title, addedItem.Title);
            Assert.Equal(1, addedItem.Id); // Since it's the first item added, its Id should be 1
        }

        [Fact]
        public void Update_ExistingItem_UpdatesProperties()
        {
            // Arrange
            var controller = new TodoController();
            
            // Seed the controller with an initial item
            var initialItem = new TodoItem { Title = "Old Title", IsCompleted = false };
            controller.Add(initialItem);

            int expectedId = initialItem.Id;
            var updatedData = new TodoItem { Title = "New Title", IsCompleted = true };

            // Act
            controller.Update(expectedId, updatedData);

            //Assert
            var actionResult = controller.GetById(expectedId);
            var okResult = Assert.IsType<OkObjectResult>(actionResult);
            Assert.NotNull(okResult.Value);
            Assert.Equal("New Title", ((TodoItem)okResult.Value).Title);
            Assert.True(((TodoItem)okResult.Value).IsCompleted);
        }
    }
}
