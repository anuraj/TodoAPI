using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using TodoApi.Models;

namespace TodoApi
{
    public class SeedData
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            var context = serviceProvider.GetRequiredService<TodoApiDbContext>();
            context.Database.EnsureCreated();
            if (!context.TodoItems.Any())
            {
                context.TodoItems.Add(new TodoItem() { Description = "Create blog post on API Management" });
                context.TodoItems.Add(new TodoItem() { Description = "Create blog post on Authentication" });
                context.SaveChanges();
            }
        }
    }
}