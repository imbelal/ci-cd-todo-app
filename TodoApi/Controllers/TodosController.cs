using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodoApi.Data;
using TodoApi.DTOs;
using TodoApi.Models;

namespace TodoApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TodosController : ControllerBase
    {
        private readonly TodoContext _context;
        private readonly ILogger<TodosController> _logger;

        public TodosController(TodoContext context, ILogger<TodosController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: api/todos
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TodoItemDto>>> GetTodos(
            [FromQuery] bool? isCompleted = null,
            [FromQuery] string? priority = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                var query = _context.TodoItems.AsQueryable();

                // Apply filters
                if (isCompleted.HasValue)
                {
                    query = query.Where(t => t.IsCompleted == isCompleted.Value);
                }

                if (!string.IsNullOrEmpty(priority))
                {
                    query = query.Where(t => t.Priority.ToLower() == priority.ToLower());
                }

                // Apply pagination
                var totalItems = await query.CountAsync();
                var items = await query
                    .OrderByDescending(t => t.CreatedAt)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Select(t => new TodoItemDto
                    {
                        Id = t.Id,
                        Title = t.Title,
                        Description = t.Description,
                        IsCompleted = t.IsCompleted,
                        CreatedAt = t.CreatedAt,
                        CompletedAt = t.CompletedAt,
                        UpdatedAt = t.UpdatedAt,
                        Priority = t.Priority,
                        DueDate = t.DueDate
                    })
                    .ToListAsync();

                Response.Headers["X-Total-Count"] = totalItems.ToString();
                Response.Headers["X-Page"] = page.ToString();
                Response.Headers["X-Page-Size"] = pageSize.ToString();

                return Ok(items);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving todos");
                return StatusCode(500, "An error occurred while retrieving todos");
            }
        }

        // GET: api/todos/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TodoItemDto>> GetTodo(int id)
        {
            try
            {
                var todo = await _context.TodoItems.FindAsync(id);

                if (todo == null)
                {
                    return NotFound($"Todo with ID {id} not found");
                }

                var todoDto = new TodoItemDto
                {
                    Id = todo.Id,
                    Title = todo.Title,
                    Description = todo.Description,
                    IsCompleted = todo.IsCompleted,
                    CreatedAt = todo.CreatedAt,
                    CompletedAt = todo.CompletedAt,
                    UpdatedAt = todo.UpdatedAt,
                    Priority = todo.Priority,
                    DueDate = todo.DueDate
                };

                return Ok(todoDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving todo with ID {TodoId}", id);
                return StatusCode(500, "An error occurred while retrieving the todo");
            }
        }

        // POST: api/todos
        [HttpPost]
        public async Task<ActionResult<TodoItemDto>> CreateTodo(CreateTodoItemDto createDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var todo = new TodoItem
                {
                    Title = createDto.Title,
                    Description = createDto.Description,
                    Priority = createDto.Priority,
                    DueDate = createDto.DueDate,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                _context.TodoItems.Add(todo);
                await _context.SaveChangesAsync();

                var todoDto = new TodoItemDto
                {
                    Id = todo.Id,
                    Title = todo.Title,
                    Description = todo.Description,
                    IsCompleted = todo.IsCompleted,
                    CreatedAt = todo.CreatedAt,
                    CompletedAt = todo.CompletedAt,
                    UpdatedAt = todo.UpdatedAt,
                    Priority = todo.Priority,
                    DueDate = todo.DueDate
                };

                return CreatedAtAction(nameof(GetTodo), new { id = todo.Id }, todoDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating todo");
                return StatusCode(500, "An error occurred while creating the todo");
            }
        }

        // PUT: api/todos/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTodo(int id, UpdateTodoItemDto updateDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var todo = await _context.TodoItems.FindAsync(id);
                if (todo == null)
                {
                    return NotFound($"Todo with ID {id} not found");
                }

                // Update only provided fields
                if (!string.IsNullOrEmpty(updateDto.Title))
                {
                    todo.Title = updateDto.Title;
                }

                if (updateDto.Description != null)
                {
                    todo.Description = updateDto.Description;
                }

                if (updateDto.IsCompleted.HasValue)
                {
                    todo.IsCompleted = updateDto.IsCompleted.Value;
                    todo.CompletedAt = updateDto.IsCompleted.Value ? DateTime.UtcNow : null;
                }

                if (!string.IsNullOrEmpty(updateDto.Priority))
                {
                    todo.Priority = updateDto.Priority;
                }

                if (updateDto.DueDate.HasValue)
                {
                    todo.DueDate = updateDto.DueDate.Value;
                }

                todo.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating todo with ID {TodoId}", id);
                return StatusCode(500, "An error occurred while updating the todo");
            }
        }

        // DELETE: api/todos/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTodo(int id)
        {
            try
            {
                var todo = await _context.TodoItems.FindAsync(id);
                if (todo == null)
                {
                    return NotFound($"Todo with ID {id} not found");
                }

                _context.TodoItems.Remove(todo);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting todo with ID {TodoId}", id);
                return StatusCode(500, "An error occurred while deleting the todo");
            }
        }

        // GET: api/todos/stats
        [HttpGet("stats")]
        public async Task<ActionResult> GetTodoStats()
        {
            try
            {
                var totalTodos = await _context.TodoItems.CountAsync();
                var completedTodos = await _context.TodoItems.CountAsync(t => t.IsCompleted);
                var pendingTodos = totalTodos - completedTodos;
                var overdueTodos = await _context.TodoItems
                    .CountAsync(t => !t.IsCompleted && t.DueDate.HasValue && t.DueDate < DateTime.UtcNow);

                var stats = new
                {
                    TotalTodos = totalTodos,
                    CompletedTodos = completedTodos,
                    PendingTodos = pendingTodos,
                    OverdueTodos = overdueTodos,
                    CompletionRate = totalTodos > 0 ? Math.Round((double)completedTodos / totalTodos * 100, 2) : 0
                };

                return Ok(stats);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving todo statistics");
                return StatusCode(500, "An error occurred while retrieving statistics");
            }
        }
    }
}
