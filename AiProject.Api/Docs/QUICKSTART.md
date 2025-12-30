# 🚀 GUÍA RÁPIDA DE INICIO

## ✅ Lo que acabamos de hacer

Hemos reestructurado completamente tu proyecto con:

1. **Arquitectura por Features (Vertical Slice)**
   - Todo lo relacionado con AI Processing está junto
   - Fácil de encontrar, modificar y entender

2. **Patrones modernos de .NET 10**
   - Minimal APIs (no Controllers)
   - Dependency Injection
   - Async/Await
   - Record types

3. **Documentación completa**
   - README.md con explicaciones
   - ARCHITECTURE.md con diagramas visuales
   - Comentarios detallados en el código

---

## 📁 Nueva Estructura del Proyecto

```
AiContext.Api/
├── 📁 Features/
│   └── 📁 AiProcessing/
│       ├── AiProcessingEndpoints.cs    ← Los endpoints HTTP
│       ├── AiProcessingService.cs      ← La lógica de negocio
│       ├── AiProcessingRequest.cs      ← El DTO de entrada
│       └── AiProcessingResponse.cs     ← El DTO de salida
│
├── 📄 Program.cs                       ← Entry point (configuración)
├── 📄 appsettings.json                ← Configuración
├── 📄 AiContext.Api.http              ← Pruebas HTTP
├── 📄 README.md                       ← Documentación principal
└── 📄 ARCHITECTURE.md                 ← Diagramas y conceptos
```

---

## 🏃 Cómo Ejecutar el Proyecto

### Opción 1: Desde Visual Studio

1. Abre el proyecto en Visual Studio 2022
2. Presiona `F5` o click en el botón ▶️ "Start"
3. La API se ejecutará en:
   - HTTPS: `https://localhost:7274`
   - HTTP: `http://localhost:5214`

### Opción 2: Desde Terminal

```bash
cd C:\Users\ramir\Desktop\Proyecto\AiContext\AiContext.Api
dotnet run
```

---

## 🧪 Cómo Probar la API

### 1️⃣ Usando el archivo .http (RECOMENDADO)

Abre `AiContext.Api.http` en Visual Studio y:

1. Click en **"Send Request"** sobre cualquier prueba
2. Ve la respuesta en el panel derecho

Pruebas disponibles:
- ✅ Health check
- ✅ Procesar prompt básico
- ✅ Prompts largos
- ✅ Caracteres especiales
- ❌ Casos de error (para validar manejo de errores)

### 2️⃣ Usando curl (Terminal)

```bash
# Health check
curl https://localhost:7274/api/ai/health

# Procesar un prompt
curl -X POST https://localhost:7274/api/ai/process ^
  -H "Content-Type: application/json" ^
  -d "{\"prompt\":\"Hola desde curl\"}"
```

### 3️⃣ Usando Postman

1. Crea nueva request POST
2. URL: `https://localhost:7274/api/ai/process`
3. Body (raw JSON):
   ```json
   {
     "prompt": "Tu texto aquí"
   }
   ```
4. Click "Send"

---

## 📋 Endpoints Disponibles

| Método | URL | Descripción |
|--------|-----|-------------|
| GET | `/` | Info de la API |
| GET | `/api/ai/health` | Health check |
| POST | `/api/ai/process` | Procesar prompt |

---

## 💡 Respuestas Esperadas

### ✅ Success (200 OK)
```json
{
  "result": "✓ Procesado exitosamente: 'Hola' (Caracteres: 4)",
  "processedAt": "2024-01-15T10:30:00Z",
  "success": true
}
```

### ❌ Error (400 Bad Request)
```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.1",
  "title": "Solicitud inválida",
  "status": 400,
  "detail": "El prompt no puede estar vacío"
}
```

---

## 🎓 Qué Aprender de Este Proyecto

### 1. **Vertical Slice Architecture**
- Cada feature es independiente
- Fácil de mantener y extender
- Reduce acoplamiento

### 2. **Minimal APIs**
```csharp
app.MapPost("/api/ai/process", async (request, service) => 
{
    var result = await service.ProcessAsync(request);
    return Results.Ok(result);
});
```

### 3. **Dependency Injection**
```csharp
// Registro en Program.cs
builder.Services.AddSingleton<AiProcessingService>();

// Inyección automática en endpoints
private static async Task<IResult> ProcessPrompt(
    [FromServices] AiProcessingService service)  // ← Inyectado automáticamente
```

### 4. **Record Types** (Immutable DTOs)
```csharp
public sealed record AiProcessingRequest
{
    public required string Prompt { get; init; }
}
```

### 5. **Async/Await Pattern**
```csharp
public async Task<AiProcessingResponse> ProcessAsync(...)
{
    await Task.Delay(100);  // No bloquea threads
    return response;
}
```

---

## 🔍 Archivos Importantes para Revisar

### 1. `Program.cs`
- Configuración de servicios (DI)
- Configuración del pipeline
- Registro de endpoints

### 2. `Features/AiProcessing/AiProcessingEndpoints.cs`
- Definición de rutas HTTP
- Manejo de errores
- Validación de inputs

### 3. `Features/AiProcessing/AiProcessingService.cs`
- Lógica de negocio
- Procesamiento de datos
- Validaciones

### 4. `README.md`
- Guía completa de aprendizaje
- Conceptos explicados
- Próximos pasos

### 5. `ARCHITECTURE.md`
- Diagramas visuales
- Flujos de datos
- Patrones de diseño

---

## 🎯 Próximos Pasos Recomendados

### Nivel Básico
1. ✅ Ejecuta el proyecto
2. ✅ Prueba todos los endpoints
3. ✅ Lee los comentarios en el código
4. ✅ Modifica el servicio para cambiar la respuesta

### Nivel Intermedio
5. 📝 Agrega un nuevo endpoint GET que liste todos los prompts procesados
6. 🔍 Agrega validación con FluentValidation
7. 📊 Agrega un contador de requests procesados

### Nivel Avanzado
8. 🗄️ Agrega Entity Framework para guardar en base de datos
9. 🔐 Implementa autenticación JWT
10. 🧪 Agrega tests unitarios con xUnit

---

## ❓ Preguntas Frecuentes

**P: ¿Por qué "sealed" en las clases?**
R: Optimización de rendimiento. Previene herencia innecesaria.

**P: ¿Por qué "record" en lugar de "class" para DTOs?**
R: Los records son inmutables por defecto, perfectos para datos que no cambian.

**P: ¿Qué es CancellationToken?**
R: Permite cancelar operaciones largas si el cliente cancela la request.

**P: ¿Por qué async/await en todos lados?**
R: Para no bloquear threads y manejar mejor la concurrencia.

**P: ¿Puedo usar Controllers en lugar de Minimal APIs?**
R: Sí, pero Minimal APIs es el enfoque moderno y recomendado para APIs simples.

---

## 🐛 Solución de Problemas

### El proyecto no compila
```bash
# Restaurar paquetes NuGet
dotnet restore

# Limpiar y reconstruir
dotnet clean
dotnet build
```

### Error de certificado HTTPS
```bash
# Confiar en el certificado de desarrollo
dotnet dev-certs https --trust
```

### Puerto en uso
Cambia los puertos en `Properties/launchSettings.json`

---

## 📚 Recursos para Seguir Aprendiendo

1. **Documentación oficial de .NET**
   - https://learn.microsoft.com/es-es/aspnet/core/

2. **Minimal APIs**
   - https://learn.microsoft.com/es-es/aspnet/core/fundamentals/minimal-apis

3. **Vertical Slice Architecture**
   - https://www.jimmybogard.com/vertical-slice-architecture/

4. **C# Moderno**
   - https://learn.microsoft.com/es-es/dotnet/csharp/

---

## ✨ Características Implementadas

- ✅ Arquitectura por Features
- ✅ Minimal APIs
- ✅ Dependency Injection
- ✅ Logging estructurado
- ✅ Manejo de errores con ProblemDetails
- ✅ OpenAPI/Swagger documentation
- ✅ CORS configurado
- ✅ Async/Await pattern
- ✅ Record types para DTOs
- ✅ Health check endpoint
- ✅ Validación de inputs
- ✅ Comentarios educativos completos

---

## 🎉 ¡Felicitaciones!

Has reestructurado exitosamente tu proyecto siguiendo las mejores prácticas de .NET moderno. Este proyecto es una excelente base para aprender y construir APIs más complejas.

**¿Dudas?** Revisa los archivos README.md y ARCHITECTURE.md que contienen explicaciones detalladas.

**¿Listo para más?** Intenta agregar una nueva feature completa siguiendo el mismo patrón de AiProcessing.
