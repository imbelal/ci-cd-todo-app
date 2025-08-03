using Microsoft.EntityFrameworkCore;
using TodoApi.Data;
using TodoApi.Models;
using Xunit;

namespace TodoApi.Tests.Data
{
    public class TodoContextTests : IDisposable
    {
        private readonly TodoContext _context;

        public TodoContextTests()
        {
            var options = new DbContextOptionsBuilder<TodoContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new TodoContext(options);
        }

        [Fact]
        public void Context_ShouldCreateDatabase()
        {
            // Act
            _context.Database.EnsureCreated();

            // Assert
            Assert.True(_context.Database.CanConnect());
        }

        [Fact]
        public void Context_ShouldHaveTodoItemsDbSet()
        {
            // Assert
            Assert.NotNull(_context.TodoItems);
        }

        [Fact]
        public void Context_ShouldSeedInitialData()
        {
            // Act
            _context.Database.EnsureCreated();

            // Assert
            var todoItems = _context.TodoItems.ToList();
            Assert.Equal(3, todoItems.Count);

            var firstTodo = todoItems.FirstOrDefault(t => t.Title == "Learn .NET 8");
            Assert.NotNull(firstTodo);
            Assert.Equal("High", firstTodo.Priority);

            var completedTodo = todoItems.FirstOrDefault(t => t.IsCompleted);
            Assert.NotNull(completedTodo);
            Assert.Equal("Write unit tests", completedTodo.Title);
        }

        [Fact]
        public void Context_ShouldSaveNewTodoItem()
        {
            // Arrange
            _context.Database.EnsureCreated();
            var newTodo = new TodoItem
            {
                Title = "Test Todo",
                Description = "Test Description",
                Priority = "Low"
            };

            // Act
            _context.TodoItems.Add(newTodo);
            var result = _context.SaveChanges();

            // Assert
            Assert.Equal(1, result);
            Assert.True(newTodo.Id > 0);

            var savedTodo = _context.TodoItems.Find(newTodo.Id);
            Assert.NotNull(savedTodo);
            Assert.Equal("Test Todo", savedTodo.Title);
        }

        [Fact]
        public void Context_ShouldUpdateTodoItem()
        {
            // Arrange
            _context.Database.EnsureCreated();
            var todo = _context.TodoItems.First();
            var originalTitle = todo.Title;

            // Act
            todo.Title = "Updated Title";
            todo.IsCompleted = true;
            todo.CompletedAt = DateTime.UtcNow;
            _context.SaveChanges();

            // Assert
            var updatedTodo = _context.TodoItems.Find(todo.Id);
            Assert.NotNull(updatedTodo);
            Assert.Equal("Updated Title", updatedTodo.Title);
            Assert.True(updatedTodo.IsCompleted);
            Assert.NotNull(updatedTodo.CompletedAt);
            Assert.NotEqual(originalTitle, updatedTodo.Title);
        }

        [Fact]
        public void Context_ShouldDeleteTodoItem()
        {
            // Arrange
            _context.Database.EnsureCreated();
            var todoToDelete = _context.TodoItems.First();
            var todoId = todoToDelete.Id;

            // Act
            _context.TodoItems.Remove(todoToDelete);
            _context.SaveChanges();

            // Assert
            var deletedTodo = _context.TodoItems.Find(todoId);
            Assert.Null(deletedTodo);
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
