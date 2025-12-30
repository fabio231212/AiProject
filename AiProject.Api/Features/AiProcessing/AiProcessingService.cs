namespace AiProject.Api.Features.AiProcessing;


/// <summary>
/// Service that handls AiProcessing operations.
/// In a real implementation, this would call OpenIA, Azure Ai, Claude...
/// </summary>
public sealed class AiProcessingService(ILogger<AiProcessingService> logger)
{
    private readonly ILogger<AiProcessingService> _logger = logger;


    public async Task<AiProcessingResponse> ProcessAsync(
        AiProcessingRequest request,
        CancellationToken cancellationToken = default)
    {
        //Exception if request is null
        //you should not use throw exception if prompt is empty
        //that because if prompt is empty in this point, it will 
        //be a programmer fault. you should check it out in a controller or endpoint
        ArgumentNullException.ThrowIfNull(request);

        _logger.LogInformation("Processing prompt: {Prompt}", request.Prompt);

        //simulate a async process
        await Task.Delay(100, cancellationToken);

        var result = $"Processed Correctly: {request.Prompt}";

        _logger.LogInformation("Process completed successfully: {Result}", result);

        return new AiProcessingResponse
        {
            Result = result,
            ProcessedAt = DateTime.UtcNow,
            Success = true
        };

    }


}

