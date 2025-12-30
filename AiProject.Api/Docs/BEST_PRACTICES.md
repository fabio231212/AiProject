# 💎 MEJORES PRÁCTICAS Y TIPS

Esta guía contiene consejos y mejores prácticas para escribir código .NET de calidad.

---

## 📏 Convenciones de Código

### Naming Conventions

```csharp
// ✅ CORRECTO
public class AiProcessingService { }              // PascalCase para clases
public interface IAiProcessor { }                 // I + PascalCase para interfaces
public sealed record AiRequest { }                // PascalCase para records
public string ProcessedResult { get; set; }       // PascalCase para propiedades
public async Task ProcessAsync() { }              // PascalCase para métodos
private readonly ILogger _logger;                 // _camelCase para campos privados
public void Process(string prompt) { }            // camelCase para parámetros
const int MaxRetries = 3;                         // PascalCase para constantes

// ❌ INCORRECTO
public class aiProcessingService { }              // Minúsculas
public class AI_Processing_Service { }            // Underscores en clases
private string RESULT;                            // Mayúsculas en campos
public void process_data() { }                    // Underscores en métodos
```

### Organización de Clases

```csharp
public sealed class AiProcessingService
{
    // 1. Campos privados
    private readonly ILogger<AiProcessingService> _logger;
    private readonly AppDbContext _dbContext;

    // 2. Constructor
    public AiProcessingService(
        ILogger<AiProcessingService> logger,
        AppDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }

    // 3. Propiedades públicas
    public int ProcessedCount { get; private set; }

    // 4. Métodos públicos
    public async Task<AiProcessingResponse> ProcessAsync(...)
    {
        // Implementación
    }

    // 5. Métodos privados
    private async Task ValidateAsync(...)
    {
        // Implementación
    }
}
```

---

## 🎯 Principios SOLID

### 1. Single Responsibility Principle (SRP)
**Una clase debe tener una sola razón para cambiar**

```csharp
// ❌ MAL: Hace demasiadas cosas
public class AiService
{
    public async Task ProcessAsync() { }
    public async Task SaveToDatabase() { }
    public async Task SendEmail() { }
    public async Task LogMetrics() { }
}

// ✅ BIEN: Cada clase tiene una responsabilidad
public class AiProcessingService
{
    public async Task ProcessAsync() { }
}

public class AiRepository
{
    public async Task SaveAsync() { }
}

public class EmailService
{
    public async Task SendAsync() { }
}

public class MetricsService
{
    public async Task LogAsync() { }
}
```

### 2. Open/Closed Principle (OCP)
**Abierto para extensión, cerrado para modificación**

```csharp
// ❌ MAL: Necesitas modificar la clase para agregar nuevos tipos
public class AiProcessor
{
    public string Process(string type, string input)
    {
        if (type == "summarize")
            return Summarize(input);
        else if (type == "translate")
            return Translate(input);
        // Necesitas modificar esto para agregar nuevos tipos
    }
}

// ✅ BIEN: Puedes agregar nuevos procesadores sin modificar código existente
public interface IAiProcessor
{
    Task<string> ProcessAsync(string input);
}

public class SummarizeProcessor : IAiProcessor
{
    public async Task<string> ProcessAsync(string input) => 
        await SummarizeAsync(input);
}

public class TranslateProcessor : IAiProcessor
{
    public async Task<string> ProcessAsync(string input) => 
        await TranslateAsync(input);
}

// Agregar nuevos procesadores sin modificar nada
public class AnalyzeProcessor : IAiProcessor
{
    public async Task<string> ProcessAsync(string input) => 
        await AnalyzeAsync(input);
}
```

### 3. Liskov Substitution Principle (LSP)
**Las subclases deben poder usarse en lugar de sus clases base**

```csharp
// ✅ BIEN: Todas las implementaciones respetan el contrato
public interface IDataStore
{
    Task<T> GetAsync<T>(string key);
    Task SetAsync<T>(string key, T value);
}

public class RedisDataStore : IDataStore
{
    public async Task<T> GetAsync<T>(string key) { /* Redis logic */ }
    public async Task SetAsync<T>(string key, T value) { /* Redis logic */ }
}

public class SqlDataStore : IDataStore
{
    public async Task<T> GetAsync<T>(string key) { /* SQL logic */ }
    public async Task SetAsync<T>(string key, T value) { /* SQL logic */ }
}

// Puedes intercambiar implementaciones sin problemas
public class Service
{
    private readonly IDataStore _store; // Puede ser Redis o SQL

    public Service(IDataStore store) => _store = store;
}
```

### 4. Interface Segregation Principle (ISP)
**No fuerces a las clases a implementar interfaces que no usan**

```csharp
// ❌ MAL: Interface demasiado grande
public interface IRepository
{
    Task<T> GetAsync<T>(int id);
    Task SaveAsync<T>(T entity);
    Task DeleteAsync<T>(int id);
    Task<List<T>> SearchAsync<T>(string query);
    Task<byte[]> ExportToCsvAsync<T>();
    Task ImportFromCsvAsync<T>(byte[] csv);
}

// ✅ BIEN: Interfaces específicas
public interface IReadRepository<T>
{
    Task<T> GetAsync(int id);
    Task<List<T>> SearchAsync(string query);
}

public interface IWriteRepository<T>
{
    Task SaveAsync(T entity);
    Task DeleteAsync(int id);
}

public interface ICsvExporter<T>
{
    Task<byte[]> ExportToCsvAsync();
    Task ImportFromCsvAsync(byte[] csv);
}

// Las clases implementan solo lo que necesitan
public class ReadOnlyRepository<T> : IReadRepository<T> { }
public class FullRepository<T> : IReadRepository<T>, IWriteRepository<T> { }
```

### 5. Dependency Inversion Principle (DIP)
**Depende de abstracciones, no de concreciones**

```csharp
// ❌ MAL: Depende de implementaciones concretas
public class AiService
{
    private readonly SqlDatabase _database;
    private readonly SmtpEmailSender _emailSender;

    public AiService()
    {
        _database = new SqlDatabase(); // Acoplamiento fuerte
        _emailSender = new SmtpEmailSender(); // Acoplamiento fuerte
    }
}

// ✅ BIEN: Depende de abstracciones
public class AiService
{
    private readonly IDatabase _database;
    private readonly IEmailSender _emailSender;

    public AiService(IDatabase database, IEmailSender emailSender)
    {
        _database = database;
        _emailSender = emailSender;
    }
}

// Puedes cambiar implementaciones fácilmente
builder.Services.AddSingleton<IDatabase, SqlDatabase>();
// O cambiar a MongoDB sin modificar AiService
builder.Services.AddSingleton<IDatabase, MongoDatabase>();
```

---

## 🔒 Manejo de Errores

### Try-Catch Apropiado

```csharp
// ❌ MAL: Catch genérico sin hacer nada
try
{
    await ProcessAsync();
}
catch (Exception)
{
    // No hacer nada es malo
}

// ❌ MAL: Catch y re-throw sin agregar información
try
{
    await ProcessAsync();
}
catch (Exception ex)
{
    throw ex; // Pierde el stack trace
}

// ✅ BIEN: Catch específico con logging
try
{
    await ProcessAsync();
}
catch (ArgumentException ex)
{
    _logger.LogWarning(ex, "Argumento inválido");
    return Results.BadRequest(ex.Message);
}
catch (InvalidOperationException ex)
{
    _logger.LogError(ex, "Operación inválida");
    return Results.Problem("No se pudo completar la operación");
}
catch (Exception ex)
{
    _logger.LogError(ex, "Error inesperado");
    throw; // Preserva el stack trace
}
```

### Validaciones

```csharp
// ✅ BIEN: Validar al inicio del método
public async Task<Result> ProcessAsync(AiRequest request)
{
    // Guard clauses al inicio
    if (request is null)
        throw new ArgumentNullException(nameof(request));

    if (string.IsNullOrWhiteSpace(request.Prompt))
        throw new ArgumentException("Prompt no puede estar vacío", nameof(request));

    if (request.Prompt.Length > MaxLength)
        throw new ArgumentException($"Prompt excede {MaxLength} caracteres");

    // Lógica principal después de las validaciones
    return await InternalProcessAsync(request);
}
```

---

## ⚡ Performance y Optimización

### Async/Await

```csharp
// ❌ MAL: Bloquear con .Result o .Wait()
public string Process()
{
    var result = ProcessAsync().Result; // Puede causar deadlocks
    return result;
}

// ✅ BIEN: Async todo el camino
public async Task<string> ProcessAsync()
{
    var result = await InternalProcessAsync();
    return result;
}
```

### LINQ Eficiente

```csharp
// ❌ MAL: Múltiples iteraciones
var items = list.Where(x => x.IsActive)
                .ToList()
                .Where(x => x.Age > 18)
                .ToList()
                .OrderBy(x => x.Name)
                .ToList();

// ✅ BIEN: Una sola iteración
var items = list.Where(x => x.IsActive && x.Age > 18)
                .OrderBy(x => x.Name)
                .ToList(); // Solo materializar al final

// ✅ MEJOR: Usar métodos específicos
var count = list.Count(x => x.IsActive); // No usar .Where().Count()
var exists = list.Any(x => x.Id == id);  // No usar .Count() > 0
var first = list.FirstOrDefault(x => x.Id == id); // No usar .Where().First()
```

### String Building

```csharp
// ❌ MAL: Concatenación en loop
string result = "";
for (int i = 0; i < 1000; i++)
{
    result += i.ToString(); // Crea 1000 strings inmutables
}

// ✅ BIEN: StringBuilder
var sb = new StringBuilder();
for (int i = 0; i < 1000; i++)
{
    sb.Append(i);
}
var result = sb.ToString();
```

### Uso de Memory y Span

```csharp
// ✅ Para operaciones en arrays sin copiar memoria
public void ProcessData(byte[] data)
{
    Span<byte> span = data.AsSpan();
    
    // Trabajar con slice sin copiar
    var firstPart = span[..100];
    var secondPart = span[100..];
}
```

---

## 🔐 Seguridad

### No exponer información sensible

```csharp
// ❌ MAL: Exponer excepciones al cliente
catch (Exception ex)
{
    return Results.Problem(ex.ToString()); // Expone stack trace
}

// ✅ BIEN: Mensajes genéricos al cliente
catch (Exception ex)
{
    _logger.LogError(ex, "Error procesando request");
    return Results.Problem("Ocurrió un error interno");
}
```

### Validar inputs

```csharp
// ✅ Siempre validar y sanitizar
public async Task<IResult> Upload(IFormFile file)
{
    // Validar tipo
    var allowedTypes = new[] { "image/jpeg", "image/png" };
    if (!allowedTypes.Contains(file.ContentType))
        return Results.BadRequest("Tipo de archivo no permitido");

    // Validar tamaño
    if (file.Length > 5 * 1024 * 1024)
        return Results.BadRequest("Archivo muy grande");

    // Validar extensión
    var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
    if (extension != ".jpg" && extension != ".png")
        return Results.BadRequest("Extensión no permitida");

    // Procesar archivo
}
```

---

## 📝 Logging Efectivo

### Niveles apropiados

```csharp
// Trace: Información extremadamente detallada (normalmente deshabilitado)
_logger.LogTrace("Iniciando iteración {Index}", i);

// Debug: Información útil para debugging (solo en desarrollo)
_logger.LogDebug("Cache miss para key {Key}", cacheKey);

// Information: Eventos importantes del flujo
_logger.LogInformation("Procesando request de usuario {UserId}", userId);

// Warning: Situaciones inusuales pero manejables
_logger.LogWarning("Rate limit cerca del límite para IP {IpAddress}", ip);

// Error: Errores que se manejan pero son importantes
_logger.LogError(ex, "Fallo al procesar request {RequestId}", requestId);

// Critical: Errores que requieren atención inmediata
_logger.LogCritical(ex, "Base de datos no responde");
```

### Structured Logging

```csharp
// ❌ MAL: String interpolation
_logger.LogInformation($"Usuario {userId} procesó {count} items");

// ✅ BIEN: Message templates (permite indexación y búsqueda)
_logger.LogInformation("Usuario {UserId} procesó {Count} items", userId, count);
```

---

## 🧪 Testing

### Arrange-Act-Assert Pattern

```csharp
[Fact]
public async Task ProcessAsync_WithValidInput_ReturnsSuccess()
{
    // Arrange: Preparar
    var service = new AiProcessingService(_loggerMock.Object);
    var request = new AiRequest { Prompt = "Test" };

    // Act: Ejecutar
    var result = await service.ProcessAsync(request);

    // Assert: Verificar
    result.Should().NotBeNull();
    result.Success.Should().BeTrue();
}
```

### Naming de Tests

```csharp
// Patrón: MethodName_Scenario_ExpectedBehavior

[Fact]
public async Task ProcessAsync_WithEmptyPrompt_ThrowsArgumentException() { }

[Fact]
public async Task ProcessAsync_WithValidPrompt_ReturnsSuccessResponse() { }

[Fact]
public async Task ProcessAsync_WhenServiceUnavailable_ReturnsError() { }
```

---

## 📦 Organización de Código

### Feature Folders (Recomendado)

```
Features/
├── AiProcessing/
│   ├── AiProcessingEndpoints.cs
│   ├── AiProcessingService.cs
│   ├── AiProcessingRequest.cs
│   ├── AiProcessingResponse.cs
│   └── AiProcessingValidator.cs
└── UserManagement/
    ├── UserEndpoints.cs
    ├── UserService.cs
    └── ...
```

### Layer Folders (Alternativa)

```
├── Endpoints/
├── Services/
├── Models/
├── Validators/
└── Repositories/
```

---

## 💡 Tips Generales

### 1. Usar Pattern Matching Moderno

```csharp
// ❌ Antiguo
if (obj is string)
{
    var str = (string)obj;
    Console.WriteLine(str.Length);
}

// ✅ Moderno
if (obj is string str)
{
    Console.WriteLine(str.Length);
}

// ✅ Switch expression
var result = request.Type switch
{
    "summarize" => await SummarizeAsync(request),
    "translate" => await TranslateAsync(request),
    "analyze" => await AnalyzeAsync(request),
    _ => throw new ArgumentException("Tipo no soportado")
};
```

### 2. Usar null-coalescing y null-conditional

```csharp
// ❌ Verboso
string result;
if (value != null)
    result = value.Trim();
else
    result = "default";

// ✅ Conciso
var result = value?.Trim() ?? "default";
```

### 3. Usar Target-typed new

```csharp
// ❌ Redundante
AiProcessingRequest request = new AiProcessingRequest { Prompt = "test" };

// ✅ Limpio
AiProcessingRequest request = new() { Prompt = "test" };
```

### 4. Usar File-scoped namespaces (C# 10+)

```csharp
// ❌ Antiguo
namespace AiContext.Api.Features.AiProcessing
{
    public class Service
    {
        // Código con indentación extra
    }
}

// ✅ Moderno
namespace AiContext.Api.Features.AiProcessing;

public class Service
{
    // Sin indentación extra
}
```

### 5. Usar Primary Constructors (C# 12+)

```csharp
// ❌ Antiguo
public class Service
{
    private readonly ILogger _logger;

    public Service(ILogger logger)
    {
        _logger = logger;
    }
}

// ✅ Moderno
public class Service(ILogger logger)
{
    // logger está disponible automáticamente
    public void DoWork() => logger.LogInformation("Working");
}
```

---

## ⚠️ Code Smells a Evitar

### 1. Métodos muy largos
- Más de 20-30 líneas suele indicar que hace demasiado
- Extraer a métodos más pequeños

### 2. Clases con demasiadas responsabilidades
- Si tiene más de 5-7 métodos públicos, probablemente hace mucho
- Dividir en clases más pequeñas

### 3. Parámetros booleanos
```csharp
// ❌ MAL
public void Process(bool isAsync, bool validate, bool log) { }

// ✅ BIEN
public void Process(ProcessingOptions options) { }
```

### 4. Magic numbers
```csharp
// ❌ MAL
if (age > 18) { }

// ✅ BIEN
const int LegalAge = 18;
if (age > LegalAge) { }
```

### 5. Código comentado
```csharp
// ❌ MAL: Eliminar código comentado
// var oldCode = DoSomething();
// return oldCode;

// ✅ BIEN: Usar control de versiones (Git)
```

---

Siguiendo estas prácticas, tu código será más mantenible, testeable y profesional. ¡Úsalo como referencia mientras desarrollas!
