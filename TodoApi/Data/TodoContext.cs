using Microsoft.EntityFrameworkCore;
using TodoApi.Models;

namespace TodoApi.Data
{
    public class TodoContext : DbContext
    {
        public TodoContext(DbContextOptions<TodoContext> options) : base(options)
        {
        }

        public DbSet<TodoItem> TodoItems { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TodoItem>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Description).HasMaxLength(1000);
                entity.Property(e => e.Priority).IsRequired().HasMaxLength(50);

                // Index for performance
                entity.HasIndex(e => e.IsCompleted);
                entity.HasIndex(e => e.CreatedAt);
                entity.HasIndex(e => e.Priority);
            });

            // Seed some initial data
            modelBuilder.Entity<TodoItem>().HasData(
                new TodoItem
                {
                    Id = 1,
                    Title = "Learn .NET 8",
                    Description = "Study the new features in .NET 8",
                    Priority = "High",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new TodoItem
                {
                    Id = 2,
                    Title = "Build Todo API",
                    Description = "Create a RESTful API for Todo management",
                    Priority = "Medium",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new TodoItem
                {
                    Id = 3,
                    Title = "Write unit tests",
                    Description = "Add comprehensive test coverage",
                    Priority = "High",
                    IsCompleted = true,
                    CompletedAt = DateTime.UtcNow,
                    CreatedAt = DateTime.UtcNow.AddDays(-1),
                    UpdatedAt = DateTime.UtcNow
                }
            );
        }
    }
}
