using AiProject.Api.Features.AiProcessing;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();


// Register AiProcessingService as a singleton
builder.Services.AddSingleton<AiProcessingService>();

// Configure CORS to allow requests from any origin
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()   // In Production, specify allowed origins
              .AllowAnyMethod()   // GET, POST, PUT, DELETE, etc.
              .AllowAnyHeader();  // Personalized headers 
    });
});

// Configure logging providers
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

// Build the app
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseCors();

app.MapGet("/", () => "AI Processing API is running.");

app.MapAiProcessing();


app.Logger.LogInformation("Starting AI Processing API...");

app.Run();


