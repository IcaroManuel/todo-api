using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodoApi.Models;
using TodoApi.Models.DTOs;

[ApiController]
[Route("api/[controller]")]
public class TasksController : ControllerBase
{
    private readonly TodoDbContext _context;
    private readonly ILogger<TasksController> _logger;

    public TasksController(TodoDbContext context, ILogger<TasksController> logger)
    {
        _context = context;
        _logger = logger;
    }

    // GET
    [HttpGet]
    public async Task<ActionResult<IEnumerable<TodoApi.Models.Task>>> GetTasks()
    {
        var tasks = await _context.Tasks
        .Include(t => t.User) 
        .ToListAsync();
    
    return tasks;
    }

    // GET BY ID
    [HttpGet("{id}")]
    public async Task<ActionResult<TodoApi.Models.Task>> GetTask(int id)
    {
        var task = await _context.Tasks.FindAsync(id);

        if (task == null)
        {
            return NotFound();
        }

        return task;
    }

    // POST
    [HttpPost]
    public async Task<ActionResult<TodoApi.Models.Task>> PostTask([FromBody] CreateTaskDto taskDto)
    {
        try
        {
            // Validar se o usuário existe
            var user = await _context.Users.FindAsync(taskDto.UserId);
            if (user == null)
            {
                _logger.LogWarning("Tentativa de criar tarefa com UserId inexistente: {UserId}", taskDto.UserId);
                return BadRequest(new
                {
                    error = "Utilizador não encontrado",
                    message = $"O utilizador com ID {taskDto.UserId} não existe."
                });
            }

            // Criar a tarefa
            var task = new TodoApi.Models.Task
            {
                Title = taskDto.Title.Trim(),
                Description = taskDto.Description?.Trim(),
                Status = taskDto.Status,
                UserId = taskDto.UserId,
                User = user,
                CreatedDate = DateTime.UtcNow,
                UpdatedDate = DateTime.UtcNow
            };

            _context.Tasks.Add(task);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Tarefa criada com sucesso: {TaskId}", task.Id);

            return CreatedAtAction(nameof(GetTask), new { id = task.Id }, task);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao criar tarefa");
            return StatusCode(500, new
            {
                error = "Erro ao criar tarefa",
                message = "Ocorreu um erro ao processar a requisição."
            });
        }
    }

    // PUT BY ID
    [HttpPut("{id}")]
    public async Task<IActionResult> PutTask(int id, [FromBody] UpdateTaskDto taskDto)
    {
        try
        {
            // Validar se o ID da URL corresponde ao ID do DTO
            if (id != taskDto.Id)
            {
                return BadRequest(new
                {
                    error = "ID inválido",
                    message = "O ID da URL não corresponde ao ID da tarefa."
                });
            }

            // Buscar a tarefa existente
            var existingTask = await _context.Tasks.FindAsync(id);
            if (existingTask == null)
            {
                _logger.LogWarning("Tentativa de atualizar tarefa inexistente: {TaskId}", id);
                return NotFound(new
                {
                    error = "Tarefa não encontrada",
                    message = $"A tarefa com ID {id} não existe."
                });
            }

            // Validar se o usuário existe
            var user = await _context.Users.FindAsync(taskDto.UserId);
            if (user == null)
            {
                _logger.LogWarning("Tentativa de atualizar tarefa com UserId inexistente: {UserId}", taskDto.UserId);
                return BadRequest(new
                {
                    error = "Utilizador não encontrado",
                    message = $"O utilizador com ID {taskDto.UserId} não existe."
                });
            }

            // Atualizar os campos
            existingTask.Title = taskDto.Title.Trim();
            existingTask.Description = taskDto.Description?.Trim();
            existingTask.Status = taskDto.Status;
            existingTask.UserId = taskDto.UserId;
            existingTask.User = user;
            existingTask.UpdatedDate = DateTime.UtcNow;

            _context.Entry(existingTask).State = EntityState.Modified;

            await _context.SaveChangesAsync();

            _logger.LogInformation("Tarefa atualizada com sucesso: {TaskId}", id);

            return NoContent();
        }
        catch (DbUpdateConcurrencyException ex)
        {
            _logger.LogError(ex, "Erro de concorrência ao atualizar tarefa: {TaskId}", id);
            if (!_context.Tasks.Any(e => e.Id == id))
            {
                return NotFound(new
                {
                    error = "Tarefa não encontrada",
                    message = $"A tarefa com ID {id} não existe."
                });
            }
            else
            {
                return StatusCode(409, new
                {
                    error = "Conflito de concorrência",
                    message = "A tarefa foi modificada por outro utilizador. Por favor, recarregue e tente novamente."
                });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao atualizar tarefa: {TaskId}", id);
            return StatusCode(500, new
            {
                error = "Erro ao atualizar tarefa",
                message = "Ocorreu um erro ao processar a requisição."
            });
        }
    }

    // DELETE
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTask(int id)
    {
        try
        {
            var task = await _context.Tasks.FindAsync(id);
            if (task == null)
            {
                _logger.LogWarning("Tentativa de excluir tarefa inexistente: {TaskId}", id);
                return NotFound(new
                {
                    error = "Tarefa não encontrada",
                    message = $"A tarefa com ID {id} não existe."
                });
            }

            _context.Tasks.Remove(task);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Tarefa excluída com sucesso: {TaskId}", id);

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao excluir tarefa: {TaskId}", id);
            return StatusCode(500, new
            {
                error = "Erro ao excluir tarefa",
                message = "Ocorreu um erro ao processar a requisição."
            });
        }
    }
}