namespace AiTravelAssistant.API.Models;

/// <summary>
/// Represents a structured error included in an <see cref="ApiResponse{T}"/> when a request fails.
/// </summary>
/// <param name="Code">The machine-readable error code (e.g. "DOCUMENT_NOT_FOUND").</param>
/// <param name="Message">A human-readable description of the error.</param>
public record ApiError(string Code, string Message);
