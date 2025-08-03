using System.ComponentModel.DataAnnotations;

namespace TodoApi.Models
{
    public class TodoItem
    {
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        [StringLength(1000)]
        public string? Description { get; set; }

        public bool IsCompleted { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? CompletedAt { get; set; }

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        [Required]
        [StringLength(50)]
        public string Priority { get; set; } = "Medium";

        public DateTime? DueDate { get; set; }
    }
}
