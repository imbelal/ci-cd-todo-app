using System.ComponentModel.DataAnnotations;

namespace TodoApi.DTOs
{
    public class TodoItemDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsCompleted { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string Priority { get; set; } = "Medium";
        public DateTime? DueDate { get; set; }
    }

    public class CreateTodoItemDto
    {
        [Required]
        [StringLength(200, MinimumLength = 1)]
        public string Title { get; set; } = string.Empty;

        [StringLength(1000)]
        public string? Description { get; set; }

        [Required]
        [RegularExpression("^(Low|Medium|High)$", ErrorMessage = "Priority must be Low, Medium, or High")]
        public string Priority { get; set; } = "Medium";

        public DateTime? DueDate { get; set; }
    }

    public class UpdateTodoItemDto
    {
        [StringLength(200, MinimumLength = 1)]
        public string? Title { get; set; }

        [StringLength(1000)]
        public string? Description { get; set; }

        public bool? IsCompleted { get; set; }

        [RegularExpression("^(Low|Medium|High)$", ErrorMessage = "Priority must be Low, Medium, or High")]
        public string? Priority { get; set; }

        public DateTime? DueDate { get; set; }
    }
}
