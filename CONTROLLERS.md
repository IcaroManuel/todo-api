# 🎮 Documentação Completa dos Controllers

Esta documentação detalha todos os controllers criados no projeto TodoApi, incluindo código-fonte completo, explicações e exemplos de uso.

---

## 📝 TasksController.cs

**Rota Base:** `/api/tasks`  
**Responsabilidade:** Gerenciar operações CRUD para tarefas (Tasks)

### Código Completo

```csharp
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

    // GET: api/tasks
    [HttpGet]
    public async Task<ActionResult<IEnumerable<TodoApi.Models.Task>>> GetTasks()
    {
        var tasks = await _context.Tasks
            .Include(t => t.User)  // Eager loading do usuário relacionado
            .ToListAsync();
        
        return tasks;
    }

    // GET: api/tasks/5
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

    // POST: api/tasks
    [HttpPost]
    public async Task<ActionResult<TodoApi.Models.Task>> PostTask(TodoApi.Models.Task task)
    {
        // Busca o usuário pelo UserId fornecido
        var user = await _context.Users.FindAsync(task.UserId);
        
        // Atribui o User automaticamente
        task.User = user;
        task.CreatedDate = DateTime.UtcNow;
        task.UpdatedDate = DateTime.UtcNow;

        _context.Tasks.Add(task);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetTask), new { id = task.Id }, task);
    }

    // PUT: api/tasks/5
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

    // DELETE: api/tasks/5
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
```

### Características Principais

#### 1. Dependency Injection
- O `TodoDbContext` é injetado via construtor
- Gerenciado automaticamente pelo container DI do ASP.NET Core

#### 2. Include (Eager Loading)
```csharp
var tasks = await _context.Tasks
    .Include(t => t.User)
    .ToListAsync();
```
- Carrega dados do usuário junto com a tarefa
- Evita o problema N+1 de consultas ao banco

#### 3. Atribuição Automática de Usuário
- Nos métodos POST e PUT, o sistema busca o usuário pelo `UserId`
- Atribui automaticamente o objeto `User` à tarefa
- Graças ao `[JsonIgnore]`, o User não aparece no JSON de resposta

#### 4. Validações
- **PUT**: Verifica se ID da URL corresponde ao ID do body
- **POST/PUT**: Valida se o `UserId` existe no banco
- Retorna códigos HTTP apropriados:
  - `200 OK`: Sucesso na busca
  - `201 Created`: Recurso criado com sucesso
  - `204 No Content`: Atualização/exclusão bem-sucedida
  - `400 Bad Request`: Dados inválidos
  - `404 Not Found`: Recurso não encontrado

#### 5. Tratamento de Concorrência
```csharp
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
```
- Detecta conflitos quando múltiplos usuários editam o mesmo registro
- Retorna erro apropriado ou propaga exceção

---

## 👥 UsersController.cs

**Rota Base:** `/api/users`  
**Responsabilidade:** Gerenciar operações CRUD para usuários (Users)

### Código Completo

```csharp
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodoApi.Models;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly TodoDbContext _context;

    public UsersController(TodoDbContext context)
    {
        _context = context;
    }

    // GET: api/users
    [HttpGet]
    public async Task<ActionResult<IEnumerable<User>>> GetUsers()
    {
        return await _context.Users.ToListAsync();
    }

    // GET: api/users/5
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

    // POST: api/users
    [HttpPost]
    public async Task<ActionResult<User>> PostUser(User user)
    {
        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetUser), new { id = user.Id }, user);
    }

    // PUT: api/users/5
    [HttpPut("{id}")]
    public async Task<IActionResult> PutUser(int id, User user)
    {
        if (id != user.Id)
        {
            return BadRequest("O ID da URL não corresponde ao ID do usuário.");
        }

        _context.Entry(user).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!_context.Users.Any(e => e.Id == id))
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

    // DELETE: api/users/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUser(int id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null)
        {
            return NotFound();
        }

        _context.Users.Remove(user);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
```

### Características Principais

#### 1. CRUD Completo
- **GET**: Lista todos os usuários
- **GET by ID**: Busca usuário específico
- **POST**: Cria novo usuário
- **PUT**: Atualiza usuário existente
- **DELETE**: Remove usuário

#### 2. Exclusão em Cascata
```sql
FOREIGN KEY (userId) REFERENCES users(id) ON DELETE CASCADE
```
- Ao deletar um usuário, todas suas tarefas são automaticamente removidas
- Comportamento definido no nível do banco de dados

#### 3. Validações
- Verifica correspondência de IDs no PUT
- Trata exceções de concorrência
- Retorna status HTTP apropriados

#### 4. CreatedAtAction
```csharp
return CreatedAtAction(nameof(GetUser), new { id = user.Id }, user);
```
- Retorna `201 Created`
- Inclui header `Location` com URL do recurso criado
- Exemplo: `Location: /api/users/5`

---

## 🏥 HealthCheckController.cs

**Rota Base:** `/api/health`  
**Responsabilidade:** Verificar saúde da API e conexão com banco de dados

### Código Completo

```csharp
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

    // GET: api/health
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
```

### Características Principais

#### 1. Verificação de Conexão
```csharp
bool canConnect = await _context.Database.CanConnectAsync();
```
- Testa se a API consegue se conectar ao banco de dados
- Útil para monitoramento e troubleshooting
- Não executa queries, apenas testa conexão

#### 2. Tratamento de Erros
- Captura exceções de conexão
- Retorna mensagens detalhadas com stack trace
- Status HTTP apropriados:
  - `200 OK`: Conexão bem-sucedida
  - `500 Internal Server Error`: Falha na conexão

#### 3. Mensagens Claras
- Usa emojis para identificação visual rápida
- Inclui detalhes técnicos do erro
- Facilita diagnóstico de problemas

#### 4. Uso Prático

```bash
# Verificar se a API está funcionando
curl http://localhost:5201/api/health

# Resposta de sucesso:
# ✅ SUCESSO: A API está conectada ao banco 'todo_db'.

# Resposta de erro:
# ❌ FALHA: Erro ao tentar conectar. Verifique sua string de conexão.
# Detalhes: Unable to connect to any of the specified MySQL hosts.
```

---

## ⚙️ Program.cs - Configuração Completa

O arquivo `Program.cs` é o coração da aplicação, responsável por configurar todos os serviços e o pipeline de middleware.

### Código Completo

```csharp
using Microsoft.EntityFrameworkCore;
using TodoApi.Models;

var builder = WebApplication.CreateBuilder(args);

var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

// Obtém a connection string do appsettings.json
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// Configura o DbContext com MySQL
builder.Services.AddDbContext<TodoDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString))
);

// Configura os controllers com opções de JSON
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = 
        System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
});

// Configura CORS para permitir requisições do frontend
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
        policy =>
        {
            policy.WithOrigins("http://localhost:3000")
                    .AllowAnyHeader()
                    .AllowAnyMethod();
        });
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Habilita Swagger apenas em desenvolvimento
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Aplica a política CORS
app.UseCors(MyAllowSpecificOrigins);

app.UseAuthorization();

app.MapControllers();

app.Run();
```

---

## 📝 Explicação Detalhada das Configurações

### 1. Entity Framework Core + MySQL

```csharp
builder.Services.AddDbContext<TodoDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString))
);
```

**O que faz:**
- Registra o `TodoDbContext` no container de Dependency Injection
- Configura o provider MySQL usando Pomelo.EntityFrameworkCore.MySql
- `ServerVersion.AutoDetect()` detecta automaticamente a versão do MySQL instalada
- A connection string vem do `appsettings.json`

**Arquivo appsettings.json:**
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=todo_db;User=root;Password=sua_senha;"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*"
}
```

**Parâmetros da Connection String:**
- `Server=localhost`: Host do MySQL (pode ser IP ou domínio)
- `Database=todo_db`: Nome do banco de dados
- `User=root`: Usuário do MySQL
- `Password=sua_senha`: Senha do MySQL

---

### 2. JSON Serialization - ReferenceHandler.IgnoreCycles

```csharp
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = 
        System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
});
```

**O que faz:**
- Evita erros de referência cíclica ao serializar entidades relacionadas
- Permite serializar objetos que referenciam uns aos outros

**Exemplo do problema:**
```csharp
// Task referencia User
public class Task {
    public User User { get; set; }
}

// User referencia Tasks
public class User {
    public ICollection<Task> Tasks { get; set; }
}

// Sem IgnoreCycles: ERRO de loop infinito
// Com IgnoreCycles: Serializa corretamente
```

**Solução alternativa usada no projeto:**
```csharp
// Task.cs
[JsonIgnore]  // Ignora completamente o campo User no JSON
public virtual User? User { get; set; }
```

**Resultado no JSON:**
```json
{
  "id": 1,
  "title": "Minha tarefa",
  "userId": 5,
  // "user": {...}  ← Este campo NÃO aparece graças ao [JsonIgnore]
}
```

---

### 3. CORS (Cross-Origin Resource Sharing)

```csharp
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
        policy =>
        {
            policy.WithOrigins("http://localhost:3000")
                    .AllowAnyHeader()
                    .AllowAnyMethod();
        });
});

// ...

app.UseCors(MyAllowSpecificOrigins);
```

**O que faz:**
- Permite que aplicações frontend em origens diferentes façam requisições à API
- Fundamental para APIs que serão consumidas por SPAs (React, Next.js, Vue, etc.)

**Configuração atual:**
- `.WithOrigins("http://localhost:3000")`: Apenas o frontend Next.js na porta 3000 pode fazer requisições
- `.AllowAnyHeader()`: Aceita qualquer header HTTP (Content-Type, Authorization, etc.)
- `.AllowAnyMethod()`: Aceita qualquer método HTTP (GET, POST, PUT, DELETE, PATCH, etc.)

**⚠️ Configuração para Produção:**
```csharp
policy.WithOrigins("https://meu-dominio.com", "https://app.meu-dominio.com")
      .AllowAnyHeader()
      .AllowAnyMethod()
      .AllowCredentials();  // Se usar cookies/autenticação
```

**Exemplo de erro sem CORS:**
```
Access to fetch at 'http://localhost:5201/api/tasks' from origin 'http://localhost:3000' 
has been blocked by CORS policy: No 'Access-Control-Allow-Origin' header is present 
on the requested resource.
```

---

### 4. Swagger/OpenAPI

```csharp
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ...

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
```

**O que faz:**
- Gera documentação interativa da API automaticamente
- Disponível em `http://localhost:5201/swagger`
- **Apenas habilitado em ambiente de desenvolvimento** por questões de segurança

**Funcionalidades do Swagger UI:**
- 📖 Lista todos os endpoints da API
- 🧪 Permite testar endpoints diretamente no navegador
- 📋 Mostra schemas dos modelos de dados
- 🔍 Exibe parâmetros, tipos de retorno e códigos de status

**Exemplo de uso:**
1. Acesse `http://localhost:5201/swagger`
2. Escolha um endpoint (ex: `POST /api/tasks`)
3. Clique em "Try it out"
4. Preencha o JSON do body
5. Clique em "Execute"
6. Veja a resposta da API

---

### 5. Middleware Pipeline

```csharp
app.UseHttpsRedirection();            // 1. Redireciona HTTP para HTTPS
app.UseCors(MyAllowSpecificOrigins);  // 2. Aplica política CORS
app.UseAuthorization();               // 3. Habilita autorização
app.MapControllers();                 // 4. Mapeia os controllers
```

**⚠️ A ordem é MUITO importante!**

#### 1. UseHttpsRedirection()
- Redireciona automaticamente requisições HTTP para HTTPS
- Garante que todas as comunicações sejam criptografadas

#### 2. UseCors()
- Aplica a política CORS configurada
- Deve vir ANTES de `UseAuthorization()` e `MapControllers()`

#### 3. UseAuthorization()
- Habilita o sistema de autorização
- Necessário para usar `[Authorize]` em controllers (futuro)
- Permite adicionar autenticação JWT posteriormente

#### 4. MapControllers()
- Mapeia todos os controllers para suas rotas
- Deve ser o último na pipeline

**Exemplo de pipeline completo com autenticação (futuro):**
```csharp
app.UseHttpsRedirection();
app.UseCors(MyAllowSpecificOrigins);
app.UseAuthentication();  // ← Adicionar quando implementar JWT
app.UseAuthorization();
app.MapControllers();
```

---

## 🔐 Boas Práticas Implementadas

### 1. Async/Await em Todos os Métodos
```csharp
public async Task<ActionResult<User>> GetUser(int id)
{
    var user = await _context.Users.FindAsync(id);
    // ...
}
```
- Melhora performance e escalabilidade
- Não bloqueia threads durante operações de I/O
- Padrão recomendado para APIs modernas

### 2. ActionResult<T> para Tipagem Forte
```csharp
public async Task<ActionResult<User>> GetUser(int id)
```
- Permite retornar tanto o objeto tipado quanto status HTTP
- Melhora IntelliSense e documentação automática
- Swagger gera documentação mais precisa

### 3. CreatedAtAction para POST
```csharp
return CreatedAtAction(nameof(GetUser), new { id = user.Id }, user);
```
- Retorna `201 Created`
- Inclui header `Location` com URL do recurso criado
- Segue padrão REST corretamente

### 4. NoContent() para PUT e DELETE
```csharp
return NoContent();  // 204 No Content
```
- Indica sucesso sem retornar dados desnecessários
- Reduz tráfego de rede
- Padrão REST para operações de modificação

### 5. Validações e Mensagens Claras
```csharp
if (id != user.Id)
{
    return BadRequest("O ID da URL não corresponde ao ID do usuário.");
}
```
- Mensagens de erro em português
- Facilitam debugging
- Melhoram experiência do desenvolvedor

---

## 🚀 Próximos Passos Sugeridos

### 1. Implementar Autenticação JWT
```csharp
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options => { /* configuração */ });
```

### 2. Adicionar Validação de Dados
```csharp
using System.ComponentModel.DataAnnotations;

public class User
{
    [Required(ErrorMessage = "O nome é obrigatório")]
    [StringLength(255)]
    public string Name { get; set; }
    
    [Required]
    [EmailAddress(ErrorMessage = "Email inválido")]
    public string Email { get; set; }
}
```

### 3. Implementar Paginação
```csharp
[HttpGet]
public async Task<ActionResult<PagedResult<Task>>> GetTasks(
    [FromQuery] int page = 1, 
    [FromQuery] int pageSize = 10)
{
    var tasks = await _context.Tasks
        .Skip((page - 1) * pageSize)
        .Take(pageSize)
        .ToListAsync();
    
    return new PagedResult<Task> 
    { 
        Items = tasks, 
        Page = page, 
        PageSize = pageSize 
    };
}
```

### 4. Adicionar Filtros e Busca
```csharp
[HttpGet]
public async Task<ActionResult<IEnumerable<Task>>> GetTasks(
    [FromQuery] string? status = null,
    [FromQuery] string? search = null)
{
    var query = _context.Tasks.AsQueryable();
    
    if (!string.IsNullOrEmpty(status))
        query = query.Where(t => t.Status == status);
    
    if (!string.IsNullOrEmpty(search))
        query = query.Where(t => t.Title.Contains(search));
    
    return await query.ToListAsync();
}
```

### 5. Implementar Logging
```csharp
public class TasksController : ControllerBase
{
    private readonly ILogger<TasksController> _logger;
    
    public TasksController(TodoDbContext context, ILogger<TasksController> logger)
    {
        _context = context;
        _logger = logger;
    }
    
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Task>>> GetTasks()
    {
        _logger.LogInformation("Buscando todas as tarefas");
        return await _context.Tasks.ToListAsync();
    }
}
```

---

## 📚 Recursos e Referências

- [ASP.NET Core Documentation](https://learn.microsoft.com/en-us/aspnet/core/)
- [Entity Framework Core](https://learn.microsoft.com/en-us/ef/core/)
- [RESTful API Design](https://restfulapi.net/)
- [HTTP Status Codes](https://developer.mozilla.org/en-US/docs/Web/HTTP/Status)
- [CORS Explained](https://developer.mozilla.org/en-US/docs/Web/HTTP/CORS)

---

**✨ Documentação criada para o projeto TodoApi**  
**🚀 Happy Coding!**
