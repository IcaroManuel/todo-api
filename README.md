# 📋 Todo API - Sistema de Gerenciamento de Tarefas

## 📖 Índice

- [Criação do Banco de Dados](#-criação-do-banco-de-dados)
- [Visão Geral](#-visão-geral)
- [Arquitetura](#-arquitetura)
- [Tecnologias Utilizadas](#-tecnologias-utilizadas)
- [Estrutura do Projeto](#-estrutura-do-projeto)
- [Modelos de Dados](#-modelos-de-dados)
- [Controllers e Endpoints](#-controllers-e-endpoints)
- [Configuração do Program.cs](#-configuração-do-programcs)
- [Instalação e Configuração](#-instalação-e-configuração)
- [Executando o Projeto](#-executando-o-projeto)
- [Exemplos de Uso](#-exemplos-de-uso)

---

## 🗄️ Criação do Banco de Dados

### Scripts SQL para Criação

Execute os seguintes scripts SQL no MySQL para criar o banco de dados e as tabelas necessárias:

```sql
-- Criar o banco de dados
CREATE DATABASE todo_db;

-- Usar o banco de dados criado
USE todo_db;

-- Criar tabela de usuários
CREATE TABLE users (
    id INT PRIMARY KEY AUTO_INCREMENT,
    name VARCHAR(255) NOT NULL,
    email VARCHAR(255) NOT NULL UNIQUE,
    birthday_date DATE NULL,
    working VARCHAR(255) NULL,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- Criar tabela de tarefas
CREATE TABLE tasks (
    id INT PRIMARY KEY AUTO_INCREMENT,
    title VARCHAR(255) NOT NULL,
    description TEXT NULL,
    status ENUM('nao_iniciada', 'em_progresso', 'concluida') NOT NULL DEFAULT 'nao_iniciada',
    initial_date DATETIME NULL,
    finished_date DATETIME NULL,
    userId INT NOT NULL,
    created_date TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_date TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    FOREIGN KEY (userId) REFERENCES users(id) ON DELETE CASCADE
);
```

### Estrutura das Tabelas

#### 📋 Tabela `users`

| Campo | Tipo | Restrições | Descrição |
|-------|------|------------|-----------|
| `id` | INT | PRIMARY KEY, AUTO_INCREMENT | Identificador único do usuário |
| `name` | VARCHAR(255) | NOT NULL | Nome completo do usuário |
| `email` | VARCHAR(255) | NOT NULL, UNIQUE | Email único do usuário |
| `birthday_date` | DATE | NULL | Data de nascimento (opcional) |
| `working` | VARCHAR(255) | NULL | Cargo ou função atual (opcional) |
| `created_at` | TIMESTAMP | DEFAULT CURRENT_TIMESTAMP | Data/hora de criação do registro |
| `updated_at` | TIMESTAMP | DEFAULT CURRENT_TIMESTAMP | Data/hora da última atualização |

#### 📝 Tabela `tasks`

| Campo | Tipo | Restrições | Descrição |
|-------|------|------------|-----------|
| `id` | INT | PRIMARY KEY, AUTO_INCREMENT | Identificador único da tarefa |
| `title` | VARCHAR(255) | NOT NULL | Título da tarefa |
| `description` | TEXT | NULL | Descrição detalhada (opcional) |
| `status` | ENUM | NOT NULL, DEFAULT 'nao_iniciada' | Status: 'nao_iniciada', 'em_progresso', 'concluida' |
| `initial_date` | DATETIME | NULL | Data e hora de início da tarefa |
| `finished_date` | DATETIME | NULL | Data e hora de conclusão |
| `userId` | INT | NOT NULL, FOREIGN KEY | ID do usuário responsável pela tarefa |
| `created_date` | TIMESTAMP | DEFAULT CURRENT_TIMESTAMP | Data/hora de criação |
| `updated_date` | TIMESTAMP | DEFAULT CURRENT_TIMESTAMP ON UPDATE | Data/hora da última modificação |

### Relacionamentos e Regras

**Relacionamento Tasks ↔ Users:**

- Uma tarefa pertence a **um único usuário** (N:1)
- Um usuário pode ter **várias tarefas** (1:N)
- **Constraint:** `FOREIGN KEY (userId) REFERENCES users(id) ON DELETE CASCADE`
  - Ao deletar um usuário, todas suas tarefas são automaticamente removidas

**Regras de Negócio:**

- O campo `email` deve ser único no sistema
- O `status` só aceita três valores: 'nao_iniciada', 'em_progresso', 'concluida'
- A tarefa sempre pertence a um usuário (`userId` NOT NULL)
- Timestamps são gerenciados automaticamente pelo banco de dados

### Dados de Teste (Opcional)

```sql
-- Inserir usuários de exemplo
INSERT INTO users (name, email, birthday_date, working) VALUES
('João Silva', 'joao.silva@email.com', '1990-05-15', 'Desenvolvedor Full Stack'),
('Maria Santos', 'maria.santos@email.com', '1995-08-20', 'Designer UI/UX'),
('Pedro Costa', 'pedro.costa@email.com', '1988-12-03', 'Tech Lead');

-- Inserir tarefas de exemplo
INSERT INTO tasks (title, description, status, userId, initial_date) VALUES
('Implementar autenticação', 'Adicionar JWT authentication ao sistema', 'em_progresso', 1, NOW()),
('Criar tela de login', 'Desenvolver interface de login responsiva', 'nao_iniciada', 2, NULL),
('Deploy em produção', 'Configurar pipeline de CI/CD', 'nao_iniciada', 3, NULL);
```

---

## 🎯 Visão Geral

**Todo API** é uma API RESTful desenvolvida em **C# .NET 8.0** para gerenciamento de tarefas e usuários. O sistema permite criar, visualizar, atualizar e deletar tarefas, além de gerenciar usuários. É projetado para ser usado com um frontend moderno (React/Next.js) e oferece endpoints simples e eficientes.

### Principais Funcionalidades

- ✅ **CRUD completo de Tarefas**
- 👥 **CRUD completo de Usuários**
- 🔄 **Relacionamento entre Tasks e Users**
- 🗄️ **Entity Framework Core com MySQL**
- 📊 **Health Check endpoint**
- 🚀 **API RESTful com Swagger/OpenAPI**
- 🔒 **CORS configurado para desenvolvimento**

---

## 🏗️ Arquitetura

A aplicação segue uma arquitetura em camadas baseada no padrão **MVC (Model-View-Controller)**:

```
TodoApi/
├── Controllers/          # Controladores da API (lógica de negócio)
│   ├── HealthCheckController.cs
│   ├── TasksController.cs
│   ├── UsersController.cs
│   └── WeatherForecastController.cs (exemplo)
├── Models/              # Modelos de dados e contexto do EF Core
│   ├── Task.cs
│   ├── User.cs
│   └── TodoDbContext.cs
├── Properties/          # Configurações de lançamento
│   └── launchSettings.json
├── appsettings.json     # Configurações da aplicação
├── Program.cs           # Ponto de entrada e configuração de serviços
└── TodoApi.csproj       # Arquivo de projeto .NET
```

### Padrões Utilizados

- **Repository Pattern** (via Entity Framework Core)
- **Dependency Injection** (nativo do .NET)
- **RESTful API Design**
- **Data Transfer Objects (DTOs)** implícito nos modelos

---

## 🛠️ Tecnologias Utilizadas

### Framework e Runtime

- **.NET 8.0** - Framework principal
- **ASP.NET Core** - Para construção da Web API
- **C# 12** - Linguagem de programação

### ORM e Banco de Dados

- **Entity Framework Core 8.x** - ORM para acesso a dados
- **Pomelo.EntityFrameworkCore.MySql** - Provider MySQL para EF Core
- **MySQL 8.0+** - Banco de dados relacional

### Documentação

- **Swashbuckle.AspNetCore** - Geração automática de documentação Swagger/OpenAPI

### Dependências Principais

```xml
<PackageReference Include="Microsoft.EntityFrameworkCore" Version="8.x" />
<PackageReference Include="Microsoft.EntityFrameworkCore.Design" Version="8.x" />
<PackageReference Include="Pomelo.EntityFrameworkCore.MySql" Version="8.x" />
<PackageReference Include="Swashbuckle.AspNetCore" Version="6.x" />
```

---

## 📁 Estrutura do Projeto

### Controllers

#### **TasksController.cs**

Responsável por gerenciar todas as operações relacionadas a tarefas.

**Endpoints:**

- `GET /api/tasks` - Lista todas as tarefas
- `GET /api/tasks/{id}` - Busca tarefa por ID
- `POST /api/tasks` - Cria nova tarefa
- `PUT /api/tasks/{id}` - Atualiza tarefa existente
- `DELETE /api/tasks/{id}` - Remove tarefa

**Características:**

- Associa automaticamente o usuário à tarefa via `UserId`
- Ignora o campo `User` no JSON (usando `[JsonIgnore]`)
- Retorna status HTTP apropriados (200, 201, 404, etc.)

#### **UsersController.cs**

Gerencia operações de usuários.

**Endpoints:**

- `GET /api/users` - Lista todos os usuários
- `GET /api/users/{id}` - Busca usuário por ID
- `POST /api/users` - Cria novo usuário
- `PUT /api/users/{id}` - Atualiza usuário existente
- `DELETE /api/users/{id}` - Remove usuário

#### **HealthCheckController.cs**

Endpoint simples para verificar se a API está funcionando.

**Endpoint:**

- `GET /api/health` - Retorna `{ "status": "API is running" }`

### Models

#### **Task.cs**

```csharp
public class Task
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string? Description { get; set; }
    public string Status { get; set; } // nao_iniciada, em_progresso, concluida
    public int UserId { get; set; }
    
    [JsonIgnore]
    public virtual User? User { get; set; }
    
    public DateTime? Initial_date { get; set; }
    public DateTime? Finished_date { get; set; }
}
```

**Campos:**

- `Id`: Identificador único (auto-incremento)
- `Title`: Título da tarefa (obrigatório)
- `Description`: Descrição detalhada (opcional)
- `Status`: Estado da tarefa (valores: `nao_iniciada`, `em_progresso`, `concluida`)
- `UserId`: ID do usuário responsável
- `User`: Navegação para o usuário (ignorado no JSON)
- `Initial_date`: Data de início (preenchida ao mudar para "em_progresso")
- `Finished_date`: Data de conclusão (preenchida ao mudar para "concluida")

#### **User.cs**

```csharp
public class User
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string? Birthday_date { get; set; }
    public string? Working { get; set; }
    
    public virtual ICollection<Task>? Tasks { get; set; }
}
```

**Campos:**

- `Id`: Identificador único (auto-incremento)
- `Name`: Nome completo do usuário
- `Email`: Email do usuário
- `Birthday_date`: Data de nascimento (opcional)
- `Working`: Cargo/função do usuário (opcional)
- `Tasks`: Coleção de tarefas associadas ao usuário

#### **TodoDbContext.cs**

```csharp
public class TodoDbContext : DbContext
{
    public TodoDbContext(DbContextOptions<TodoDbContext> options)
        : base(options) { }

    public DbSet<Task> Tasks { get; set; }
    public DbSet<User> Users { get; set; }
}
```

**Responsabilidades:**

- Gerencia a conexão com o banco de dados
- Define as entidades (DbSets) disponíveis
- Mapeia classes C# para tabelas do banco

---

## 🌐 Endpoints da API

### Base URL

```
http://localhost:5201
```

### Tasks Endpoints

#### 1. Listar todas as tarefas

```http
GET /api/tasks
```

**Resposta (200 OK):**

```json
[
  {
    "id": 1,
    "title": "Implementar autenticação",
    "description": "Adicionar JWT authentication",
    "status": "em_progresso",
    "userId": 1,
    "initial_date": "2025-11-10T10:00:00",
    "finished_date": null
  }
]
```

#### 2. Buscar tarefa por ID

```http
GET /api/tasks/{id}
```

**Resposta (200 OK):**

```json
{
  "id": 1,
  "title": "Implementar autenticação",
  "description": "Adicionar JWT authentication",
  "status": "em_progresso",
  "userId": 1,
  "initial_date": "2025-11-10T10:00:00",
  "finished_date": null
}
```

**Resposta (404 Not Found):**

```json
{
  "message": "Task not found"
}
```

#### 3. Criar nova tarefa

```http
POST /api/tasks
Content-Type: application/json
```

**Body:**

```json
{
  "title": "Nova tarefa",
  "description": "Descrição da tarefa",
  "status": "nao_iniciada",
  "userId": 1
}
```

**Resposta (201 Created):**

```json
{
  "id": 2,
  "title": "Nova tarefa",
  "description": "Descrição da tarefa",
  "status": "nao_iniciada",
  "userId": 1,
  "initial_date": null,
  "finished_date": null
}
```

#### 4. Atualizar tarefa

```http
PUT /api/tasks/{id}
Content-Type: application/json
```

**Body:**

```json
{
  "id": 1,
  "title": "Tarefa atualizada",
  "description": "Nova descrição",
  "status": "concluida",
  "userId": 1
}
```

**Resposta (204 No Content)**

**Resposta (400 Bad Request):**

```json
{
  "message": "ID mismatch"
}
```

#### 5. Deletar tarefa

```http
DELETE /api/tasks/{id}
```

**Resposta (204 No Content)**

**Resposta (404 Not Found)**

---

### Users Endpoints

#### 1. Listar todos os usuários

```http
GET /api/users
```

**Resposta (200 OK):**

```json
[
  {
    "id": 1,
    "name": "João Silva",
    "email": "joao.silva@email.com",
    "birthday_date": "1990-05-15",
    "working": "Desenvolvedor Full Stack"
  }
]
```

#### 2. Buscar usuário por ID

```http
GET /api/users/{id}
```

**Resposta (200 OK):**

```json
{
  "id": 1,
  "name": "João Silva",
  "email": "joao.silva@email.com",
  "birthday_date": "1990-05-15",
  "working": "Desenvolvedor Full Stack"
}
```

#### 3. Criar novo usuário

```http
POST /api/users
Content-Type: application/json
```

**Body:**

```json
{
  "name": "Maria Santos",
  "email": "maria.santos@email.com",
  "birthday_date": "1995-08-20",
  "working": "Designer UI/UX"
}
```

**Resposta (201 Created):**

```json
{
  "id": 2,
  "name": "Maria Santos",
  "email": "maria.santos@email.com",
  "birthday_date": "1995-08-20",
  "working": "Designer UI/UX"
}
```

#### 4. Atualizar usuário

```http
PUT /api/users/{id}
Content-Type: application/json
```

**Body:**

```json
{
  "id": 1,
  "name": "João Silva",
  "email": "joao.silva@novoemail.com",
  "birthday_date": "1990-05-15",
  "working": "Tech Lead"
}
```

**Resposta (204 No Content)**

#### 5. Deletar usuário

```http
DELETE /api/users/{id}
```

**Resposta (204 No Content)**

---

### Health Check

```http
GET /api/health
```

**Resposta (200 OK):**

```json
{
  "status": "API is running"
}
```

---

## 🚀 Instalação e Configuração

### Pré-requisitos

- **.NET 8.0 SDK** ou superior
- **MySQL Server 8.0+**
- **Visual Studio 2022** ou **Visual Studio Code**
- **Git** (opcional)

### 1. Clone o Repositório

```bash
git clone <repository-url>
cd chalenge-tech/TodoApi
```

### 2. Instalar Dependências

As dependências são restauradas automaticamente, mas você pode forçar:

```bash
dotnet restore
```

### 3. Configurar Banco de Dados

#### Criar Banco de Dados MySQL

```sql
CREATE DATABASE todo_db;
```

#### Configurar Connection String

Edite o arquivo `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=todo_db;User=root;Password=SUA_SENHA;"
  }
}
```

**Parâmetros:**

- `Server`: Host do MySQL (geralmente `localhost`)
- `Database`: Nome do banco de dados (`todo_db`)
- `User`: Usuário do MySQL (padrão: `root`)
- `Password`: Senha do MySQL

### 4. Aplicar Migrations

```bash
dotnet ef migrations add InitialCreate
dotnet ef database update
```

**Nota:** Se você não tiver a ferramenta EF Core instalada:

```bash
dotnet tool install --global dotnet-ef
```

### 5. Verificar Tabelas Criadas

Conecte-se ao MySQL e verifique:

```sql
USE todo_db;
SHOW TABLES;

-- Deve mostrar:
-- Tasks
-- Users
```

---

## ▶️ Executando o Projeto

### Modo Development

```bash
dotnet run
```

ou

```bash
dotnet watch run
```

A API estará disponível em:

- **HTTP:** `http://localhost:5201`
- **HTTPS:** `https://localhost:7060`

### Acessar Swagger UI

Abra no navegador:

```
http://localhost:5201/swagger
```

O Swagger UI fornece:

- 📖 Documentação interativa da API
- 🧪 Testes de endpoints diretamente no navegador
- 📋 Schemas dos modelos de dados

### Modo Production

```bash
dotnet publish -c Release
cd bin/Release/net8.0/publish
dotnet TodoApi.dll
```

---

## 💡 Exemplos de Uso

### Usando cURL

#### Criar um usuário

```bash
curl -X POST http://localhost:5201/api/users \
  -H "Content-Type: application/json" \
  -d '{
    "name": "João Silva",
    "email": "joao@email.com",
    "working": "Developer"
  }'
```

#### Criar uma tarefa

```bash
curl -X POST http://localhost:5201/api/tasks \
  -H "Content-Type: application/json" \
  -d '{
    "title": "Minha primeira tarefa",
    "description": "Testar a API",
    "status": "nao_iniciada",
    "userId": 1
  }'
```

#### Listar todas as tarefas

```bash
curl http://localhost:5201/api/tasks
```

#### Atualizar status da tarefa

```bash
curl -X PUT http://localhost:5201/api/tasks/1 \
  -H "Content-Type: application/json" \
  -d '{
    "id": 1,
    "title": "Minha primeira tarefa",
    "description": "Testar a API",
    "status": "concluida",
    "userId": 1
  }'
```

### Usando JavaScript (Fetch)

```javascript
// Criar tarefa
async function criarTarefa() {
  const response = await fetch('http://localhost:5201/api/tasks', {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json',
    },
    body: JSON.stringify({
      title: 'Nova tarefa',
      description: 'Descrição',
      status: 'nao_iniciada',
      userId: 1
    })
  });
  
  const data = await response.json();
  console.log(data);
}

// Listar tarefas
async function listarTarefas() {
  const response = await fetch('http://localhost:5201/api/tasks');
  const tasks = await response.json();
  console.log(tasks);
}
```

---

## 🗄️ Configuração do Banco de Dados

### Schema das Tabelas

#### Tabela `Users`

```sql
CREATE TABLE `Users` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `Name` varchar(255) NOT NULL,
  `Email` varchar(255) NOT NULL,
  `Birthday_date` varchar(50) DEFAULT NULL,
  `Working` varchar(255) DEFAULT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
```

#### Tabela `Tasks`

```sql
CREATE TABLE `Tasks` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `Title` varchar(255) NOT NULL,
  `Description` text,
  `Status` varchar(50) NOT NULL,
  `UserId` int NOT NULL,
  `Initial_date` datetime DEFAULT NULL,
  `Finished_date` datetime DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `IX_Tasks_UserId` (`UserId`),
  CONSTRAINT `FK_Tasks_Users_UserId` 
    FOREIGN KEY (`UserId`) REFERENCES `Users` (`Id`) 
    ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
```

### Relacionamentos

- **Tasks → Users**: Relação **muitos-para-um** (N:1)
  - Uma tarefa pertence a **um** usuário
  - Um usuário pode ter **várias** tarefas
  - `FK_Tasks_Users_UserId` com `ON DELETE CASCADE`

---

## 🔧 Configurações Adicionais

### CORS

O CORS está configurado para permitir requisições de qualquer origem em desenvolvimento:

```csharp
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});
```

**⚠️ Importante:** Em produção, configure CORS para permitir apenas origens confiáveis:

```csharp
policy.WithOrigins("https://seu-frontend.com")
      .AllowAnyMethod()
      .AllowAnyHeader();
```

### JSON Serialization

Configurado para ignorar referências cíclicas:

```csharp
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = 
            System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    });
```

---

## 📝 Scripts SQL Úteis

### Popular Banco com Dados de Teste

```sql
-- Inserir usuários
INSERT INTO Users (Name, Email, Birthday_date, Working) VALUES
('João Silva', 'joao.silva@email.com', '1990-05-15', 'Desenvolvedor Full Stack'),
('Maria Santos', 'maria.santos@email.com', '1995-08-20', 'Designer UI/UX'),
('Pedro Costa', 'pedro.costa@email.com', '1988-12-03', 'Tech Lead');

-- Inserir tarefas
INSERT INTO Tasks (Title, Description, Status, UserId, Initial_date) VALUES
('Implementar autenticação', 'Adicionar JWT authentication ao sistema', 'em_progresso', 1, NOW()),
('Criar tela de login', 'Desenvolver interface de login responsiva', 'nao_iniciada', 2, NULL),
('Deploy em produção', 'Configurar pipeline de CI/CD', 'nao_iniciada', 3, NULL);
```

### Consultas Úteis

```sql
-- Listar tarefas com informações do usuário
SELECT 
    t.Id, 
    t.Title, 
    t.Status, 
    u.Name as UserName, 
    u.Email
FROM Tasks t
INNER JOIN Users u ON t.UserId = u.Id;

-- Contar tarefas por status
SELECT Status, COUNT(*) as Total
FROM Tasks
GROUP BY Status;

-- Buscar tarefas de um usuário específico
SELECT * FROM Tasks WHERE UserId = 1;

-- Limpar todas as tarefas
TRUNCATE TABLE Tasks;

-- Resetar IDs
ALTER TABLE Tasks AUTO_INCREMENT = 1;
ALTER TABLE Users AUTO_INCREMENT = 1;
```

---

## 🐛 Troubleshooting

### Erro: "Unable to connect to MySQL server"

**Solução:**

- Verifique se o MySQL está rodando
- Confirme usuário e senha no `appsettings.json`
- Teste a conexão: `mysql -u root -p`

### Erro: "Table doesn't exist"

**Solução:**

```bash
dotnet ef database update
```

### Erro: CORS bloqueando requisições

**Solução:**

- Verifique se a política CORS está aplicada no `Program.cs`
- Confirme que `app.UseCors("AllowAll")` está antes de `app.MapControllers()`

### Porta já em uso

**Solução:**
Altere a porta no `launchSettings.json`:

```json
"applicationUrl": "http://localhost:5202;https://localhost:7061"
```

---

## 📚 Recursos Adicionais

### Documentação Oficial

- [.NET 8 Documentation](https://learn.microsoft.com/en-us/dotnet/core/whats-new/dotnet-8)
- [Entity Framework Core](https://learn.microsoft.com/en-us/ef/core/)
- [ASP.NET Core Web API](https://learn.microsoft.com/en-us/aspnet/core/web-api/)
- [MySQL Connector/NET](https://dev.mysql.com/doc/connector-net/en/)

### Ferramentas Recomendadas

- **Postman** - Teste de APIs
- **MySQL Workbench** - Gerenciamento de banco de dados
- **Swagger UI** - Documentação interativa (já incluído)

---

## 👨‍💻 Desenvolvedor

**Projeto:** Challenge Tech - Todo API  
**Framework:** .NET 8.0 + ASP.NET Core  
**Banco de Dados:** MySQL 8.0+  
**Ano:** 2025

---

## 📄 Licença

Este projeto é de uso educacional e demonstrativo.

---

## 🎯 Próximos Passos

- [ ] Adicionar autenticação JWT
- [ ] Implementar paginação nas listagens
- [ ] Adicionar filtros e busca
- [ ] Implementar validações mais robustas
- [ ] Adicionar testes unitários e de integração
- [ ] Implementar cache com Redis
- [ ] Adicionar logging com Serilog
- [ ] Dockerizar a aplicação
- [ ] Implementar versionamento da API

---

**🚀 Happy Coding!**
