namespace AiProject.Api.Features.AiProcessing;

    public sealed record AiProcessingResponse
    {

        /// <summary>
        /// Gets the result value produced by the operation.
        /// For example: "The sea is a vast and endless expanse..."
        /// </summary>
        public required string Result { get; init; }

        /// <summary>
        /// Gets the date and time when the item was processed, in Coordinated Universal Time (UTC).
        /// </summary>
        public DateTime ProcessedAt { get; init; } = DateTime.UtcNow;

        /// <summary>
        /// Gets a value indicating whether the operation completed successfully.
        /// </summary>
        public bool Success { get; init; } = true;

}

