using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodoApi.Models;
using TodoApi.Models.DTOs;
using System.Security.Cryptography; 
using System.Text; 

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly TodoDbContext _context;
    private readonly ILogger<UsersController> _logger;

    public UsersController(TodoDbContext context, ILogger<UsersController> logger)
    {
        _context = context;
        _logger = logger;
    }

    // GET
    [HttpGet]
    public async Task<ActionResult<IEnumerable<User>>> GetUsers()
    {
        return await _context.Users.ToListAsync();
    }

    // GET BY ID
    [HttpGet("{id}")]
    public async Task<ActionResult<User>> GetUser(int id)
    {
        var user = await _context.Users.FindAsync(id);

        if (user == null)
        {
            return NotFound();
        }

        return user;
    }

    // POST
    [HttpPost]
    public async Task<ActionResult<User>> PostUser([FromBody] CreateUserDto userDto)
    {
        try
        {
            // Verificar se o email já existe
            var existingUser = await _context.Users
                .FirstOrDefaultAsync(u => u.Email.ToLower() == userDto.Email.ToLower());

            if (existingUser != null)
            {
                _logger.LogWarning("Tentativa de criar utilizador com email duplicado: {Email}", userDto.Email);
                return BadRequest(new
                {
                    error = "Email já existe",
                    message = "Já existe um utilizador com este email."
                });
            }

            var user = new User
            {
                Name = userDto.Name.Trim(),
                Email = userDto.Email.Trim().ToLower(),
                BirthdayDate = userDto.BirthdayDate,
                Working = userDto.Working?.Trim(),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Utilizador criado com sucesso: {UserId}", user.Id);

            return CreatedAtAction(nameof(GetUser), new { id = user.Id }, user);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao criar utilizador");
            return StatusCode(500, new
            {
                error = "Erro ao criar utilizador",
                message = "Ocorreu um erro ao processar a requisição."
            });
        }
    }

    // PUT BY ID
    [HttpPut("{id}")]
    public async Task<IActionResult> PutUser(int id, [FromBody] UpdateUserDto userDto)
    {
        try
        {
            if (id != userDto.Id)
            {
                return BadRequest(new
                {
                    error = "ID inválido",
                    message = "O ID da URL não corresponde ao ID do utilizador."
                });
            }

            var existingUser = await _context.Users.FindAsync(id);
            if (existingUser == null)
            {
                _logger.LogWarning("Tentativa de atualizar utilizador inexistente: {UserId}", id);
                return NotFound(new
                {
                    error = "Utilizador não encontrado",
                    message = $"O utilizador com ID {id} não existe."
                });
            }

            // Verificar se o email já está em uso por outro usuário
            var emailExists = await _context.Users
                .AnyAsync(u => u.Email.ToLower() == userDto.Email.ToLower() && u.Id != id);

            if (emailExists)
            {
                _logger.LogWarning("Tentativa de atualizar utilizador com email duplicado: {Email}", userDto.Email);
                return BadRequest(new
                {
                    error = "Email já existe",
                    message = "Já existe outro utilizador com este email."
                });
            }

            existingUser.Name = userDto.Name.Trim();
            existingUser.Email = userDto.Email.Trim().ToLower();
            existingUser.BirthdayDate = userDto.BirthdayDate;
            existingUser.Working = userDto.Working?.Trim();
            existingUser.UpdatedAt = DateTime.UtcNow;

            _context.Entry(existingUser).State = EntityState.Modified;

            await _context.SaveChangesAsync();

            _logger.LogInformation("Utilizador atualizado com sucesso: {UserId}", id);

            return NoContent();
        }
        catch (DbUpdateConcurrencyException ex)
        {
            _logger.LogError(ex, "Erro de concorrência ao atualizar utilizador: {UserId}", id);
            if (!_context.Users.Any(e => e.Id == id))
            {
                return NotFound(new
                {
                    error = "Utilizador não encontrado",
                    message = $"O utilizador com ID {id} não existe."
                });
            }
            else
            {
                return StatusCode(409, new
                {
                    error = "Conflito de concorrência",
                    message = "O utilizador foi modificado por outro utilizador. Por favor, recarregue e tente novamente."
                });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao atualizar utilizador: {UserId}", id);
            return StatusCode(500, new
            {
                error = "Erro ao atualizar utilizador",
                message = "Ocorreu um erro ao processar a requisição."
            });
        }
    }

    // DELETE
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUser(int id)
    {
        try
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                _logger.LogWarning("Tentativa de excluir utilizador inexistente: {UserId}", id);
                return NotFound(new
                {
                    error = "Utilizador não encontrado",
                    message = $"O utilizador com ID {id} não existe."
                });
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Utilizador excluído com sucesso: {UserId}", id);

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao excluir utilizador: {UserId}", id);
            return StatusCode(500, new
            {
                error = "Erro ao excluir utilizador",
                message = "Ocorreu um erro ao processar a requisição."
            });
        }
    }
}