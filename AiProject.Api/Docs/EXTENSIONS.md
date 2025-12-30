# 🚀 EJEMPLOS DE EXTENSIÓN DEL PROYECTO

Este archivo te muestra cómo agregar nuevas funcionalidades al proyecto siguiendo la misma arquitectura.

---

## 📝 Ejemplo 1: Agregar una Segunda Feature (ImageProcessing)

### Paso 1: Crear la estructura

```
Features/
├── AiProcessing/        ← Ya existe
└── ImageProcessing/     ← Nueva feature
    ├── ImageProcessingRequest.cs
    ├── ImageProcessingResponse.cs
    ├── ImageProcessingService.cs
    └── ImageProcessingEndpoints.cs
```

### Paso 2: Crear el Request DTO

```csharp
// Features/ImageProcessing/ImageProcessingRequest.cs
namespace AiContext.Api.Features.ImageProcessing;

public sealed record ImageProcessingRequest
{
    public required string ImageUrl { get; init; }
    public required string Operation { get; init; } // resize, filter, etc.
}
```

### Paso 3: Crear el Response DTO

```csharp
// Features/ImageProcessing/ImageProcessingResponse.cs
namespace AiContext.Api.Features.ImageProcessing;

public sealed record ImageProcessingResponse
{
    public required string ProcessedImageUrl { get; init; }
    public required string Operation { get; init; }
    public DateTime ProcessedAt { get; init; } = DateTime.UtcNow;
    public bool Success { get; init; } = true;
}
```

### Paso 4: Crear el Service

```csharp
// Features/ImageProcessing/ImageProcessingService.cs
namespace AiContext.Api.Features.ImageProcessing;

public sealed class ImageProcessingService
{
    private readonly ILogger<ImageProcessingService> _logger;

    public ImageProcessingService(ILogger<ImageProcessingService> logger)
    {
        _logger = logger;
    }

    public async Task<ImageProcessingResponse> ProcessImageAsync(
        ImageProcessingRequest request,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "Procesando imagen: {ImageUrl} con operación: {Operation}",
            request.ImageUrl,
            request.Operation);

        // Aquí iría la lógica real de procesamiento de imagen
        await Task.Delay(200, cancellationToken);

        return new ImageProcessingResponse
        {
            ProcessedImageUrl = $"processed_{request.ImageUrl}",
            Operation = request.Operation,
            Success = true
        };
    }
}
```

### Paso 5: Crear los Endpoints

```csharp
// Features/ImageProcessing/ImageProcessingEndpoints.cs
using Microsoft.AspNetCore.Mvc;

namespace AiContext.Api.Features.ImageProcessing;

public static class ImageProcessingEndpoints
{
    public static IEndpointRouteBuilder MapImageProcessing(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/images")
            .WithTags("Image Processing")
            .WithOpenApi();

        group.MapPost("/process", ProcessImage)
            .WithName("ProcessImage")
            .WithSummary("Procesa una imagen");

        return app;
    }

    private static async Task<IResult> ProcessImage(
        [FromBody] ImageProcessingRequest request,
        [FromServices] ImageProcessingService service,
        CancellationToken cancellationToken)
    {
        try
        {
            var response = await service.ProcessImageAsync(request, cancellationToken);
            return Results.Ok(response);
        }
        catch (Exception ex)
        {
            return Results.Problem(
                title: "Error procesando imagen",
                detail: ex.Message,
                statusCode: StatusCodes.Status500InternalServerError);
        }
    }
}
```

### Paso 6: Registrar en Program.cs

```csharp
// Agregar en Program.cs

// En la sección de servicios:
builder.Services.AddSingleton<ImageProcessingService>();

// En la sección de endpoints:
app.MapImageProcessing();
```

---

## 🔍 Ejemplo 2: Agregar Validación con FluentValidation

### Paso 1: Instalar el paquete

```bash
dotnet add package FluentValidation
dotnet add package FluentValidation.DependencyInjectionExtensions
```

### Paso 2: Crear el validador

```csharp
// Features/AiProcessing/AiProcessingRequestValidator.cs
using FluentValidation;

namespace AiContext.Api.Features.AiProcessing;

public sealed class AiProcessingRequestValidator : AbstractValidator<AiProcessingRequest>
{
    public AiProcessingRequestValidator()
    {
        RuleFor(x => x.Prompt)
            .NotEmpty()
            .WithMessage("El prompt no puede estar vacío")
            .MaximumLength(5000)
            .WithMessage("El prompt no puede exceder 5000 caracteres")
            .MinimumLength(3)
            .WithMessage("El prompt debe tener al menos 3 caracteres");

        // Validaciones adicionales
        RuleFor(x => x.Prompt)
            .Must(NotContainBadWords)
            .WithMessage("El prompt contiene palabras no permitidas");
    }

    private bool NotContainBadWords(string prompt)
    {
        var badWords = new[] { "spam", "hack" };
        return !badWords.Any(word => 
            prompt.Contains(word, StringComparison.OrdinalIgnoreCase));
    }
}
```

### Paso 3: Registrar validadores

```csharp
// En Program.cs
using FluentValidation;

// Registrar todos los validadores del assembly
builder.Services.AddValidatorsFromAssemblyContaining<Program>();
```

### Paso 4: Usar en el endpoint

```csharp
// En AiProcessingEndpoints.cs
private static async Task<IResult> ProcessPrompt(
    [FromBody] AiProcessingRequest request,
    [FromServices] AiProcessingService service,
    [FromServices] IValidator<AiProcessingRequest> validator,
    CancellationToken cancellationToken)
{
    // Validar antes de procesar
    var validationResult = await validator.ValidateAsync(request, cancellationToken);
    
    if (!validationResult.IsValid)
    {
        return Results.ValidationProblem(validationResult.ToDictionary());
    }

    // Continuar con el procesamiento...
}
```

---

## 💾 Ejemplo 3: Agregar Entity Framework y Base de Datos

### Paso 1: Instalar paquetes

```bash
dotnet add package Microsoft.EntityFrameworkCore.SqlServer
dotnet add package Microsoft.EntityFrameworkCore.Design
dotnet add package Microsoft.EntityFrameworkCore.Tools
```

### Paso 2: Crear la entidad

```csharp
// Infrastructure/Database/Entities/ProcessedPrompt.cs
namespace AiContext.Api.Infrastructure.Database.Entities;

public sealed class ProcessedPrompt
{
    public Guid Id { get; set; }
    public required string Prompt { get; set; }
    public required string Result { get; set; }
    public DateTime ProcessedAt { get; set; }
}
```

### Paso 3: Crear el DbContext

```csharp
// Infrastructure/Database/AppDbContext.cs
using Microsoft.EntityFrameworkCore;
using AiContext.Api.Infrastructure.Database.Entities;

namespace AiContext.Api.Infrastructure.Database;

public sealed class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<ProcessedPrompt> ProcessedPrompts => Set<ProcessedPrompt>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ProcessedPrompt>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Prompt).IsRequired().HasMaxLength(5000);
            entity.Property(e => e.Result).IsRequired();
            entity.Property(e => e.ProcessedAt).IsRequired();
        });
    }
}
```

### Paso 4: Configurar en Program.cs

```csharp
// En Program.cs
using Microsoft.EntityFrameworkCore;

// Agregar DbContext
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));
```

### Paso 5: Agregar connection string

```json
// En appsettings.json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=AiContextDb;Trusted_Connection=true;"
  }
}
```

### Paso 6: Crear migraciones

```bash
dotnet ef migrations add InitialCreate
dotnet ef database update
```

### Paso 7: Usar en el servicio

```csharp
// Modificar AiProcessingService.cs
public sealed class AiProcessingService
{
    private readonly ILogger<AiProcessingService> _logger;
    private readonly AppDbContext _dbContext;

    public AiProcessingService(
        ILogger<AiProcessingService> logger,
        AppDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }

    public async Task<AiProcessingResponse> ProcessAsync(
        AiProcessingRequest request,
        CancellationToken cancellationToken = default)
    {
        var result = $"Procesado: {request.Prompt}";

        // Guardar en base de datos
        var entity = new ProcessedPrompt
        {
            Id = Guid.NewGuid(),
            Prompt = request.Prompt,
            Result = result,
            ProcessedAt = DateTime.UtcNow
        };

        _dbContext.ProcessedPrompts.Add(entity);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return new AiProcessingResponse
        {
            Result = result,
            Success = true
        };
    }

    // Nuevo método para obtener historial
    public async Task<List<ProcessedPrompt>> GetHistoryAsync(
        int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.ProcessedPrompts
            .OrderByDescending(p => p.ProcessedAt)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }
}
```

---

## 🔐 Ejemplo 4: Agregar Autenticación JWT

### Paso 1: Instalar paquetes

```bash
dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer
```

### Paso 2: Configurar JWT

```csharp
// En Program.cs
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = jwtSettings["SecretKey"]!;

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings["Issuer"],
            ValidAudience = jwtSettings["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(secretKey))
        };
    });

builder.Services.AddAuthorization();

// En el pipeline:
app.UseAuthentication();
app.UseAuthorization();
```

### Paso 3: Proteger endpoints

```csharp
// En AiProcessingEndpoints.cs
group.MapPost("/process", ProcessPrompt)
    .RequireAuthorization()  // ← Requiere autenticación
    .WithName("ProcessAiPrompt");
```

---

## 📊 Ejemplo 5: Agregar Caching con Redis

### Paso 1: Instalar paquetes

```bash
dotnet add package Microsoft.Extensions.Caching.StackExchangeRedis
```

### Paso 2: Configurar Redis

```csharp
// En Program.cs
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("Redis");
    options.InstanceName = "AiContext_";
});
```

### Paso 3: Usar en el servicio

```csharp
// Modificar AiProcessingService.cs
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

public sealed class AiProcessingService
{
    private readonly IDistributedCache _cache;

    public async Task<AiProcessingResponse> ProcessAsync(
        AiProcessingRequest request,
        CancellationToken cancellationToken = default)
    {
        // Intentar obtener del cache
        var cacheKey = $"prompt_{request.Prompt.GetHashCode()}";
        var cachedResult = await _cache.GetStringAsync(cacheKey, cancellationToken);

        if (cachedResult is not null)
        {
            _logger.LogInformation("Resultado obtenido del cache");
            return JsonSerializer.Deserialize<AiProcessingResponse>(cachedResult)!;
        }

        // Procesar y guardar en cache
        var response = new AiProcessingResponse { /* ... */ };

        await _cache.SetStringAsync(
            cacheKey,
            JsonSerializer.Serialize(response),
            new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
            },
            cancellationToken);

        return response;
    }
}
```

---

## 🧪 Ejemplo 6: Agregar Tests Unitarios

### Paso 1: Crear proyecto de tests

```bash
dotnet new xunit -n AiContext.Api.Tests
cd AiContext.Api.Tests
dotnet add reference ../AiContext.Api/AiContext.Api.csproj
dotnet add package Moq
dotnet add package FluentAssertions
```

### Paso 2: Crear test del servicio

```csharp
// Tests/AiProcessingServiceTests.cs
using Xunit;
using Moq;
using FluentAssertions;
using Microsoft.Extensions.Logging;

namespace AiContext.Api.Tests;

public class AiProcessingServiceTests
{
    private readonly Mock<ILogger<AiProcessingService>> _loggerMock;
    private readonly AiProcessingService _sut; // System Under Test

    public AiProcessingServiceTests()
    {
        _loggerMock = new Mock<ILogger<AiProcessingService>>();
        _sut = new AiProcessingService(_loggerMock.Object);
    }

    [Fact]
    public async Task ProcessAsync_WithValidPrompt_ReturnsSuccessResponse()
    {
        // Arrange
        var request = new AiProcessingRequest { Prompt = "Test prompt" };

        // Act
        var result = await _sut.ProcessAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Result.Should().Contain("Test prompt");
    }

    [Fact]
    public async Task ProcessAsync_WithEmptyPrompt_ThrowsArgumentException()
    {
        // Arrange
        var request = new AiProcessingRequest { Prompt = "" };

        // Act
        Func<Task> act = async () => await _sut.ProcessAsync(request);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>();
    }
}
```

### Paso 3: Ejecutar tests

```bash
dotnet test
```

---

## 📈 Ejemplo 7: Agregar Health Checks Avanzados

```csharp
// En Program.cs
builder.Services.AddHealthChecks()
    .AddCheck<AiProcessingHealthCheck>("ai_processing")
    .AddDbContextCheck<AppDbContext>("database")
    .AddRedis(builder.Configuration.GetConnectionString("Redis")!, "redis");

// Mapear endpoint
app.MapHealthChecks("/health", new HealthCheckOptions
{
    ResponseWriter = async (context, report) =>
    {
        context.Response.ContentType = "application/json";
        var response = new
        {
            status = report.Status.ToString(),
            checks = report.Entries.Select(e => new
            {
                name = e.Key,
                status = e.Value.Status.ToString(),
                duration = e.Value.Duration.TotalMilliseconds
            })
        };
        await context.Response.WriteAsJsonAsync(response);
    }
});
```

---

## 🎯 Checklist de Features a Implementar

- [ ] Segunda feature (ImageProcessing)
- [ ] Validación con FluentValidation
- [ ] Base de datos con Entity Framework
- [ ] Autenticación JWT
- [ ] Autorización basada en roles
- [ ] Caching con Redis
- [ ] Tests unitarios
- [ ] Tests de integración
- [ ] Health checks avanzados
- [ ] Rate limiting
- [ ] API versioning
- [ ] Logging estructurado con Serilog
- [ ] Documentación Swagger mejorada
- [ ] CQRS con MediatR
- [ ] Event sourcing
- [ ] Background jobs con Hangfire

---

Cada uno de estos ejemplos puede implementarse de forma independiente siguiendo la misma arquitectura por features. ¡Empieza con lo básico y ve agregando funcionalidades a medida que aprendes!
