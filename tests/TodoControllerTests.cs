using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using TodoApi.Controllers;
using TodoApi.Models;
using Xunit;

namespace TodoApi.Tests
{
    public class TodoControllerTests
    {
        [Fact]
        public async Task GetAll_Returns_ListOfTodos()
        {
            var options = CreateDbContextOptions();
            using (var todoApiDbContext = new TodoApiDbContext(options))
            {
                var mockLogger = new Mock<ILogger<TodoController>>();
                var todoController = new TodoController(mockLogger.Object, todoApiDbContext);

                var listOfTodosResult = await todoController.GetAll();

                Assert.IsType<List<TodoItem>>(listOfTodosResult.Value);
            }
        }

        [Fact]
        public async Task GetAll_Logging_Working()
        {
            var options = CreateDbContextOptions();
            using (var todoApiDbContext = new TodoApiDbContext(options))
            {
                var mockLogger = new Mock<ILogger<TodoController>>();
                var todoController = new TodoController(mockLogger.Object, todoApiDbContext);

                var listOfTodosResult = await todoController.GetAll();

                mockLogger.Verify(x => x.Log(
                    It.IsAny<LogLevel>(),
                    It.IsAny<EventId>(),
                    It.IsAny<It.IsAnyType>(),
                    It.IsAny<Exception>(),
                    (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()), Times.Once);
            }
        }

        [Fact]
        public void GetOne_Returns_OneTodo()
        {
            var options = CreateDbContextOptions();
            using (var todoApiDbContext = new TodoApiDbContext(options))
            {
                var mockLogger = new Mock<ILogger<TodoController>>();
                var todoController = new TodoController(mockLogger.Object, todoApiDbContext);

                var todo = todoController.GetById(1);

                Assert.IsType<TodoItem>(todo.Value);
            }
        }

        [Fact]
        public async Task GetOne_Logging_Working()
        {
            var options = CreateDbContextOptions();
            using (var todoApiDbContext = new TodoApiDbContext(options))
            {
                var mockLogger = new Mock<ILogger<TodoController>>();
                var todoController = new TodoController(mockLogger.Object, todoApiDbContext);
                var todoCreateResult = await todoController.Create(new TodoItem()
                {
                    Description = $"Random-{Guid.NewGuid().ToString("N")}"
                });
                var todoTemp = todoCreateResult as CreatedAtRouteResult;
                var id = (todoTemp.Value as TodoItem).Id;
                var todo = todoController.GetById(id);

                Assert.IsType<TodoItem>(todo.Value);

                mockLogger.Verify(x => x.Log(
                    It.IsAny<LogLevel>(),
                    It.IsAny<EventId>(),
                    It.IsAny<It.IsAnyType>(),
                    It.IsAny<Exception>(),
                    (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()));
            }
        }

        [Fact]
        public void GetOne_Returns_NotFound()
        {
            var options = CreateDbContextOptions();
            using (var todoApiDbContext = new TodoApiDbContext(options))
            {
                var mockLogger = new Mock<ILogger<TodoController>>();
                var todoController = new TodoController(mockLogger.Object, todoApiDbContext);

                var todo = todoController.GetById(11355);

                Assert.IsType<NotFoundResult>(todo.Result);
            }
        }

        [Fact]
        public void GetOne_Logging_Working_When_NotFound()
        {
            var options = CreateDbContextOptions();
            using (var todoApiDbContext = new TodoApiDbContext(options))
            {
                var mockLogger = new Mock<ILogger<TodoController>>();
                var todoController = new TodoController(mockLogger.Object, todoApiDbContext);

                var todo = todoController.GetById(11355);

                Assert.IsType<NotFoundResult>(todo.Result);

                mockLogger.Verify(x => x.Log(
                    It.IsAny<LogLevel>(),
                    It.IsAny<EventId>(),
                    It.IsAny<It.IsAnyType>(),
                    It.IsAny<Exception>(),
                    (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()), Times.Exactly(2));
            }
        }

        [Fact]
        public async Task Create_Logging_Working()
        {
            var options = CreateDbContextOptions();
            using (var todoApiDbContext = new TodoApiDbContext(options))
            {
                var mockLogger = new Mock<ILogger<TodoController>>();
                var todoController = new TodoController(mockLogger.Object, todoApiDbContext);

                var todo = await todoController.Create(new TodoItem() { Description = "Check logging working on Create" });

                Assert.IsType<CreatedAtRouteResult>(todo);

                mockLogger.Verify(x => x.Log(
                    It.IsAny<LogLevel>(),
                    It.IsAny<EventId>(),
                    It.IsAny<It.IsAnyType>(),
                    It.IsAny<Exception>(),
                    (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()), Times.Exactly(2));
            }
        }

        [Fact]
        public async Task Create_Creates_A_TodoItem()
        {
            var options = CreateDbContextOptions();
            using (var todoApiDbContext = new TodoApiDbContext(options))
            {
                var mockLogger = new Mock<ILogger<TodoController>>();
                var todoController = new TodoController(mockLogger.Object, todoApiDbContext);
                var createdOn = DateTime.UtcNow;
                var todo = await todoController.Create(new TodoItem()
                {
                    IsCompleted = false,
                    Description = $"Write test on Create todo item",
                    CreatedOn = createdOn
                });

                Assert.IsType<CreatedAtRouteResult>(todo);
                var result = todo as CreatedAtRouteResult;
                Assert.NotNull(result);
                Assert.IsType<TodoItem>(result.Value);
                Assert.Equal("GetTodo", result.RouteName);
                var todoItem = result.Value as TodoItem;
                Assert.NotEqual(0, todoItem.Id);
                Assert.Equal(createdOn, todoItem.CreatedOn);
                Assert.False(todoItem.IsCompleted);
            }
        }

        [Fact]
        public async Task Create_Returns_Conflict_If_Todo_Exists()
        {
            var options = CreateDbContextOptions();
            using (var todoApiDbContext = new TodoApiDbContext(options))
            {
                var mockLogger = new Mock<ILogger<TodoController>>();
                var todoController = new TodoController(mockLogger.Object, todoApiDbContext);

                var todo = await todoController.Create(new TodoItem() { Description = "Write Blog Post" });
                var todoDuplicate = await todoController.Create(new TodoItem() { Description = "Write Blog Post" });

                Assert.IsType<CreatedAtRouteResult>(todo);
                Assert.IsType<StatusCodeResult>(todoDuplicate);
            }
        }

        [Fact]
        public async Task Update_Update_An_Existing_Todo()
        {
            var options = CreateDbContextOptions();
            using (var todoApiDbContext = new TodoApiDbContext(options))
            {
                var mockLogger = new Mock<ILogger<TodoController>>();
                var todoController = new TodoController(mockLogger.Object, todoApiDbContext);
                var todoCreateResult = await todoController.Create(new TodoItem()
                {
                    Description = "Update this description"
                });
                var todoTemp = todoCreateResult as CreatedAtRouteResult;
                var id = (todoTemp.Value as TodoItem).Id;
                var todo = todoController.Update(id, new TodoItem()
                {
                    Id = id,
                    Description = "Write Blog Post"
                });

                Assert.IsType<NoContentResult>(todo);
            }
        }

        [Fact]
        public void Update_Returns_BadRequest_If_Ids_Not_Matching()
        {
            var options = CreateDbContextOptions();
            using (var todoApiDbContext = new TodoApiDbContext(options))
            {
                var mockLogger = new Mock<ILogger<TodoController>>();
                var todoController = new TodoController(mockLogger.Object, todoApiDbContext);

                var todo = todoController.Update(1, new TodoItem()
                {
                    Id = 15,
                    Description = "Write Blog Post"
                });

                Assert.IsType<BadRequestResult>(todo);
            }
        }

        [Fact]
        public void Update_Returns_NotFound_If_Todo_NotFound()
        {
            var options = CreateDbContextOptions();
            using (var todoApiDbContext = new TodoApiDbContext(options))
            {
                var mockLogger = new Mock<ILogger<TodoController>>();
                var todoController = new TodoController(mockLogger.Object, todoApiDbContext);

                var todo = todoController.Update(15, new TodoItem()
                {
                    Id = 15,
                    Description = "Write Blog Post"
                });

                Assert.IsType<NotFoundResult>(todo);
            }
        }

        [Fact]
        public void Delete_Returns_NotFound_If_Todo_NotFound()
        {
            var options = CreateDbContextOptions();
            using (var todoApiDbContext = new TodoApiDbContext(options))
            {
                var mockLogger = new Mock<ILogger<TodoController>>();
                var todoController = new TodoController(mockLogger.Object, todoApiDbContext);

                var todo = todoController.Delete(1500);

                Assert.IsType<NotFoundResult>(todo);
            }
        }

        [Fact]
        public void Delete_Returns_NoContent_If_Success()
        {
            var options = CreateDbContextOptions();
            using (var todoApiDbContext = new TodoApiDbContext(options))
            {
                var mockLogger = new Mock<ILogger<TodoController>>();
                var todoController = new TodoController(mockLogger.Object, todoApiDbContext);

                var todo = todoController.Delete(1);

                Assert.IsType<NoContentResult>(todo);
            }
        }

        private DbContextOptions<TodoApiDbContext> CreateDbContextOptions()
        {
            var options = new DbContextOptionsBuilder<TodoApiDbContext>()
                .UseInMemoryDatabase(databaseName: "TodoItems")
                .Options;

            using (var context = new TodoApiDbContext(options))
            {
                context.TodoItems.Add(new TodoItem() { Description = "Implement unit testing." });
                context.SaveChanges();
            }

            return options;
        }
    }
}
