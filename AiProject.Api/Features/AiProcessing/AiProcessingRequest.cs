namespace AiProject.Api.Features.AiProcessing
{

    /// <summary>
    /// Represents a request to process an AI prompt.
    /// </summary>
    public sealed record AiProcessingRequest
    {

        /// <summary>
        /// Gets the prompt text to be used as input.
        ///For example: "Write a poem about the sea."
        /// </summary>
        public required string Prompt { get; init; }
    }
}
