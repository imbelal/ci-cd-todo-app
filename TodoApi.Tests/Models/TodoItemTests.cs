using System.ComponentModel.DataAnnotations;
using TodoApi.DTOs;
using TodoApi.Models;
using Xunit;

namespace TodoApi.Tests.Models
{
    public class TodoItemTests
    {
        [Fact]
        public void TodoItem_ShouldHaveDefaultValues()
        {
            // Act
            var todo = new TodoItem();

            // Assert
            Assert.Equal(0, todo.Id);
            Assert.Equal(string.Empty, todo.Title);
            Assert.Null(todo.Description);
            Assert.False(todo.IsCompleted);
            Assert.Equal("Medium", todo.Priority);
            Assert.Null(todo.CompletedAt);
            Assert.Null(todo.DueDate);

            // Check that dates are recent (within last 5 seconds)
            Assert.True(DateTime.UtcNow.Subtract(todo.CreatedAt).TotalSeconds < 5);
            Assert.True(DateTime.UtcNow.Subtract(todo.UpdatedAt).TotalSeconds < 5);
        }

        [Fact]
        public void TodoItem_ShouldAllowValidData()
        {
            // Arrange
            var dueDate = DateTime.UtcNow.AddDays(7);
            var completedAt = DateTime.UtcNow;

            // Act
            var todo = new TodoItem
            {
                Id = 1,
                Title = "Test Todo",
                Description = "Test Description",
                IsCompleted = true,
                Priority = "High",
                DueDate = dueDate,
                CompletedAt = completedAt
            };

            // Assert
            Assert.Equal(1, todo.Id);
            Assert.Equal("Test Todo", todo.Title);
            Assert.Equal("Test Description", todo.Description);
            Assert.True(todo.IsCompleted);
            Assert.Equal("High", todo.Priority);
            Assert.Equal(dueDate, todo.DueDate);
            Assert.Equal(completedAt, todo.CompletedAt);
        }
    }

    public class CreateTodoItemDtoTests
    {
        [Fact]
        public void CreateTodoItemDto_WithValidData_ShouldPassValidation()
        {
            // Arrange
            var dto = new CreateTodoItemDto
            {
                Title = "Valid Title",
                Description = "Valid Description",
                Priority = "High"
            };

            // Act
            var validationResults = ValidateModel(dto);

            // Assert
            Assert.Empty(validationResults);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        public void CreateTodoItemDto_WithInvalidTitle_ShouldFailValidation(string invalidTitle)
        {
            // Arrange
            var dto = new CreateTodoItemDto
            {
                Title = invalidTitle,
                Priority = "Medium"
            };

            // Act
            var validationResults = ValidateModel(dto);

            // Assert
            Assert.NotEmpty(validationResults);
            Assert.Contains(validationResults, vr => vr.MemberNames.Contains("Title"));
        }

        [Fact]
        public void CreateTodoItemDto_WithNullTitle_ShouldFailValidation()
        {
            // Arrange
            var dto = new CreateTodoItemDto
            {
                Title = null!,
                Priority = "Medium"
            };

            // Act
            var validationResults = ValidateModel(dto);

            // Assert
            Assert.NotEmpty(validationResults);
            Assert.Contains(validationResults, vr => vr.MemberNames.Contains("Title"));
        }

        [Theory]
        [InlineData("InvalidPriority")]
        [InlineData("low")] // Case sensitive
        [InlineData("HIGH")] // Case sensitive
        public void CreateTodoItemDto_WithInvalidPriority_ShouldFailValidation(string invalidPriority)
        {
            // Arrange
            var dto = new CreateTodoItemDto
            {
                Title = "Valid Title",
                Priority = invalidPriority
            };

            // Act
            var validationResults = ValidateModel(dto);

            // Assert
            Assert.NotEmpty(validationResults);
            Assert.Contains(validationResults, vr => vr.MemberNames.Contains("Priority"));
        }

        [Fact]
        public void CreateTodoItemDto_WithTooLongTitle_ShouldFailValidation()
        {
            // Arrange
            var dto = new CreateTodoItemDto
            {
                Title = new string('A', 201), // 201 characters, exceeds 200 limit
                Priority = "Medium"
            };

            // Act
            var validationResults = ValidateModel(dto);

            // Assert
            Assert.NotEmpty(validationResults);
            Assert.Contains(validationResults, vr => vr.MemberNames.Contains("Title"));
        }

        [Fact]
        public void CreateTodoItemDto_WithTooLongDescription_ShouldFailValidation()
        {
            // Arrange
            var dto = new CreateTodoItemDto
            {
                Title = "Valid Title",
                Description = new string('A', 1001), // 1001 characters, exceeds 1000 limit
                Priority = "Medium"
            };

            // Act
            var validationResults = ValidateModel(dto);

            // Assert
            Assert.NotEmpty(validationResults);
            Assert.Contains(validationResults, vr => vr.MemberNames.Contains("Description"));
        }

        private static IList<ValidationResult> ValidateModel(object model)
        {
            var validationResults = new List<ValidationResult>();
            var ctx = new ValidationContext(model, null, null);
            Validator.TryValidateObject(model, ctx, validationResults, true);
            return validationResults;
        }
    }

    public class UpdateTodoItemDtoTests
    {
        [Fact]
        public void UpdateTodoItemDto_WithValidData_ShouldPassValidation()
        {
            // Arrange
            var dto = new UpdateTodoItemDto
            {
                Title = "Updated Title",
                Description = "Updated Description",
                IsCompleted = true,
                Priority = "Low",
                DueDate = DateTime.UtcNow.AddDays(5)
            };

            // Act
            var validationResults = ValidateModel(dto);

            // Assert
            Assert.Empty(validationResults);
        }

        [Fact]
        public void UpdateTodoItemDto_WithNullValues_ShouldPassValidation()
        {
            // Arrange - All properties are optional for updates
            var dto = new UpdateTodoItemDto();

            // Act
            var validationResults = ValidateModel(dto);

            // Assert
            Assert.Empty(validationResults);
        }

        [Theory]
        [InlineData("InvalidPriority")]
        [InlineData("medium")] // Case sensitive
        public void UpdateTodoItemDto_WithInvalidPriority_ShouldFailValidation(string invalidPriority)
        {
            // Arrange
            var dto = new UpdateTodoItemDto
            {
                Priority = invalidPriority
            };

            // Act
            var validationResults = ValidateModel(dto);

            // Assert
            Assert.NotEmpty(validationResults);
            Assert.Contains(validationResults, vr => vr.MemberNames.Contains("Priority"));
        }

        [Fact]
        public void UpdateTodoItemDto_WithEmptyTitle_ShouldFailValidation()
        {
            // Arrange
            var dto = new UpdateTodoItemDto
            {
                Title = "" // Empty string should fail validation
            };

            // Act
            var validationResults = ValidateModel(dto);

            // Assert
            Assert.NotEmpty(validationResults);
            Assert.Contains(validationResults, vr => vr.MemberNames.Contains("Title"));
        }

        private static IList<ValidationResult> ValidateModel(object model)
        {
            var validationResults = new List<ValidationResult>();
            var ctx = new ValidationContext(model, null, null);
            Validator.TryValidateObject(model, ctx, validationResults, true);
            return validationResults;
        }
    }
}
