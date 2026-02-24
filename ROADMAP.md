# ROADMAP.md — Proyecto Sun

> Visión de alto nivel del producto. Qué existe, qué viene, en qué orden.
> Última actualización: Febrero 2026

---

## Leyenda

| Estado | Significado |
|--------|-------------|
| ✅ | Implementado y funcionando |
| 🔧 | En progreso o parcialmente implementado |
| 📋 | Planificado — próximo en la cola |
| 💡 | Idea futura — sin prioridad definida |

---

## Fase 1 — Fundamentos de Arquitectura ✅

> Objetivo: establecer la estructura base de Clean Architecture con todos los patrones core funcionando de punta a punta.

| Feature | Estado | Detalle |
|---------|--------|---------|
| Clean Architecture (4 capas) | ✅ | Domain, Application, Infrastructure, Api |
| Entidad Base | ✅ | Id, CreatedAtUtc, UpdatedAtUtc |
| Result Pattern | ✅ | Result, Result\<T\>, Error record |
| Value Objects | ✅ | Email, Password, Address, Price — con validación encapsulada |
| CQRS con MediatR | ✅ | Commands y Queries separados con handlers |
| Dependency Injection | ✅ | AddInfrastructure() centralizado |
| EF Core con SQL Server | ✅ | Docker, configuraciones separadas por entidad (IEntityTypeConfiguration) |
| Migraciones automáticas | ✅ | ApplyMigrationsAsync() al arrancar la API |
| Constructores privados para EF Core | ✅ | Constructor sin parámetros con null! en entidades |

---

## Fase 2 — Autenticación y Seguridad ✅

> Objetivo: registro, login y protección de endpoints con JWT.

| Feature | Estado | Detalle |
|---------|--------|---------|
| Registro de usuario | ✅ | POST /api/auth/register |
| Login | ✅ | POST /api/auth/login |
| JWT con API moderna | ✅ | JsonWebTokenHandler + SecurityTokenDescriptor |
| Argon2id password hashing | ✅ | Salt 16 bytes, hash 32 bytes, 64MB RAM, timing-safe verify |
| Endpoints protegidos con [Authorize] | ✅ | Users y Products requieren JWT |
| Swagger con botón Authorize | 📋 | Configurar OpenApiSecurityScheme para probar JWT desde Swagger |

---

## Fase 3 — Infraestructura Transversal ✅

> Objetivo: logging, manejo de errores y protección contra abuso.

| Feature | Estado | Detalle |
|---------|--------|---------|
| Serilog a consola | ✅ | Logs estructurados en terminal |
| Serilog a archivo | ✅ | logs/log-{fecha}.txt, rotación diaria, retención 30 días |
| Request logging | ✅ | UseSerilogRequestLogging() en pipeline |
| Middleware global de errores | ✅ | ExceptionMiddleware → HTTP 500 genérico + log detallado |
| Result → HTTP mapping | ✅ | ResultExtensions: 400, 401, 404, 409 según Error.Code |
| Rate Limiting | ✅ | Fixed Window: 60 req/min por IP |
| Swagger UI | ✅ | Disponible en /swagger/ (Development) |

---

## Fase 4 — CRUD Completo de Entidades 📋

> Objetivo: completar todas las operaciones CRUD para Users y Products.

| Feature | Estado | Detalle |
|---------|--------|---------|
| **Users** | | |
| Obtener usuario por Id | ✅ | GET /api/users/{id} |
| Listar usuarios | 📋 | GET /api/users (con paginación) |
| Actualizar usuario | 📋 | PUT /api/users/{id} |
| Actualizar dirección | 📋 | PUT /api/users/{id}/address (usa UpdateAddress() existente) |
| Eliminar usuario | 📋 | DELETE /api/users/{id} (soft delete vs hard delete) |
| Obtener usuario actual | 📋 | GET /api/users/me (extraer Id del JWT) |
| **Products** | | |
| Crear producto | ✅ | POST /api/products |
| Obtener producto por Id | 📋 | GET /api/products/{id} |
| Listar productos | 📋 | GET /api/products (con paginación) |
| Actualizar producto | 📋 | PUT /api/products/{id} |
| Actualizar stock | 📋 | PATCH /api/products/{id}/stock |
| Eliminar producto | 📋 | DELETE /api/products/{id} |

---

## Fase 5 — Paginación y Filtrado 📋

> Objetivo: respuestas paginadas y búsquedas eficientes para endpoints de listado.

| Feature | Estado | Detalle |
|---------|--------|---------|
| Objeto de paginación genérico | 📋 | PagedRequest(Page, PageSize), PagedResponse\<T\>(Items, TotalCount, TotalPages) |
| Paginación en listado de productos | 📋 | GET /api/products?page=1&pageSize=10 |
| Paginación en listado de usuarios | 📋 | GET /api/users?page=1&pageSize=10 |
| Filtrado por nombre/precio | 📋 | Query parameters: ?name=x&minPrice=0&maxPrice=100 |
| Ordenamiento | 📋 | ?sortBy=name&sortOrder=asc |

---

## Fase 6 — Validación con FluentValidation 📋

> Objetivo: validar Commands/Queries antes de que lleguen al Handler, usando un pipeline behavior de MediatR.

| Feature | Estado | Detalle |
|---------|--------|---------|
| Paquete FluentValidation | 📋 | FluentValidation.DependencyInjectionExtensions |
| ValidationBehavior para MediatR | 📋 | IPipelineBehavior que ejecuta validadores antes del handler |
| RegisterUserCommand validator | 📋 | Validar campos requeridos, formato email, largo password |
| CreateProductCommand validator | 📋 | Validar nombre, precio > 0, stock >= 0 |
| Respuesta de validación unificada | 📋 | HTTP 422 con lista de errores por campo |

---

## Fase 7 — Roles y Autorización 💡

> Objetivo: control de acceso basado en roles (RBAC).

| Feature | Estado | Detalle |
|---------|--------|---------|
| Entidad Role | 💡 | Admin, User (como mínimo) |
| Relación User-Role | 💡 | Muchos a muchos o claim-based |
| Claims de rol en JWT | 💡 | Incluir rol en el token |
| Políticas de autorización | 💡 | [Authorize(Roles = "Admin")] o policies |
| Endpoint admin-only | 💡 | Ej: DELETE /api/users/{id} solo para Admin |
| Seed de usuario admin | 💡 | Crear admin por defecto en la primera migración |

---

## Fase 8 — Relaciones entre Entidades 💡

> Objetivo: modelar relaciones reales entre entidades del dominio.

| Feature | Estado | Detalle |
|---------|--------|---------|
| Entidad Order | 💡 | User hace pedidos de Products |
| Entidad OrderItem | 💡 | Producto + cantidad + precio al momento de compra |
| Relación User → Orders | 💡 | Un usuario tiene muchos pedidos |
| Relación Order → OrderItems | 💡 | Un pedido tiene muchos items |
| Reducción de stock al crear orden | 💡 | Lógica de dominio en Order.Create() |

---

## Fase 9 — Testing 💡

> Objetivo: confianza en el código con tests automatizados.

| Feature | Estado | Detalle |
|---------|--------|---------|
| Tests unitarios de Domain | 💡 | Value Objects, entidades, Result Pattern |
| Tests unitarios de Application | 💡 | Handlers con repositorios mockeados |
| Tests de integración | 💡 | WebApplicationFactory con BD en memoria o Testcontainers |
| Test de autenticación | 💡 | Verificar flujo register → login → acceso protegido |

---

## Fase 10 — Mejoras de Infraestructura 💡

> Objetivo: preparar el proyecto para producción.

| Feature | Estado | Detalle |
|---------|--------|---------|
| Health Checks | 💡 | /health endpoint para monitoreo (BD, servicios) |
| CORS | 💡 | Configurar orígenes permitidos |
| Response Caching | 💡 | Cache para endpoints de lectura frecuente |
| Refresh Tokens | 💡 | JWT de corta duración + refresh token para renovar |
| Dockerización de la API | 💡 | Dockerfile + docker-compose (API + SQL Server) |
| Variables de entorno | 💡 | Mover secrets de appsettings a env vars / User Secrets |
| Seq o Elasticsearch | 💡 | Sink de Serilog para búsqueda de logs en producción |

---

## Notas

- Cada fase se construye sobre la anterior. No saltar fases.
- Las fases ✅ están completas pero pueden recibir mejoras incrementales.
- Las fases 📋 son el trabajo inmediato siguiente.
- Las fases 💡 son ideas que se priorizarán cuando las anteriores estén completas.
- Este documento se actualiza conforme avanza el desarrollo.
