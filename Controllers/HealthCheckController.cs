using Microsoft.AspNetCore.Mvc;
using TodoApi.Models;

[ApiController]
[Route("api/[controller]")]
public class HealthCheckController : ControllerBase
{
    private readonly TodoDbContext _context;

    public HealthCheckController(TodoDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> CheckConnection()
    {
        try
        {
            bool canConnect = await _context.Database.CanConnectAsync();
            if (canConnect)
            {
                return Ok("✅ SUCESSO: A API está conectada ao banco 'todo_db'.");
            }
            else
            {
                return StatusCode(500, "❌ FALHA: CanConnectAsync() retornou false.");
            }
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"❌ FALHA: Erro ao tentar conectar. Verifique sua string de conexão.\n\nDetalhes: {ex.Message}");
        }
    }
}