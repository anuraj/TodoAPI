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
        [ExcludeFromCodeCoverage]
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Tag>()
                .HasOne(tag => tag.TodoItem)
                .WithMany(item => item.Tags)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}