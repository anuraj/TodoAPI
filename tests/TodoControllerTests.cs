using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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
