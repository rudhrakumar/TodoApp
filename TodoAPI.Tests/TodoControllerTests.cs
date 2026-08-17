using Xunit;
using TodoAPI.Controllers;
using Microsoft.AspNetCore.Mvc;
using TodoAPI.Models;
using TodoAPI.Services;
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
            var controller = new TodoController(new TodoService());

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
            var controller = new TodoController(new TodoService());
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
        public void Add_ReturnsOk_WithAddedTodoItem()
        {
            // Arrange
            var controller = new TodoController(new TodoService());
            var newItem = new TodoItem { Title = "Test Todo Item" };

            // Act
            var result = controller.Add(newItem);

            //Assert
            Assert.IsType<OkResult>(result);
        }

        [Fact]
        public void Update_ExistingItem_UpdatesProperties()
        {
            // Arrange
            var controller = new TodoController(new TodoService());
            
            // Seed the controller with an initial item
            var initialItem = new TodoItem { Title = "Old Title", IsCompleted = false };
            controller.Add(initialItem);

            int expectedId = initialItem.Id;
            var updatedData = new TodoItem { Title = "New Title", IsCompleted = true };

            // Act
            controller.Update(expectedId, updatedData);

            //Assert
            var actionResult = controller.GetById(expectedId);
            var updatedItem = Assert.IsType<OkObjectResult>(actionResult);
            Assert.NotNull(updatedItem.Value);
            Assert.Equal("New Title", ((TodoItem)updatedItem.Value).Title);
            Assert.True(((TodoItem)updatedItem.Value).IsCompleted);
        }

        [Fact]
        public void Delete_ExistingItem_RemovesItem()
        {
            // Arrange
            var controller = new TodoController(new TodoService());
            var newItem = new TodoItem { Title = "Test Todo Item" };
            controller.Add(newItem);
            int itemIdToDelete = newItem.Id;
            // Act
            controller.Delete(itemIdToDelete);
            // Assert
            var result = controller.GetById(itemIdToDelete);
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public void Delete_OnlyRemovesSelectedItem_WhenMultipleItemsExist()
        {
            // Arrange
            var sharedService = new TodoService();
            var controller1 = new TodoController(sharedService);
            var controller2 = new TodoController(sharedService);

            controller1.Add(new TodoItem { Title = "First item" });
            controller1.Add(new TodoItem { Title = "Second item" });

            // Act
            controller2.Delete(1);

            // Assert
            var result = controller1.GetAll();
            var okResult = Assert.IsType<OkObjectResult>(result);
            var items = Assert.IsAssignableFrom<IEnumerable<TodoItem>>(okResult.Value).ToList();
            Assert.Equal(1, items.Count);
            Assert.Equal("Second item", items[0].Title);
        }

        [Fact]
        public void Add_UsesSharedServiceState_AcrossControllerInstances()
        {
            // Arrange
            var sharedService = new TodoService();
            var controller1 = new TodoController(sharedService);
            var controller2 = new TodoController(sharedService);

            // Act
            controller1.Add(new TodoItem { Title = "Persisted item" });
            var result = controller2.GetAll();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var items = Assert.IsAssignableFrom<IEnumerable<TodoItem>>(okResult.Value);
            Assert.Single(items);
        }
    }
}
