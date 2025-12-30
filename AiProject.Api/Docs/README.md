# 📚 AiContext API - Guía de Aprendizaje

## 🎯 Propósito del Proyecto

Este proyecto es una API REST moderna construida con .NET 10 que sirve como base para aprender:
- Arquitectura por Features (Vertical Slice Architecture)
- Minimal APIs
- Dependency Injection
- Patrones de diseño modernos
- Mejores prácticas de C#

---

## 🏗️ Arquitectura Explicada

### Vertical Slice Architecture

En lugar de organizar por capas horizontales (Controllers, Services, Models), organizamos por **features verticales**:

```
Features/
└── AiProcessing/              ← Una "feature" completa
    ├── AiProcessingRequest.cs     (DTO de entrada)
    ├── AiProcessingResponse.cs    (DTO de salida)
    ├── AiProcessingService.cs     (Lógica de negocio)
    └── AiProcessingEndpoints.cs   (Endpoints HTTP)
```

**Ventajas:**
- ✅ Todo relacionado con una feature está junto
- ✅ Fácil de encontrar y modificar
- ✅ Fácil de eliminar features completas
- ✅ Reduce el acoplamiento
- ✅ Ideal para equipos que trabajan en paralelo

---

## 🔄 Flujo de una Request

```
1. Cliente hace POST a /api/ai/process
   ↓
2. ASP.NET Core recibe la request
   ↓
3. Pasa por el middleware pipeline (CORS, HTTPS, etc.)
   ↓
4. Llega al endpoint MapPost en AiProcessingEndpoints
   ↓
5. Se inyecta automáticamente AiProcessingService
   ↓
6. El endpoint llama a service.ProcessAsync()
   ↓
7. El servicio procesa y devuelve AiProcessingResponse
   ↓
8. El endpoint convierte la response a JSON
   ↓
9. El cliente recibe la respuesta HTTP 200 OK
```

---

## 📦 Dependency Injection Explicado

### ¿Qué es DI?

En lugar de crear dependencias con `new`:
```csharp
// ❌ MAL: Acoplamiento fuerte
var logger = new Logger();
var service = new AiProcessingService(logger);
```

Dejamos que el framework las inyecte:
```csharp
// ✅ BIEN: Inyección automática
public AiProcessingService(ILogger<AiProcessingService> logger)
{
    _logger = logger;  // El framework lo proporciona automáticamente
}
```

### Lifetimes

| Lifetime | Cuándo usar | Ejemplo |
|----------|-------------|---------|
| **Singleton** | Una instancia para toda la app | Configuración, Cache |
| **Scoped** | Una instancia por request HTTP | DbContext, Servicios con estado |
| **Transient** | Nueva instancia cada vez | Servicios ligeros sin estado |

---

## 🧪 Cómo Probar la API

### 1. Usando el archivo .http (Visual Studio)

Crea un archivo `AiContext.Api.http`:
```http
### Health Check
GET https://localhost:7274/api/ai/health

### Procesar Prompt
POST https://localhost:7274/api/ai/process
Content-Type: application/json

{
  "prompt": "Hola, esta es una prueba"
}
```

### 2. Usando curl (Terminal)

```bash
# Health check
curl https://localhost:7274/api/ai/health

# Procesar prompt
curl -X POST https://localhost:7274/api/ai/process \
  -H "Content-Type: application/json" \
  -d '{"prompt":"Hola mundo"}'
```

### 3. Usando Postman o Insomnia

1. Importa la URL base: `https://localhost:7274`
2. Crea request POST a `/api/ai/process`
3. Agrega body JSON:
   ```json
   {
     "prompt": "Tu texto aquí"
   }
   ```

### 4. Usando el navegador (solo GET)

Abre: `https://localhost:7274/api/ai/health`

---

## 🎨 Patrones de Diseño Utilizados

### 1. **Record Types** (DTOs)
```csharp
public sealed record AiProcessingRequest
{
    public required string Prompt { get; init; }
}
```
- Inmutables por defecto
- Perfectos para DTOs
- Sintaxis concisa

### 2. **Extension Methods** (Organización de endpoints)
```csharp
public static IEndpointRouteBuilder MapAiProcessing(this IEndpointRouteBuilder app)
```
- Extiende funcionalidad sin modificar clases
- Organiza código de forma limpia

### 3. **Async/Await Pattern**
```csharp
public async Task<AiProcessingResponse> ProcessAsync(...)
```
- No bloquea threads
- Mejor rendimiento
- Esencial para I/O operations

### 4. **Repository Pattern** (Implícito en el Service)
```csharp
public class AiProcessingService  // Este sería el "repository"
{
    public async Task<AiProcessingResponse> ProcessAsync(...)
}
```

---

## 📝 Conceptos Clave de C# Moderno

### `required` keyword (C# 11)
```csharp
public required string Prompt { get; init; }
// Fuerza que la propiedad se inicialice al crear el objeto
```

### `sealed` keyword
```csharp
public sealed class AiProcessingService
// Previene herencia - optimización de rendimiento
```

### `init` accessor (C# 9)
```csharp
public string Prompt { get; init; }
// Solo se puede asignar durante la inicialización
```

### Primary Constructor (C# 12)
```csharp
public class Service(ILogger logger)  // Parámetro del constructor
{
    // logger está disponible automáticamente
}
```

---

## 🚀 Próximos Pasos para Aprender

1. **Agregar validación con FluentValidation**
2. **Implementar CQRS con MediatR**
3. **Agregar Entity Framework para persistencia**
4. **Implementar autenticación JWT**
5. **Agregar tests unitarios con xUnit**
6. **Implementar rate limiting**
7. **Agregar caching con Redis**
8. **Implementar logging estructurado con Serilog**

---

## 📚 Recursos Recomendados

- [Minimal APIs en .NET](https://learn.microsoft.com/es-es/aspnet/core/fundamentals/minimal-apis)
- [Vertical Slice Architecture](https://www.jimmybogard.com/vertical-slice-architecture/)
- [Dependency Injection en .NET](https://learn.microsoft.com/es-es/dotnet/core/extensions/dependency-injection)
- [C# 12 Features](https://learn.microsoft.com/es-es/dotnet/csharp/whats-new/csharp-12)

---

## ❓ Preguntas Frecuentes

**P: ¿Por qué no usar Controllers?**
R: Minimal APIs son más ligeras y modernas. Para APIs simples son perfectas.

**P: ¿Cuándo usar Scoped vs Singleton?**
R: Singleton para servicios sin estado, Scoped para servicios con estado por request.

**P: ¿Necesito Entity Framework?**
R: No para este ejemplo básico. Lo agregarías cuando necesites base de datos.

**P: ¿Esto escala?**
R: Sí, esta arquitectura escala muy bien. Puedes agregar más features independientes.
