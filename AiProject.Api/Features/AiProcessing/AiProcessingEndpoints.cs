using Microsoft.AspNetCore.Mvc;

namespace AiProject.Api.Features.AiProcessing;

/// <summary>
/// Provides a centralized location for AI processing endpoint definitions and related constants.
/// </summary>
/// <remarks>This class is intended to group endpoint URIs or identifiers used for AI processing
/// operations. It is static and cannot be instantiated.</remarks>
public static class AiProcessingEndpoints
{

    public static IEndpointRouteBuilder MapAiProcessing(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/ai").WithTags("Ai Processing");


        //POST /api/ai/process
        group.MapPost("/process", ProcessPrompt)
            //these tags are just to documentation, you can delete it
            .WithName("ProcessAiPrompt")
            .WithSummary("Process a prompt using IA")
            .WithDescription("Send a text and receive an processed answer")
            .Produces<AiProcessingResponse>(StatusCodes.Status200OK)
            .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError);

        group.MapGet("/health", HealthCheck)
        .WithName("AiHealthCheck")
        .WithSummary("Verify service status")
        .Produces<object>(StatusCodes.Status200OK);


        return app;

    }


    /// <summary>
    /// Processes an AI prompt request asynchronously and returns the result of the operation.
    /// </summary>
    /// <param name="request">The AI processing request containing the prompt and any associated parameters. Cannot be null.</param>
    /// <param name="service">The service used to process the AI prompt. Cannot be null.</param>
    /// <param name="logger">The logger instance used for logging processing events. Cannot be null.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains an <see cref="IResult"/>
    /// representing the outcome of the AI prompt processing.</returns>
    private static async Task<IResult> ProcessPrompt(
        [FromBody] AiProcessingRequest request,
        [FromServices] AiProcessingService service,
        [FromServices] ILogger<AiProcessingService> logger,
        CancellationToken cancellationToken)
    {
        try
        {
            logger.LogInformation("Enpoint /api/ai/process called");

            //basic validation
            if (string.IsNullOrWhiteSpace(request.Prompt))
                return Results.BadRequest(new ProblemDetails
                {
                    Title = "Invalid Prompt",
                    Detail = "Prompt cant be empty",
                    Status = StatusCodes.Status400BadRequest
                });

            //call the service to process
            var response = await service.ProcessAsync(request, cancellationToken);

            //return success result
            return Results.Ok(response);


        }
        catch (ArgumentException ex)
        {
            logger.LogWarning(ex, "Invalid received Argument");

            return Results.BadRequest(new ProblemDetails
            {
                Title = "Invalid request",
                Detail = ex.Message,
                Status = StatusCodes.Status400BadRequest
            });


        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error processing the prompt");
            return Results.Problem(
           title: "Internal server error",
           detail: "An error occurred while processing your request",
           statusCode: StatusCodes.Status500InternalServerError
           );
        }

    }



    /// <summary>
    /// Health check simple
    /// </summary>
    private static IResult HealthCheck()
    {
        return Results.Ok(new
        {
            Status = "Healthy",
            Service = "AI Processing",
            Timestamp = DateTime.UtcNow,
            Version = "1.0.0"
        });
    }


}

