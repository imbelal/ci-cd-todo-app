using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http.Json;
using System.Text.Json;
using TodoApi.Controllers;
using TodoApi.Data;
using TodoApi.DTOs;
using Xunit;

namespace TodoApi.IntegrationTests
{
    public class TodoApiIntegrationTests : IClassFixture<WebApplicationFactory<TodosController>>
    {
        private readonly WebApplicationFactory<TodosController> _factory;
        private readonly HttpClient _client;

        public TodoApiIntegrationTests(WebApplicationFactory<TodosController> factory)
        {
            _factory = factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    // Remove the existing DbContext registration
                    var descriptor = services.SingleOrDefault(
                        d => d.ServiceType == typeof(DbContextOptions<TodoContext>));
                    if (descriptor != null)
                        services.Remove(descriptor);

                    // Add a test database
                    services.AddDbContext<TodoContext>(options =>
                    {
                        options.UseInMemoryDatabase("TestDb");
                    });
                });
            });

            _client = _factory.CreateClient();
        }

        [Fact]
        public async Task Get_Health_ShouldReturnHealthy()
        {
            // Act
            var response = await _client.GetAsync("/health");

            // Assert
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            Assert.Contains("Healthy", content);
        }

        [Fact]
        public async Task Get_Todos_ShouldReturnTodos()
        {
            // Act
            var response = await _client.GetAsync("/api/todos");

            // Assert
            response.EnsureSuccessStatusCode();
            var todos = await response.Content.ReadFromJsonAsync<TodoItemDto[]>();
            Assert.NotNull(todos);
            Assert.True(todos.Length >= 3); // At least 3 from seed data
        }

        [Fact]
        public async Task Get_Todo_WithValidId_ShouldReturnTodo()
        {
            // Arrange - Get the first todo's ID
            var todosResponse = await _client.GetAsync("/api/todos");
            var todos = await todosResponse.Content.ReadFromJsonAsync<TodoItemDto[]>();
            var firstTodoId = todos![0].Id;

            // Act
            var response = await _client.GetAsync($"/api/todos/{firstTodoId}");

            // Assert
            response.EnsureSuccessStatusCode();
            var todo = await response.Content.ReadFromJsonAsync<TodoItemDto>();
            Assert.NotNull(todo);
            Assert.Equal(firstTodoId, todo.Id);
        }

        [Fact]
        public async Task Get_Todo_WithInvalidId_ShouldReturnNotFound()
        {
            // Act
            var response = await _client.GetAsync("/api/todos/999");

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task Post_Todo_WithValidData_ShouldCreateTodo()
        {
            // Arrange
            var createDto = new CreateTodoItemDto
            {
                Title = "Integration Test Todo",
                Description = "Created via integration test",
                Priority = "High",
                DueDate = DateTime.UtcNow.AddDays(7)
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/todos", createDto);

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.Equal(System.Net.HttpStatusCode.Created, response.StatusCode);

            var createdTodo = await response.Content.ReadFromJsonAsync<TodoItemDto>();
            Assert.NotNull(createdTodo);
            Assert.Equal("Integration Test Todo", createdTodo.Title);
            Assert.Equal("High", createdTodo.Priority);
            Assert.False(createdTodo.IsCompleted);
        }

        [Fact]
        public async Task Post_Todo_WithInvalidData_ShouldReturnBadRequest()
        {
            // Arrange
            var createDto = new CreateTodoItemDto
            {
                Title = "", // Invalid - empty title
                Priority = "InvalidPriority" // Invalid priority
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/todos", createDto);

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task Put_Todo_WithValidData_ShouldUpdateTodo()
        {
            // Arrange - Get a todo to update
            var todosResponse = await _client.GetAsync("/api/todos");
            var todos = await todosResponse.Content.ReadFromJsonAsync<TodoItemDto[]>();
            var todoToUpdate = todos![0];

            var updateDto = new UpdateTodoItemDto
            {
                Title = "Updated via Integration Test",
                IsCompleted = true,
                Priority = "Low"
            };

            // Act
            var response = await _client.PutAsJsonAsync($"/api/todos/{todoToUpdate.Id}", updateDto);

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.Equal(System.Net.HttpStatusCode.NoContent, response.StatusCode);

            // Verify the update
            var updatedResponse = await _client.GetAsync($"/api/todos/{todoToUpdate.Id}");
            var updatedTodo = await updatedResponse.Content.ReadFromJsonAsync<TodoItemDto>();
            Assert.NotNull(updatedTodo);
            Assert.Equal("Updated via Integration Test", updatedTodo.Title);
            Assert.True(updatedTodo.IsCompleted);
            Assert.Equal("Low", updatedTodo.Priority);
        }

        [Fact]
        public async Task Delete_Todo_WithValidId_ShouldDeleteTodo()
        {
            // Arrange - Create a todo to delete
            var createDto = new CreateTodoItemDto
            {
                Title = "Todo to Delete",
                Priority = "Medium"
            };
            var createResponse = await _client.PostAsJsonAsync("/api/todos", createDto);
            var createdTodo = await createResponse.Content.ReadFromJsonAsync<TodoItemDto>();

            // Act
            var response = await _client.DeleteAsync($"/api/todos/{createdTodo!.Id}");

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.Equal(System.Net.HttpStatusCode.NoContent, response.StatusCode);

            // Verify deletion
            var getResponse = await _client.GetAsync($"/api/todos/{createdTodo.Id}");
            Assert.Equal(System.Net.HttpStatusCode.NotFound, getResponse.StatusCode);
        }

        [Fact]
        public async Task Get_TodoStats_ShouldReturnCorrectStatistics()
        {
            // Act
            var response = await _client.GetAsync("/api/todos/stats");

            // Assert
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();

            using var document = JsonDocument.Parse(content);
            var root = document.RootElement;

            Assert.True(root.GetProperty("totalTodos").GetInt32() >= 3);
            Assert.True(root.TryGetProperty("completedTodos", out _));
            Assert.True(root.TryGetProperty("pendingTodos", out _));
            Assert.True(root.TryGetProperty("completionRate", out _));
        }

        [Fact]
        public async Task Get_Todos_WithFilters_ShouldReturnFilteredResults()
        {
            // Act - Test completion filter
            var response = await _client.GetAsync("/api/todos?isCompleted=true");

            // Assert
            response.EnsureSuccessStatusCode();
            var todos = await response.Content.ReadFromJsonAsync<TodoItemDto[]>();
            Assert.NotNull(todos);
            Assert.All(todos, todo => Assert.True(todo.IsCompleted));
        }

        [Fact]
        public async Task Get_Todos_WithPagination_ShouldReturnPagedResults()
        {
            // Act
            var response = await _client.GetAsync("/api/todos?page=1&pageSize=2");

            // Assert
            response.EnsureSuccessStatusCode();
            var todos = await response.Content.ReadFromJsonAsync<TodoItemDto[]>();
            Assert.NotNull(todos);
            Assert.True(todos.Length <= 2);

            // Check pagination headers
            Assert.True(response.Headers.Contains("X-Total-Count"));
            Assert.True(response.Headers.Contains("X-Page"));
            Assert.True(response.Headers.Contains("X-Page-Size"));
        }
    }
}
