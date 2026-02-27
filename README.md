# ☀️ Sun API

API REST moderna construida con **.NET 10** y **C# 14** siguiendo **Clean Architecture** y buenas prácticas de desarrollo backend.

---

## Stack Tecnológico

| Componente | Tecnología |
|---|---|
| Framework | .NET 10 / C# 14 |
| Arquitectura | Clean Architecture (4 capas) |
| Base de datos | SQL Server 2022 (Docker) |
| ORM | Entity Framework Core 10 |
| CQRS | MediatR 14 |
| Autenticación | JWT (JsonWebTokenHandler) |
| Hashing | Argon2id |
| Logging | Serilog (consola + archivo) |
| Documentación | Swagger / Swashbuckle |
| Rate Limiting | Fixed Window (60 req/min) |

---

## Requisitos Previos

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [Docker Desktop](https://www.docker.com/products/docker-desktop)
- [EF Core CLI tools](https://learn.microsoft.com/en-us/ef/core/cli/dotnet)

```bash
dotnet tool install --global dotnet-ef
```

---

## Inicio Rápido

### 1. Clonar el repositorio

```bash
git clone https://github.com/tu-usuario/Sun.git
cd Sun
```

### 2. Levantar SQL Server en Docker

```bash
docker run -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=YourStrong!Passw0rd" \
  -p 1433:1433 --name sqlserver \
  -d mcr.microsoft.com/mssql/server:2025-latest
```

### 3. Crear la migración inicial

```bash
dotnet ef migrations add InitialCreate \
  --project src/Infrastructure \
  --startup-project src/Api \
  --output-dir Persistence/Migrations
```

### 4. Ejecutar la API

```bash
dotnet run --project src/Api
```

Las migraciones se aplican automáticamente al arrancar. La API estará disponible en:

- **HTTP:** http://localhost:5000
- **HTTPS:** https://localhost:5001
- **Swagger UI:** http://localhost:5000/swagger/

---

## Endpoints

### Autenticación (sin JWT)

| Método | Ruta | Descripción |
|---|---|---|
| POST | `/api/auth/register` | Registro de usuario |
| POST | `/api/auth/login` | Login (retorna JWT) |

### Usuarios (requiere JWT)

| Método | Ruta | Descripción |
|---|---|---|
| GET | `/api/users/{id}` | Obtener usuario por Id |

### Productos (requiere JWT)

| Método | Ruta | Descripción |
|---|---|---|
| POST | `/api/products` | Crear producto |

### Ejemplos de uso

**Registro:**

```bash
curl -X POST http://localhost:5000/api/auth/register \
  -H "Content-Type: application/json" \
  -d '{
    "firstName": "Juan",
    "lastName": "Pérez",
    "email": "juan@email.com",
    "password": "MiPassword123"
  }'
```

**Login:**

```bash
curl -X POST http://localhost:5000/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{
    "email": "juan@email.com",
    "password": "MiPassword123"
  }'
```

**Crear producto (con token):**

```bash
curl -X POST http://localhost:5000/api/products \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer <tu_token_jwt>" \
  -d '{
    "name": "Laptop",
    "description": "Laptop gaming 16GB RAM",
    "priceAmount": 999.99,
    "priceCurrency": "USD",
    "stock": 50
  }'
```

---

## Estructura del Proyecto

```
Sun/
├── Sun.slnx
└── src/
    ├── Domain/                 ← Entidades, Value Objects, Result Pattern
    │   ├── Commons/            ← BaseEntity, Error, Result
    │   ├── Entities/           ← User, Product
    │   └── ValueObjects/       ← Email, Password, Address, Price
    │
    ├── Application/            ← Casos de uso (CQRS), DTOs, Interfaces
    │   ├── DTOs/               ← AuthResponse, UserResponse, ProductResponse
    │   ├── Features/           ← Auth, Users, Products (Commands + Queries)
    │   └── Interfaces/         ← IUserRepository, IProductRepository, etc.
    │
    ├── Infrastructure/         ← EF Core, Repositorios, JWT, Argon2id
    │   ├── Persistence/        ← AppDbContext, Configs, Repositories, Migrations
    │   └── Security/           ← Argon2PasswordHasher, JwtTokenGenerator
    │
    └── Api/                    ← Controllers, Middleware, Program.cs
        ├── Controllers/        ← AuthController, UsersController, ProductsController
        ├── Extensions/         ← ResultExtensions (Result → HTTP response)
        └── Middlewares/        ← ExceptionMiddleware (errores globales)
```

### Flujo de dependencias

```
Domain ← Application ← Infrastructure ← Api
(sin dependencias)                      (punto de entrada)
```

Domain no conoce a nadie. Application define interfaces. Infrastructure las implementa. Api conecta todo.

---

## Patrones Implementados

- **Clean Architecture** — Separación estricta en 4 capas con dependencias hacia adentro
- **CQRS** — Commands (escritura) y Queries (lectura) separados con MediatR
- **Result Pattern** — Errores de negocio como valores de retorno, sin excepciones
- **Value Objects** — Email, Password, Address, Price con validación encapsulada
- **Repository Pattern** — Interfaces en Application, implementaciones en Infrastructure
- **Dependency Injection** — Registro centralizado por capa

---

## Seguridad

- **Argon2id** para hashing de contraseñas (salt 16 bytes, hash 32 bytes, 64MB RAM, 3 iteraciones)
- **JWT** con API moderna (`JsonWebTokenHandler` / `SecurityTokenDescriptor`)
- **Rate Limiting** — 60 requests por minuto por IP (Fixed Window)
- **Comparación en tiempo constante** (`CryptographicOperations.FixedTimeEquals`) para evitar timing attacks
- **Mensajes de error genéricos** en login para prevenir enumeración de emails

---

## Logging

Serilog escribe logs estructurados en dos destinos:

- **Consola** — durante desarrollo
- **Archivo** — `logs/log-{fecha}.txt`, rotación diaria, retención de 30 días

Cada request HTTP se loguea automáticamente con método, ruta, status code y duración.

---

## Configuración

La configuración se encuentra en `src/Api/appsettings.json`:

| Clave | Descripción |
|---|---|
| `ConnectionStrings:Sun` | Connection string de SQL Server |
| `Jwt:Secret` | Clave secreta para firmar tokens (Base64, ≥32 chars) |
| `Jwt:Issuer` | Emisor del token |
| `Jwt:Audience` | Audiencia del token |

---

## Comandos Útiles

```bash
# Ejecutar la API
dotnet run --project src/Api

# Compilar sin ejecutar
dotnet build

# Agregar nueva migración
dotnet ef migrations add NombreMigracion \
  --project src/Infrastructure \
  --startup-project src/Api \
  --output-dir Persistence/Migrations

# Listar migraciones
dotnet ef migrations list \
  --project src/Infrastructure \
  --startup-project src/Api

# Revertir última migración (si no se aplicó)
dotnet ef migrations remove \
  --project src/Infrastructure \
  --startup-project src/Api

# Levantar SQL Server
docker start sqlserver

# Detener SQL Server
docker stop sqlserver
```

---

## Documentación del Proyecto

| Archivo | Descripción |
|---|---|
| `CLAUDE.md` | Contexto técnico completo para Claude Code / Claude AI |
| `ROADMAP.md` | Visión de features: completadas, en progreso y planificadas |

---

## Licencia

Este proyecto es de uso educativo y de aprendizaje de arquitectura de software.
