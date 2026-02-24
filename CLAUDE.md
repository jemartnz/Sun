# CLAUDE.md — Proyecto Sun

## Descripción General

Sun es una API REST construida con **.NET 10** y **C# 14** siguiendo **Clean Architecture**. El proyecto implementa buenas prácticas modernas de desarrollo backend incluyendo CQRS, Result Pattern, Value Objects, JWT Authentication y más.

---

## Arquitectura

El proyecto sigue Clean Architecture con 4 capas. Las dependencias apuntan hacia adentro:

```
[Api] → [Infrastructure] → [Application] → [Domain]
```

### Estructura de Carpetas

```
Sun/
├── Sun.slnx
└── src/
    ├── Domain/                          ← Capa más interna. Cero dependencias externas.
    │   ├── Commons/
    │   │   ├── BaseEntity.cs            ← Clase base: Id (Guid), CreatedAtUtc, UpdatedAtUtc
    │   │   ├── Error.cs                 ← Record Error(Code, Message) para Result Pattern
    │   │   └── Result.cs                ← Result y Result<T> para flujo sin excepciones
    │   ├── Entities/
    │   │   ├── User.cs                  ← Entidad con Email, PasswordHash, Address (opcional)
    │   │   └── Product.cs               ← Entidad con Price (Value Object), Stock
    │   ├── ValueObjects/
    │   │   ├── Email.cs                 ← Validación con Regex, normalización a lowercase
    │   │   ├── Password.cs              ← Validación: min 8 chars, mayúscula, dígito
    │   │   ├── Address.cs               ← Street, City, Country, ZipCode
    │   │   └── Price.cs                 ← Amount (decimal) + Currency (string)
    │   └── Domain.csproj                ← Sin paquetes NuGet externos
    │
    ├── Application/                     ← Casos de uso. Depende solo de Domain.
    │   ├── DTOs/
    │   │   ├── AuthResponse.cs          ← record(Token, UserId, Email)
    │   │   ├── UserResponse.cs          ← record(Id, FirstName, LastName, Email, CreatedAtUtc)
    │   │   └── ProductResponse.cs       ← record(Id, Name, Description, PriceAmount, PriceCurrency, Stock)
    │   ├── Features/
    │   │   ├── Auth/
    │   │   │   ├── RegisterUserCommand.cs
    │   │   │   ├── RegisterUserHandler.cs   ← Valida → hashea password → persiste → genera JWT
    │   │   │   ├── LoginUserCommand.cs
    │   │   │   └── LoginUserHandler.cs      ← Busca user → verifica hash → genera JWT
    │   │   ├── Products/
    │   │   │   ├── CreateProductCommand.cs
    │   │   │   └── CreateProductHandler.cs
    │   │   └── Users/
    │   │       ├── GetUserByIdQuery.cs
    │   │       └── GetUserByIdHandler.cs
    │   ├── Interfaces/
    │   │   ├── IUserRepository.cs       ← GetByIdAsync, GetByEmailAsync, AddAsync, SaveChangesAsync
    │   │   ├── IProductRepository.cs    ← GetByIdAsync, GetAllAsync, AddAsync, SaveChangesAsync
    │   │   ├── IPasswordHasher.cs       ← Hash(string), Verify(string, string)
    │   │   └── ITokenGenerator.cs       ← Generate(User)
    │   └── Application.csproj           ← Paquete: MediatR 14.0.0
    │
    ├── Infrastructure/                  ← Implementaciones técnicas. Depende de Application.
    │   ├── Persistence/
    │   │   ├── AppDbContext.cs           ← DbContext con auto-discovery de configuraciones
    │   │   ├── DatabaseMigrator.cs      ← Extension method ApplyMigrationsAsync() para auto-migrar al arrancar
    │   │   ├── Configs/
    │   │   │   ├── UserConfig.cs        ← IEntityTypeConfiguration<User> con OwnsOne para Email y Address
    │   │   │   └── ProductConfig.cs     ← IEntityTypeConfiguration<Product> con OwnsOne para Price
    │   │   ├── UserRepository.cs        ← Implementación con EF Core (AppDbContext inyectado)
    │   │   ├── ProductRepository.cs     ← Implementación con EF Core
    │   │   └── Migrations/              ← Generadas con dotnet ef (auto-aplicadas al iniciar)
    │   ├── Security/
    │   │   ├── Argon2PasswordHasher.cs  ← Argon2id: salt 16 bytes, hash 32 bytes, 64MB RAM, 3 iteraciones
    │   │   └── JwtTokenGenerator.cs     ← API moderna: JsonWebTokenHandler + SecurityTokenDescriptor
    │   ├── DependencyInjection.cs       ← Extension method AddInfrastructure() para registrar servicios
    │   └── Infrastructure.csproj        ← Paquetes: EF Core SqlServer, Argon2, JwtBearer
    │
    └── Api/                             ← Punto de entrada HTTP. Depende de todo.
        ├── Controllers/
        │   ├── AuthController.cs        ← POST register, POST login (sin [Authorize])
        │   ├── UsersController.cs       ← GET {id:guid} (con [Authorize])
        │   └── ProductsController.cs    ← POST create (con [Authorize])
        ├── Extensions/
        │   └── ResultExtensions.cs      ← ToActionResult() convierte Result<T> en HTTP 200/400/401/404/409
        ├── Middlewares/
        │   └── ExceptionMiddleware.cs   ← Atrapa excepciones no manejadas → HTTP 500 + log
        ├── Properties/
        │   └── launchSettings.json      ← Puertos: https://localhost:5001, http://localhost:5000
        ├── Program.cs                   ← Configura Serilog, MediatR, JWT, Rate Limiting, Swagger, pipeline
        ├── appsettings.json             ← Connection string SQL Server, JWT config
        ├── appsettings.Development.json
        └── Api.csproj                   ← Paquetes: Serilog.AspNetCore, Serilog.Sinks.File, Swashbuckle
```

---

## Patrones y Decisiones Arquitectónicas

### Result Pattern
Las entidades y Value Objects retornan `Result<T>` en vez de lanzar excepciones para errores de negocio. Las excepciones se reservan para errores inesperados (capturados por ExceptionMiddleware).

### Value Objects con EF Core
Los Value Objects (Email, Address, Price) se mapean con `OwnsOne()` en las configuraciones de EF Core. Se almacenan como columnas en la tabla padre, no como tablas separadas. Las entidades tienen un constructor privado sin parámetros (`private User() { ... }`) con `null!` para que EF Core pueda materializarlas.

### CQRS con MediatR
Commands (escritura) y Queries (lectura) separados. El Controller envía a MediatR → MediatR encuentra el Handler → Handler usa interfaces de repositorio.

### Configuraciones de Entidades Separadas
Cada entidad tiene su propio archivo `IEntityTypeConfiguration<T>` en `Infrastructure/Persistence/Configs/`. AppDbContext las detecta automáticamente con `ApplyConfigurationsFromAssembly()`.

### Migraciones Automáticas
`DatabaseMigrator.ApplyMigrationsAsync()` se ejecuta al arrancar la API. Detecta migraciones pendientes y las aplica automáticamente.

---

## Stack Tecnológico

| Componente | Tecnología |
|---|---|
| Framework | .NET 10, C# 14 |
| Base de datos | SQL Server en Docker (puerto 1433) |
| ORM | Entity Framework Core 10 |
| CQRS | MediatR 14 |
| Autenticación | JWT con JsonWebTokenHandler (API moderna) |
| Hashing | Argon2id (Konscious.Security.Cryptography) |
| Logging | Serilog → Consola + Archivo (logs/log-{fecha}.txt) |
| Documentación | Swagger/Swashbuckle |
| Rate Limiting | Fixed Window: 60 requests/minuto |

---

## Endpoints

| Método | Ruta | Auth | Descripción |
|---|---|---|---|
| POST | `/api/auth/register` | No | Registro de usuario. Body: FirstName, LastName, Email, Password |
| POST | `/api/auth/login` | No | Login. Body: Email, Password. Retorna JWT |
| GET | `/api/users/{id}` | JWT | Obtener usuario por Id |
| POST | `/api/products` | JWT | Crear producto. Body: Name, Description, PriceAmount, PriceCurrency, Stock |

Swagger UI disponible en: `http://localhost:5000/swagger/`

---

## Configuración

### appsettings.json
```json
{
  "ConnectionStrings": {
    "Sun": "Server=localhost,1433;Database=SunDb;User Id=sa;Password=YourStrong!Passw0rd;TrustServerCertificate=true"
  },
  "Jwt": {
    "Secret": "RXN0YUVzVW5hQ2xhdmVTZWNyZXRhTXV5TGFyZ2FRdWVEZWJlVGVuZXJBbE1lbm9zMzJDYXJhY3RlcmVzIQ==",
    "Issuer": "SunApp",
    "Audience": "SunApp"
  }
}
```

### Docker SQL Server
```bash
docker run -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=YourStrong!Passw0rd" \
  -p 1433:1433 --name sqlserver \
  -d mcr.microsoft.com/mssql/server:2022-latest
```

---

## Comandos Útiles

```bash
# Crear migración
dotnet ef migrations add NombreMigracion --project src/Infrastructure --startup-project src/Api --output-dir Persistence/Migrations

# Listar migraciones
dotnet ef migrations list --project src/Infrastructure --startup-project src/Api

# Revertir última migración (si no se aplicó)
dotnet ef migrations remove --project src/Infrastructure --startup-project src/Api

# Ejecutar la API (migraciones se aplican automáticamente)
dotnet run --project src/Api
```

---

## Flujo de Errores

```
Error de validación (Value Object)  → Result.Failure  → Controller → HTTP 400
Error de negocio (email duplicado)  → Result.Failure  → Controller → HTTP 409
Credenciales inválidas              → Result.Failure  → Controller → HTTP 401
Entidad no encontrada               → Result.Failure  → Controller → HTTP 404
Error inesperado (BD caída)         → Excepción       → ExceptionMiddleware → HTTP 500 + log
```

---

## Dependencia entre Capas (Regla Estricta)

- **Domain**: No referencia a ningún otro proyecto. Cero paquetes NuGet.
- **Application**: Referencia solo a Domain. Paquete: MediatR.
- **Infrastructure**: Referencia a Application y Domain. Paquetes: EF Core, Argon2, JWT.
- **Api**: Referencia a Application, Domain e Infrastructure. Paquetes: Serilog, Swashbuckle.

---

## Convenciones del Proyecto

- Namespaces sin prefijo de solución: `Domain.Entities`, `Application.Features.Auth`, `Infrastructure.Persistence`, `Api.Controllers`
- Entidades tienen factory method estático `Create()` que retorna `Result<T>`
- Entidades tienen constructor privado sin parámetros para EF Core con `null!`
- Value Objects tienen constructor privado y factory method `Create()` con validación
- Errores de dominio agrupados en clases estáticas: `UserErrors`, `ProductErrors`, `EmailErrors`, etc.
- Cada feature tiene su propia carpeta con Command/Query + Handler
- Controllers solo reciben request → envían a MediatR → convierten Result a HTTP response
- Connection string se llama `"Sun"` en appsettings.json

## Estado del proyecto
Ver ROADMAP.md para el estado actual de features y el plan en progreso.
