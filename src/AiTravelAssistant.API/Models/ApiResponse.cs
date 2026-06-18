namespace AiTravelAssistant.API.Models;

/// <summary>
/// Standard API response envelope used by all endpoints to provide consistent success and error shapes.
/// </summary>
/// <typeparam name="T">The type of the data payload returned on success.</typeparam>
public class ApiResponse<T>
{
    /// <summary>Gets a value indicating whether the request completed successfully.</summary>
    public bool Success { get; init; }

    /// <summary>Gets the response payload on success; <see langword="null"/> on failure.</summary>
    public T? Data { get; init; }

    /// <summary>Gets the error details on failure; <see langword="null"/> on success.</summary>
    public ApiError? Error { get; init; }

    /// <summary>Gets the trace identifier for correlating logs with the originating request.</summary>
    public string? TraceId { get; init; }

    /// <summary>
    /// Creates a successful response envelope containing the provided data.
    /// </summary>
    /// <param name="data">The operation payload to include in the response.</param>
    /// <param name="traceId">The optional trace identifier for the request.</param>
    /// <returns>A successful <see cref="ApiResponse{T}"/> instance.</returns>
    public static ApiResponse<T> Ok(T data, string? traceId = null) => new()
    {
        Success = true,
        Data = data,
        TraceId = traceId
    };

    /// <summary>
    /// Creates a failed response envelope containing an <see cref="ApiError"/>.
    /// </summary>
    /// <param name="code">The machine-readable error code.</param>
    /// <param name="message">A human-readable description of the error.</param>
    /// <param name="traceId">The optional trace identifier for the request.</param>
    /// <returns>A failed <see cref="ApiResponse{T}"/> instance.</returns>
    public static ApiResponse<T> Fail(string code, string message, string? traceId = null) => new()
    {
        Success = false,
        Error = new ApiError(code, message),
        TraceId = traceId
    };
}
