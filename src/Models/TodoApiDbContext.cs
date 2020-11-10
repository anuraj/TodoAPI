using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;

namespace TodoApi.Models
{
    public class TodoApiDbContext : DbContext
    {
        public TodoApiDbContext(DbContextOptions options) : base(options)
        {
        }
        [ExcludeFromCodeCoverage]
        protected TodoApiDbContext()
        {
        }
        public DbSet<TodoItem> TodoItems { get; set; }
        public DbSet<Tag> Tags { get; set; }
    }
}