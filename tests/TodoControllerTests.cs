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
using Microsoft.Extensions.Logging.Abstractions;

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
        public void GetOne_Logging_Working()
        {
            var options = CreateDbContextOptions();
            using (var todoApiDbContext = new TodoApiDbContext(options))
            {
                var mockLogger = new Mock<ILogger<TodoController>>();
                var todoController = new TodoController(mockLogger.Object, todoApiDbContext);

                var todo = todoController.GetById(1);

                Assert.IsType<TodoItem>(todo.Value);

                mockLogger.Verify(x => x.Log(
                    It.IsAny<LogLevel>(),
                    It.IsAny<EventId>(),
                    It.IsAny<It.IsAnyType>(),
                    It.IsAny<Exception>(),
                    (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()), Times.Exactly(2));
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
        public void Create_Logging_Working()
        {
            var options = CreateDbContextOptions();
            using (var todoApiDbContext = new TodoApiDbContext(options))
            {
                var mockLogger = new Mock<ILogger<TodoController>>();
                var todoController = new TodoController(mockLogger.Object, todoApiDbContext);

                var todo = todoController.Create(new TodoItem() { Description = "Check logging working on Create" });

                Assert.IsType<CreatedAtRouteResult>(todo.Result);

                mockLogger.Verify(x => x.Log(
                    It.IsAny<LogLevel>(),
                    It.IsAny<EventId>(),
                    It.IsAny<It.IsAnyType>(),
                    It.IsAny<Exception>(),
                    (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()), Times.Exactly(2));
            }
        }

        [Fact]
        public void Create_Creates_A_TodoItem()
        {
            var options = CreateDbContextOptions();
            using (var todoApiDbContext = new TodoApiDbContext(options))
            {
                var mockLogger = new Mock<ILogger<TodoController>>();
                var todoController = new TodoController(mockLogger.Object, todoApiDbContext);
                var todo = todoController.Create(new TodoItem() { Description = $"Write test on Create todo item" });

                Assert.IsType<CreatedAtRouteResult>(todo.Result);
                var result = todo.Result as CreatedAtRouteResult;
                Assert.NotNull(result);
                Assert.IsType<TodoItem>(result.Value);
                Assert.Equal("GetTodo", result.RouteName);
            }
        }

        [Fact]
        public void Create_Returns_Conflict_If_Todo_Exists()
        {
            var options = CreateDbContextOptions();
            using (var todoApiDbContext = new TodoApiDbContext(options))
            {
                var mockLogger = new Mock<ILogger<TodoController>>();
                var todoController = new TodoController(mockLogger.Object, todoApiDbContext);

                var todo = todoController.Create(new TodoItem() { Description = "Write Blog Post" });
                var todoDuplicate = todoController.Create(new TodoItem() { Description = "Write Blog Post" });

                Assert.IsType<CreatedAtRouteResult>(todo.Result);
                Assert.IsType<StatusCodeResult>(todoDuplicate.Result);
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
