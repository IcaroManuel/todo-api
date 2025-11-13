using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodoApi.Models;

[ApiController]
[Route("api/[controller]")]
public class TasksController : ControllerBase
{
    private readonly TodoDbContext _context;

    public TasksController(TodoDbContext context)
    {
        _context = context;
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
    public async Task<ActionResult<TodoApi.Models.Task>> PostTask(TodoApi.Models.Task task)
    {
        // Busca o usuário pelo UserId fornecido
        var user = await _context.Users.FindAsync(task.UserId);
        // if (user == null)
        // {
        //     return BadRequest("O UserId fornecido não existe.");
        // }

        // Atribdui o User por debaixo os panos
        task.User = user;
        task.CreatedDate = DateTime.UtcNow;
        task.UpdatedDate = DateTime.UtcNow;

        _context.Tasks.Add(task);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetTask), new { id = task.Id }, task);
    }

    // PUT BY ID
    [HttpPut("{id}")]
    public async Task<IActionResult> PutTask(int id, TodoApi.Models.Task task)
    {
        if (id != task.Id)
        {
            return BadRequest("O ID da URL não corresponde ao ID da tarefa.");
        }

        var user = await _context.Users.FindAsync(task.UserId);
        if (user == null)
        {
            return BadRequest("O UserId fornecido não existe.");
        }


        task.User = user;
        task.UpdatedDate = DateTime.UtcNow;

        _context.Entry(task).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!_context.Tasks.Any(e => e.Id == id))
            {
                return NotFound();
            }
            else
            {
                throw;
            }
        }

        return NoContent(); 
    }

    // DELETE
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTask(int id)
    {
        var task = await _context.Tasks.FindAsync(id);
        if (task == null)
        {
            return NotFound();
        }

        _context.Tasks.Remove(task);
        await _context.SaveChangesAsync();

        return NoContent(); 
    }
}